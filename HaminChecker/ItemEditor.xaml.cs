using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using LogChecker.Do;

namespace HaminChecker {
	/// <summary>
	/// ItemEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class ItemEditor : Window {
		public CheckInfo Result { get; private set; }
		private IItemEditor editor;
		private CheckInfo firstTmp;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="checks"></param>
		public ItemEditor(CheckInfo checks) {
			Result = null;
			firstTmp = checks;
			InitializeComponent();

			this.Title = "Edit: " + checks.Name + "(" + checks.Rem + ")";
			switch(checks.Mode) {
				case SetMode.String:
				case SetMode.CommaEd:
					var estring = new EditString(checks);
					GdEditor.Children.Add(estring);
					editor = estring;
					break;
				case SetMode.Terms:
					var eterms = new EditTerms(checks);
					GdEditor.Children.Add(eterms);
					editor = eterms;
					break;
				case SetMode.Frequency:
					var efreq = new EditFreq(checks);
					GdEditor.Children.Add(efreq);
					editor = efreq;
					break;
				case SetMode.PowerMode:
					var emode = new EditPowerMode(checks);
					GdEditor.Children.Add(emode);
					editor = emode;
					break;
				case SetMode.Sector:
					var esect = new EditSectors(checks);
					GdEditor.Children.Add(esect);
					editor = esect;
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
	}
}
