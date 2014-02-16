namespace Script.Docks.Mission_Control
{
    partial class Filter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.AddItemButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ItemSelector = new System.Windows.Forms.ComboBox();
            this.SelectedItemNames = new System.Windows.Forms.ListBox();
            this.RemoveItemButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // AddItemButton
            // 
            this.AddItemButton.Location = new System.Drawing.Point(15, 50);
            this.AddItemButton.Name = "AddItemButton";
            this.AddItemButton.Size = new System.Drawing.Size(75, 23);
            this.AddItemButton.TabIndex = 0;
            this.AddItemButton.Text = "Add";
            this.AddItemButton.UseVisualStyleBackColor = true;
            this.AddItemButton.Click += new System.EventHandler(this.AddItemButtonClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(196, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose item or enter part of Item\'s name";
            // 
            // ItemSelector
            // 
            this.ItemSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemSelector.FormattingEnabled = true;
            this.ItemSelector.Location = new System.Drawing.Point(15, 23);
            this.ItemSelector.Name = "ItemSelector";
            this.ItemSelector.Size = new System.Drawing.Size(394, 21);
            this.ItemSelector.TabIndex = 2;
            this.ItemSelector.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ItemSelectorKeyPress);
            // 
            // SelectedItemNames
            // 
            this.SelectedItemNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedItemNames.FormattingEnabled = true;
            this.SelectedItemNames.Location = new System.Drawing.Point(15, 79);
            this.SelectedItemNames.Name = "SelectedItemNames";
            this.SelectedItemNames.Size = new System.Drawing.Size(394, 498);
            this.SelectedItemNames.TabIndex = 3;
            // 
            // RemoveItemButton
            // 
            this.RemoveItemButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.RemoveItemButton.Location = new System.Drawing.Point(15, 583);
            this.RemoveItemButton.Name = "RemoveItemButton";
            this.RemoveItemButton.Size = new System.Drawing.Size(75, 23);
            this.RemoveItemButton.TabIndex = 4;
            this.RemoveItemButton.Text = "Remove";
            this.RemoveItemButton.UseVisualStyleBackColor = true;
            this.RemoveItemButton.Click += new System.EventHandler(this.RemoveItemButtonClick);
            // 
            // Filter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(421, 616);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.RemoveItemButton);
            this.Controls.Add(this.SelectedItemNames);
            this.Controls.Add(this.ItemSelector);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AddItemButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Filter";
            this.ShowInTaskbar = false;
            this.Text = "Filter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddItemButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ItemSelector;
        private System.Windows.Forms.ListBox SelectedItemNames;
        private System.Windows.Forms.Button RemoveItemButton;
    }
}