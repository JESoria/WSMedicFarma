using Microsoft.Owin.Security.OAuth;
using WService.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WService.Code
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {


        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var data = await context.Request.ReadFormAsync();
            tokenModel model = new tokenModel()
            {
                sender_id = data["sender_id"],
                outh_name = data["outh_name"]
            };
            if (model.sender_id == null)
            {
                context.SetError("sender_id", "sender_id es requerido");
                return;
            }
            else if (model.outh_name == null)
            {
                context.SetError("outh_name", "outh_name es requerido");
                return;
            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                using (AuthRepository Repos = new AuthRepository())
                {
                    t_oauthtoken obj_oauthtoken = await Repos.SaveUser(model);
                    identity.AddClaim(new Claim("middlepart", obj_oauthtoken.authtoken_id.ToString()));
                    identity.AddClaim(new Claim("lastpart", obj_oauthtoken.sender_id.ToString()));
                    identity.AddClaim(new Claim("firstpart", obj_oauthtoken.outh_name.ToString()));
                }

                identity.AddClaim(new Claim("role", "user"));
                context.Validated(identity);
        }
    }

}