using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dmMoWizz.Models.ViewModels
{
    public class ImageViewModel
    {
        public string FilePath { get; set; }
        public string AspectRatio { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }
}