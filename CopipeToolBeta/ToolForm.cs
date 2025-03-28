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

                // datasource.tag からチェックボックスを生成する。
                var checks = this.CreateTagCheckButtons( datasource );
                if (checks.Count != 0)
                {
                    this.flowLayoutPanel1.Controls.Clear();
                    // タグ由来のカテゴリチェックボタンを作っていた場合は、
                    // 『全オン／全オフ』のボタンも生成する。
                    Button onButton = new Button() {
                            AutoSize = true,
                            Width = 24,
                            Text = "<ON>",
                            FlatStyle = FlatStyle.Flat,
                            ForeColor = Color.Blue,
                            BackColor = Color.White,
                    };
                    onButton.Click += (s, a) =>
                    {
                        foreach (CheckBox check in checks)
                        {
                            check.Checked = true;
                        }
                    };
                    this.flowLayoutPanel1.Controls.Add( onButton );
                    Button offButton = new Button() { 
                            AutoSize = true,
                            Width = 24,
                            Text = "<OFF>",
                            FlatStyle = FlatStyle.Flat,
                            ForeColor = Color.Red,
                            BackColor = Color.White,
                    };
                    offButton.Click += (s, a) =>
                    {
                        foreach (CheckBox check in checks)
                        {
                            check.Checked = false;
                        }
                    };
                    this.flowLayoutPanel1.Controls.Add( offButton );

    
                    // タグチェックボタンを追加。
                    this.flowLayoutPanel1.Controls.AddRange( checks.ToArray() );
                }

                // datasource からコピペボタンを生成する。
                var buttons = this.CreateCopipeButtons( datasource );
                this.panel1.Controls.Clear();
                this.panel1.Controls.AddRange( buttons.ToArray() );

                // ボタンの初期レイアウト。
                this.LayoutButtons();


                // デザイナ時のサイズを最小サイズに設定。
                this.MinimumSize = this.Size;

                // 表示時に初期サイズ設定。
                this.Shown += (s, a) =>
                {
                    this.Size = new Size( 320, 600 );
                };
            }
            catch (Exception ex)
            {
                this.Enabled = false;
                MessageBox.Show(ex.Message);
            }
		}
        // xml データのロード。
        private IEnumerable<CopipeData> LoadCopipeData(string path)
        {
            string xml = File.ReadAllText( path );

            return DataSchema.Parse( xml );
        }
        #endregion

        #region チェックボックス（ボタン型）
        private List<CheckBox> CreateTagCheckButtons(IEnumerable<CopipeData> datasource)
        {
            var checks = new List<CheckBox>();

            var tags = datasource.AsEnumerable()
                .Where( x => !string.IsNullOrWhiteSpace( x.tag ) )
                .Select( x => x.tag )
                .Distinct();

            foreach (string tag in tags)
            {
                CheckBox check = new CheckBox();
                check.Appearance = Appearance.Button;
                check.Text = tag;
                check.AutoSize = true;
                check.FlatStyle = FlatStyle.Flat;
                check.Checked = true;
                check.ForeColor = Color.Green;
                check.BackColor = Color.LightGreen;

                check.CheckedChanged += OnTagCheckedChanged;

                checks.Add( check );
            }
            return checks;
        }

        private void OnTagCheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = sender as CheckBox;
            check.ForeColor = check.Checked ? Color.Green : Color.DarkRed;
            check.BackColor = check.Checked ? Color.LightGreen : Color.LightCoral;
            string tag = check.Text;

            // タグに紐づくボタンの有効状態を変更する。
            foreach (Button button in this.panel1
                    .Controls
                    .OfType<Button>()
                    .Where( x => x.Tag as string == tag ))
            {
                button.Visible = check.Checked;
                button.Enabled = check.Checked;
            }

            // 更新されたボタンの有効状態で再レイアウトする。
            this.LayoutButtons();
        }
        #endregion

        #region コピペボタン
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
                button.Tag = data.tag;
                button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = Color.White;
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

        #endregion

        #region ボタンのレイアウト処理（StackPanel風）
        private void LayoutButtons()
        {
            // 可視状態のボタンを縦に並べるローカルメソッド：
            void DoLayout()
            {
                int dy = 0;
                foreach (Button button in this.panel1
                        .Controls
                        .OfType<Button>()
                        .Where( x => x.Visible ) )
                {
                    button.Location = new Point( 1, 1 + dy );
                    dy += button.Height + 1;
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
                // レイアウトし終えたら最後にオートスクロールをオンにする。
                this.panel1.AutoScroll = true;
            }
        }
        #endregion

        #region フォルダを開く...
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
