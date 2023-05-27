using ImageProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace JigsawPuzzle.Wpf
{
    public static class ImageUtil
    {
        /// <summary>
        /// 将Image类型转换成BitmapImage类型
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this Image image)
        {
            var bitmap = new Bitmap(image);
            return bitmap.ToBitmapImage();
        }
        /// <summary>
        /// 将Image类型转换成BitmapImage类型
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
        /// <summary>
        /// 将图片转换成宫格且第一格是白图
        /// </summary>
        /// <param name="image">图片实例</param>
        /// <param name="count">宫格数</param>
        /// <returns></returns>
        public static List<Image> ToGridImages(this Image image, int count = 3)
        {
            List<Image> images = new List<Image>();
            int size = image.Width / count;
            using (ImageFactory imageFactory = new ImageFactory(true))
            {
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < count; j++)
                    {
                        var rectangle = new Rectangle(j * size, i * size, size, size);
                        imageFactory.Load(image);
                        images.Add((Image)imageFactory.Crop(rectangle).Image.Clone());
                    }
                }
                using (Graphics g = Graphics.FromImage(images[0]))
                {
                    g.Clear(Color.White);
                }
            }
            return images;
        }
        /// <summary>
        /// 将宫格图拼接成一张图
        /// </summary>
        /// <param name="images"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static BitmapImage GridToBitmapImage(this List<Image> images, int[,] map)
        {
            // 初始化画布(最终的拼图画布)并设置宽高
            Bitmap bitMap = new Bitmap(500, 500);
            // 初始化画板
            Graphics g1 = Graphics.FromImage(bitMap);
            // 将画布涂为白色(底部颜色可自行设置)
            g1.FillRectangle(Brushes.White, new Rectangle(0, 0, 500, 500));
            int mapLength = map.GetLength(0);
            int nextX = 0;
            int nextY = 0;
            for (int y = 0; y < mapLength; y++)
            {
                int currentHeight = 0;
                for (int x = 0; x < mapLength; x++)
                {
                    Image image = images[map[x, y]];
                    g1.DrawImage(image, nextX, nextY, image.Width, image.Height);
                    nextX += image.Width;
                    currentHeight = image.Height;
                }
                nextX = 0;
                nextY += currentHeight;
            }
            return bitMap.ToBitmapImage();
        }
        /// <summary>
        /// 将图片裁剪成正方形后调整大小
        /// </summary>
        /// <param name="image"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static ImageFactory CropAndResize(this ImageFactory image, int size)
        {
            int width = image.Image.Width;
            int height = image.Image.Height;
            Rectangle rectangle;
            if (width > height)
            {
                rectangle = new Rectangle((width - height) / 2, 0, height, height);
            }
            else
            {
                rectangle = new Rectangle(0, (height - width) / 2, height, height);
            }
            return image.Crop(rectangle).Resize(new Size(size, size));
        }
        /// <summary>
        /// 从文件路径加载一个图片
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image LoadImgByFilePath(string path, int size = 500)
        {
            using (ImageFactory imageFactory = new ImageFactory(true))
            {
                try
                {
                    // 加载，调整大小，设置格式和质量并保存图像。
                    return (Image)imageFactory.Load(path)
                                       .CropAndResize(size)
                                       .Image.Clone();
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
