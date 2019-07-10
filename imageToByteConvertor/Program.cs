using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace imageToByteConvertor
{
    class Program
    {
        public static byte[] ImageToASCII(Image img)
        {
            Bitmap bmp = null;
            try
            {
                // Create a bitmap from the image
                bmp = new Bitmap(img);
                byte[] byteArray = new byte[bmp.Height * bmp.Width / 8];

                // Loop through each pixel in the bitmap

                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        // Get the color of the current pixel
                        Color col = bmp.GetPixel(x, y);
                        bool luma = (int)(col.R * 0.3 + col.G * 0.59 + col.B * 0.11) > 127 ? true : false;
                        if (luma == false)
                            byteArray[x / 8 + y * bmp.Width / 8] |= (byte)(1 << (x % 8));
                    }
                }
                return byteArray;
            }
            catch (Exception exc)
            {
                return null;
            }
            finally
            {
                bmp.Dispose();
            }
        }

        public static void PrintByteArrayOnConsole(byte[] byteArray)
        {
            if (byteArray != null)
            {
                for (int i = 0; i < byteArray.Length; i++)
                {
                    if (i % 16 == 0)
                        Console.WriteLine("");
                    Console.Write("0x{0:X2}, ", byteArray[i]);
                }
            }
        }

        static void Main(string[] args)
        {
            ///Image newImage = Image.FromFile("shaparak.bmp");
            //byte[] byteArray = ImageToASCII(newImage);
            string filePathPattern = @"(.*\.)(.*)";
            string newFilePathReplacePattern = @"$1h";
            if (args.Length != 1)
            {
                Console.WriteLine("imageToByteConvertor version 1.00");
                Console.WriteLine("Usage : Convert a bitmap file ascii array of bytes");
                Console.WriteLine("Format: imageToByteConvertor filepath");
                return;
            }
            Image newImage = Image.FromFile(args[0]);
            byte[] byteArray = ImageToASCII(newImage);

            var headerfile = Regex.Replace(args[0], filePathPattern, newFilePathReplacePattern);
            using (StreamWriter header = new StreamWriter(headerfile, false))
            {
                header.WriteLine("const unsigned char logo [] = {");
                    int i = 1;
                    foreach (var eachbyte in byteArray)
                    {
                        string str = string.Format("0x{0:X2}, ", eachbyte);
                        header.Write(str);
                        Console.Write(str);
                        if (i % 16 == 0)
                        {
                            header.Write("\r\n");
                            Console.WriteLine("");
                        }
                        i++;
                    }
                    header.WriteLine("};");
            }
            //PrintByteArrayOnConsole(byteArray);
        }
    }
}
