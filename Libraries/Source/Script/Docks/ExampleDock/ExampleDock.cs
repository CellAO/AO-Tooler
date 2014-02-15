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

namespace Script.Scripts.ExampleDock
{
    #region Usings ...

    using System;
    using System.Collections.Generic;

    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using WeifenLuo.WinFormsUI.Docking;

    #endregion

    /// <summary>
    /// </summary>
    public partial class ExampleDock : DockContent, IAOToolerScript
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public ExampleDock()
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
            List<N3MessageType> types = new List<N3MessageType>() { N3MessageType.CharDCMove };
            return types;
        }

        /// <summary>
        /// </summary>
        /// <param name="args">
        /// </param>
        public void Initialize(string[] args)
        {
            this.textBox1.Text = string.Empty;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public DockState PreferredDockState()
        {
            return DockState.DockLeft;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="message">
        /// </param>
        public void PushPacket(N3MessageType type, N3Message message)
        {
            switch (type)
            {
                case N3MessageType.CharDCMove:
                    CharDCMoveMessage moveMessage = (CharDCMoveMessage)message;
                    this.textBox1.Text = "X: " + moveMessage.Coordinates.X.ToString("0.0") + Environment.NewLine + "Y: "
                                         + moveMessage.Coordinates.Y.ToString("0.0") + Environment.NewLine + "Z: "
                                         + moveMessage.Coordinates.Z.ToString("0.0");
                    break;
            }
        }

        #endregion
    }
}