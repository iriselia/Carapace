using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Carapace
{
	public partial class DlgNewDoc : Form
	{
		public int NewWidth, NewHeight;

		public DlgNewDoc()
		{
			InitializeComponent();

			NewWidth = NewHeight = 0;
		}

		private void OKButton_Click( object sender, EventArgs e )
		{
			NewWidth = int.Parse( WidthTextControl.Text );
			NewHeight = int.Parse( HeightTextControl.Text );

			NewWidth = Math.Max( NewWidth, 1 );
			NewHeight = Math.Max( NewHeight, 1 );
		}

		private void DlgNewDoc_Load( object sender, EventArgs e )
		{
			WWorld W = WWorld.GetWorld();

			WidthTextControl.Text = string.Format( "{0}", W.PicturePlaneWidth );
			HeightTextControl.Text = string.Format( "{0}", W.PicturePlaneHeight );
		}
	}
}
