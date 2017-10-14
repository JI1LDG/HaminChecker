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

using InfoGetter.Do;
using LogChecker.Do;
using System.Collections.ObjectModel;

namespace HaminChecker {
	/// <summary>
	/// ScriptEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class ScriptEditor : Window {
		private ObservableCollection<CheckInfo> GeneralChecks = new ObservableCollection<CheckInfo>();
		private List<AreaTelegram> Areas = new List<AreaTelegram>();
		private string NowFile = null;

		public ScriptEditor() {
			InitializeComponent();
			
			foreach (var item in System.IO.Directory.GetFiles(System.IO.Directory.GetCurrentDirectory() + "\\Scripts", "*.scp.txt", System.IO.SearchOption.TopDirectoryOnly)) {
				CbFiles.Items.Add(item.Remove(0, System.IO.Directory.GetCurrentDirectory().Length + 1));
			}
			DgGeneral.ItemsSource = GeneralChecks;
		}

		/// <summary>
		/// DataGridにおいて、メニューを呼び出します。
		/// </summary>
		private void DgGeneral_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			DataGrid dg = sender as DataGrid;
			Point pt = e.GetPosition(dg);
			DataGridCell dgcell = null;

			VisualTreeHelper.HitTest(dg, null, (result) => {
				DataGridCell cell = FindVisualParent<DataGridCell>(result.VisualHit);
				if(cell != null) {
					dgcell = cell;
					return HitTestResultBehavior.Stop;
				} else return HitTestResultBehavior.Continue;
			}, new PointHitTestParameters(pt));

			if(dgcell == null) return;
			CheckInfo ci = dgcell.DataContext as CheckInfo;
			var cm = new ContextMenu();
			var mi = new MenuItem();
			mi.Click += this.Mi_Click;
			mi.Header = ci.Name + "編集";
			mi.CommandParameter = ci;
			cm.Items.Add(mi);

			ContextMenuService.SetContextMenu(dgcell, cm);
		}

		/// <summary>
		/// DGメニュー/ダイアログ
		/// </summary>
		private void Mi_Click(object sender, RoutedEventArgs e) {
			var chk = ((sender as MenuItem).CommandParameter) as CheckInfo;
			var ie = new ItemEditor(chk);
			ie.ShowDialog();
			if(ie.Result != null) {
				int idx = GeneralChecks.IndexOf(GeneralChecks.Where(x => x.Name == chk.Name).First());
				GeneralChecks.Insert(idx, ie.Result);
				GeneralChecks.RemoveAt(idx + 1);
			}
		}

		/// <summary>
		/// DGメニュー呼び出し用
		/// </summary>
		private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject {
			DependencyObject parentobj = VisualTreeHelper.GetParent(child);
			if(parentobj == null) return null;
			if(parentobj is T parent) {
				return parent;
			} else return FindVisualParent<T>(parentobj);
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e) {
			string ErrorStr = "";
			IItemEditor iie;

			foreach(var gc in GeneralChecks) {
				iie = null;

				switch(gc.Mode) {
					case SetMode.String:
						iie = new EditString(gc);
						break;
					case SetMode.Terms:
						iie = new EditTerms(gc);
						break;
					case SetMode.Frequency:
						iie = new EditFreq(gc);
						break;
					case SetMode.PowerMode:
						iie = new EditPowerMode(gc);
						break;
					case SetMode.Sector:
						iie = new EditSectors(gc);
						break;
					default:
						break;
				}

				if(iie == null) continue;

				if(!iie.IsNotInvalid() && gc.Data != "<Empty>") {
					ErrorStr += gc.Name + " / 「" + iie.ErrorStr + "」\r\n";
				}
			}

			if(ErrorStr.Length > 0) {
				MessageBox.Show(ErrorStr);
			} else {
				MessageBox.Show("問題なし");
			}
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
			this.Title = this.ActualHeight + ", " + this.ActualWidth;
		}

		private void BtnOpen_Click(object sender, RoutedEventArgs e) {
			string filename = CbFiles.SelectedItem?.ToString();

			if (filename == null) {
				MessageBox.Show("ファイルを選択してください。");
			} else {
				NowFile = filename;
				var json = Utilities.ScriptIo.GetJsons(filename);

				GeneralChecks = new ObservableCollection<CheckInfo>(Utilities.ScriptIo.ReadJsonCi<CheckInfo>(json["General"]));
				DgGeneral.ItemsSource = GeneralChecks;

				TbArea.Text = "";

				Areas = Utilities.ScriptIo.ReadJsonCi<AreaTelegram>(json["Area"]);
				foreach (var a in Areas) {
					TbArea.Text += a.ToString() + "\r\n";
				}
			}
		}

		private void BtAreaChk_Click(object sender, RoutedEventArgs e) {
			var TelgObj = TbArea.Text.Replace("\r\n", "`").Split('`').Select(x => AreaTelegram.GetObject(x)).Where(x => x != null).ToArray();

			var areaList = InfoGetter.AreaLoader.AnalyzeTelegram(TelgObj);

			var dlg = new AreaChecker(areaList.ToArray());
			dlg.ShowDialog();
		}
	}
}
