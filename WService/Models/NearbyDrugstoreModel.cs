using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class NearbyDrugstoreModel
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int idFarmacia { get; set; }
    }
}