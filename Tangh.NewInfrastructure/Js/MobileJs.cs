using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangh.NewInfrastructure.Js
{
    public class MobileJs
    {
        private static bool is_web = true;

        /// <summary>
        /// 下列站点都是用此方式解密
        /// cmf.cmpay.com
        /// </summary>
        /// <returns></returns>
        private static string Get_Js()
        {
            if (is_web)
            {
                return File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Helper", "mobile.js"), Encoding.UTF8);
            }
            else
            {
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
				
				// 资源路径太长导致不成功，自行处理
                System.IO.StreamReader txtStream = new System.IO.StreamReader(asm.GetManifestResourceStream("Tangh.NewInfrastructure.Js.mobile.js"));

                return txtStream.ReadToEnd();
            }
        }

        /// <summary>
        /// 执行JS
        /// </summary>
        /// <param name="sExpression">参数体</param>
        /// <param name="sCode">JavaScript代码的字符串</param>
        /// <returns></returns>
        private static string ExecuteScript(string sExpression, string sCode)
        {
            MSScriptControl.ScriptControl scriptControl = new MSScriptControl.ScriptControl();
            scriptControl.UseSafeSubset = true;
            scriptControl.Language = "JScript";
            scriptControl.AddCode(sCode);
            try
            {
                string str = scriptControl.Eval(sExpression).ToString();
                return str;
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
            return null;
        }

        public static string encode_mobile(string text)
        {
            string js = Get_Js();

            string function = string.Format("encode_mobile('{0}')", text);
            return ExecuteScript(function, js);
        }

        public static string decode_mobile(string text)
        {
            string js = Get_Js();

            string function = string.Format("decode_mobile('{0}')", text);
            return ExecuteScript(function, js);
        }
    }
}
