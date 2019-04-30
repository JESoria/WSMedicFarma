using System;
using WService.Models;
using System.Threading.Tasks;

namespace WService.Code
{
    public class AuthRepository : IDisposable
    {
        public async Task<T_OAUTHTOKEN> SaveUser(tokenModel model)
        {
            
                using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                {
                    var token = new T_OAUTHTOKEN()
                    {
                        FECHA = DateTime.Now,
                        OUTH_NAME = model.outh_name,
                        SENDER_ID = model.sender_id,
                    };
                    db.T_OAUTHTOKEN.Add(token);
                    await db.SaveChangesAsync();
                    return token;
                }
            
           
        }
        public void Dispose()
        {

        }
    }
}