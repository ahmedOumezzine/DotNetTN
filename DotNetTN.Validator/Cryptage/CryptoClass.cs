using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTN.Common.Cryptage
{
    internal class CryptoClass
    {
        private static string Algor = "Fy5lfsRlh6bBXq3m";
        private static string Key = "e4aFy5lfHrsRlhw6bBmdXqd33mlCtO8E";

        public string Encrypt(string Decryptd)
        {
            byte[] byteEn = ASCIIEncoding.ASCII.GetBytes(Decryptd);
            AesCryptoServiceProvider encod = new AesCryptoServiceProvider();
            encod.BlockSize = 128;
            encod.KeySize = 256;
            encod.Key = ASCIIEncoding.ASCII.GetBytes(Key);
            encod.IV = ASCIIEncoding.ASCII.GetBytes(Algor);
            encod.Padding = PaddingMode.PKCS7;
            encod.Mode = CipherMode.CBC;

            ICryptoTransform trnas = encod.CreateEncryptor(encod.Key, encod.IV);
            byte[] enc = trnas.TransformFinalBlock(byteEn, 0, byteEn.Length);
            trnas.Dispose();
            return Convert.ToBase64String(enc);
        }
    }
}