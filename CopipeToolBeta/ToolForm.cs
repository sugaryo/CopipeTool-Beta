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
	public partial class ToolForm : Form
	{
		private readonly List<CopipeData> datasource = new List<CopipeData>();

        #region ctor
        public ToolForm()
		{
			InitializeComponent();
		}
        #endregion

		#region Load
		private void Form_Load( object sender, EventArgs e )
		{
            try
            {
#warning パスは外部定義化した方がいい。
#warning 相対パス起点も念の為exeのLocation拾って来るか。
                string path = "Data/dat.xml";

                this.LoadCopipeData( path );

                this.CreateCopipeButtons();

                this.SetOpenFolderLink( path );
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                MessageBox.Show(ex.Message);
            }
		}

        private void LoadCopipeData(string path)
        {
            string xml = File.ReadAllText( path );

            var data = DataSchema.Parse( xml );

            this.datasource.Clear();
            this.datasource.AddRange( data );
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

        private void SetOpenFolderLink(string path)
        {
            FileInfo file = new FileInfo( path );
            string fullpath = file.FullName;

            this.linkOpenDat.Click += (s, args) =>
            {
                // Explorerを /select オプション指定で叩く。
                System.Diagnostics.Process.Start( 
                    "EXPLORER.EXE", 
                    $@"/select,""{fullpath}""" 
                );
            };
        }
        #endregion
    }
}
