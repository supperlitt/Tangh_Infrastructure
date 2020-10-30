using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCMCCDataSearch
{
    public class MobileJs2
    {
        private static bool is_web = true;

        /// <summary>
        /// 下列站点都是用此方式解密
        /// cmf.cmpay.com
        /// </summary>
        /// <returns></returns>
        private static string Get_Js()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Helper", "mobile.js");
        }

        public static string encode_mobile(string text)
        {
            string js = Get_Js();
            using (var engine = new V8ScriptEngine())
            {
                engine.DocumentSettings.AccessFlags = Microsoft.ClearScript.DocumentAccessFlags.EnableFileLoading;
                engine.DefaultAccess = Microsoft.ClearScript.ScriptAccess.Full; // 这两行是为了允许加载js文件
                // do something

                V8Script script = engine.CompileDocument(js);   // 载入并编译js文件, 然后Execute, 就可以直接调用。
                engine.Execute(script);
                var result = engine.Script.encode_mobile(text);

                return result;
            }
        }

        public static string decode_mobile(string text)
        {
            string js = Get_Js();
            using (var engine = new V8ScriptEngine())
            {
                engine.DocumentSettings.AccessFlags = Microsoft.ClearScript.DocumentAccessFlags.EnableFileLoading;
                engine.DefaultAccess = Microsoft.ClearScript.ScriptAccess.Full; // 这两行是为了允许加载js文件
                // do something

                V8Script script = engine.CompileDocument(js);   // 载入并编译js文件, 然后Execute, 就可以直接调用。
                engine.Execute(script);
                var result = engine.Script.decode_mobile(text);

                return result;
            }
        }
    }
}
