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

                this.SetOpenFolderLink( path );

                var datasource = this.LoadCopipeData( path );

                var buttons = this.CreateCopipeButtons( datasource );

                this.LayoutButtons( buttons );


                this.MinimumSize = this.Size;
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                MessageBox.Show(ex.Message);
            }
		}

        private IEnumerable<CopipeData> LoadCopipeData(string path)
        {
            string xml = File.ReadAllText( path );

            return DataSchema.Parse( xml );
        }
        
        private List<Button> CreateCopipeButtons(IEnumerable<CopipeData> datasource)
		{
            var buttons = new List<Button>();

            // ツールチップ
            var tooltip = new ToolTip();

            int h = 36;
            int w = this.panel1.Width - 4;
            foreach (CopipeData data in datasource)
            {
                // コピペデータごとにコピペ用ボタンを生成してパネルに入れる。
                Button button = new Button();
                button.Text = data.title;
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                button.FlatStyle = FlatStyle.Flat;
                button.Width = w;
                button.Height = h;

                // ツールチップ
                tooltip.SetToolTip( button, data.Value );

                // コピペ処理
                button.Click += (s, a) =>
                {
                    Clipboard.Clear();
                    Clipboard.SetText( data.Value );
                };

                // リストに追加。
                buttons.Add( button );
            }
            return buttons;
		}

        private void LayoutButtons(IEnumerable<Button> buttons)
        {
            // 局所関数：
            void DoLayout()
            {
                int x = 1;
                int y = 1;

                this.panel1.Controls.Clear();
                foreach (Button button in buttons)
                {
                    this.panel1.Controls.Add( button );

                    // 座標設定してインクリメント（StackPanel的なアレ）
                    button.Location = new Point( x, y );
                    int dy = button.Height + 1;
                    y += dy;
                }
            }

            try
            {
                // 追加している最中にスクロールバー表示の閾値を超えると幅が崩れるので一旦オフる。
                this.panel1.AutoScroll = false;

                // コピペデータを基にボタンを生成する。
                DoLayout();
            }
            finally
            {
                // コントロールを追加し終えたら最後にオートスクロールをオンにする。
                this.panel1.AutoScroll = true;
            }
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
