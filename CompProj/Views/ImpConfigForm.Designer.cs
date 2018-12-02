namespace CompProj.Views {
    partial class ImpConfigForm {
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
            this.richTextBoxFileContent = new System.Windows.Forms.RichTextBox();
            this.textBoxDelimiter = new System.Windows.Forms.TextBox();
            this.labelDelimiter = new System.Windows.Forms.Label();
            this.textBoxSkipRows = new System.Windows.Forms.TextBox();
            this.textBoxHeadersRow = new System.Windows.Forms.TextBox();
            this.labelSkipRows = new System.Windows.Forms.Label();
            this.labelHeaders = new System.Windows.Forms.Label();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // richTextBoxFileContent
            // 
            this.richTextBoxFileContent.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxFileContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxFileContent.Location = new System.Drawing.Point(3, 2);
            this.richTextBoxFileContent.Name = "richTextBoxFileContent";
            this.richTextBoxFileContent.ReadOnly = true;
            this.richTextBoxFileContent.Size = new System.Drawing.Size(435, 267);
            this.richTextBoxFileContent.TabIndex = 0;
            this.richTextBoxFileContent.Text = "";
            // 
            // textBoxDelimiter
            // 
            this.textBoxDelimiter.Location = new System.Drawing.Point(444, 40);
            this.textBoxDelimiter.Name = "textBoxDelimiter";
            this.textBoxDelimiter.Size = new System.Drawing.Size(162, 20);
            this.textBoxDelimiter.TabIndex = 1;
            // 
            // labelDelimiter
            // 
            this.labelDelimiter.AutoSize = true;
            this.labelDelimiter.Location = new System.Drawing.Point(444, 24);
            this.labelDelimiter.Name = "labelDelimiter";
            this.labelDelimiter.Size = new System.Drawing.Size(47, 13);
            this.labelDelimiter.TabIndex = 2;
            this.labelDelimiter.Text = "Delimiter";
            // 
            // textBoxSkipRows
            // 
            this.textBoxSkipRows.Location = new System.Drawing.Point(444, 79);
            this.textBoxSkipRows.Name = "textBoxSkipRows";
            this.textBoxSkipRows.Size = new System.Drawing.Size(162, 20);
            this.textBoxSkipRows.TabIndex = 3;
            // 
            // textBoxHeadersRow
            // 
            this.textBoxHeadersRow.Location = new System.Drawing.Point(444, 116);
            this.textBoxHeadersRow.Name = "textBoxHeadersRow";
            this.textBoxHeadersRow.Size = new System.Drawing.Size(162, 20);
            this.textBoxHeadersRow.TabIndex = 4;
            // 
            // labelSkipRows
            // 
            this.labelSkipRows.AutoSize = true;
            this.labelSkipRows.Location = new System.Drawing.Point(444, 63);
            this.labelSkipRows.Name = "labelSkipRows";
            this.labelSkipRows.Size = new System.Drawing.Size(68, 13);
            this.labelSkipRows.TabIndex = 5;
            this.labelSkipRows.Text = "Rows to skip";
            // 
            // labelHeaders
            // 
            this.labelHeaders.AutoSize = true;
            this.labelHeaders.Location = new System.Drawing.Point(444, 102);
            this.labelHeaders.Name = "labelHeaders";
            this.labelHeaders.Size = new System.Drawing.Size(59, 13);
            this.labelHeaders.TabIndex = 6;
            this.labelHeaders.Text = "Headers at";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Location = new System.Drawing.Point(531, 164);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(75, 23);
            this.buttonLoad.TabIndex = 7;
            this.buttonLoad.Text = "Load";
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.ButtonLoadClick);
            // 
            // ImpConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(614, 271);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.labelHeaders);
            this.Controls.Add(this.labelSkipRows);
            this.Controls.Add(this.textBoxHeadersRow);
            this.Controls.Add(this.textBoxSkipRows);
            this.Controls.Add(this.labelDelimiter);
            this.Controls.Add(this.textBoxDelimiter);
            this.Controls.Add(this.richTextBoxFileContent);
            this.Name = "ImpConfigForm";
            this.Text = "File Preview";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxFileContent;
        private System.Windows.Forms.TextBox textBoxDelimiter;
        private System.Windows.Forms.Label labelDelimiter;
        private System.Windows.Forms.TextBox textBoxSkipRows;
        private System.Windows.Forms.TextBox textBoxHeadersRow;
        private System.Windows.Forms.Label labelSkipRows;
        private System.Windows.Forms.Label labelHeaders;
        private System.Windows.Forms.Button buttonLoad;
    }
}