using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteJPGImages
{
    public static class LogWriter
    {
        public static string TargetFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// Creates and write logfile
        /// </summary>
        /// <param name="message"></param>
        public static void LogMessage(string message)
        {
            using (StreamWriter sw = new StreamWriter(Path.Combine(TargetFolder, "DeletedItems.txt"), true))
            {
                sw.WriteLine(message);
            }
        }
    }
}