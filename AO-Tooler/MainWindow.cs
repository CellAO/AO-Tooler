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

namespace AOTooler
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    using AOTooler.Hook;

    using CellAO.Core.Items;

    using Extractor;

    using Script;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Serialization;

    using Utility;

    using WeifenLuo.WinFormsUI.Docking;

    using Message = SmokeLounge.AOtomation.Messaging.Messages.Message;

    #endregion

    /// <summary>
    /// </summary>
    public partial class MainWindow : Form
    {
        #region Static Fields

        /// <summary>
        /// </summary>
        public static bool Pinged;

        /// <summary>
        /// </summary>
        private static Stack<byte[]> localDataStack = new Stack<byte[]>();

        /// <summary>
        /// </summary>
        private static MessageSerializer serializer;

        #endregion

        #region Fields

        /// <summary>
        /// </summary>
        private ScriptCompiler CSC = new ScriptCompiler();

        /// <summary>
        /// </summary>
        private Dictionary<IAOToolerScript, List<N3MessageType>> DockWatch =
            new Dictionary<IAOToolerScript, List<N3MessageType>>();

        /// <summary>
        /// </summary>
        private List<DockContent> dockList = new List<DockContent>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.Text = "AO-Tooler V" + Assembly.GetExecutingAssembly().GetName().Version;
            serializer = new MessageSerializer();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="data">
        /// </param>
        public static void Enqueue(byte[] data)
        {
            lock (localDataStack)
            {
                localDataStack.Push(data);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ConnectTimerTick(object sender, EventArgs e)
        {
            this.ConnectTimer.Enabled = false;
            Process[] processes = Process.GetProcessesByName("anarchyonline");
            if (!processes.Any())
            {
                this.ConnectTimer.Enabled = true;
                return;
            }

            if (AOHook.Inject(processes[0].Id))
            {
                AOData.AOProcess = processes[0];
                this.connectedLabel.Text = "Connected [" + processes[0].Id + "]";
                this.statusLabel.Text = "Connected to Anarchy Online client";
                this.PickupTimer.Enabled = true;
                this.connectionTestTimer.Enabled = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ConnectionTestTimerTick(object sender, EventArgs e)
        {
            if (!Pinged)
            {
                this.connectionTestTimer.Enabled = false;
                this.ConnectTimer.Enabled = true;
                this.statusLabel.Text = "Connection lost";
                this.connectedLabel.Text = "not connected";
            }

            Pinged = false;
        }

        /// <summary>
        /// </summary>
        /// <param name="persistString">
        /// </param>
        /// <returns>
        /// </returns>
        private IDockContent DockCallBack(string persistString)
        {
            foreach (DockContent dc in this.dockList)
            {
                if (dc.GetType().ToString() == persistString)
                {
                    return dc;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ExtractItemsToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = "E:\\AOBeta";
            if (this.folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ItemCollector ic = new ItemCollector(this.folderBrowserDialog1.SelectedPath);
                    this.statusLabel.Text = "Extracting items and their icons";
                    this.Update();

                    ic.CollectItems();
                    this.statusLabel.Text = "Ready...";
                }
                catch (Exception)
                {
                    try
                    {
                        ItemCollector ic =
                            new ItemCollector(
                                Path.Combine(this.folderBrowserDialog1.SelectedPath, "cd_image", "data", "db"));
                        this.statusLabel.Text = "Extracting items and their icons";
                        this.Update();
                        ic.CollectItems();
                        this.statusLabel.Text = "Ready...";
                        return;
                    }
                    catch (Exception e2)
                    {
                        MessageBox.Show(e2.Message);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        private void LoadItemsAndIcons()
        {
            this.statusLabel.Text = "Loading items...";
            this.Update();
            ItemLoader.CacheAllItems("items.dat");
            this.Text = this.Text + " - AO Client " + MessagePackZip.Version;
            ItemIcon.instance.Read("icons.dat");
            PlayfieldList.instance.Read("playfields.dat");
            ItemNames.instance.Read("itemnames.dat");
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MainWindowFormClosing(object sender, FormClosingEventArgs e)
        {
            this.MainDock.SaveAsXml("AO-Tooler.xml");
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MainWindowShown(object sender, EventArgs e)
        {
            bool configFound = false;
            this.CSC.Compile(true);
            this.CSC.AddScriptMembers();
            if (File.Exists("AO-Tooler.xml"))
            {
                foreach (Assembly assembly in this.CSC.multipleDllList)
                {
                    IAOToolerScript dock = ScriptCompiler.RunScript(assembly);
                    this.dockList.AddRange(dock.ReturnDocks());
                }

                DeserializeDockContent ddc = new DeserializeDockContent(this.DockCallBack);

                this.MainDock.LoadFromXml("AO-Tooler.xml", ddc);
                foreach (DockContent dc in this.dockList)
                {
                    dc.Update();
                    ToolStripMenuItem mi = new ToolStripMenuItem(dc.Name);
                    mi.Checked = !dc.IsHidden;
                    mi.Click += this.ShowHide;
                    this.DockMenuItem.DropDown.Items.Add(mi);
                }

                configFound = true;
            }

            if (File.Exists("items.dat") && File.Exists("icons.dat") && File.Exists("playfields.dat")
                && File.Exists("itemnames.dat"))
            {
                this.LoadItemsAndIcons();
            }
            else
            {
                MessageBox.Show("No items/icons found. Please locate your AO folder.");
                this.ExtractItemsToolStripMenuItemClick(null, null);
                this.LoadItemsAndIcons();
            }

            if (!configFound)
            {
                foreach (Assembly assembly in this.CSC.multipleDllList)
                {
                    IAOToolerScript dock = ScriptCompiler.RunScript(assembly);
                    ((IDockContent)dock).DockHandler.Show(this.MainDock, dock.PreferredDockState());
                    this.DockWatch.Add(dock, dock.GetPacketWatcherList());
                }
            }

            this.statusLabel.Text = "Ready...";
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void PickupTimerTick(object sender, EventArgs e)
        {
            this.PickupTimer.Enabled = false;
            byte[][] packets;
            lock (localDataStack)
            {
                packets = localDataStack.ToArray();
                localDataStack.Clear();
            }

            List<Message> pickedUp = new List<Message>();
            foreach (byte[] data in packets)
            {
                try
                {
                    Message ms = serializer.Deserialize(new MemoryStream(data));
                    if (ms != null)
                    {
                        pickedUp.Add(ms);
                    }
                }
                catch (Exception)
                {
                }
            }

            foreach (Message message in pickedUp)
            {
                foreach (KeyValuePair<IAOToolerScript, List<N3MessageType>> dock in this.DockWatch)
                {
                    N3Message n3 = message.Body as N3Message;
                    if (n3 != null)
                    {
                        if (dock.Value.Contains(n3.N3MessageType))
                        {
                            dock.Key.PushPacket(n3.N3MessageType, n3, message);
                        }
                    }
                }
            }

            this.PickupTimer.Enabled = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ShowHide(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = (ToolStripMenuItem)sender;

            foreach (DockContent dock in this.dockList)
            {
                if (dock.Name == tsmi.Text)
                {
                    if (dock.IsHidden)
                    {
                        dock.Show();
                    }
                    else
                    {
                        dock.Hide();
                    }

                    tsmi.Checked = !tsmi.Checked;
                }
            }
        }

        #endregion
    }
}