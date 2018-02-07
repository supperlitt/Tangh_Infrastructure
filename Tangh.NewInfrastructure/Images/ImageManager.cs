using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;

namespace Tangh.NewInfrastructure.Images
{
    /// <summary>
    /// 图片管理
    /// </summary>
    public class ImageManager
    {
        private static object lockThread = new object();

        private static List<Point> result = new List<Point>();

        private static int width = 0;

        private static int height = 0;

        /// <summary>
        /// 多线程找图【大图片，切割成多个小图，然后多线程比较】
        /// </summary>
        /// <param name="bigImage"></param>
        /// <param name="smallImage"></param>
        /// <returns></returns>
        public static Point ThreadCompare(Bitmap bigImage, Bitmap smallImage)
        {
            // 先拆分大图成为16个小图片，每个小图片都需要加上smallImage的长宽组成一个新图片
            // 需要16个线程来完成。
            width = (int)Math.Ceiling(bigImage.Width / 4.0);
            height = (int)Math.Ceiling(bigImage.Height / 4.0);
            int maxWidth = width + smallImage.Width;
            int maxHeight = +smallImage.Height;
            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Bitmap bitMap = null;
                    if (i == 3 && j == 3)
                    {
                        bitMap = new Bitmap(width, height);
                    }
                    else if (j == 3)
                    {

                    }
                    else if (i == 3)
                    {
                    }
                    else
                    {
                        bitMap = new Bitmap(maxWidth, maxHeight);
                    }

                    Graphics resultG = Graphics.FromImage(bitMap);
                    resultG.DrawImage(bigImage, new Rectangle(i * width, j * height, maxWidth, maxHeight), new Rectangle(0, 0, maxWidth, maxHeight), GraphicsUnit.Pixel);
                    resultG.Dispose();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CompareThread), new object[] { bitMap, CloneImg(smallImage), i, j });
                    index++;
                }
            }

            while (result.Count != 16)
            {
                Thread.Sleep(200);
            }

            var point = new Point(-1, -1);
            if (result.Exists(p => p.X >= 0))
            {
                point = result.Find(a => a.X >= 0);
            }

            return point;
        }

        /// <summary>
        /// 基本色度找图
        /// </summary>
        /// <param name="bigImage"></param>
        /// <param name="smallImage"></param>
        /// <returns></returns>
        public static Point Compare(Bitmap bigImage, Bitmap smallImage)
        {
            for (int i = 0; i < bigImage.Width; i++)
            {
                for (int j = 0; j < bigImage.Height; j++)
                {
                    Color c1 = bigImage.GetPixel(i, j);
                    Color c2 = smallImage.GetPixel(0, 0);

                    // 颜色相等，且没有超出边界
                    if (Compare(c1, c2) && bigImage.Width >= (i + smallImage.Width) && bigImage.Height >= (j + smallImage.Height))
                    {
                        bool iscontinue = false;
                        for (int x = 0; x < smallImage.Width; x++)
                        {
                            for (int y = 0; y < smallImage.Height; y++)
                            {
                                Color c3 = smallImage.GetPixel(x, y);
                                Color c4 = bigImage.GetPixel(i + x, j + y);
                                if (!Compare(c3, c4))
                                {
                                    iscontinue = true;
                                    break;
                                }
                            }

                            if (iscontinue)
                            {
                                break;
                            }
                        }

                        if (!iscontinue)
                        {
                            return new Point(i, j);
                        }
                    }
                }
            }

            return new Point(-1, -1);
        }

        /// <summary>
        /// 内存找图：效率最高
        /// </summary>
        /// <param name="bigImage"></param>
        /// <param name="smallImage"></param>
        /// <returns></returns>
        public static Point ComparePic(Bitmap bigImage, Bitmap smallImage)
        {
            LockBitmap bigMap = new LockBitmap(bigImage);
            LockBitmap smallMap = new LockBitmap(smallImage);
            try
            {
                bigMap.LockBits();
                smallMap.LockBits();
                for (int i = 0; i < bigMap.Width; i++)
                {
                    for (int j = 0; j < bigMap.Height; j++)
                    {
                        Color c1 = bigMap.GetPixel(i, j);
                        Color c2 = smallMap.GetPixel(0, 0);

                        // 颜色相等，且没有超出边界
                        if (Compare(c1, c2) && bigMap.Width >= (i + smallMap.Width) && bigMap.Height >= (j + smallMap.Height))
                        {
                            bool iscontinue = false;
                            for (int x = 0; x < smallMap.Width; x++)
                            {
                                for (int y = 0; y < smallMap.Height; y++)
                                {
                                    Color c3 = smallMap.GetPixel(x, y);
                                    Color c4 = bigMap.GetPixel(i + x, j + y);
                                    if (!Compare(c3, c4))
                                    {
                                        iscontinue = true;
                                        break;
                                    }
                                }

                                if (iscontinue)
                                {
                                    break;
                                }
                            }

                            if (!iscontinue)
                            {
                                return new Point(i, j);
                            }
                        }
                    }
                }

                return new Point(-1, -1);
            }
            finally
            {
                smallMap.UnlockBits();
                bigMap.UnlockBits();
            }
        }

        private static void CompareThread(object obj)
        {
            object[] objs = obj as object[];
            Bitmap bigImage = objs[0] as Bitmap;
            Bitmap smallImage = objs[1] as Bitmap;
            int indexI = Convert.ToInt32(objs[2]);
            int indexJ = Convert.ToInt32(objs[3]);
            bool isbreak = false;
            Point p = new Point(-1, -1);
            for (int i = 0; i < bigImage.Width; i++)
            {
                for (int j = 0; j < bigImage.Height; j++)
                {
                    Color c1 = bigImage.GetPixel(i, j);
                    Color c2 = smallImage.GetPixel(0, 0);

                    // 颜色相等，且没有超出边界
                    if (Compare(c1, c2) && bigImage.Width >= (i + smallImage.Width) && bigImage.Height >= (j + smallImage.Height))
                    {
                        bool iscontinue = false;
                        for (int x = 0; x < smallImage.Width; x++)
                        {
                            for (int y = 0; y < smallImage.Height; y++)
                            {
                                Color c3 = smallImage.GetPixel(x, y);
                                Color c4 = bigImage.GetPixel(i + x, j + y);
                                if (!Compare(c3, c4))
                                {
                                    iscontinue = true;
                                    break;
                                }
                            }

                            if (iscontinue)
                            {
                                break;
                            }
                        }

                        if (!iscontinue)
                        {
                            isbreak = true;
                            p = new Point(i + indexI * width, j + indexJ * height);
                            break;
                        }
                    }
                }

                if (isbreak)
                {
                    break;
                }
            }

            result.Add(p);
        }

        private static bool Compare(Color c1, Color c2)
        {
            if (c1.A == c2.A && c1.R == c2.R && c1.B == c2.B && c1.G == c2.G)
            {
                return true;
            }

            return false;
        }

        private static Bitmap CloneImg(System.Drawing.Image img)
        {
            using (MemoryStream mostream = new MemoryStream())
            {
                Bitmap bmp = new Bitmap(img);
                bmp.Save(mostream, System.Drawing.Imaging.ImageFormat.Jpeg);//将图像以指定的格式存入缓存内存流
                byte[] bt = new byte[mostream.Length];
                mostream.Position = 0;//设置留的初始位置
                mostream.Read(bt, 0, Convert.ToInt32(bt.Length));

                return bmp;
            }
        }
    }
}
