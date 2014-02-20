using System.Collections.Generic;

namespace Script
{
    using System.IO;


    public class SettingsManager
    {

        private static Dictionary<string,string> settings = new Dictionary<string, string>();

        public static string Get(string dockName, string settingsName)
        {
            if (settings.ContainsKey(dockName + "|" + settingsName))
            {
                return settings[dockName + "|" + settingsName];
            }
            return null;
        }

        public static void Set(string dockName, string settingsName, string value)
        {
            if (settings.ContainsKey(dockName + "|" + settingsName))
            {
                settings.Remove(dockName + "|" + settingsName);
            }
            settings.Add(dockName + "|" + settingsName, value);
        }

        public static void Load(string filename = "AO-Tooler.cfg")
        {
            TextReader tr = new StreamReader(filename);
            string line = "";
            while ((line = tr.ReadLine()) != null)
            {
                string[] parts = line.Split('=');
                settings.Add(parts[0], parts[1]);
            }
            tr.Close();
        }

        public static void Save(string filename = "AO-Tooler.cfg")
        {
            TextWriter tw = new StreamWriter(filename, false);
            foreach (KeyValuePair<string, string> kv in settings)
            {
                tw.WriteLine(kv.Key+"|"+kv.Value);
            }
            tw.Close();
        }
    }
}
