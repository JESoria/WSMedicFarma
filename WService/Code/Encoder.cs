using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WService.Code
{
    public class Encoder
    {
        public static string Encodig(string value)
        {
            value = value + "S3rv1F4M4";
            dynamic hash = System.Security.Cryptography.SHA1.Create();
            dynamic enconder = new System.Text.ASCIIEncoding();
            dynamic combined = enconder.GetBytes(value ?? "");
            return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
        }
    }
}