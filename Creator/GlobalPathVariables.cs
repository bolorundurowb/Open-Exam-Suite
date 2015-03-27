using System;
using System.IO;

namespace Creator
{
    public class GlobalPathVariables
    {
        public static const string creatorFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Creator");
        public static const string simulatorFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Simulator");
    }
}
