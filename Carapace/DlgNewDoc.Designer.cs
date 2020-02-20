namespace Carapace
{
    partial class DlgNewDoc
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.HeightTextControl = new System.Windows.Forms.TextBox();
			this.WidthTextControl = new System.Windows.Forms.TextBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.CnclButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.HeightTextControl);
			this.groupBox1.Controls.Add(this.WidthTextControl);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(262, 56);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Dimensions";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(122, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(20, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = " X ";
			// 
			// HeightTextControl
			// 
			this.HeightTextControl.Location = new System.Drawing.Point(148, 19);
			this.HeightTextControl.Name = "HeightTextControl";
			this.HeightTextControl.Size = new System.Drawing.Size(100, 20);
			this.HeightTextControl.TabIndex = 2;
			this.HeightTextControl.Text = "1000";
			// 
			// WidthTextControl
			// 
			this.WidthTextControl.Location = new System.Drawing.Point(16, 19);
			this.WidthTextControl.Name = "WidthTextControl";
			this.WidthTextControl.Size = new System.Drawing.Size(100, 20);
			this.WidthTextControl.TabIndex = 1;
			this.WidthTextControl.Text = "1000";
			// 
			// OKButton
			// 
			this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OKButton.Location = new System.Drawing.Point(291, 12);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(75, 23);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CnclButton
			// 
			this.CnclButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.CnclButton.Location = new System.Drawing.Point(291, 41);
			this.CnclButton.Name = "CnclButton";
			this.CnclButton.Size = new System.Drawing.Size(75, 23);
			this.CnclButton.TabIndex = 2;
			this.CnclButton.Text = "Cancel";
			this.CnclButton.UseVisualStyleBackColor = true;
			// 
			// DlgNewDoc
			// 
			this.AcceptButton = this.OKButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.CnclButton;
			this.ClientSize = new System.Drawing.Size(378, 82);
			this.Controls.Add(this.CnclButton);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgNewDoc";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resize Document";
			this.Load += new System.EventHandler(this.DlgNewDoc_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox HeightTextControl;
        private System.Windows.Forms.TextBox WidthTextControl;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CnclButton;
    }
}