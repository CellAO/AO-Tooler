﻿#region License

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

    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

    using CellAO.Core.Items;

    using Script.Docks.Mission_Control;

    using SmokeLounge.AOtomation.Messaging.GameData;
    using SmokeLounge.AOtomation.Messaging.Messages;
    using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;

    using Utility;

    using WeifenLuo.WinFormsUI.Docking;

    using Message = SmokeLounge.AOtomation.Messaging.Messages.Message;

    #endregion

    /// <summary>
    /// </summary>
    public partial class MissionControl : DockContent, IAOToolerScript
    {
        #region Fields

        /// <summary>
        /// </summary>
        private int iconCounter = 0;

        /// <summary>
        /// </summary>
        private Panel[] panels = new Panel[5];

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
            List<N3MessageType> types = new List<N3MessageType>() { N3MessageType.QuestAlternative };
            return types;
        }

        /// <summary>
        /// </summary>
        /// <param name="args">
        /// </param>
        public void Initialize(string[] args)
        {
            this.panels[0] = this.panel1;
            this.panels[1] = this.panel2;
            this.panels[2] = this.panel3;
            this.panels[3] = this.panel4;
            this.panels[4] = this.panel5;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public DockState PreferredDockState()
        {
            return DockState.Document;
        }

        /// <summary>
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <param name="message">
        /// </param>
        /// <param name="fullMessage">
        /// </param>
        public void PushPacket(N3MessageType type, N3Message message, Message fullMessage)
        {
            QuestAlternativeMessage mes = (QuestAlternativeMessage)message;
            this.ClearOldPanels();
            int i = 0;
            foreach (QuestInfo qi in mes.QuestInfos)
            {
                PictureBox missionIcon = new PictureBox();
                this.panels[i].Controls.Add(missionIcon);
                missionIcon.Left = 5;
                missionIcon.Top = 5;
                missionIcon.Size = new Size(48, 48);
                missionIcon.Image = ItemIcon.instance.Get(qi.MissionIconId);
                Label l1 = new Label
                           {
                               Text = "Location: " + this.GetLocationOfMission(qi),
                               Left = 58,
                               Top = 5,
                               AutoSize = true
                           };
                this.panels[i].Controls.Add(l1);

                int cashfromItems = 0;
                foreach (QuestItemShort qis in qi.ItemRewards)
                {
                    cashfromItems += new Item(qis.Quality,qis.LowId,qis.HighId).GetAttribute(74); // Value stat
                }

                Label l2 = new Label
                           {
                               Left = 58,
                               Top = l1.Top + l1.Height + 5,
                               AutoSize = true,
                               Text = "Cash/Cash from Items/XP: " + qi.CashReward+"/"+cashfromItems + "/" + qi.ExperienceReward
                           };
                this.panels[i].Controls.Add(l2);


                int item = 0;
                foreach (QuestItemShort qis in qi.ItemRewards)
                {
                    PictureBox itemIcon = new PictureBox
                                          {
                                              Size = new Size(48, 48),
                                              Top = 5 + item * 53,
                                              Left = 380,
                                              Image = ItemLoader.ItemList[qis.HighId].GetIcon()
                                          };
                    this.panels[i].Controls.Add(itemIcon);
                    Label l3 = new Label
                               {
                                   Top = itemIcon.Top,
                                   AutoSize = true,
                                   Text = ItemLoader.ItemList[qis.HighId].ItemName + " (QL " + qis.Quality + ")",
                                   Left = 380 + 48 + 5
                               };
                    panels[i].Controls.Add(l3);
                    Label l4 = new Label
                               {
                                   Top = l3.Top + l3.Height + 5,
                                   Text = "Worth: " + ItemLoader.ItemList[qis.HighId].getItemAttribute(61)
                               };
                    item++;
                }

                i++;
            }

            this.AlignPanels();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        private void AlignPanels()
        {
            foreach (Panel p in this.panels)
            {
                p.Height = this.MaxPanelHeight(p) + 5;
            }
        }

        private int MaxPanelHeight(Panel p)
        {
            int maxY = 0;
            foreach (Control c in p.Controls)
            {
                maxY = maxY < c.Top + c.Height ? c.Top + c.Height : maxY;
            }
            return maxY;
        }
        /// <summary>
        /// </summary>
        private void ClearOldPanels()
        {
            foreach (Panel p in this.panels)
            {
                p.Controls.Clear();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="qi">
        /// </param>
        /// <returns>
        /// </returns>
        private string GetLocationOfMission(QuestInfo qi)
        {
            return PlayfieldList.instance.Get(qi.QuestActions[0].Playfield.Instance).Name
                   + string.Format(
                       " ({0}, {1})",
                       qi.QuestActions[0].X.ToString("0.0"),
                       qi.QuestActions[0].Z.ToString("0.0"));
        }

        #endregion

        private Filter filterWindow = null;

        private void MissionControl_DockChanged(object sender, System.EventArgs e)
        {

        }

        private void MissionControl_DockStateChanged(object sender, System.EventArgs e)
        {
            if (filterWindow != null)
            {
                return;
            }
            filterWindow = new Filter();
            filterWindow.DockHandler.Show(this.DockHandler.DockPanel);
            filterWindow.DockState = DockState.DockRightAutoHide;
        }
    }
}