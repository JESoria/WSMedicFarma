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

        [HttpPost]
        public async Task<IHttpActionResult> NearbyBranchOffices(NearbyDrugstoreModel data)
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
                int idFarmacia = data.idFarmacia;
                using (MedicFarmaEntities db = new MedicFarmaEntities())
                {
                    var coord = new GeoCoordinate(data.latitude, data.longitude);
                    List<BranchOfficesModel> allBranchOffices = new List<BranchOfficesModel>();
                    List<BranchOfficesModel> nearby  = new List<BranchOfficesModel>();

                    db.SUCURSAL.OrderBy(x => x.ID_FARMACIA).Where(x => x.ID_FARMACIA == idFarmacia).ToList().ForEach(x =>
                    {
                        allBranchOffices.Add(new BranchOfficesModel() {
                            idSucursal = x.ID_SUCURSAL,
                            sucursal = x.SUCURSAL1,
                            direccion = x.DIRECCION,
                            longitud = x.LONGITUD,
                            latitud = x.LATITUD
                        });
                    });


                    foreach (var x in allBranchOffices)
                    {
                        double lat = Convert.ToDouble(x.latitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double lon = Convert.ToDouble(x.longitud, CultureInfo.CreateSpecificCulture("en-US"));
                        double distance = SearchModel.Distance(data.latitude, data.longitude, lat, lon);
                        if (distance < 2)          //nearbyplaces which are within 2 kms 
                        {
                            BranchOfficesModel offices = new BranchOfficesModel();
                            offices.idSucursal = x.idSucursal;
                            offices.sucursal = x.sucursal;
                            offices.direccion = x.direccion;
                            offices.latitud = x.latitud;
                            offices.longitud = x.longitud;
                            offices.distancia = distance;
                            nearby.Add(offices);
                        }
                    }
                    return Ok(nearby.OrderBy(x => x.distancia));

                }

            }
        }
    }
}