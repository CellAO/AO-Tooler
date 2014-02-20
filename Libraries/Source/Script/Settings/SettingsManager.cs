#region License

// Copyright (c) 2005-2014, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

namespace Script.Settings
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Script.Attributes;

    using WeifenLuo.WinFormsUI.Docking;

    #endregion

    /// <summary>
    /// </summary>
    public class SettingsManager
    {
        // Get all classes decorated with IAOToolerScriptSettings 

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="docks">
        /// </param>
        /// <returns>
        /// </returns>
        public Dictionary<string, string> GetSettingsValues(IEnumerable<DockContent> docks)
        {
            List<IAOToolerScriptSettings> settingsDocks = this.GetSettingsDocks(docks);

            Dictionary<string, string> temp = new Dictionary<string, string>();
            foreach (IAOToolerScriptSettings dock in settingsDocks)
            {
                foreach (KeyValuePair<string, string> kv in
                    this.GetValues(dock, this.GetSettingsPropertyInfos(dock.GetType())))
                {
                    temp.Add(kv.Key, kv.Value);
                }
            }

            return temp;
        }

        /// <summary>
        /// </summary>
        /// <param name="docks">
        /// </param>
        /// <param name="filename">
        /// </param>
        public void Load(IEnumerable<DockContent> docks, string filename = "AO-Tooler.cfg")
        {
            if (!File.Exists(filename))
            {
                return;
            }
            List<string> values = new List<string>();
            TextReader tr = new StreamReader(filename);
            string line = string.Empty;
            while ((line = tr.ReadLine()) != null)
            {
                string[] parts = line.Split('=');
                values.Add(line);
            }

            this.SetSettingsValues(docks, values);
        }

        /// <summary>
        /// </summary>
        /// <param name="docks">
        /// </param>
        /// <param name="filename">
        /// </param>
        public void Save(IEnumerable<DockContent> docks, string filename = "AO-Tooler.cfg")
        {
            Dictionary<string, string> settings = this.GetSettingsValues(docks);
            TextWriter tw = new StreamWriter(filename, false);
            foreach (KeyValuePair<string, string> kv in settings)
            {
                tw.WriteLine(kv.Key + "=" + kv.Value);
            }

            tw.Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="docks">
        /// </param>
        /// <returns>
        /// </returns>
        private List<IAOToolerScriptSettings> GetSettingsDocks(IEnumerable<DockContent> docks)
        {
            List<IAOToolerScriptSettings> settingsDocks = new List<IAOToolerScriptSettings>();
            foreach (DockContent dc in docks.Where(x => x is IAOToolerScript))
            {
                if (((IAOToolerScript)dc).GetSettingsDock() != null)
                {
                    settingsDocks.Add((IAOToolerScriptSettings)((IAOToolerScript)dc).GetSettingsDock());
                }
            }

            return settingsDocks;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns>
        /// </returns>
        private IEnumerable<PropertyInfo> GetSettingsPropertyInfos(Type type)
        {
            IEnumerable<PropertyInfo> props =
                type.GetProperties()
                    .Where(x => x.GetCustomAttributes().Any(y => y.GetType() == typeof(SettingsValueAttribute)));

            return props;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private IEnumerable<Type> GetSettingsTypes()
        {
            Type type = typeof(IAOToolerScriptSettings);
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(type.IsAssignableFrom);
        }

        /// <summary>
        /// </summary>
        /// <param name="dock">
        /// </param>
        /// <param name="props">
        /// </param>
        /// <returns>
        /// </returns>
        private Dictionary<string, string> GetValues(IAOToolerScriptSettings dock, IEnumerable<PropertyInfo> props)
        {
            PropertyInfo[] propertyInfos = props as PropertyInfo[] ?? props.ToArray();
            Dictionary<string, string> temp = new Dictionary<string, string>(propertyInfos.Count());

            foreach (PropertyInfo pi in propertyInfos)
            {
                if (pi.PropertyType == typeof(ObservableCollection<string>))
                {
                    int i = 0;
                    foreach (string s in (ObservableCollection<string>)pi.GetValue(dock, null))
                    {
                        temp.Add(dock.GetType().FullName + "|" + pi.Name+";"+i, s);
                        i++;
                    }
                }
                else
                {
                    string val = pi.GetValue(dock, null).ToString();
                    temp.Add(dock.GetType().FullName + "|" + pi.Name, val);
                }
            }

            return temp;
        }

        /// <summary>
        /// </summary>
        /// <param name="docks">
        /// </param>
        /// <param name="values">
        /// </param>
        private void SetSettingsValues(IEnumerable<DockContent> docks, List<string> values)
        {
            List<IAOToolerScriptSettings> settignsDocks = this.GetSettingsDocks(docks);
            foreach (IAOToolerScriptSettings sDock in settignsDocks)
            {
                IEnumerable<PropertyInfo> props = this.GetSettingsPropertyInfos(sDock.GetType());
                foreach (PropertyInfo pi in props)
                {
                    foreach (string kv in values)
                    {
                        string key = GetPropertyName(kv);
                        string idx = GetIndexValue(kv);
                        string value = this.GetValue(kv);

                        if (sDock.GetType().FullName + "|" + pi.Name == key)
                        {
                            if (pi.PropertyType == typeof(int))
                            {
                                pi.SetValue(sDock, int.Parse(value));
                            }

                            if (pi.PropertyType == typeof(string))
                            {
                                pi.SetValue(sDock, value);
                            }

                            if (pi.PropertyType == typeof(float))
                            {
                                pi.SetValue(sDock, float.Parse(value));
                            }

                            if (pi.PropertyType == typeof(ObservableCollection<string>))
                            {
                                ((ObservableCollection<string>)pi.GetValue(sDock, null)).Add(value);
                            }
                        }
                    }
                }
            }
        }

        private string GetIndexValue(string kv)
        {
            string[] parts = kv.Split('=');
            if (parts[0].IndexOf(";") > -1)
            {
                return parts[0].Split(';')[1];
            }
            return "";
        }

        private string GetPropertyName(string kv)
        {
            string[] parts = kv.Split('=');
            if (parts[0].IndexOf(";") > -1)
            {
                return parts[0].Split(';')[0];
            }
            return parts[0];
        }

        private string GetValue(string kv)
        {
            string[] parts = kv.Split('=');
            return parts[1];
        }


        

        #endregion
    }
}