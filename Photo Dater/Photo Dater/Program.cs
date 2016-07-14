using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Photo_Dater
{
    class Program
    {
        private static int count = 0;

        static void Main(string[] args)
        {
            List<string> extensions = new List<string>();
            extensions.Add(".jpg"); extensions.Add(".png"); extensions.Add(".mp4");

            foreach(string item in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (extensions.Contains(Path.GetExtension(item)))
                {
                    RenameItem(item);
                }
            }
            Console.WriteLine("{0} item(s) renamed to proper dates.\nPress any key to continue...", count);
            Console.ReadLine();
        }

        private static void RenameItem(string item)
        {
            string dt = "";
            string ext = Path.GetExtension(item);

            string itemName = Path.GetFileNameWithoutExtension(item);
            string pattern = @"(\d{4})-(\d{2})-(\d{2}) (\d{2}).(\d{2}).(\d{2})";

            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!r.IsMatch(itemName))
            {
                if (ext == ".mp4")
                    dt = GetVideoDateTime(item);
                else
                    dt = GetImageDateTime(item);

                if (dt != "")
                {
                    File.Move(item, Path.GetDirectoryName(item) + "\\" + dt + Path.GetExtension(item));
                    count++;
                }
            }
        }

        private static string GetVideoDateTime(string item)
        {
            DateTime creationDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;
            try
            {
                FileInfo fi = new FileInfo(item);
                creationDate = fi.CreationTime;
                modifiedDate = fi.LastWriteTime;
            }
            catch (Exception ex) { ReportMissingTag(item); }

            if (creationDate.CompareTo(modifiedDate) == -1)
                return creationDate.ToString("yyyy-MM-dd HH.mm.ss");
            else
                return modifiedDate.ToString("yyyy-MM-dd HH.mm.ss");
        }

        private static string GetImageDateTime(string item)
        {
            string dateTime = "";
            Image myImage = null;
            try
            {
                myImage = Image.FromFile(item);
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = new Regex(":").Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                DateTime dt = DateTime.Parse(dateTaken);
                dateTime = dt.ToString("yyyy-MM-dd HH.mm.ss");
                Console.WriteLine(dateTime);
                myImage.Dispose();
            }
            catch (Exception ex)
            {
                ReportMissingTag(item);
                myImage.Dispose();
            }

            return dateTime;
        }

        private static void ReportMissingTag(string item)
        {
            Console.WriteLine("{0} does not have a property - Skipped.", item);
        }
    }
}
