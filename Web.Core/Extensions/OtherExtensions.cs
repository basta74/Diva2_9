using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Diva2.Core.Extensions
{
   public static class OtherExtensions
    {

        public static string GetMd5Hash(string message, string key) {
            
            string hash;
            

            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);

            HMACMD5 hmacmd5 = new HMACMD5(keyByte);
            HMACSHA1 hmacsha1 = new HMACSHA1(keyByte);

            byte[] messageBytes = encoding.GetBytes(message);
            byte[] hashmessage = hmacmd5.ComputeHash(messageBytes);
            hash = ByteToString(hashmessage);

            return hash;
           
        }
        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                sbinary += buff[i].ToString("X2"); // hex format
            }
            return (sbinary);


        }

    }
}
