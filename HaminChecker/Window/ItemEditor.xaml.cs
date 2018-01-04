using System.Windows;

using HaminChecker.Declares;

namespace HaminChecker {
	/// <summary>
	/// ItemEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class ItemEditor : System.Windows.Window {
		public CheckInfo Result { get; private set; }
		private IItemEditor editor;
		private CheckInfo firstTmp;

		/// <param name="checks">コンテスト設定データ</param>
		public ItemEditor(CheckInfo checks) {
			Result = null;
			firstTmp = checks;
			InitializeComponent();

			this.Title = "Edit: " + checks.Name + "(" + checks.Rem + ")";
			switch(checks.Mode) {
				case SetMode.String:
					var estring = new EditString(checks);
					GdEditor.Children.Add(estring);
					editor = estring;
					this.Height = 130;
					break;
				case SetMode.Terms:
					var eterms = new EditTerms(checks);
					GdEditor.Children.Add(eterms);
					editor = eterms;
					this.Height = 400;
					this.Width = 370;
					break;
				case SetMode.Frequency:
					var efreq = new EditFreq(checks);
					GdEditor.Children.Add(efreq);
					editor = efreq;
					this.Height = 350;
					this.Width = 260;
					break;
				case SetMode.PowerMode:
					var emode = new EditPowerMode(checks);
					GdEditor.Children.Add(emode);
					editor = emode;
					this.Height = 250;
					this.Width = 300;
					break;
				case SetMode.Sector:
					var esect = new EditSectors(checks);
					GdEditor.Children.Add(esect);
					editor = esect;
					this.Width = 900;
					break;
				default:
					break;
			}
		}

		private void BtUpdate_Click(object sender, RoutedEventArgs e) {
			if(editor == null) this.Close();

			if(editor.IsNotInvalid()) {
				editor.Update();
				Result = editor.Result;
				this.Close();
			} else {
				if(MessageBox.Show(editor.ErrorStr + "\r\n\r\n空欄にする場合は「はい」を選択してください。", "Information", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
					editor.Update();
					editor.Result.Data = "<Empty>";
					Result = editor.Result;
					this.Close();
				}
			}
		}

		private void BtCancel_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void BtInitialize_Click(object sender, RoutedEventArgs e) {
			if(editor == null) this.Close();

			editor.Result = firstTmp;
			switch(firstTmp.Mode) {
				case SetMode.PowerMode:
					editor.Result.Data = ";";
					break;
				default:
					editor.Result.Data = "<Empty>";
					break;
			}
			Result = editor.Result;
			this.Close();
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
			this.Title = this.ActualHeight + ", " + this.ActualWidth;
		}
	}
}
