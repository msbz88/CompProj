namespace CompProj {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonOpenFiles = new System.Windows.Forms.Button();
            this.listViewInfo = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // buttonOpenFiles
            // 
            this.buttonOpenFiles.Location = new System.Drawing.Point(244, 258);
            this.buttonOpenFiles.Name = "buttonOpenFiles";
            this.buttonOpenFiles.Size = new System.Drawing.Size(75, 23);
            this.buttonOpenFiles.TabIndex = 1;
            this.buttonOpenFiles.Text = "Open Files";
            this.buttonOpenFiles.UseVisualStyleBackColor = true;
            this.buttonOpenFiles.Click += new System.EventHandler(this.ButtonOpenFilesClick);
            // 
            // listViewInfo
            // 
            this.listViewInfo.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewInfo.FullRowSelect = true;
            this.listViewInfo.HoverSelection = true;
            this.listViewInfo.Location = new System.Drawing.Point(4, 6);
            this.listViewInfo.MultiSelect = false;
            this.listViewInfo.Name = "listViewInfo";
            this.listViewInfo.Size = new System.Drawing.Size(327, 285);
            this.listViewInfo.TabIndex = 2;
            this.listViewInfo.TileSize = new System.Drawing.Size(327, 50);
            this.listViewInfo.UseCompatibleStateImageBehavior = false;
            this.listViewInfo.View = System.Windows.Forms.View.Tile;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(335, 323);
            this.Controls.Add(this.buttonOpenFiles);
            this.Controls.Add(this.listViewInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main Form";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonOpenFiles;
        private System.Windows.Forms.ListView listViewInfo;
    }
}

