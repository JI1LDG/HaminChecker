using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using LogChecker;
using LogChecker.Do;

namespace HaminChecker {
	/// <summary>
	/// EditSectors.xaml の相互作用ロジック
	/// </summary>
	public partial class EditSectors : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }
		private ObservableCollection<Sectors> sectorData;

		public EditSectors(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();

			sectorData = new ObservableCollection<Sectors>(Definer.Sectors(src.Data));

			DgSectors.ItemsSource = sectorData;
		}

		public bool IsNotInvalid() {
			ErrorStr = "";

			foreach (var sd in sectorData) {
				if (sd.EnabledFreqStr != "" && sd.UnabledFreqStr != "") {
					ErrorStr = "対象・非対象周波数共に書き込まれています。どちらか一方にしてください。";
					return false;
				}

				if (sd.Name == "" || sd.WrittenName == "") {
					ErrorStr = "(表示)部門名を入力してください。";
					return false;
				}

				if (sd.Code == "") {
					ErrorStr = "部門コードが未入力です。";
					return false;
				}
			}

			return true;
		}

		public void Update() {
			tmp.Data = GetData();
			Result = tmp;
		}

		private string GetData() {
			return string.Join("`", sectorData.Select(x => string.Join(";", new string[] { x.Name, x.WrittenName, x.Code, x.ModeStr, x.EnabledFreqStr, x.UnabledFreqStr }) + ";"));
		}

		/// <summary>
		/// DataGridにおいて、メニューを呼び出します。
		/// </summary>
		private void DgSectors_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
			DataGrid dg = sender as DataGrid;
			Point pt = e.GetPosition(dg);
			DataGridCell dgcell = null;

			VisualTreeHelper.HitTest(dg, null, (result) => {
				DataGridCell cell = FindVisualParent<DataGridCell>(result.VisualHit);
				if (cell != null) {
					dgcell = cell;
					return HitTestResultBehavior.Stop;
				} else return HitTestResultBehavior.Continue;
			}, new PointHitTestParameters(pt));

			if (dgcell == null) return;
			Sectors st = dgcell.DataContext as Sectors;
			var cm = new ContextMenu();
			var mi = new MenuItem();
			mi.Click += (o, f) => {
				st.WrittenName = st.Name;
				DgSectors.Items.Refresh();
			};
			mi.Header = "表示部門名を部門名にコピー";
			cm.Items.Add(mi);

			var mj = new MenuItem();
			mj.Click += (o, f) => {
				st.Name = st.WrittenName;
				DgSectors.Items.Refresh();
			};
			mj.Header = "部門名を表示部門名にコピー";
			cm.Items.Add(mj);

			ContextMenuService.SetContextMenu(dgcell, cm);
		}

		/// <summary>
		/// DGメニュー呼び出し用
		/// </summary>
		private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject {
			DependencyObject parentobj = VisualTreeHelper.GetParent(child);
			if (parentobj == null) return null;
			if (parentobj is T parent) {
				return parent;
			} else return FindVisualParent<T>(parentobj);
		}

		private void BtAddRow_Click(object sender, RoutedEventArgs e) {
			sectorData.Add(new Sectors());
		}
	}
}
