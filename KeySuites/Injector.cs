using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;
using ElasticsearchCRUD.Model.GeoModel;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

using System.Text;

namespace Vidly
{
    public static class Injector
    {
        public static string SConnection = System.Configuration.ConfigurationManager.ConnectionStrings["KeySuitesCon"].ConnectionString;
        public static string PasswordHash = "sblw-3hn8-sqoy19";
        public static string UserName = "sglz-9hr8-saoy74";
        public static string SecurityStamp = "tbaw-2hw8-hqoy79";
        public static string ConcurrencyStamp = "fgvm-6kr4-saoy94";
        public static string Id = "sglz-9hr8-saoy74";

        public static string Encrypt(string input, string key)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        public static string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}