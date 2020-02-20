using System.Windows.Forms;
using System.Diagnostics;

namespace Carapace
{
	public partial class AboutBox : Form
	{
		public AboutBox()
		{
			InitializeComponent();
		}

		private void WMB_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
		{
			Process.Start( @"www.warrenmarshall.biz" );
		}
	}
}
