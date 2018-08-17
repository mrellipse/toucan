using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Toucan.Contract.Security;
using Toucan.Service.Security;

namespace Toucan.Server.Security
{
    // NB: functionality varies, depending on whether char or string params are supplied as input values
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class AuthorizeClaimAttribute : AuthorizeAttribute
    {
        public const string PolicyName = "ClaimAttribute";

        public static Func<AuthorizationHandlerContext, bool> PolicyHandler = (context) =>
        {
            if (context.Resource is AuthorizationFilterContext mvcContext)
            {
                if (context.User.IsInRole(RoleTypes.Admin))
                    return true;

                if (mvcContext.ActionDescriptor is Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor actionContext)
                {
                    var controller = actionContext.ControllerTypeInfo.GetCustomAttributes(true).OfType<AuthorizeClaimAttribute>();
                    var action = actionContext.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeClaimAttribute>();

                    var matches = controller.Union(action);

                    var inspector = mvcContext.HttpContext.RequestServices.GetService<ISecurityClaimsInspector>();

                    return matches.All(o => inspector.Satisifies(context.User, o.RequirementType, o.ClaimType, o.Values));
                }
            };

            return false;
        };

        // checks for the existence of a claim
        public AuthorizeClaimAttribute(string claimType) : this(ClaimRequirementType.Exists, claimType)
        {
            Values = new object[] { };
        }

        // checks for the existence of a claim with a specific value
        public AuthorizeClaimAttribute(string claimType, string value) : this(ClaimRequirementType.All, claimType)
        {
            Values = new string[] { value };
        }

        // checks for the existence of a claim with a specific value
        public AuthorizeClaimAttribute(string claimType, char value) : this(ClaimRequirementType.All, claimType)
        {
            Values = new object[] { value };
        }

        public AuthorizeClaimAttribute(ClaimRequirementType requirementType, string claimType, params char[] values) : this(requirementType, claimType)
        {
            Values = values.Cast<object>().ToArray();
        }

        public AuthorizeClaimAttribute(ClaimRequirementType requirementType, string claimType, params string[] values) : this(requirementType, claimType)
        {
            Values = values;
        }

        private AuthorizeClaimAttribute(ClaimRequirementType requirementType, string claimType) : base(PolicyName)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentNullException(nameof(claimType));

            ClaimType = claimType;
            RequirementType = requirementType;
        }

        public string ClaimType { get; }
        public ClaimRequirementType RequirementType { get; }
        public object[] Values { get; }
    }
}