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
        public async Task<T_OAUTHTOKEN> getOauthoken(ClaimsPrincipal principal, string tokenx)
        {
            try
            {
                var firstpart = Convert.ToString(principal.Claims.Where(c => c.Type == "firstpart").Single().Value);
                var lastpart = Convert.ToString(principal.Claims.Where(c => c.Type == "lastpart").Single().Value);
                var middlepart = Convert.ToInt32(principal.Claims.Where(c => c.Type == "middlepart").Single().Value);

                using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                {
                    var token = db.T_OAUTHTOKEN.Where(x => x.AUTHTOKEN_ID == middlepart && x.OUTH_NAME == firstpart && x.SENDER_ID == lastpart);
                    if (token.Count() > 0)
                    {
                        if (token != null)
                        {
                            var xx = token.FirstOrDefault();
                            xx.TOKEN = tokenx;
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
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
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