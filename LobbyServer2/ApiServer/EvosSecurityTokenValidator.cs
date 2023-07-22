using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EvoS.DirectoryServer;
using Microsoft.IdentityModel.Tokens;

namespace CentralServer.ApiServer;

public class EvosSecurityTokenValidator : JwtSecurityTokenHandler, ISecurityTokenValidator
{
    private readonly EvosAuth.Context context;

    public EvosSecurityTokenValidator(EvosAuth.Context context)
    {
        this.context = context;
    }
    
    public override ClaimsPrincipal ValidateToken(
        string token,
        TokenValidationParameters validationParameters,
        [UnscopedRef] out SecurityToken validatedToken)
    {
        return EvosAuth.ValidateToken(context, token, out validatedToken);
    }
}