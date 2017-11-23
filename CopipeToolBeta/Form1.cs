using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CopipeToolBeta.Data;
using static CopipeToolBeta.Data.DataSchema;

namespace CopipeToolBeta
{
	public partial class Form1 : Form
	{
		private readonly List<CopipeData> datasource = new List<CopipeData>();

		public Form1()
		{
			InitializeComponent();
		}

		#region Load
		private void Form1_Load( object sender, EventArgs e )
		{
			string xml = File.ReadAllText("Data/dat.xml");

			var copipedata = DataSchema.Parse(xml);

			this.datasource.Clear();
			this.datasource.AddRange( copipedata );

			CreateCopipeButtons();	
		}
		private void CreateCopipeButtons()
		{
			int x = 1;
			int y = 1;

			int h = 36;
			int dy = h + 1;

			int w = this.panel1.Width - 4;


			// 追加している最中にスクロールバー表示の閾値を超えると幅が崩れるので一旦オフる。
			this.panel1.AutoScroll = false;

			// ツールチップ
			var tip = new ToolTip();

			List<Button> buttons = new List<Button>();
			foreach ( CopipeData data in this.datasource )
			{
				// コピペデータごとにコピペ用ボタンを生成してパネルに入れる。
				Button b = new Button();
				buttons.Add( b );

				this.panel1.Controls.Add( b );



				b.Text = data.title;
				b.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
				b.FlatStyle = FlatStyle.Flat;
				b.Width = w;
				b.Height = h;


				// ツールチップ
				tip.SetToolTip( b, data.Value );


				// コピペ処理
				b.Click += ( s, a ) =>
				{
					Clipboard.Clear();
					Clipboard.SetText( data.Value );
				};
				
				// 座標設定してインクリメント（StackPanel的なアレ）
				b.Location = new Point( x, y );
				y += dy;
			}

			// コントロールを追加し終えたら最後にオートスクロールをオンにする。
			this.panel1.AutoScroll = true;
		}
		#endregion
	}
}
