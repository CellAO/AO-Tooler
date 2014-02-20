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

namespace Script.Docks.Mission_Control
{
    #region Usings ...

    using System;
    using System.Collections.ObjectModel;

    using Script.Attributes;

    using WeifenLuo.WinFormsUI.Docking;

    #endregion

    /// <summary>
    /// </summary>
    public partial class MissionControlSettings : DockContent, IAOToolerScriptSettings
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public MissionControlSettings()
        {
            this.InitializeComponent();
            this.NumberOfRolls = 32;
            this.StringTest = "TESTTEST";
            this.FloatTest = 9173.26618f;
            this.StringList = new ObservableCollection<string>();
            this.StringList.Add("test1");
            this.StringList.Add("Test2");
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        [SettingsValue]
        public float FloatTest
        {
            get
            {
                return float.Parse(this.textBox1.Text);
            }

            set
            {
                this.textBox1.Text = value.ToString("0.0000");
            }
        }

        /// <summary>
        /// </summary>
        [SettingsValue]
        public int NumberOfRolls
        {
            get
            {
                return Convert.ToInt32(this.numericUpDown1.Value);
            }

            set
            {
                this.numericUpDown1.Value = value;
            }
        }

        /// <summary>
        /// </summary>
        [SettingsValue]
        public string StringTest
        {
            get
            {
                return this.textBox2.Text;
            }

            set
            {
                this.textBox2.Text = value;
            }
        }

        [SettingsValue]
        public ObservableCollection<string> StringList { get; set; }

        #endregion
    }
}