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

namespace Script.Scripts.Mission_Control
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.IO;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
    using SmokeLounge.AOtomation.Messaging.Serialization;

    using Utility;

    using WeifenLuo.WinFormsUI.Docking;

    using StreamWriter = System.IO.StreamWriter;

    #endregion

    /// <summary>
    /// </summary>
    public partial class MissionControl : DockContent, IAOToolerScript
    {
        #region Fields

        /// <summary>
        /// </summary>
        private int iconCounter = 0;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public MissionControl()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public List<N3MessageType> GetPacketWatcherList()
        {
            List<N3MessageType> types = new List<N3MessageType>(){N3MessageType.GenericCmd} ;
            return types;
        }

        /// <summary>
        /// </summary>
        /// <param name="args">
        /// </param>
        public void Initialize(string[] args)
        {
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public DockState PreferredDockState()
        {
            return DockState.DockRightAutoHide;
        }

        public void PushPacket(N3MessageType type, N3Message message, Message fullMessage)
        {
            GenericCmdMessage cmd = (GenericCmdMessage)message;


            TextWriter tw = new StreamWriter(@"F:\test.txt", true);
            tw.WriteLine(message.GetType().ToString());
            tw.WriteLine("user: " + cmd.User);
            tw.WriteLine("target: " + cmd.Target);
            tw.WriteLine("temp1: " + cmd.Temp1);
            tw.WriteLine("Temp4: " + cmd.Temp4);
            tw.WriteLine("Action: " + cmd.Action);
            tw.WriteLine("Unknown: " + cmd.Unknown);
            tw.WriteLine("tostring: " + cmd.ToString());
            MessageSerializer serializer = new MessageSerializer();
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, (Message)fullMessage);
            byte[] temp = ms.ToArray();
            tw.WriteLine(BitConverter.ToString(temp));
            tw.Close();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void button1_Click(object sender, EventArgs e)
        {
            int a = -1;
            while (a == -1)
            {
                a = ItemIcon.instance.GetRandomIconId();
            }

            this.pictureBox1.Image = ItemIcon.instance.Get(a);
        }

        #endregion
    }
}