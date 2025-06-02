using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace MegaClean
{
    public class AppSettings
    {
        public bool EnableDarkMode { get; set; }
        public bool ConfirmBeforeDelete { get; set; }
        public bool AutoScanOnStartup { get; set; }
    }
}
