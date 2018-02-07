using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Tangh.Infrastructure
{
    /// <summary>
    /// 对称加密算法类，使用系统自带的函数
    /// </summary>
    public class SymmetricMethod
    {
        /// <summary>
        /// 加密系统类
        /// </summary>
        private static SymmetricAlgorithm mobjCryptoService;

        /// <summary>
        /// 密匙
        /// </summary>
        private static string key;

        /// <summary>
        /// 偏移量
        /// </summary>
        private static string stemp;

        /// <summary>
        /// 对称加密类的构造函数
        /// </summary>
        static SymmetricMethod()
        {
            mobjCryptoService = new RijndaelManaged();
            key = "XXzd(%&hj7x89H$faudyou345lj4kl5&fvHUFCy76*h%(HilJ$lhj!y6jklj6%jl54jlk";
            stemp = "W3cda*Ghg7!rNIfb&5jl6ljlkj6445l#er57HBh(u%g6HJ($jhWk7&!&$%jlkjfa";
        }

        /// <summary>
        /// 获得密钥
        /// </summary>
        /// <returns>密钥</returns>
        private static byte[] GetLegalKey()
        {
            string sTemp = key;
            mobjCryptoService.GenerateKey();
            byte[] bytTemp = mobjCryptoService.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        /// <summary>
        /// 获得初始向量IV
        /// </summary>
        /// <returns>初试向量IV</returns>
        private static byte[] GetLegalIV()
        {
            mobjCryptoService.GenerateIV();
            byte[] bytTemp = mobjCryptoService.IV;
            int IVLength = bytTemp.Length;
            if (stemp.Length > IVLength)
                stemp = stemp.Substring(0, IVLength);
            else if (stemp.Length < IVLength)
                stemp = stemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(stemp);
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="source">待加密的串</param>
        /// <returns>经过加密的串</returns>
        private static string Encrypto(string source)
        {
            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(source);
            using (MemoryStream ms = new MemoryStream())
            {
                mobjCryptoService.Key = GetLegalKey();
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateEncryptor();
                using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write))
                {
                    cs.Write(bytIn, 0, bytIn.Length);
                    cs.FlushFinalBlock();
                    cs.Close();

                    ms.Close();
                    byte[] bytOut = ms.ToArray();
                    return Convert.ToBase64String(bytOut);
                }
            }
        }

        /// <summary>
        /// 解密方法
        /// </summary>
        /// <param name="source">待解密的串</param>
        /// <returns>经过解密的串</returns>
        private static string Decrypto(string source)
        {
            byte[] bytIn = Convert.FromBase64String(source);
            using (MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length))
            {
                mobjCryptoService.Key = GetLegalKey();
                mobjCryptoService.IV = GetLegalIV();
                ICryptoTransform encrypto = mobjCryptoService.CreateDecryptor();
                using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 加密字符串：不符合条件的字符串，返回string.Empty
        /// </summary>
        /// <param name="key">待加密的字符串</param>
        /// <returns>加密后的返回值</returns>
        public static string Encrypt(string key)
        {
            try
            {
                return Encrypto(key);
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// 解密字符串：不符合条件的字符串，或者无法解密的字符串返回string.Empty
        /// </summary>
        /// <param name="key">待解密的字符串</param>
        /// <returns>返回解密后的结果</returns>
        public static string Decrypt(string key)
        {
            try
            {
                return Decrypto(key);
            }
            catch
            {
                return key;
            }
        }
    }
}
