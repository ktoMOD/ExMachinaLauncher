using ExMachinaLauncher.Entities;
using ExMachinaLauncher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ExMachinaLauncher.Services
{
    class LoadModInformationService
    {
        public List<ModEntity> ModEntityList { get; set; }

        public LoadModInformationService()
        {
            ModEntityList = new List<ModEntity>();
        }

        public void CreateModEntityList(string mainPath)
        {
            var modsPath = mainPath + "\\mods";
            CheckGameFolder(mainPath);
            List<string> modFoldersParts = new List<string>(Directory.GetDirectories(modsPath));
            modFoldersParts.Remove(modsPath + "\\BackUp");
            modFoldersParts.Remove(modsPath + "\\ProfileBackUp");

            for (int i = 0; i < modFoldersParts.Count; i++)
            {

                var infoStrings = ParseTxtInfo(modFoldersParts[i]);
                ModEntity modEntity = new ModEntity() { ModParth = modFoldersParts[i], Name = infoStrings[0], Description = infoStrings[1], ImageParth = GetImageSurce(modFoldersParts[i]) };
                ModEntityList.Add(modEntity);
            }
        }

        private void CheckGameFolder(string mainPath)
        {
            var files = Directory.GetFiles(mainPath);
            if (files.Contains(mainPath + "\\start.exe"))
            {
                Directory.CreateDirectory(mainPath + "\\mods");
            }
            else
            {
                ResourceManager rm = new ResourceManager(typeof(Resources));
                InfoWindow infoWindow = new InfoWindow(rm.GetString("noExe"));
                infoWindow.ShowDialog();
                Environment.Exit(0);
            }
        }

        private string[] ParseTxtInfo(string modFolderPart)
        {
            string infoName = string.Empty;
            string infoDescription = string.Empty;
            if (Directory.GetFiles(modFolderPart).FirstOrDefault() != null)
            {
                string[] lines = File.ReadAllLines(modFolderPart + "\\Info.txt", Encoding.GetEncoding(1251));
                foreach (var line in lines)
                {
                    if (line.Contains("Name:"))
                    {
                        infoName = line.Replace("Name: ", string.Empty);
                    }

                    else if (line.Contains("Description:"))
                    {
                        infoDescription = line.Replace("Description: ", string.Empty);
                    }

                    else
                    {
                        infoDescription = infoDescription + Environment.NewLine + line;
                    }
                }
                return new string[] { infoName, infoDescription };
            }
            return new string[] { "No Name", "No Description" };
        }

        private ImageSource GetImageSurce(string modFoldersPart)
        {
            var imageParth = Directory.GetFiles(modFoldersPart, "*.png");
            Uri uri;
            if (imageParth.Length > 0)
            {
                uri = new Uri(imageParth[0], UriKind.Absolute);
            }
            else
            {
                uri = new Uri("pack://application:,,,/InnerResources/NoImage.png");
            }
            return new BitmapImage(uri);
        }

        public string ParseTxtStateInfo(string mainPath)
        {
            var files = Directory.GetFiles(mainPath + "\\mods");
            foreach (var file in files)
            {
                if (file.Contains("StateInfo.txt"))
                {
                    string infoState = string.Empty;
                    string infoModName = string.Empty;
                    string[] lines = File.ReadAllLines(mainPath + "\\mods\\StateInfo.txt", Encoding.GetEncoding(1251));
                    foreach (var line in lines)
                    {
                        if (line.Contains("Mode state:"))
                        {
                            infoState = line.Replace("Mode state: ", string.Empty);
                        }

                        if (line.Contains("Mod Name:"))
                        {
                            infoModName = line.Replace("Mod Name: ", string.Empty);
                        }
                    }
                    if (infoState == "1") return infoModName;
                    if (infoState == "2") return infoModName + "(!!!)";
                    else return "No loaded mods";
                }
            }
            return "No loaded mods";
        }
    }
}
