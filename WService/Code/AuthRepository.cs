using System;
using WService.Models;
using System.Threading.Tasks;

namespace WService.Code
{
    public class AuthRepository : IDisposable
    {
        public async Task<t_oauthtoken> SaveUser(tokenModel model)
        {
            
                using (MEDICFARMAEntities db = new MEDICFARMAEntities())
                {
                    var token = new t_oauthtoken()
                    {
                        date = DateTime.Now,
                        outh_name = model.outh_name,
                        sender_id = model.sender_id,
                    };
                    db.t_oauthtoken.Add(token);
                    await db.SaveChangesAsync();
                    return token;
                }
            
           
        }
        public void Dispose()
        {

        }
    }
}