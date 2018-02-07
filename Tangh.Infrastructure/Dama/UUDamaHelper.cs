using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace Tangh.Infrastructure
{
    public class UUDamaHelper
    {
        private int SoftId = 100541;

        private string SoftKey = "94950701daf04ff9944b1437867effb1";

        private string name = string.Empty;

        private string pwd = string.Empty;

        private bool uuLogin = false;

        public UUDamaHelper(string name, string pwd)
        {
            this.name = name;
            this.pwd = pwd;
        }

        public string GetVeryCode(Image img, int codeType)
        {
            string resultCode = string.Empty;
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Jpeg);
            Byte[] buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Flush();
            ms.Close();
            if (!this.uuLogin)
            {
                // 4.5 调用UU.dll开始识别验证码
                Wrapper.uu_setSoftInfo(this.SoftId, this.SoftKey);
                int uuresult = Wrapper.uu_login(this.name, this.pwd);
                if (uuresult <= 0)
                {
                    throw new Exception("登陆UU账号异常！");
                }
                else
                {
                    this.uuLogin = true;
                }
            }

            //新版本dll需要预先分配50个字节的空间，否则dll会崩溃！！！！
            StringBuilder res = new StringBuilder(50);
            string strCheckKey = "286F00F4-3673-4A33-8747-362BE09D5046".ToUpper();
            int codeId = Wrapper.uu_recognizeByCodeTypeAndBytes(buffer, buffer.Length, codeType, res);
            resultCode = CheckResult(res.ToString(), Convert.ToInt32(this.SoftId), codeId, strCheckKey);

            return resultCode;
        }

        private string CheckResult(string result, int softId, int codeId, string checkKey)
        {
            //对验证码结果进行校验，防止dll被替换
            if (string.IsNullOrEmpty(result))
                return result;
            else
            {
                if (result[0] == '-')
                    //服务器返回的是错误代码
                    return result;

                string[] modelReult = result.Split('_');
                //解析出服务器返回的校验结果
                string strServerKey = modelReult[0];
                string strCodeResult = modelReult[1];
                //本地计算校验结果
                string localInfo = softId.ToString() + checkKey + codeId.ToString() + strCodeResult.ToUpper();
                string strLocalKey = MD5Encoding(localInfo).ToUpper();
                //相等则校验通过
                if (strServerKey.Equals(strLocalKey))
                    return strCodeResult;
                return "结果校验不正确";
            }
        }

        private static string MD5Encoding(string rawPass)
        {
            // 创建MD5类的默认实例：MD5CryptoServiceProvider
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(rawPass);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                // 以十六进制格式格式化
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }

    public enum UUType
    {
        /// <summary>
        /// 1-4位数字和英文字母
        /// </summary>
        Word14 = 1004,
    }
}
