using System;
using System.Windows.Forms;

namespace Carapace
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			WWorld.GetWorld().ViewportPanel = ViewportPanel;
			Invalidate();
		}

		private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutBox dlg = new AboutBox();
			dlg.ShowDialog();
		}
	}
}
