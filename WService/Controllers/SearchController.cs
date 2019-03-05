using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using WService.Code;
using WService.Models;

namespace WService.Controllers
{
    [Authorize]
    public class SearchController : ApiController
    {
        private DataAccess da = new DataAccess();

        [HttpPost]
        public async Task<IHttpActionResult> DrugstoresList()
        {
            IEnumerable<string> headerValues = Request.Headers.GetValues("Authorization");
            string header = headerValues.FirstOrDefault();
            t_oauthtoken token = await da.getOauthoken(Request.GetRequestContext().Principal as ClaimsPrincipal, header);

            if (token == null)
            {
                return BadRequest();
            }
            else
            {
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    List<DrugstoresModel> list = new List<DrugstoresModel>();
                    db.FARMACIA.OrderBy(x => x.ID_FARMACIA).ToList().ForEach(x => {
                        //MemoryStream ms = new MemoryStream(x.IMAGEN);
                        //Image image = Image.FromStream(ms);

                        list.Add(new DrugstoresModel()
                        {
                            idFarmacia = x.ID_FARMACIA,
                            NombreFarmacia = x.FARMACIA1,
                            //imagenFarmacia = image
                        });
                    });
                    return Ok(list);
                }
            }
        }
    }
}