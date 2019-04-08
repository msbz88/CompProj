namespace CompProj.Views {
    partial class FileForm {
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
            this.textBoxDelimiter = new System.Windows.Forms.TextBox();
            this.labelDelimiter = new System.Windows.Forms.Label();
            this.textBoxHeadersRow = new System.Windows.Forms.TextBox();
            this.labelHeaders = new System.Windows.Forms.Label();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.listViewFileContent = new System.Windows.Forms.ListView();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.labelFileName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxDelimiter
            // 
            this.textBoxDelimiter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDelimiter.Location = new System.Drawing.Point(836, 88);
            this.textBoxDelimiter.Name = "textBoxDelimiter";
            this.textBoxDelimiter.Size = new System.Drawing.Size(123, 20);
            this.textBoxDelimiter.TabIndex = 3;
            // 
            // labelDelimiter
            // 
            this.labelDelimiter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDelimiter.AutoSize = true;
            this.labelDelimiter.Location = new System.Drawing.Point(836, 72);
            this.labelDelimiter.Name = "labelDelimiter";
            this.labelDelimiter.Size = new System.Drawing.Size(47, 13);
            this.labelDelimiter.TabIndex = 2;
            this.labelDelimiter.Text = "Delimiter";
            // 
            // textBoxHeadersRow
            // 
            this.textBoxHeadersRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxHeadersRow.Location = new System.Drawing.Point(836, 130);
            this.textBoxHeadersRow.Name = "textBoxHeadersRow";
            this.textBoxHeadersRow.Size = new System.Drawing.Size(123, 20);
            this.textBoxHeadersRow.TabIndex = 5;
            // 
            // labelHeaders
            // 
            this.labelHeaders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHeaders.AutoSize = true;
            this.labelHeaders.Location = new System.Drawing.Point(836, 116);
            this.labelHeaders.Name = "labelHeaders";
            this.labelHeaders.Size = new System.Drawing.Size(59, 13);
            this.labelHeaders.TabIndex = 4;
            this.labelHeaders.Text = "Headers at";
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxInfo.Location = new System.Drawing.Point(3, 23);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.ReadOnly = true;
            this.richTextBoxInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.richTextBoxInfo.Size = new System.Drawing.Size(827, 10);
            this.richTextBoxInfo.TabIndex = 0;
            this.richTextBoxInfo.TabStop = false;
            this.richTextBoxInfo.Text = "";
            // 
            // listViewFileContent
            // 
            this.listViewFileContent.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFileContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewFileContent.CausesValidation = false;
            this.listViewFileContent.ForeColor = System.Drawing.Color.Black;
            this.listViewFileContent.FullRowSelect = true;
            this.listViewFileContent.GridLines = true;
            this.listViewFileContent.Location = new System.Drawing.Point(3, 37);
            this.listViewFileContent.MinimumSize = new System.Drawing.Size(477, 254);
            this.listViewFileContent.MultiSelect = false;
            this.listViewFileContent.Name = "listViewFileContent";
            this.listViewFileContent.ShowGroups = false;
            this.listViewFileContent.Size = new System.Drawing.Size(827, 303);
            this.listViewFileContent.TabIndex = 1;
            this.listViewFileContent.UseCompatibleStateImageBehavior = false;
            this.listViewFileContent.View = System.Windows.Forms.View.Details;
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonLoad.ForeColor = System.Drawing.Color.Black;
            this.buttonLoad.Location = new System.Drawing.Point(875, 315);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(77, 25);
            this.buttonLoad.TabIndex = 6;
            this.buttonLoad.TabStop = false;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.ButtonLoadClick);
            // 
            // labelFileName
            // 
            this.labelFileName.AutoSize = true;
            this.labelFileName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFileName.Location = new System.Drawing.Point(3, 0);
            this.labelFileName.Name = "labelFileName";
            this.labelFileName.Size = new System.Drawing.Size(0, 20);
            this.labelFileName.TabIndex = 7;
            // 
            // FileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(964, 352);
            this.Controls.Add(this.labelFileName);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.richTextBoxInfo);
            this.Controls.Add(this.listViewFileContent);
            this.Controls.Add(this.labelHeaders);
            this.Controls.Add(this.textBoxHeadersRow);
            this.Controls.Add(this.labelDelimiter);
            this.Controls.Add(this.textBoxDelimiter);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(980, 391);
            this.Name = "FileForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "File Preview";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBoxDelimiter;
        private System.Windows.Forms.Label labelDelimiter;
        private System.Windows.Forms.TextBox textBoxHeadersRow;
        private System.Windows.Forms.Label labelHeaders;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.ListView listViewFileContent;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.Label labelFileName;
    }
}