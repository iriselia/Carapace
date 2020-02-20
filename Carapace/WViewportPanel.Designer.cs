namespace Carapace
{
	partial class WViewportPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// WViewportPanel
			// 
			this.Size = new System.Drawing.Size(438, 381);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.ViewportPanel_Paint);
			this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ViewportPanel_MouseDown);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ViewportPanel_MouseMove);
			this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ViewportPanel_MouseUp);
			this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ViewportPanel_MouseWheel);
			this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.WViewportPanel_PreviewKeyDown);
			this.ResumeLayout(false);

		}

		#endregion
	}
}
