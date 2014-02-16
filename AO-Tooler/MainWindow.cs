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
        private Dictionary<int, byte[]> iconDict = new Dictionary<int, byte[]>();

        /// <summary>
        /// </summary>
        private List<ItemTemplate> itemList = new List<ItemTemplate>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
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
                this.connectedLabel.Text = "Connected [" + processes[0].Id + "]";
                this.statusLabel.Text = "Connected to Anarchy Online client";
                this.PickupTimer.Enabled = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MainWindowShown(object sender, EventArgs e)
        {
            this.CSC.Compile(true);
            this.CSC.AddScriptMembers();
            foreach (Assembly assembly in this.CSC.multipleDllList)
            {
                IAOToolerScript dock = ScriptCompiler.RunScript(assembly);
                ((IDockContent)dock).DockHandler.Show(this.MainDock, dock.PreferredDockState());
                this.DockWatch.Add(dock, dock.GetPacketWatcherList());
            }

            if (File.Exists("items.dat") && File.Exists("icons.dat") && File.Exists("playfields.dat"))
            {
                this.LoadItemsAndIcons();
            }
            else
            {
                MessageBox.Show("No items/icons found. Please locate your AO folder.");
                this.extractItemsToolStripMenuItem_Click(null, null);
                this.LoadItemsAndIcons();
            }

            this.statusLabel.Text = "Ready...";
        }

        private void LoadItemsAndIcons()
        {
            this.statusLabel.Text = "Loading items...";
            this.Update();
            ItemLoader.CacheAllItems("items.dat");
            ItemIcon.instance.Read("icons.dat");
            PlayfieldList.instance.Read("playfields.dat");

        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void PickupTimer_Tick(object sender, EventArgs e)
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
        private void extractItemsToolStripMenuItem_Click(object sender, EventArgs e)
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
                catch (Exception ee)
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
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        #endregion
    }
}