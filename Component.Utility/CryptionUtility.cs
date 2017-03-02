using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Enken.Component.Utility
{
    public static class CryptionUtility
    {
        const int ivSize    = 128;
        const int keySize   = 128;
        const int byteSize  = 8;

        //パスワード
        const string password = "aikotoba";

        //ソルト
        const string salt = "oshio";
        
        //IV(初期化ベクタ)
        static readonly byte[] iv;
        //Key
        static readonly byte[] key;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static CryptionUtility()
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var deriveBytes = new Rfc2898DeriveBytes(password, saltBytes);

            deriveBytes.IterationCount = 1000;

            key = deriveBytes.GetBytes(keySize / byteSize);
            iv = deriveBytes.GetBytes(ivSize / byteSize);

        }

        /// <summary>
        /// 文字列暗号化
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            using (var csp = new AesCryptoServiceProvider()
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
            })
            using (var encryptor = csp.CreateEncryptor(key, iv))
            {
                var bytes = Encoding.UTF8.GetBytes(plainText);
                return Convert.ToBase64String(
                    execute(bytes, encryptor));
            }
        }

        /// <summary>
        /// 文字列復号化
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            using (var csp = new AesCryptoServiceProvider()
            {
                Padding = PaddingMode.PKCS7,
                Mode = CipherMode.CBC,
            })
            using (var decryptor = csp.CreateDecryptor(key, iv))
            {
                var bytes = Convert.FromBase64String(cipherText);
                return Encoding.UTF8.GetString(
                    execute(bytes, decryptor));
            }
        }

        static byte[] execute(byte[] src, ICryptoTransform cryptor)
        {
            using (var ms = new MemoryStream())
            {
                CryptoStream cs = null;
                try
                {
                    cs = new CryptoStream(ms, cryptor, CryptoStreamMode.Write);
                    cs.Write(src, 0, src.Length);
                }
                finally
                {
                    cs.Close();
                }
                return ms.ToArray();
            }
        } 
    }
}
