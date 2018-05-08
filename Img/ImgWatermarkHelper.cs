using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Img
{
    public class ImgHelper
    {
        /// 获取缩略图
        public static Image GetThumbnailImage(Image image, int width, int height)
        {
            if (image == null || width < 1 || height < 1)
                return null;
            // 新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(width, height);

            // 新建一个画板
            using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {

                // 设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                // 设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // 高质量、低速度复合
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // 清空画布并以透明背景色填充
                g.Clear(Color.Transparent);

                // 在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(image, new Rectangle(0, 0, width, height),
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
                return bitmap;
            }
        }
        /// <summary>
        /// 生成缩略图，并保持纵横比
        /// </summary>
        /// <returns>生成缩略图后对象</returns>
        public static Image GetThumbnailImageKeepRatio(Image image, int width, int height)
        {
            Size imageSize = GetImageSize(image, width, height);
            Image newImage = GetThumbnailImage(image, imageSize.Width, imageSize.Height);

            // 新建一个bmp图片
            Image bitmap = new System.Drawing.Bitmap(width, height);

            // 新建一个画板
            using (Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            {
                // 在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(newImage
                    , new Rectangle(width / 2 - newImage.Width / 2, height / 2 - newImage.Height / 2, newImage.Width, newImage.Height)
                    , new Rectangle(0, 0, newImage.Width, newImage.Height),
                    GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        /// <summary>
        /// 根据百分比获取图片的尺寸
        /// </summary>
        public static Size GetImageSize(Image picture, int percent)
        {
            if (picture == null || percent < 1)
                return Size.Empty;

            int width = picture.Width * percent / 100;
            int height = picture.Height * percent / 100;

            return GetImageSize(picture, width, height);
        }
        /// <summary>
        /// 根据设定的大小返回图片的大小，考虑图片长宽的比例问题
        /// </summary>
        public static Size GetImageSize(Image picture, int width, int height)
        {
            if (picture == null || width < 1 || height < 1)
                return Size.Empty;
            Size imageSize;
            imageSize = new Size(width, height);
            double heightRatio = (double)picture.Height / picture.Width;
            double widthRatio = (double)picture.Width / picture.Height;
            int desiredHeight = imageSize.Height;
            int desiredWidth = imageSize.Width;
            imageSize.Height = desiredHeight;
            if (widthRatio > 0)
                imageSize.Width = Convert.ToInt32(imageSize.Height * widthRatio);
            if (imageSize.Width > desiredWidth)
            {
                imageSize.Width = desiredWidth;
                imageSize.Height = Convert.ToInt32(imageSize.Width * heightRatio);
            }
            return imageSize;
        }
        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        public static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType) return ici;
            }
            return null;
        }
        public static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo icf in encoders)
            {
                if (icf.FormatID == format.Guid)
                {
                    return icf;
                }
            }
            return null;
        }
        public static void SaveImage(Image image, string savePath, ImageFormat format)
        {
            SaveImage(image, savePath, GetImageCodecInfo(format));
        }
        /// <summary>
        /// 高质量保存图片
        /// </summary>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            // 设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parms = new EncoderParameters(1);
            EncoderParameter parm = new EncoderParameter(Encoder.Quality, ((long)95));
            parms.Param[0] = parm;
            image.Save(savePath, ici, parms);
            parms.Dispose();
        }

        /// <summary>
        /// 图片叠加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void WaterMark(string imgPath, string logoPath)
        {
            System.Drawing.Image imgSrc = System.Drawing.Image.FromFile(imgPath);
            System.Drawing.Image imgWarter = System.Drawing.Image.FromFile(logoPath);
            using (Graphics g = Graphics.FromImage(imgSrc))
            {
                g.DrawImage(imgWarter, new Rectangle(imgSrc.Width - imgWarter.Width,
                                                 imgSrc.Height - imgWarter.Height,
                                                 imgWarter.Width,
                                                 imgWarter.Height),
                        0, 0, imgWarter.Width, imgWarter.Height, GraphicsUnit.Pixel);
            }
            imgSrc.Save(imgPath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }



        /// <summary>
        /// 截取图片方法
        /// </summary>
        /// <param name="url">图片地址</param>
        /// <param name="beginX">开始位置-X</param>
        /// <param name="beginY">开始位置-Y</param>
        /// <param name="getX">截取宽度</param>
        /// <param name="getY">截取长度</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="fileExt">后缀名</param>
        public static void CutImage(string url, int beginX, int beginY, int getX, int getY, string fileName, string savePath, string fileExt)
        {
            Bitmap bitmap = new Bitmap(url);//原图
            Bitmap destBitmap = new Bitmap(getX, getY);//目标图
            Rectangle destRect = new Rectangle(0, 0, getX, getY);//矩形容器
            Rectangle srcRect = new Rectangle(beginX, beginY, getX, getY);

            Graphics grap = Graphics.FromImage(destBitmap);
            grap.DrawImage(bitmap, destRect, destRect, GraphicsUnit.Pixel);

            ImageFormat format = ImageFormat.Png;
            switch (fileExt.ToLower())
            {
                case "png":
                    format = ImageFormat.Png;
                    break;
                case "bmp":
                    format = ImageFormat.Bmp;
                    break;
                default:
                    format = ImageFormat.Jpeg;
                    break;
            }
            destBitmap.Save(savePath + "//" + fileName, format);

        }



        // 按比例缩放图片
        public static Image ZoomPicture(Image SourceImage, int TargetWidth, int TargetHeight)
        {
            int IntWidth; //新的图片宽
            int IntHeight; //新的图片高
            try
            {
                System.Drawing.Imaging.ImageFormat format = SourceImage.RawFormat;
                System.Drawing.Bitmap SaveImage = new System.Drawing.Bitmap(TargetWidth, TargetHeight);
                Graphics g = Graphics.FromImage(SaveImage);
                g.Clear(Color.White);

                //计算缩放图片的大小 http://www.cnblogs.com/roucheng/

                if (SourceImage.Width > TargetWidth && SourceImage.Height <= TargetHeight)//宽度比目的图片宽度大，长度比目的图片长度小
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height > TargetHeight)//宽度比目的图片宽度小，长度比目的图片长度大
                {
                    IntHeight = TargetHeight;
                    IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                }
                else if (SourceImage.Width <= TargetWidth && SourceImage.Height <= TargetHeight) //长宽比目的图片长宽都小
                {
                    IntHeight = SourceImage.Width;
                    IntWidth = SourceImage.Height;
                }
                else//长宽比目的图片的长宽都大
                {
                    IntWidth = TargetWidth;
                    IntHeight = (IntWidth * SourceImage.Height) / SourceImage.Width;
                    if (IntHeight > TargetHeight)//重新计算
                    {
                        IntHeight = TargetHeight;
                        IntWidth = (IntHeight * SourceImage.Width) / SourceImage.Height;
                    }
                }

                g.DrawImage(SourceImage, (TargetWidth - IntWidth) / 2, (TargetHeight - IntHeight) / 2, IntWidth, IntHeight);
                SourceImage.Dispose();

                return SaveImage;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public static void SetFillPath(System.Drawing.Image imgSrc, Graphics g)
        {
            //设置画布平滑性
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //设置填充画布规格及颜色
            var path = new GraphicsPath();

            int length = imgSrc.Height * 65 / 100;//设置Y轴距离原点的长度
            int graHeight = imgSrc.Height * 85 / 100;//设置水印背景区域的高度
            path.AddLine(0, length, imgSrc.Width, length);
            path.AddLine(imgSrc.Width, length, imgSrc.Width, graHeight);
            path.AddLine(imgSrc.Width, graHeight, 0, graHeight);
            path.AddLine(0, graHeight, 0, length);
            
            //等效果  添加矩形区域
            path.AddRectangle(new Rectangle(0, length, imgSrc.Width, imgSrc.Height * 20 / 100));

            g.FillPath(new SolidBrush(Color.FromArgb(102, 33, 137, 249)), path);
        }


        public static void SetFontBrush(System.Drawing.Image imgSrc, Graphics g, string strDateBrand, string strName, string strAreaDealerName, int brandID)
        {
            //logo信息
            string logoPath = @"E:\public\StarPartnerAdService\Logo\111.png";
            System.Drawing.Image imgWarter = System.Drawing.Image.FromFile(logoPath);

            //top之星图标
            string topPath = @"E:\public\StarPartnerAdService\Logo\topStar.png";
            System.Drawing.Image topWarter = System.Drawing.Image.FromFile(topPath);

            //定位文本位置，height
            int firstMidd, firstRS, secondMidd, secondRS, threwMidd, sizeNo1, sizeNo2;
            int heightMidd = imgSrc.Height * 75 / 100;
            firstMidd = 40;
            firstRS = 40;
            secondMidd = 70;
            secondRS = 15;
            threwMidd = 65;
            sizeNo1 = 20;
            sizeNo2 = 30;

            var font = new Font("方正兰亭准黑简体", sizeNo1, FontStyle.Italic);
            var size1 = g.MeasureString(strDateBrand, font, imgSrc.Width);
            g.DrawString(strDateBrand, font
                , new SolidBrush(Color.White) //设置颜色
                , imgSrc.Width / 2 - size1.Width / 2 + imgWarter.Width / 2 + 20
                , heightMidd - secondMidd / 2 - firstRS - firstMidd / 2);
            g.DrawString("|", new Font("方正兰亭准黑简体", sizeNo1, FontStyle.Regular)
                , new SolidBrush(Color.White) //设置颜色
                , imgSrc.Width / 2 - size1.Width / 2 + imgWarter.Width / 2
                , heightMidd - secondMidd / 2 - firstRS - firstMidd / 2);
            g.DrawImage(imgWarter
                       , new Rectangle(Convert.ToInt32(imgSrc.Width / 2 - (size1.Width + imgWarter.Width) / 2)
                                        , Convert.ToInt32(heightMidd - secondMidd / 2 - firstRS - firstMidd / 2 - (imgWarter.Height - size1.Height) / 2)
                                        , imgWarter.Width
                                        , imgWarter.Height)
                        , 0, 0, imgWarter.Width, imgWarter.Height
                       , GraphicsUnit.Pixel);
            //用完资源释放
            imgWarter.Dispose();
            imgWarter = null;
            font.Dispose();


            //设置名称文本
            font = new Font("方正兰亭大黑简体", sizeNo2, FontStyle.Bold);
            var size2 = g.MeasureString(strName, font);
            g.DrawString(strName, font
                , new SolidBrush(Color.White) //设置颜色
                , imgSrc.Width / 2 - size2.Width / 2
                , heightMidd - size2.Height / 2
                );
            //设置top图标
            g.DrawImage(topWarter
               , new Rectangle(Convert.ToInt32(imgSrc.Width / 2 + size2.Width / 2)
                                , Convert.ToInt32(heightMidd - secondMidd / 2)
                                , topWarter.Width
                                , secondMidd)
                , 0, 0, topWarter.Width, secondMidd
               , GraphicsUnit.Pixel);
            //用完资源释放
            topWarter.Dispose();
            topWarter = null;
            font.Dispose();

            font = new Font("方正兰亭准黑简体", sizeNo2, FontStyle.Regular);
            var size3 = g.MeasureString(strAreaDealerName, font);
            g.DrawString(strAreaDealerName, font
                , new SolidBrush(Color.White) //设置颜色,
                , imgSrc.Width / 2 - size3.Width / 2
                , heightMidd + secondRS + threwMidd / 2);
            //用完资源释放
            font.Dispose();
        }

    }
}
