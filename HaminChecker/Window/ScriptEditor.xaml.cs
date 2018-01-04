using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// ScriptEditor.xaml の相互作用ロジック
	/// </summary>
	public partial class ScriptEditor : System.Windows.Window {
		private ObservableCollection<CheckInfo> Generals = new ObservableCollection<CheckInfo>();
		private List<AreaTelegram> Areas = new List<AreaTelegram>();
		private ObservableCollection<CheckInfo> Checks = new ObservableCollection<CheckInfo>();

		private AreaPane Ap1 = new AreaPane();
		private AreaPane Ap2 = new AreaPane();
		private string NowFile = null;

		public List<CheckInfo> Gen { get { return Generals.ToList(); } }
		public List<CheckInfo> Che { get { return Checks.ToList(); } }
		public List<AreaData> Area1 { get { return Ap1.GetAreaList().ToList(); } }
		public List<AreaData> Area2 { get { return Ap2.GetAreaList().ToList(); } }

		public bool IsCloseCancel = true;

		public ScriptEditor() {
			InitializeComponent();
			
			InitializeSettings();
			DgGeneral.ItemsSource = Generals;

			GdArea1.Children.Add(Ap1);
			GdArea2.Children.Add(Ap2);

			DgCheck.ItemsSource = Checks;
		}

		/// <summary>
		/// DataGridにおいて、メニューを呼び出します。
		/// </summary>
		private void DgPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
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
			if (ci.Mode == SetMode.None) return;

			var cm = new ContextMenu();
			var mi = new MenuItem();
			if (ci.Mode == SetMode.Bool) {
				var changeTo = (ci.Data == true.ToString()) ? false : true;
				mi.Header = changeTo.ToString() + "に変更";
				mi.Click += (miSender, miE) => {
					ci.Data = changeTo.ToString();
				};
			} else {
				mi.Click += this.Mi_Click;
				mi.Header = ci.Name + "編集";
			}
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
				var gen = Generals.Where(x => x.Name == chk.Name).FirstOrDefault();
				if (gen != null) {
					int idx = Generals.IndexOf(gen);
					Generals.Insert(idx, ie.Result);
					Generals.RemoveAt(idx + 1);
				} else {
					var idxc = Checks.IndexOf(Checks.Where(x => x.Name == chk.Name).First());
					Checks.Insert(idxc, ie.Result);
					Checks.RemoveAt(idxc + 1);
				}
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

			foreach(var gc in Generals) {
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

				if(!iie.IsNotInvalid() && !gc.Data.IsEmpty()) {
					ErrorStr += gc.Name + " / 「" + iie.ErrorStr + "」\r\n";
				}
			}

			if(ErrorStr.Length > 0) {
				MessageBox.Show(ErrorStr);
			} else {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
				SaveFileDialog sfd = new SaveFileDialog();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
				sfd.Title = "集計設定ファイルの保存";
				sfd.Filter = "集計設定ファイル(*.scp.txt)|*.scp.txt";
				if (sfd.ShowDialog() == true) {
					SaveAsJsons(sfd.FileName);
				}
			}
		}


		/// <summary>
		/// JSONファイルとしてスクリプトを保存します。
		/// </summary>
		/// <param name="filename">ファイル名</param>
		public void SaveAsJsons(string filename) {
			Json.Save(filename, Generals, Ap1.GetTelegrams(), Ap2.GetTelegrams(), Checks);
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}

		private void BtnOpen_Click(object sender, RoutedEventArgs e) {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			OpenFileDialog ofd = new OpenFileDialog();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
			ofd.Title = "設定ファイルの読み込み";
			ofd.Filter = "設定ファイル(*.scp.txt)|*.scp.txt";
			if (ofd.ShowDialog() == true) {
				var filename = ofd.FileName;
				NowFile = filename;
				var json = Json.Get(filename);

				LoadFromJsons(json);
			} else {
				MessageBox.Show("ファイル読み込みに失敗しました。");
			}
		}

		/// <summary>
		/// JSONデータから読み込み
		/// </summary>
		/// <param name="json">JSONデータ</param>
		public void LoadFromJsons(Dictionary<string, string> json) {
			InitializeSettings();

			var gens = new ObservableCollection<CheckInfo>(Json.Read<CheckInfo>(json["General"]));
			foreach (var g in gens) {
				if (Generals.FirstOrDefault(x => x.Name == g.Name) != null) {
					Generals.First(x => x.Name == g.Name).Data = g.Data;
				} else {
					Generals.Add(g);
				}
			}
			DgGeneral.ItemsSource = Generals;

			Ap1.DeleteScript();
			Ap2.DeleteScript();

			Areas = Json.Read<AreaTelegram>(json["Area1"]).ToList();
			Ap1.SetTelegram(Areas);

			Areas = Json.Read<AreaTelegram>(json["Area2"]).ToList();
			Ap2.SetTelegram(Areas);

			var chks = new ObservableCollection<CheckInfo>(Json.Read<CheckInfo>(json["Checks"]));
			foreach (var c in chks) {
				if (Checks.FirstOrDefault(x => x.Name == c.Name) != null) {
					Checks.First(x => x.Name == c.Name).Data = c.Data;
				} else {
					Checks.Add(c);
				}
			}
			DgCheck.ItemsSource = Checks;
		}

		/// <summary>
		/// 初期値設定
		/// </summary>
		private void InitializeSettings() {
			Generals = new ObservableCollection<CheckInfo>() {
			   new CheckInfo("ContestName", "コンテスト名", SetMode.String),
			   new CheckInfo("Terms1", "期間", SetMode.Terms),
			   new CheckInfo("Terms2", "期間", SetMode.Terms),
			   new CheckInfo("Terms3", "期間", SetMode.Terms),
			   new CheckInfo("Freq1", "対象周波数", SetMode.Frequency),
			   new CheckInfo("Freq2", "対象周波数", SetMode.Frequency),
			   new CheckInfo("Freq3", "対象周波数", SetMode.Frequency),
			   new CheckInfo("PowerMode", "最大電力・部門", SetMode.PowerMode),
			   new CheckInfo("Sectors", "部門", SetMode.Sector),
		   };

			Checks = new ObservableCollection<CheckInfo>() {
				new CheckInfo("Category: Callsign", "カテゴリ: コールサイン", SetMode.None, " "),
				new CheckInfo("CheckCallsign", "コールサイン判定", SetMode.Bool, false.ToString()),
				new CheckInfo("OnlyJAStation", "日本局限定", SetMode.Bool, false.ToString()),
				new CheckInfo("Category: ContestNo", "カテゴリ: コンテストナンバ", SetMode.None, " "),
				new CheckInfo("CheckRecvCn", "相手CN判定", SetMode.Bool, false.ToString()),
				new CheckInfo("RSTExists", "RSTあり", SetMode.Bool, false.ToString()),
				new CheckInfo("AreaByFreq", "周波数による地域番号", SetMode.Frequency),
				new CheckInfo("AreaNoExists", "地域番号あり", SetMode.Bool, false.ToString()),
				new CheckInfo("ExcludedAreaNos", "除外地域番号", SetMode.String),
				new CheckInfo("PowerSignExists", "記号あり", SetMode.Bool, false.ToString()),
				new CheckInfo("NonSignAllowed", "記号なし許容", SetMode.Bool, false.ToString()),
				new CheckInfo("PointBySign", "記号による得点変更設定", SetMode.PointBySign),
				new CheckInfo("Category: Frequency", "カテゴリ: 周波数", SetMode.None, " "),
				new CheckInfo("CheckBySection", "部門による判定", SetMode.Bool, false.ToString()),
				new CheckInfo("Category: Other", "カテゴリ: その他", SetMode.None, " "),
				new CheckInfo("CoeffSetting", "局種係数設定あり", SetMode.Bool, false.ToString()),
				new CheckInfo("ZeroByDupe", "ログ重複時に0点にする", SetMode.Bool, false.ToString()),
			};
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = IsCloseCancel;
			this.Hide();
		}
	}
}
