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

namespace Script.Docks.Mission_Control
{
    #region Usings ...

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Forms;

    using WeifenLuo.WinFormsUI.Docking;

    #endregion

    /// <summary>
    /// </summary>
    public partial class Filter : DockContent
    {
        #region Fields

        /// <summary>
        /// </summary>
        public ObservableCollection<string> selectedItems = null;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public Filter()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void AddItemButtonClick(object sender, EventArgs e)
        {
            if (!this.SelectedItemNames.Items.Contains(this.ItemSelector.Text))
            {
                this.SelectedItemNames.Items.Add(this.ItemSelector.Text);
                this.TransferToList();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ItemSelectorKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                this.AddItemButtonClick(null, null);
                this.ItemSelector.Text = string.Empty;
                e.Handled = true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void RemoveItemButtonClick(object sender, EventArgs e)
        {
            if (this.SelectedItemNames.SelectedIndex >= 0)
            {
                this.SelectedItemNames.Items.RemoveAt(this.SelectedItemNames.SelectedIndex);
                this.TransferToList();
                if (this.SelectedItemNames.Items.Count > 0)
                {
                    this.SelectedItemNames.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// </summary>
        private void TransferToList()
        {
            dontUpdate = true;
            lock (this.selectedItems)
            {
                this.selectedItems.Clear();
                foreach (string s in this.SelectedItemNames.Items)
                {
                    this.selectedItems.Add(s.ToLower());
                }
            }
            dontUpdate = false;
        }

        #endregion

        private bool dontUpdate = false;

        public void SetChangedHandler()
        {
            this.selectedItems.CollectionChanged += selectedItems_CollectionChanged;
        }

        void selectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (dontUpdate)
            {
                return;
            }
            this.SelectedItemNames.Items.Clear();
            foreach (string s in (ObservableCollection<string>)sender)
            {
                this.SelectedItemNames.Items.Add(s);
            }
        }
    }
}