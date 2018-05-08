using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Img
{

    public class Demo
    {
        public const string savePath = @"E:\public\ImageTest\";
        //1.图片格式化大小拉伸
        public void test1(string path)
        {
            Image image = Image.FromFile(path);
            Image newImage = ImgHelper.GetThumbnailImage(image, 1080, 1920);
            newImage.Save(savePath + @"test1.jpg", ImageFormat.Jpeg);
        }

        //2.图片裁剪
        public void test3(string path)
        {
            Image image = Image.FromFile(path);
            ImgHelper.CutImage(path, 0, 0, 100, 100, "testCutImage.jpg", savePath, ".jpg");
        }
        //3.图片等比例拉伸到固定大小
        public void test3(string path)
        {
            Image image = Image.FromFile(path);
            Image newImage = ImgHelper.GetThumbnailImageKeepRatio(image, 1080, 1920);
            newImage.Save(savePath + @"test3.jpg", ImageFormat.Jpeg);
        }
        //4.图片添加图片水印
        //5.图片添加文字水印
        public void test5(string path)
        {

            Image image = Image.FromFile(path);
            using (Graphics g = Graphics.FromImage(image))
            {
                ImgHelper.SetFontBrush(image, g, "第一行文本", "第二行文本", "测试文本", 111);
                g.Dispose();
            };
            image.Save(savePath + @"test5.jpg", ImageFormat.Jpeg);
        }
        //6.图片添加蒙层
        public void test6(string path)
        {
            Image image = Image.FromFile(path);
            using (Graphics g = Graphics.FromImage(image))
            {
                ImgHelper.SetFillPath(image, g);
                g.Dispose();
            }
            image.Save(savePath + @"test6.jpg", ImageFormat.Jpeg);
        }






    }
}
