using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WService.Models;
using System.Web.Script.Serialization;

namespace WService.Code
{
    public class Helper
    {
        public async Task<t_oauthtoken> getOauthoken(ClaimsPrincipal principal, string tokenx)
        {
            try
            {
                var firstpart = Convert.ToString(principal.Claims.Where(c => c.Type == "firstpart").Single().Value);
                var lastpart = Convert.ToString(principal.Claims.Where(c => c.Type == "lastpart").Single().Value);
                var middlepart = Convert.ToInt32(principal.Claims.Where(c => c.Type == "middlepart").Single().Value);

                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    var token = db.t_oauthtoken.Where(x => x.authtoken_id == middlepart && x.outh_name == firstpart && x.sender_id == lastpart);
                    if (token.Count() > 0)
                    {
                        if (token != null)
                        {
                            var xx = token.FirstOrDefault();
                            xx.token = tokenx;
                            db.Entry(xx).State = System.Data.Entity.EntityState.Modified;
                            await db.SaveChangesAsync();
                        }
                        return token.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string Encodig(string value)
        {
            value = value + "C4t4";
            dynamic hash = System.Security.Cryptography.SHA1.Create();
            dynamic encoder = new System.Text.ASCIIEncoding();
            dynamic combined = encoder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }

        public string GetErrorToString(ErrorModel error)
        {
            var json = new JavaScriptSerializer().Serialize(error);
            return json.ToString();
        }

    }
}