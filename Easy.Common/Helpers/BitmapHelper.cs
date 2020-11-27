using System;
using System.Drawing;
using System.IO;

namespace Easy.Common.Helpers
{
    public class BitmapHelper
    {
        /// <summary>
        /// base64转bitmap
        /// </summary>
        public Bitmap Base64ToBitmap(string base64)
        {
            byte[] b = Convert.FromBase64String(base64);

            using (MemoryStream ms = new MemoryStream(b))
            {
                Bitmap bitmap = new Bitmap(ms);
                return bitmap;
            }
        }

        /// <summary>
        /// bitmap转base64
        /// </summary>
        public string BitmapToBase64(Bitmap bimap, System.Drawing.Imaging.ImageFormat format)
        {
            var bast64Result = string.Empty;

            using (MemoryStream ms = new MemoryStream())
            {
                bimap.Save(ms, format);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                bast64Result = Convert.ToBase64String(arr);
                ms.Close();
            }

            return bast64Result;
        }

        /// <summary>
        /// 根据原始给定的图片，进行添加文本提示信息的处理（金额文本，提示文本1，提示文本2），并返回处理后图片的base64字符串
        /// </summary>
        /// <param name="originImageBase64">原始图片文件绝对路径</param>
        /// <param name="moneyText">金额文本信息（如：充值金额：1.20 元）</param>
        /// <param name="moneyTextPointY">金额文本的纵坐标</param>
        /// <param name="tip1TextPointY">提示文本1的纵坐标</param>
        /// <param name="tip2TextPointY">提示文本2的纵坐标</param>
        /// <param name="moneyTextEmSize">金额文本的字体大小（默认16）</param>
        /// <param name="moneyTextHtmlColorStr">金额文本的字体颜色（默认#e80c88）</param>
        /// <param name="isMoneyTextBold">金额文本是否粗体（默认粗体）</param>
        /// <param name="tip1Text">提示1文本的文案（默认:请按当前的指定金额充值，否则无法到账）</param>
        /// <param name="tip1TextEmSize">提示1文本的字体大小（默认12）</param>
        /// <param name="tip1TextHtmlColorStr">提示1文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip1TextBold">提示1文本是否粗体（默认粗体）</param>
        /// <param name="tip2Text">提示2文本（默认：小数点后的金额也会到账，谢谢！）</param>
        /// <param name="tip2TextEmSize">提示2文本的字体大小(默认12)</param>
        /// <param name="tip2TextHtmlColorStr">提示2文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip2TextBold">提示2文本是否粗体（默认粗体）</param>
        public string GetNewBitmapBase64(string orginFilePath, string moneyText, int moneyTextPointY, int tip1TextPointY, int tip2TextPointY,
            int moneyTextEmSize = 16, string moneyTextHtmlColorStr = "#e80c88", bool isMoneyTextBold = true,
            string tip1Text = "请按当前的指定金额充值，否则无法到账", int tip1TextEmSize = 12, string tip1TextHtmlColorStr = "#933eea",
            bool isTip1TextBold = true,
            string tip2Text = "小数点后的金额也会到账，谢谢！", int tip2TextEmSize = 12, string tip2TextHtmlColorStr = "#933eea",
            bool isTip2TextBold = true)
        {
            if (!File.Exists(orginFilePath)) throw new Exception("原始图片文件不存在！");

            using (FileStream fs = new FileStream(orginFilePath, FileMode.Open))
            {
                //把文件读取到字节数组 
                byte[] data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
                fs.Close();

                //实例化一个内存流--->把从文件流中读取的内容[字节数组]放到内存流中去 
                MemoryStream ms = new MemoryStream(data);
                return GetNewBitmapBase64(ms, moneyText, moneyTextPointY, tip1TextPointY, tip2TextPointY,
                   moneyTextEmSize, moneyTextHtmlColorStr, isMoneyTextBold,
                   tip1Text, tip1TextEmSize, tip1TextHtmlColorStr,
                   isTip1TextBold,
                   tip2Text, tip2TextEmSize, tip2TextHtmlColorStr,
                   isTip2TextBold);
            }
        }

        /// <summary>
        /// 根据原始给定的图片，进行添加文本提示信息的处理（金额文本，提示文本1，提示文本2），并返回处理后图片的base64字符串
        /// </summary>
        /// <param name="originImageBase64">原始图片的base64码</param>
        /// <param name="moneyText">金额文本信息（如：充值金额：1.20 元）</param>
        /// <param name="moneyTextPointY">金额文本的纵坐标</param>
        /// <param name="tip1TextPointY">提示文本1的纵坐标</param>
        /// <param name="tip2TextPointY">提示文本2的纵坐标</param>
        /// <param name="moneyTextEmSize">金额文本的字体大小（默认16）</param>
        /// <param name="moneyTextHtmlColorStr">金额文本的字体颜色（默认#e80c88）</param>
        /// <param name="isMoneyTextBold">金额文本是否粗体（默认粗体）</param>
        /// <param name="tip1Text">提示1文本的文案（默认:请按当前的指定金额充值，否则无法到账）</param>
        /// <param name="tip1TextEmSize">提示1文本的字体大小（默认12）</param>
        /// <param name="tip1TextHtmlColorStr">提示1文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip1TextBold">提示1文本是否粗体（默认粗体）</param>
        /// <param name="tip2Text">提示2文本（默认：小数点后的金额也会到账，谢谢！）</param>
        /// <param name="tip2TextEmSize">提示2文本的字体大小(默认12)</param>
        /// <param name="tip2TextHtmlColorStr">提示2文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip2TextBold">提示2文本是否粗体（默认粗体）</param>
        public string GetNewBitmapBase64FromBase64(string originImageBase64, string moneyText, int moneyTextPointY, int tip1TextPointY, int tip2TextPointY,
        int moneyTextEmSize = 16, string moneyTextHtmlColorStr = "#e80c88", bool isMoneyTextBold = true,
        string tip1Text = "请按当前的指定金额充值，否则无法到账", int tip1TextEmSize = 12, string tip1TextHtmlColorStr = "#933eea",
        bool isTip1TextBold = true,
        string tip2Text = "小数点后的金额也会到账，谢谢！", int tip2TextEmSize = 12, string tip2TextHtmlColorStr = "#933eea",
        bool isTip2TextBold = true)
        {
            if (string.IsNullOrEmpty(originImageBase64))
            {
                throw new Exception("原始图片的base64码为空！");
            }

            byte[] arr = Convert.FromBase64String(originImageBase64);

            using (MemoryStream ms = new MemoryStream(arr))
            {
                return GetNewBitmapBase64(ms, moneyText, moneyTextPointY, tip1TextPointY, tip2TextPointY,
                   moneyTextEmSize, moneyTextHtmlColorStr, isMoneyTextBold,
                   tip1Text, tip1TextEmSize, tip1TextHtmlColorStr,
                   isTip1TextBold,
                   tip2Text, tip2TextEmSize, tip2TextHtmlColorStr,
                   isTip2TextBold);
            }
        }


        /// <summary>
        /// 根据原始给定的图片，进行添加文本提示信息的处理（金额文本，提示文本1，提示文本2），并返回处理后图片的base64字符串
        /// </summary>
        /// <param name="bitmapStream">原始图片的流</param>
        /// <param name="moneyText">金额文本信息（如：充值金额：1.20 元）</param>
        /// <param name="moneyTextPointY">金额文本的纵坐标</param>
        /// <param name="tip1TextPointY">提示文本1的纵坐标</param>
        /// <param name="tip2TextPointY">提示文本2的纵坐标</param>
        /// <param name="moneyTextEmSize">金额文本的字体大小（默认16）</param>
        /// <param name="moneyTextHtmlColorStr">金额文本的字体颜色（默认#e80c88）</param>
        /// <param name="isMoneyTextBold">金额文本是否粗体（默认粗体）</param>
        /// <param name="tip1Text">提示1文本的文案（默认:请按当前的指定金额充值，否则无法到账）</param>
        /// <param name="tip1TextEmSize">提示1文本的字体大小（默认12）</param>
        /// <param name="tip1TextHtmlColorStr">提示1文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip1TextBold">提示1文本是否粗体（默认粗体）</param>
        /// <param name="tip2Text">提示2文本（默认：小数点后的金额也会到账，谢谢！）</param>
        /// <param name="tip2TextEmSize">提示2文本的字体大小(默认12)</param>
        /// <param name="tip2TextHtmlColorStr">提示2文本的字体颜色（默认#933eea）</param>
        /// <param name="isTip2TextBold">提示2文本是否粗体（默认粗体）</param>
        private string GetNewBitmapBase64(Stream bitmapStream, string moneyText, int moneyTextPointY, int tip1TextPointY, int tip2TextPointY,
            int moneyTextEmSize = 16, string moneyTextHtmlColorStr = "#e80c88", bool isMoneyTextBold = true,
            string tip1Text = "请按当前的指定金额充值，否则无法到账", int tip1TextEmSize = 12, string tip1TextHtmlColorStr = "#933eea",
            bool isTip1TextBold = true,
            string tip2Text = "小数点后的金额也会到账，谢谢！", int tip2TextEmSize = 12, string tip2TextHtmlColorStr = "#933eea",
            bool isTip2TextBold = true)
        {
            string imgBase64 = string.Empty;

            try
            {
                if (null == bitmapStream)
                {
                    throw new Exception("原始图片流为空！");
                }

                Image bitmap = null;
                try
                {
                    //如果不是图片流，即转换会失败，向外层抛出异常
                    bitmap = Bitmap.FromStream(bitmapStream);

                    if (null == bitmap)
                    {
                        throw new Exception("原始图片流为空！");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                //根据image创建graphic处理对象
                Graphics graphicsImage = Graphics.FromImage(bitmap);

                //创建金钱信息文本的字符串输出格式（横向，纵向都居中）
                StringFormat moneyTextStringFormat = new StringFormat();
                moneyTextStringFormat.Alignment = StringAlignment.Center;
                moneyTextStringFormat.LineAlignment = StringAlignment.Center;

                //创建tip1提示文本的字符串输出格式（横向，纵向都居中）
                StringFormat tip1TextStringFormat = new StringFormat();
                tip1TextStringFormat.Alignment = StringAlignment.Center;
                tip1TextStringFormat.LineAlignment = StringAlignment.Center;

                //创建tip1提示文本的字符串输出格式（横向，纵向都居中）
                StringFormat tip2TextStringFormat = new StringFormat();
                tip2TextStringFormat.Alignment = StringAlignment.Center;
                tip2TextStringFormat.LineAlignment = StringAlignment.Center;

                //金额文本信息的字体颜色
                Color moneyTextColor = System.Drawing.ColorTranslator.FromHtml(moneyTextHtmlColorStr);

                //tip1提示文本信息的字体颜色
                Color tip1TextColor = System.Drawing.ColorTranslator.FromHtml(tip1TextHtmlColorStr);

                //tip2提示文本信息的字体颜色
                Color tip2TextColor = System.Drawing.ColorTranslator.FromHtml(tip2TextHtmlColorStr);


                if (!string.IsNullOrEmpty(moneyText))
                {
                    //绘制金钱文本信息
                    int pointMoneyTextX = bitmap.Width / 2;
                    int pointMoneyTextY = moneyTextPointY;

                    graphicsImage.DrawString(moneyText, new Font("arial", moneyTextEmSize, isMoneyTextBold ? FontStyle.Bold : FontStyle.Regular),
                        new SolidBrush(moneyTextColor), new Point(pointMoneyTextX, pointMoneyTextY), moneyTextStringFormat);
                }

                if (!string.IsNullOrEmpty(tip1Text))
                {
                    //绘制tip1文本信息
                    int pointTip1TextX = bitmap.Width / 2;
                    int pointTip1TextY = tip1TextPointY;

                    graphicsImage.DrawString(tip1Text, new Font("Edwardian Script ITC", tip1TextEmSize, isTip1TextBold ? FontStyle.Bold : FontStyle.Regular),
                        new SolidBrush(tip1TextColor), new Point(pointTip1TextX, pointTip1TextY), tip1TextStringFormat);
                }

                if (!string.IsNullOrEmpty(tip2Text))
                {
                    //绘制tip2文本信息
                    int pointTip2TextX = bitmap.Width / 2;
                    int pointTip2TextY = tip2TextPointY;

                    graphicsImage.DrawString(tip2Text, new Font("Edwardian Script ITC", tip2TextEmSize, isTip2TextBold ? FontStyle.Bold : FontStyle.Regular),
                        new SolidBrush(tip2TextColor), new Point(pointTip2TextX, pointTip2TextY), tip2TextStringFormat);
                }

                using (MemoryStream msNewBitmap = new MemoryStream())
                {
                    //System.Drawing.Imaging.ImageFormat.Png
                    bitmap.Save(msNewBitmap, bitmap.RawFormat);
                    byte[] arr = new byte[msNewBitmap.Length];
                    msNewBitmap.Position = 0;
                    msNewBitmap.Read(arr, 0, (int)msNewBitmap.Length);
                    msNewBitmap.Close();
                    imgBase64 = Convert.ToBase64String(arr);
                }
            }
            catch (Exception ee)
            {
                throw ee;
            }

            return imgBase64;
        }

        /// <summary>
        /// 将base64码的图片保存到指定路径(按指定文件名，及图片文件流格式)
        /// </summary>
        /// <param name="imageBase64">图片的64码</param>
        /// <param name="saveImagePath">保存的图片文件绝对路径</param>
        /// <param name="imgFormat">图片流格式</param>
        public void Base64ImageToImageFile(string imageBase64, string saveImagePath, System.Drawing.Imaging.ImageFormat imgFormat)
        {
            //将Base64String转为图片并保存
            byte[] arr = Convert.FromBase64String(imageBase64);
            using (MemoryStream ms = new MemoryStream(arr))
            {
                Bitmap bmp = new Bitmap(ms);
                bmp.Save(saveImagePath, imgFormat);
            }
        }
    }
}