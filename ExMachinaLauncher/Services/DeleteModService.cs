using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ExMachinaLauncher.Services
{
    class DeleteModService
    {
        public static void CleareData(string mainPath)
        {
            var dataFolderContent = Directory.GetFileSystemEntries(mainPath + "\\data");
            foreach (var fileAndFolder in dataFolderContent)
            {
                if (!(fileAndFolder.Contains(".cfg") || fileAndFolder.Contains("datasources.txt") || fileAndFolder.Contains("config.cfg") || fileAndFolder.Contains(".gdp") || fileAndFolder.Contains("data\\video")))
                {
                    if (!fileAndFolder.Contains("."))
                    {
                        Directory.Delete(fileAndFolder, true);
                    }
                    else File.Delete(fileAndFolder);
                }
            }
            File.Delete(mainPath + "\\mods\\StateInfo.txt");
            DirectoryCopyService.DirectoryCopy(mainPath + "\\mods\\BackUp", mainPath + "\\data");
        }
    }
}
