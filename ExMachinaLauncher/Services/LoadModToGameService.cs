using ExMachinaLauncher.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExMachinaLauncher.Services
{
    public class LoadModToGameService
    {
        public LoadModToGameService()
        {
        }

        internal void Load(ModEntity modEntity, string mainPath)
        {
            DirectoryCopyService.DirectoryCopy(modEntity.ModParth + "\\data", mainPath + "\\data");
        }
    }
}
