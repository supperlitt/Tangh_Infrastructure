using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tangh.NewInfrastructure.Http;
using System.Text.RegularExpressions;
using System.Web.Security;

namespace Tangh.NewInfrastructure.SMS
{
    /// <summary>
    /// 发送短信帮助类
    /// </summary>
    public class SendSMSHelper
    {
        private HttpHelper helper = null;

        private string sid = string.Empty;

        private string logincguid = string.Empty;

        private string name = string.Empty;

        public SendSMSHelper()
        {
            this.helper = new HttpHelper();
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns>返回登录结果</returns>
        public bool Login(string name, string pwd)
        {
            this.name = name;
            string url = "http://mail.10086.cn/";
            string content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            url = "http://imagecode.mail.10086.cn/getimage?clientid=1&r=" + JsTool.GetRandrom();
            content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            // 头部请求
            url = "http://mail.10086.cn/?rnd=" + JsTool.GetRandrom();
            content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            logincguid = getCGUID();
            // _需要sha1编码name
            string sha1 = FormsAuthentication.HashPasswordForStoringInConfigFile(name, "SHA1").ToLower();
            url = string.Format("https://mail.10086.cn/Login/Login.ashx?_fv=4&cguid={0}&_={1}", logincguid, sha1);
            string postdata = string.Format("UserName={0}&Password={1}&VerifyCode=&=%E7%99%BB+%E5%BD%95&=%E6%B3%A8+%E5%86%8C", name, pwd);
            content = helper.PostAndGetHtml(url, postdata, null, null, true, Encoding.UTF8);

            if (content.Contains("收件箱$inbox.count"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="phone">收件人手机号</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public bool SendSMS(string phone, string smsContent)
        {
            sid = JsTool.GetCookieLike("Os_SSo_", helper.CC);

            // Os_SSO_MTQxNzUwODY2NjAwMTIwODc2OTkzNQAA000007
            sid = JsTool.GetCookie(sid, helper.CC);
            string url = string.Format("http://appmail.mail.10086.cn/m2012/html/index.html?sid={0}&rnd=104&tab=&comefrom=54&cguid={1}&mtime=239", sid, getCGUID());
            string apprefer = url;
            string content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            helper.CC.Add(new System.Net.Cookie()
            {
                Name = "matrix_version",
                Value = "2.0",
                Domain = "mail.10086.cn"
            });

            url = string.Format("http://smsrebuild1.mail.10086.cn/sharpapi/userconfig/service/ajaxhandler.ashx?func=user:adlink&sid={0}&_={1}", sid, JsTool.GetLongFromTime());
            content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            helper.CC.Add(new System.Net.Cookie()
            {
                Name = "matrix_version",
                Value = "2.0",
                Domain = "appmail.mail.10086.cn"
            });

            url = "http://appmail.mail.10086.cn/m2012server/welcome?sid=" + sid;
            content = helper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

            string smsrefer = "http://smsrebuild1.mail.10086.cn//proxy.htm";
            string refer = smsrefer;
            url = string.Format("http://smsrebuild1.mail.10086.cn/addrsvr/GetUserAddrJsonData?sid={0}&formattype=json&&comefrom=54&cguid={1}", sid, getCGUID());
            string postdata = "<GetUserAddrJsonData></GetUserAddrJsonData>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            Regex sdRegex = new Regex(@"""CloseContacts"":\[\{""sd"":""(?<sd>[^""]+)""");
            string sd = sdRegex.Match(content).Groups["sd"].Value;

            url = string.Format("http://smsrebuild1.mail.10086.cn/setting/s?func=disk:getDiskAttConf&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            postdata = "<null />";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);
            refer = apprefer;
            url = string.Format("http://appmail.mail.10086.cn/RmWeb/mail?func=user:moveHOMail&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            postdata = @"<object>
</object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/setting/s?func=umc:getArtifact&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            refer = smsrefer;
            postdata = @"<object>
</object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/setting/s?func=bill:getTypeList&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            postdata = @"<object>
</object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/setting/s?func=user:getOnlineFriends&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            postdata = "<object><int name=\"lineType\">400</int></object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://appmail.mail.10086.cn/mw2/weather/weather?func=user:monitorLogAction&sid={0}", sid);
            refer = apprefer;
            postdata = string.Format("<object><string name=\"version\">m2012</string><array name=\"messages\"><object><string name=\"TIME\">6000</string><string name=\"NAME\">LOGINSUCCESS</string><string name=\"LEVEL\">INFO</string><string name=\"SERVICETIME\">239</string><string name=\"FROMLOGINTIME\">92254</string><string name=\"WELCOMELOADTIME\">96815</string><string name=\"LOGINCGUID\">{0}</string></object></array></object>", logincguid);
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/addrsvr/QueryUserInfo?sid={0}&formattype=json&cguid={1}", sid, getCGUID());
            refer = smsrefer;
            postdata = string.Format("<QueryUserInfo><UserNumber>86{0}</UserNumber></QueryUserInfo>", name);
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = "http://appmail.mail.10086.cn/m2012/html/sms/sms_send.html?sid=" + sid;
            refer = apprefer;
            content = helper.GetAndGetHtml(url, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:getSmsMainData&sid={0}&rnd={1}&cguid={2}", sid, JsTool.GetLongFromTime(), getCGUID());
            refer = smsrefer;
            postdata = "<object><int name=\"serialId\">-1</int><int name=\"dataType\">0</int></object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:getSmsClass&sid={0}&rnd={1}", sid, JsTool.GetLongFromTime());
            postdata = "<object><int name=\"type\">1</int></object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:getSmsClass&sid={0}&rnd={1}", sid, JsTool.GetLongFromTime());
            postdata = "<object><int name=\"type\">1</int></object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            url = string.Format("http://smsrebuild1.mail.10086.cn/together/s?func=user:getFetionLoginInfo&sid={0}&&comefrom=54&cguid={1}", sid, getCGUID());
            postdata = @"<object>
</object>";
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

            helper.CC.Add(new System.Net.Cookie()
            {
                Name = string.Format("isReadUser{0}", sid),
                Value = "0",
                Domain = "10086.cn"
            });

            url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:sendSms&sid={0}&rnd={2}&cguid={1}", sid, getCGUID(), JsTool.GetLongFromTime());
            postdata = string.Format("<object><int name=\"doubleMsg\">0</int><int name=\"submitType\">1</int><string name=\"smsContent\">{0}</string><string name=\"receiverNumber\">86{1}</string><string name=\"comeFrom\">104</string><int name=\"sendType\">0</int><int name=\"smsType\">1</int><int name=\"serialId\">-1</int><int name=\"isShareSms\">0</int><string name=\"sendTime\"></string><string name=\"validImg\"></string><int name=\"groupLength\">10</int><int name=\"isSaveRecord\">1</int></object>", smsContent, phone);
            content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8, Encoding.UTF8);

            // 发送中文内容搞定，原来cmcc的程序员根本没有使用编码，就用的Encoding.Default。日，他们的程序员也太坑爹了，不能高看他们了。
            // 发送的中文有点问题。。。。。短信过去之后是乱码。。。坑  娴嬭瘯鐭俊鍐呭-testcontent
            // { "code": "SMS_CONTENT_VALIDATECODE", "summary":"请输入验证码!", "var":{ "validateUrl":"http:\/\/imagecode.mail.10086.cn\/getimage?clientid=2&88270.4346436345949769", "cacheExist":0 } }
            if (content.Contains("S_OK"))
            {
                // 到上面位置，短信已经成功发送了。
                url = string.Format("http://smsrebuild1.mail.10086.cn/addrsvr/QueryUserInfo?sid={0}&formattype=json&&comefrom=54&cguid={1}", sid, getCGUID());
                postdata = string.Format("<QueryUserInfo><UserNumber>86{0}</UserNumber></QueryUserInfo>", name);
                content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

                url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:success&sid={0}&rnd={1}&cguid={2}", sid, JsTool.GetLongFromTime(), getCGUID());
                postdata = "<object><int name=\"actionId\">0</int><int name=\"pageSize\">3</int><string name=\"mobiles\"></string></object>";
                content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

                url = string.Format("http://smsrebuild1.mail.10086.cn/sharpapi/addr/apiserver/autosavecontact.ashx?sid={0}&r={1}&cguid={2}", sid, JsTool.GetLongFromTime(), getCGUID());
                postdata = string.Format("xml=%3CAddLastContacts%3E%3CAddContactsInfo%3E%3CSerialId%3E{1}%3C%2FSerialId%3E%3CAddrName%3E{0}%3C%2FAddrName%3E%3CAddrType%3EM%3C%2FAddrType%3E%3CAddrContent%3E86{0}%3C%2FAddrContent%3E%3C%2FAddContactsInfo%3E%3C%2FAddLastContacts%3E", phone, sd);
                content = helper.PostAndGetHtml(url, postdata, null, refer, false, Encoding.UTF8);

                url = string.Format("http://smsrebuild1.mail.10086.cn/sms/sms?func=sms:getSmsMainData&sid={0}&rnd={1}&cguid={2}", sid, JsTool.GetLongFromTime(), getCGUID());
                postdata = "<object><int name=\"serialId\">-1</int><int name=\"dataType\">0</int></object>";
                content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

                url = string.Format("http://smsrebuild1.mail.10086.cn/weather/weather?func=user:logBehaviorAction&sid={0}&cguid={1}", sid, getCGUID());
                postdata = @"<object>
  <string name=""version"">m2012</string>
  <array name=""behaviors"">
    <object>
      <string name=""pageId"">24</string>
      <string name=""action"">100366</string>
      <string name=""thingId"">1</string>
      <string name=""module"">14</string>
    </object>
    <object>
      <string name=""pageId"">24</string>
      <string name=""action"">9301</string>
      <string name=""thingId"">2</string>
      <string name=""module"">14</string>
    </object>
  </array>
</object>";
                content = helper.PostAndGetHtml(url, postdata, "application/xml", refer, false, Encoding.UTF8);

                return true;
            }
            else
            {
                return false;
            }
        }

        private string getCGUID()
        {
            var a = DateTime.Now;
            Random rand = new Random(a.Millisecond);
            return DateTime.Now.ToString("HHmmssfff") + rand.Next(1, 10000).ToString().PadLeft(4, '0');
        }
    }

    public class ImageInfo
    {
        public string code
        {
            get;
            set;
        }

        public string summary
        {
            get;
            set;
        }

        public string var
        {
            get;
            set;
        }
    }

    public class VarInfo
    {
        public string validateUrl
        {
            get;
            set;
        }

        public int cacheExist
        {
            get;
            set;
        }
    }
}
