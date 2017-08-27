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
using System.Collections.ObjectModel;

namespace HaminChecker {
	/// <summary>
	/// ScriptEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class ScriptEditor : Window {
		private ObservableCollection<CheckInfo> GeneralChecks;
		public ScriptEditor() {
			GeneralChecks = new ObservableCollection<CheckInfo>() {
				new CheckInfo("ContestName", "コンテスト名", SetMode.String, "全市全郡コンテスト"),
				new CheckInfo("Terms1", "期間", SetMode.Terms, "@[1 Saturday October]-21:00 24Hs"),
				new CheckInfo("Terms2", "期間", SetMode.Terms),
				new CheckInfo("Terms3", "期間", SetMode.Terms),
				new CheckInfo("Freq1", "対象周波数", SetMode.Frequency, "3.5,7,14,21,28,50,144,430,1200,2400,5600"),
				new CheckInfo("Freq2", "対象周波数", SetMode.Frequency),
				new CheckInfo("Freq3", "対象周波数", SetMode.Frequency),
				new CheckInfo("PowerMode", "最大電力・部門", SetMode.PowerMode, "[H,H]免許範囲内;[M,M]100;[,M]10(20);[P,P]5;"),
				new CheckInfo("PowerSign", "空中線電力記号", SetMode.CommaEd, "H,M,L,P"),
				new CheckInfo("Sectors", "部門", SetMode.Sector, "電話部門シングルオペオールバンド(除14MHz);電話部門シングルオペオールバンド;PA;2;;14;`電信電話部門マルチオペオールバンド;;XMA;0,1;;;"),
			};
			InitializeComponent();
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
					case SetMode.CommaEd:
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
	}
}
