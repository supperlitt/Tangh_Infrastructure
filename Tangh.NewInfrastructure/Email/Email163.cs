using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Threading;
namespace Tangh.NewInfrastructure.Email
{
    /// <summary>
    /// 执行操作
    /// </summary>
    public class Email163
    {
        EMailHttpHelper httphelper = new EMailHttpHelper();//封包基础类
        string yzm = string.Empty;

        /// <summary>
        /// 设置cookie
        /// </summary>
        /// <param name="cookies"></param>
        public void SetCurrentCookie(CookieContainer cookies)
        {
            httphelper.CC = cookies;
        }

        #region 邮箱登录-取得邮箱-
        /// <summary>
        /// 邮箱登陆并取得所有邮箱
        /// </summary>
        /// <param name="email">邮箱包含eg: @test.com</param>
        /// <param name="password"></param>
        /// <param name="emailTitle">右边标题</param>
        /// <param name="isContainsRead">是否包含已读</param>
        /// <returns></returns>
        public string LoginAndReadContent(string email, string password, string emailTitle, bool isContainsRead)
        {
            try
            {
                List<EmailFileInfo> list = new List<EmailFileInfo>();
                var suff = email.Split('@')[1];
                Thread.Sleep(500);
                string url = string.Format("http://mail.{0}", suff);
                string result = httphelper.GetAndGetHtml(url, null, null, true, Encoding.UTF8);
                //输入帐号和密码
                string action = string.Format("https://mail.163.com/entry/cgi/ntesdoor?df=mail163_letter&from=web&funcid=loginone&iframe=1&language=-1&passtype=1&product=mail163&net=t&style=-1&race=59_59_161_gz&uid={0}@163.com", email);
                email = email.Split('@')[0];
                string postData = string.Format("username={0}&password={1}&savelogin={2}&url2={3}", email, password, 0, HttpUtility.UrlEncode("http://mail.163.com/errorpage/error163.htm"));
                Thread.Sleep(500);
                string result2 = httphelper.PostAndGetHtml(action, postData, "application/x-www-form-urlencoded", "http://mail.163.com/", true, Encoding.GetEncoding("gbk"));
                Regex regex = new Regex("location.href\\s+=\\s+\"(?<url>[\\s\\S]+?)\"");
                Match url3_match = regex.Match(result2);
                string url3 = url3_match.Groups["url"].Value;
                if (url3.Contains("error"))
                {
                    return string.Empty;
                }

                string result3 = string.Empty;
                if (string.IsNullOrEmpty(url3))
                {
                    // 这种应该是走的验证码逻辑，其实走验证码逻辑可言忽略了
                    /*
                     <form action="https://reg.163.com/login.jsp?username=ysbiao0387&url=http://entry.mail.163.com/coremail/fcg/ntesdoor2" method="post" target="_parent">
            <input value="ysbiao0387" type="hidden" name="username"/>
            <input value="13830620387" type="hidden" name="password"/>
        </form>
                     */
                    Regex actionRegex = new Regex(@"action=""(?<action>[^""]+)");
                    string url3action = actionRegex.Match(result2).Groups["action"].Value;
                    Regex nRegex = new Regex(@"value=""(?<n>[^""]+)""\s+type=""hidden""\s+name=""username""");
                    string n = nRegex.Match(result2).Groups["n"].Value;
                    Regex pRegex = new Regex(@"value=""(?<n>[^""]+)""\s+type=""hidden""\s+name=""password""");
                    string p = pRegex.Match(result2).Groups["n"].Value;

                    result3 = httphelper.PostAndGetHtml(url3action, "username=" + n + "&password=" + p, null, null, true, Encoding.UTF8);
                }
                else
                {
                    result3 = httphelper.GetAndGetHtml(url3, null, null, true, Encoding.GetEncoding("utf-8"));
                }

                Regex regex2 = new Regex("sid=(?<sid>[\\S]+?)&");
                Match rid_match = regex2.Match(url3);
                string rid = rid_match.Groups["sid"].Value;//获取rid参数
                string url4 = "http://mail.163.com/contacts/call.do?uid=" + email + "@163.com&sid=" + rid + "&from=webmail&cmd=newapi.getContacts&vcardver=3.0&ctype=markedcontacts";
                Thread.Sleep(500);
                string result4 = httphelper.GetAndGetHtml(url4, null, null, true, Encoding.GetEncoding("utf-8"));
                //获取邮箱列表
                string now = DateTime.Now.ToString("yyyyMMdd");
                string youjian = string.Format("http://mail.163.com/js6/s?sid={0}&func=mbox:listMessages&LeftNavfolder1Click=1&mbox_folder_enter=1", rid);
                string youjianpost = "var=" + HttpUtility.UrlEncode("<?xml version=\"1.0\"?><object><array name=\"items\"><object><string name=\"func\">mbox:getAllFolders</string><object name=\"var\"><boolean name=\"stats\">true</boolean><boolean name=\"threads\">false</boolean></object></object><object><string name=\"func\">mbox:getFolderStats</string><object name=\"var\"><array name=\"ids\"><string>1,3</string></array><boolean name=\"messages\">true</boolean><boolean name=\"threads\">false</boolean></object></object><object><string name=\"func\">mbox:listTags</string><object name=\"var\"><boolean name=\"stats\">true</boolean><boolean name=\"threads\">false</boolean></object></object><object><string name=\"func\">mbox:statMessages</string><object name=\"var\"><array name=\"fids\"><int>1</int><int>2</int><int>3</int><int>4</int><int>5</int></array><object name=\"filter\"><string name=\"defer\">19700101:</string></object></object></object><object><string name=\"func\">mbox:statMessages</string><object name=\"var\"><array name=\"fids\"><int>1</int><int>2</int><int>3</int><int>4</int><int>5</int></array><object name=\"filter\"><string name=\"defer\">:" + now + "</string></object></object></object></array></object>");
                Thread.Sleep(500);
                string youjianlist = httphelper.PostAndGetHtml(youjian, youjianpost, null, url3, true, Encoding.GetEncoding("utf-8"));
                string subject = emailTitle;
                if (youjianlist != null)
                {
                    list = LoadxmlfromQQEamilFileInfo(youjianlist);
                    if (list != null)
                    {
                        // 读取邮件内容
                        string ID = string.Empty;

                        //筛选可读，指定的邮箱
                        foreach (EmailFileInfo model in list)
                        {
                            if (!isContainsRead && model.flgs.ToString().Contains("true"))
                            {
                                continue;
                            }

                            if (model.subject == subject)
                            {

                                // 得到id
                                string id = model.id;
                                url = string.Format("http://mail.163.com/js6/read/readhtml.jsp?mid={0}&font=15&color=064977", id);
                                Thread.Sleep(500);
                                string content = httphelper.GetAndGetHtml(url, null, null, false, Encoding.UTF8);

                                return content;
                            }
                        }
                    }
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 遍历163邮箱的邮件资源
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<EmailFileInfo> LoadxmlfromQQEamilFileInfo(string result)
        {
            List<EmailFileInfo> list = new List<EmailFileInfo>();
            try
            {
                XmlDocument dom = new XmlDocument();
                dom.LoadXml(result);
                XmlNode gen = dom["result"]["array"];
                XmlNodeList nodelist = gen.ChildNodes;

                foreach (XmlNode nodemodel in nodelist)
                {
                    EmailFileInfo entity = new EmailFileInfo();
                    entity.id = nodemodel.ChildNodes[0].InnerText;
                    entity.fid = int.Parse(nodemodel.ChildNodes[1].InnerText);
                    entity.size = int.Parse(nodemodel.ChildNodes[2].InnerText);
                    entity.from = nodemodel.ChildNodes[3].InnerText;
                    entity.to = nodemodel.ChildNodes[4].InnerText;
                    entity.subject = nodemodel.ChildNodes[5].InnerText;
                    entity.sentDate = DateTime.Parse(nodemodel.ChildNodes[6].InnerText);
                    entity.receivedDate = DateTime.Parse(nodemodel.ChildNodes[7].InnerText);
                    entity.priority = int.Parse(nodemodel.ChildNodes[8].InnerText);
                    entity.backgroundColor = int.Parse(nodemodel.ChildNodes[9].InnerText);
                    entity.antiVirusStatus = nodemodel.ChildNodes[10].InnerText;
                    entity.label0 = int.Parse(nodemodel.ChildNodes[11].InnerText);
                    XmlNode mm = nodemodel.ChildNodes[12];
                    entity.flgs = mm.InnerXml;
                    list.Add(entity);

                }
                return list;
            }
            catch
            {
                return null;

            }
        }
        #endregion
    }

    /// <summary>
    /// email文件的实体类
    /// </summary>
    public class EmailFileInfo
    {
        public string id { set; get; }
        public int fid { set; get; }
        public int size { set; get; }
        public string from { set; get; }
        public string to { set; get; }
        public string subject { set; get; }
        public DateTime sentDate { set; get; }
        public DateTime receivedDate { set; get; }
        public int priority { set; get; }
        public int backgroundColor { set; get; }
        public string antiVirusStatus { set; get; }
        public int label0 { set; get; }
        public object flgs { set; get; }
    }
}
