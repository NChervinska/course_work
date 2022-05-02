using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace server.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetEmailFromToken(this HttpContext context)
        {
            Claim userClaim = context?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            return userClaim?.Value;
        }
    }

}
