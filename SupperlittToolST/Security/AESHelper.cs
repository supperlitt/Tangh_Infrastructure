using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SupperlittTool
{
    /// <summary>
    /// 对称密钥加密，解密
    /// </summary>
    public class AESHelper
    {
        /// <summary>
        /// AES加解密
        /// </summary>
        private static Random rnd = new Random((int)DateTime.Now.ToFileTimeUtc());

        /// <summary>
        /// 有密码的AES加密 默认 CBC PKCS7（java 支持 PKCS5)
        /// </summary>
        /// <param name="text">加密字符</param>
        /// <param name="iv">密钥</param>
        /// <returns></returns>
        public static byte[] AESEncrypt(byte[] text, byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = mode;
            rijndaelCipher.Padding = padding;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = key;
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = iv;
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = text;
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);

            return cipherBytes;
        }

        /// <summary>
        /// AES解密  默认 CBC PKCS7（java 支持 PKCS5)
        /// </summary>
        /// <param name="text"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] AESDecrypt(byte[] text, byte[] key, byte[] iv, CipherMode mode = CipherMode.CBC, PaddingMode padding = PaddingMode.PKCS7)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = mode;
            rijndaelCipher.Padding = padding;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = text;
            byte[] pwdBytes = key;
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length) len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            byte[] ivBytes = iv;
            rijndaelCipher.IV = ivBytes;
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return plainText;
        }

        /// <summary>
        /// 随机生成密钥
        /// </summary>
        /// <param name="n">长度 16/8</param>
        /// <returns></returns>
        public static string GetRandomIV(int n)
        {
            char[] arrChar = new char[]{
               'a','b','d','c','e','f','g','h','i','j','k','l','m','n','p','r','q','s','t','u','v','w','z','y','x',
               '0','1','2','3','4','5','6','7','8','9',
               'A','B','C','D','E','F','G','H','I','J','K','L','M','N','Q','P','R','T','S','V','U','W','X','Y','Z'
            };
            StringBuilder num = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                num.Append(arrChar[rnd.Next(0, arrChar.Length)].ToString());
            }

            return num.ToString();
        }
    }
}
