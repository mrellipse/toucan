// Copyright (c) Nate Barbettini. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Toucan.Contract;
using System.Text;

namespace Toucan.Service
{
    public class TokenProviderService : ITokenProviderService<Token>
    {
        private readonly TokenProviderOptions options;
        private readonly TokenProviderConfig config;

        public TokenProviderService(IOptions<Toucan.Service.TokenProviderConfig> config)
        {
            this.config = config.Value;

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.config.TokenSecurityKey));

            this.options = new TokenProviderOptions
            {
                Audience = this.config.TokenAudience,
                Issuer = this.config.TokenIssuer,
                Expiration = new TimeSpan(0, this.config.TokenExpiration, 0),
                SigningCredentials = new SigningCredentials(signingKey, this.config.TokenSecurityAlgorithm)
            };

            ThrowIfInvalidOptions(this.options);
        }

        public Task<Token> IssueToken(ClaimsIdentity identity, string subject)
        {
            if (identity == null)
                throw new Exception("Invalid username or password.");

            var now = DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:

            var claims = identity.Claims == null ? new List<Claim>() : identity.Claims.ToList();

            if (!string.IsNullOrWhiteSpace(subject) && !claims.Any(o => o.Type == JwtRegisteredClaimNames.Sub))
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: options.Issuer,
                audience: options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(options.Expiration),
                signingCredentials: options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var token = new Token(encodedJwt, jwt.Payload.Exp.Value);

            return Task.FromResult<Token>(token);
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }
    }
}
