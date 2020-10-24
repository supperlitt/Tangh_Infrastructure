using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangh.NewInfrastructure
{
    public class CreateImage
    {
        public static byte[] CreatePicture(string text, int width, int height)
        {
            String[] array = text.Split(new char[] { '\n' }, StringSplitOptions.None);
            if (array.Length > 1)
            {
                using (Bitmap img = new Bitmap(width, height))
                {
                    // _code = GetRandomCode();
                    // System.Web.HttpContext.Current.Session["vcode"] = _code;
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.Clear(Color.White);//绘画背景颜色
                        g.DrawString(array[0], new Font("宋体", 14, FontStyle.Bold), Brushes.DarkRed, 5, 25);
                        for (int i = 1; i < array.Length; i++)
                        {
                            g.DrawString(array[0], new Font("宋体", 14, FontStyle.Bold), Brushes.DarkRed, 5, 25 + i * 20);
                        }
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Jpeg);

                        return ms.ToArray();
                    }
                }
            }
            else
            {
                using (Bitmap img = new Bitmap(width, height))
                {
                    // _code = GetRandomCode();
                    // System.Web.HttpContext.Current.Session["vcode"] = _code;
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        g.Clear(Color.White);//绘画背景颜色
                        g.DrawString(array[0], new Font("宋体", 14, FontStyle.Bold), Brushes.Red, 5, 25);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Jpeg);

                        return ms.ToArray();
                    }
                }
            }
        }
    }
}
