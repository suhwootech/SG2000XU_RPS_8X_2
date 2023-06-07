using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace SuhwooUtil
{
    public class SuhwooUtils
    {
        public static string EncryptSHA256(string data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }

        static string suhwoolickey1 = "http://www.suhwoo.co.kr/grindsys";
        static string suhwoolickey2 = "http://www,suhwoo.co.kr/grindsys";
        static string suhwoolickey3 = "http://www.suhwoo,co.kr/grindsys";
        static string suhwoolickey4 = "http://www.suhwoo.co,kr/grindsys";

        //AES_256 암호화
        public static bool EncryptAES256(string Input, out string Output)
        {
            //bool ret = false;
            try
            { 
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(suhwoolickey2);
                //new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };            
                aes.IV = Encoding.UTF8.GetBytes("2020072920131215");

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Convert.ToBase64String(xBuff);
                return true;
            }
            catch (Exception err)
            {
                //Console.WriteLine( err.StackTrace);
                Output = Input;
                return false;
            }
        }


        //AES_256 복호화
        public static bool DecryptAES256(string Input, out string Output)
        {
            try
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = Encoding.UTF8.GetBytes(suhwoolickey2);
                //aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                aes.IV = Encoding.UTF8.GetBytes("2020072920131215");

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Encoding.UTF8.GetString(xBuff);
                return true;
            }
            catch
            {
                Output = Input;
                return false;
            }
        }
    }
}
