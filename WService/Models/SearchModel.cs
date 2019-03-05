using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Models
{
    public class SearchModel
    {
        public int idFarmacia { get; set; }
        public string producto { get; set; }
        public double latitud { get; set; }
        public double longitud { get; set; }
        public int idSucursalProducto { get; set; }


        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {

            double theta = Math.Round(lon1, 2) - Math.Round(lon2, 2);
            double dist = Math.Sin(deg2rad(Math.Round(lat1, 2))) * Math.Sin(deg2rad(Math.Round(lat2, 2))) + Math.Cos(deg2rad(Math.Round(lat1, 2))) * Math.Cos(deg2rad(Math.Round(lat2, 2))) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = (dist * 60 * 1.1515) / 0.6213711922;          //miles to kms

            return Math.Round(dist, 2);

        }

        public static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        public static double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
    }
}