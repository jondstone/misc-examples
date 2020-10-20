using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DeleteJPGImages
{
    class RunInterface
    {
        string _folderPath = @"path";
        List<string> _jpgFiles = new List<string>();
        List<string> _rawFiles = new List<string>();

        /// <summary>
        /// Method for main interface
        /// </summary>
        public void RunRemovalProgram()
        {
            GetListOfFiles();
            RemoveUnnecessaryJPGFile();

            MessageBox.Show("Program is complete.\nClick OK to open log file");
            Process.Start(Path.Combine(LogWriter.TargetFolder, "DeletedItems.txt"));
        }

        /// <summary>
        /// Gets a list of all files in a given directory.
        /// </summary>
        void GetListOfFiles()
        {
            try
            {
                if (Directory.Exists(_folderPath))
                {
                    string[] allFiles = Directory.GetFiles(_folderPath, "*.*", SearchOption.AllDirectories);
                    string fileOnly = string.Empty;
                    string pathOnly = string.Empty;
                    string fileWithoutExtension = string.Empty;

                    foreach (var file in allFiles)
                    {
                        pathOnly = file.ToLower().Substring(0, file.LastIndexOf(@"\") + 1);
                        fileOnly = file.ToLower().Substring(file.LastIndexOf(@"\")).Replace(@"\", "");
                        if (fileOnly.EndsWith(".jpg"))
                        {
                            fileWithoutExtension = fileOnly.Replace(".jpg", "");
                            _jpgFiles.Add(pathOnly + "," + fileWithoutExtension);
                        }
                        if (fileOnly.EndsWith(".cr2"))
                        {
                            fileWithoutExtension = fileOnly.Replace(".cr2", "");
                            _rawFiles.Add(pathOnly + "," + fileWithoutExtension);
                        }

                        fileWithoutExtension = string.Empty; //clear string
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exceptioned trigger in GetListOfFiles/n{ex.Message}");
            }
        }

        /// <summary>
        /// Deletes JPG file, if a copy of the RAW file exists
        /// </summary>
        void RemoveUnnecessaryJPGFile()
        {
            try
            {
                string fileToDelete = string.Empty;

                foreach (var file in _jpgFiles)
                {
                    var jpgFile = file.Substring(file.LastIndexOf(',')).Replace(",", "");
                    var filePathSplit = file.Substring(0, file.LastIndexOf(@",") + 1);
                    var filePath = filePathSplit.Remove(filePathSplit.Length - 1, 1); //some file paths have a comma, cannot do a replace, so just removes the comma at the end

                    //needs to be an exact match, location and file name
                    var fileExists = _rawFiles.FirstOrDefault(x => x.Equals(file));
                    if (!string.IsNullOrWhiteSpace(fileExists))
                    {
                        fileToDelete = filePath + jpgFile + ".jpg"; 
                        if (File.Exists(fileToDelete))
                        {
                            File.Delete(fileToDelete);
                            LogWriter.LogMessage(DateTime.Now + " " + fileToDelete);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exceptioned trigger in GetListOfFiles\n{ex.Message}");
            }
        }
    }
}