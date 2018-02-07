using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tangh.NewInfrastructure.IMEI
{
    public class IMEITool
    {
        private static Random rand = new Random((int)DateTime.Now.ToFileTimeUtc());
        public static List<ImeiInfo> dataList = new List<ImeiInfo>();
        static IMEITool()
        {
            dataList.Add(new ImeiInfo() { IMEI = "866962026031165", Brand = "Meizu", PhoneModel = "m1 note", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "866624022216485", Brand = "Xiaomi", PhoneModel = "HM NOTE 1S", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "864678021324221", Brand = "Jinli", PhoneModel = "GN151", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "866856027763364", Brand = "Huawei", PhoneModel = "HUAWEI RIO-AL00", Pixel = "1080_1776" });
            dataList.Add(new ImeiInfo() { IMEI = "867271029831921", Brand = "Yijia", PhoneModel = "ONE A2001", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "868588020351131", Brand = "vivo", PhoneModel = "vivo X5M", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "867640021400234", Brand = "Huawei", PhoneModel = "Che2-TL00M", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "868510023861759", Brand = "Jinli", PhoneModel = "F103", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "867650026791446", Brand = "vivo", PhoneModel = "vivo X5S L", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "99000583795163 ", Brand = "SAMSUNG", PhoneModel = "SM-G9209", Pixel = "1440_2560" });
            dataList.Add(new ImeiInfo() { IMEI = "867246024419021", Brand = "Meizu", PhoneModel = "MX5", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "865508021685520", Brand = "TCL", PhoneModel = "TCL P301M", Pixel = "480_800" });
            dataList.Add(new ImeiInfo() { IMEI = "866333028322293", Brand = "Meizu", PhoneModel = "MI 4LTE", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "868586027838746", Brand = "Meizu", PhoneModel = "m2 note", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "866624022216485", Brand = "Xiaomi", PhoneModel = "HM NOTE 1S", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866032026616256", Brand = "Xiaomi", PhoneModel = "HM NOTE 1LTE", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866819029887631", Brand = "vivo", PhoneModel = "vivo X5Max+", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "867113020677889", Brand = "Lenovo", PhoneModel = "Lenovo A5860", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "867625020004822", Brand = "Boway", PhoneModel = "BOWAY_V95Pro", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "99000314793269 ", Brand = "Coolpad", PhoneModel = "Coolpad 5217", Pixel = "480_800" });
            dataList.Add(new ImeiInfo() { IMEI = "866907022104961", Brand = "Jinli", PhoneModel = "F303", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "864961022417297", Brand = "Jinli", PhoneModel = "GN9005", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "867994022743559", Brand = "Xiaomi", PhoneModel = "HM 2A", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "868588020351131", Brand = "vivo", PhoneModel = "vivo X5M", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866856027763364", Brand = "Huawei", PhoneModel = "HUAWEI RIO-AL00", Pixel = "1080_1776" });
            dataList.Add(new ImeiInfo() { IMEI = "868024022854880", Brand = "Meizu", PhoneModel = "m1 metal", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "864513021460578", Brand = "Huawei", PhoneModel = "HUAWEI G730-L075", Pixel = "540_960" });
            dataList.Add(new ImeiInfo() { IMEI = "861999002843655", Brand = "Huawei", PhoneModel = "HUAWEI G520-T10", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "865567022668885", Brand = "OPPO", PhoneModel = "R8007", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "357143049996827", Brand = "Huawei", PhoneModel = "H60-L01", Pixel = "1080_1776" });
            dataList.Add(new ImeiInfo() { IMEI = "358584057015126", Brand = "SAMSUNG", PhoneModel = "SM-N9008V", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "359415050150272", Brand = "Sony", PhoneModel = "L39t", Pixel = "1080_1776" });
            dataList.Add(new ImeiInfo() { IMEI = "868853022873148", Brand = "OPPO", PhoneModel = "OPPO R7 Plus", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "868853028455601", Brand = "OPPO", PhoneModel = "R7Plus", Pixel = "1080_1800" });
            dataList.Add(new ImeiInfo() { IMEI = "865266020565975", Brand = "Lenovo", PhoneModel = "Lenovo A768t", Pixel = "540_960" });
            dataList.Add(new ImeiInfo() { IMEI = "864264022157975", Brand = "Huawei", PhoneModel = "HUAWEI G750-T01", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "868859021993902", Brand = "vivo", PhoneModel = "vivo Y33", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "868617024845515", Brand = "Jinli", PhoneModel = "GN5001", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "868808020229065", Brand = "vivo", PhoneModel = "vivo X6D", Pixel = "1080_1920" });
            dataList.Add(new ImeiInfo() { IMEI = "865852029763727", Brand = "OPPO", PhoneModel = "R6007", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "864590022632157", Brand = "ZTE", PhoneModel = "ZTE Q505T", Pixel = "480_782" });
            dataList.Add(new ImeiInfo() { IMEI = "864678021324221", Brand = "Jinli", PhoneModel = "GN151", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "863777029366720", Brand = "Coolpad", PhoneModel = "Coolpad 8297", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866288025592852", Brand = "Coolpad", PhoneModel = "Coolpad 8297-T01", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866331021851177", Brand = "Huawei", PhoneModel = "CHM-TL00", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "865019027169352", Brand = "Lenovo", PhoneModel = "Lenovo A788t", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "357457044779230", Brand = "Huawei", PhoneModel = "HUAWEI P7-L07", Pixel = "720_1184" });
            dataList.Add(new ImeiInfo() { IMEI = "866789022066451", Brand = "Huawei", PhoneModel = "GEM-703L", Pixel = "1200_1830" });
            dataList.Add(new ImeiInfo() { IMEI = "863473021800052", Brand = "Huawei", PhoneModel = "Che2-TL00M", Pixel = "720_1280" });
            dataList.Add(new ImeiInfo() { IMEI = "866568020930565", Brand = "Huawei", PhoneModel = "H60-L01", Pixel = "1080_1776" });
            dataList.Add(new ImeiInfo() { IMEI = "864837026331322", Brand = "Coolpad", PhoneModel = "Coolpad 8670", Pixel = "540_960" });
            dataList.Add(new ImeiInfo() { IMEI = "866531020025323", Brand = "KOPO", PhoneModel = "KOPO L128", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "868978026735843", Brand = "OPPO", PhoneModel = "OPPO A31", Pixel = "480_854" });
            dataList.Add(new ImeiInfo() { IMEI = "864832020902822", Brand = "Coolpad", PhoneModel = "Coolpad 8690", Pixel = "1080_1776" });
        }

        /// <summary>
        /// 获取随机一个设备
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="imei"></param>
        /// <param name="model"></param>
        /// <param name="pixel"></param>
        public static void GetRandom(ref string imei, ref string model, ref string pixel, ref string brand)
        {
            int key = rand.Next(0, dataList.Count);
            var imeiModel = dataList[key];
            var oldimei = imeiModel.IMEI.Trim().Substring(0, 6);
            int count = 9;
            if (oldimei.StartsWith("9"))
            {
                count = 8;
            }

            for (int i = 0; i < count; i++)
            {
                oldimei += rand.Next(0, 10);
            }

            imei = oldimei;
            model = imeiModel.PhoneModel.Trim();
            pixel = imeiModel.Pixel.Trim();
            brand = imeiModel.Brand.Trim();
        }
    }

    public class ImeiInfo
    {
        /// <summary>
        /// IMEI值
        /// </summary>
        public string IMEI { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 手机型号
        /// </summary>
        public string PhoneModel { get; set; }

        /// <summary>
        /// 分辨率
        /// </summary>
        public string Pixel { get; set; }
    }
}