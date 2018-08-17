using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Toucan.Contract.Security;
using Microsoft.Extensions.Options;

namespace Toucan.Service.Security
{
    public class SecurityClaimsInspector : ISecurityClaimsInspector
    {
        private static Dictionary<ClaimRequirementType, Func<Claim, object[], bool>> strategies = new Dictionary<ClaimRequirementType, Func<Claim, object[], bool>>()
        {
            { ClaimRequirementType.Any, StrategyAny},
            { ClaimRequirementType.Exists, StrategyExists},
            { ClaimRequirementType.RegexPattern, StrategyRegexPattern},
            { ClaimRequirementType.All, StrategyStrict}
        };

        private readonly Config config;

        public SecurityClaimsInspector(IOptions<Service.Config> config)
        {
            this.config = config.Value;
        }

        public bool Satisifies(ClaimsPrincipal principal, ClaimRequirementType requirementType, string claimType, params object[] values)
        {
            string type = this.config.ClaimsNamespace + claimType;

            Claim claim = principal.Claims.FirstOrDefault(o => o.Type == type);

            if (claim != null && claim.Value == SecurityClaimValueTypes.Deny.ToString())
                return false;

            Func<Claim, object[], bool> strategy = strategies.Single(o => o.Key == requirementType).Value;

            return strategy(claim, values);
        }

        private static bool StrategyAny(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            if (values.Length == 0)
                return true;

            bool any = values.Any(o =>
            {
                if (o.GetType() == typeof(char))
                    return claim.Value.Contains(o.ToString());
                else
                    return claim.Value == o.ToString();
            });

            return any;
        }

        private static bool StrategyExists(Claim claim, params object[] values)
        {
            return claim != null;
        }

        private static bool StrategyRegexPattern(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            string pattern = values.Length > 0 ? values[0].ToString() : null;

            if (string.IsNullOrWhiteSpace(pattern))
                throw new ArgumentOutOfRangeException($"Authorization attribute is incorrectly configured. Expected the first parameter to be a valid regex pattern");

            try
            {
                return Regex.IsMatch(claim.Value, pattern);
            }
            catch (ArgumentException)
            {
                throw new ArgumentOutOfRangeException($"Authorization attribute is incorrectly configured. A regular expression parsing error occurred");
            }
        }

        private static bool StrategyStrict(Claim claim, params object[] values)
        {
            if (claim == null || string.IsNullOrWhiteSpace(claim.Value))
                return false;

            if (values.Length == 0)
                return true;

            var found = values.Count(o =>
            {
                if (o.GetType() == typeof(char))
                    return claim.Value.Contains(o.ToString());
                else
                    return claim.Value == o.ToString();
            });

            return found >= values.Length;
        }
    }
}