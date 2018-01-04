using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Win32;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : System.Windows.Window {
		private ObservableCollection<LogData> LogGrid;
		private Sequence dgs = new Sequence();
		private DispatcherTimer dispatch;

		ScriptEditor se = new ScriptEditor();
		public MainWindow() {
			InitializeComponent();
			LogGrid = new ObservableCollection<LogData>() { };

			if (Directory.Exists("_tmp")) {
				if(MessageBox.Show("プログラムが異常終了してしまったようです。前回終了時のデータ・設定を可能な限り復元します。\r\n復元を実行しますか?", "復元", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
					LoadWork();
				}
			} else {
				Directory.CreateDirectory("_tmp");
			}

			DgLog.ItemsSource = LogGrid;

#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			dispatch = new DispatcherTimer(DispatcherPriority.Normal);
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
			dispatch.Interval = new TimeSpan(0, 1, 0);
			dispatch.Tick += Dispatch_Tick;
			dispatch.Start();
		}

		/// <summary>
		/// 定期実行処理
		/// </summary>
		private void Dispatch_Tick(object sender, EventArgs e) {
			se.SaveAsJsons("_tmp\\sedata");
			Json.Save("_tmp\\ctgen", Json.Get(new ConfigGeneral[] { UcConfigTab.GetGeneral() }));
			Json.Save("_tmp\\ctsol", Json.Get(new ConfigSolid[] { UcConfigTab.GetSolid() }));
			Json.Save("_tmp\\logs", Json.Get(LogGrid));
		}

		/// <summary>
		/// ワーク読み込み処理
		/// </summary>
		private void LoadWork() {
			se.LoadFromJsons(Json.Get("_tmp\\sedata"));
			UcConfigTab.Set(se.Gen, se.Che);
			UcConfigTab.SetGeneral(Json.GetSingle<ConfigGeneral>("_tmp\\ctgen"));
			UcConfigTab.SetSolid(Json.GetSingle<ConfigSolid>("_tmp\\ctsol"));
			LogGrid = new ObservableCollection<LogData>(Json.GetEnumerable<LogData>("_tmp\\logs"));
			DgLog.ItemsSource = LogGrid;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			se.IsCloseCancel = false;
			se.Close();
			se = null;

			if (Directory.Exists("_tmp")) {
				foreach(var path in Directory.GetFiles("_tmp")) {
					File.SetAttributes(path, FileAttributes.Normal);
					File.Delete(path);
				}

				Directory.Delete("_tmp", false);
			}
		}

		private void MiSetting_Click(object sender, RoutedEventArgs e) {
			se.ShowDialog();
			UcConfigTab.Set(se.Gen, se.Che);
		}

		private void MiLoadLog_Click(object sender, RoutedEventArgs e) {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			OpenFileDialog ofd = new OpenFileDialog();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
			ofd.Title = "CTESTWINログファイルの読み込み";
			ofd.Filter = "CTESTWINログファイル(*.lg8)|*.lg8";
			if (ofd.ShowDialog() == true) {
				CtestwinLoad cl = new CtestwinLoad(ofd.FileName);

				foreach (var cw in cl.Execute()) {
					LogGrid.Add(new LogData() {
						Date = cw.Date,
						Callsign = cw.Callsign,
						SentCn = cw.SentContestNo,
						RecvCn = cw.ReceivedContestNo,
						Mode = cw.Mode.ToString(),
						Freq = Frequency.ToString(cw.Frequency),
						Operator = cw.Operator,
						Rem = cw.Rem,
						ErrLv = 0,
						FailedStr = "",
						Point = 0,
					});
				}
			}
		}

		string begin = null;
		private void DgLog_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
			begin = Json.Get(LogGrid);
		}

		private void DgLog_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
			if (begin == null) return;
			dgs.Add(SeqMode.Log, begin, Json.Get(LogGrid));

			begin = null;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e) {
			ModifierKeys modifierKeys = Keyboard.Modifiers;
			if ((modifierKeys & ModifierKeys.Control) != ModifierKeys.None) {
				SeqData ret = null;
				if (e.Key == Key.Z) {
					ret = dgs.Undo();
				} else if (e.Key == Key.Y) {
					ret = dgs.Redo();
				}

				if (ret == null) return;
				switch (ret.Mode) {
					case SeqMode.Log:
						LogGrid = new ObservableCollection<LogData>(Json.Read<LogData>(ret?.Data1));
						if (LogGrid == null) return;
						DgLog.ItemsSource = LogGrid;
						break;
					default:
						break;
				}
			}
		}

		private void MiLoadWork_Click(object sender, RoutedEventArgs e) {
			dispatch.Stop();
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			OpenFileDialog ofd = new OpenFileDialog();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
			ofd.Title = "ワークファイルの読み込み";
			ofd.Filter = "ワークファイル(*.hcw)|*.hcw";
			if (ofd.ShowDialog() == true) {
				foreach (var path in Directory.GetFiles("_tmp")) {
					File.SetAttributes(path, FileAttributes.Normal);
					File.Delete(path);
				}

				ZipFile.ExtractToDirectory(ofd.FileName, "_tmp");
				LoadWork();
			}

			dispatch.Start();
		}

		private void MiWriteWork_Click(object sender, RoutedEventArgs e) {
			dispatch.Stop();
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			SaveFileDialog sfd = new SaveFileDialog();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
			sfd.Title = "ワークファイルの保存";
			sfd.Filter = "ワークファイル(*.hcw)|*.hcw";
			if (sfd.ShowDialog() == true) {
				if (File.Exists(sfd.FileName)) {
					File.SetAttributes(sfd.FileName, FileAttributes.Normal);
					File.Delete(sfd.FileName);
				}
				Dispatch_Tick(null, null);

				ZipFile.CreateFromDirectory("_tmp", sfd.FileName, CompressionLevel.Optimal, false);
			}

			dispatch.Start();
		}

		/// <summary>
		/// DataGridにおいて、メニューを呼び出します。
		/// </summary>
		private void DgLog_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
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
			var data = dgcell.DataContext as LogData;

			var selected = DgLog.SelectedItems.Cast<LogData>().ToList();
			if (selected.Count == 0) return;

			var cm = new ContextMenu();
			var mi = new MenuItem();
			if (selected.Count == 1) {
				mi.Header = "1件確認";
				mi.Click += Mi_SingleCheck;
				mi.CommandParameter = data;
			} else {
				mi.Header = selected.Count + "件確認";
				mi.Click += Mi_MultiCheck;
				mi.CommandParameter = selected;
			}
			cm.Items.Add(mi);

			var mi_ChkT = new MenuItem();
			mi_ChkT.Header = "確認除外True";
			mi_ChkT.Click += Mi_ChkT_Click;
			mi_ChkT.CommandParameter = selected;
			cm.Items.Add(mi_ChkT);

			var mi_ChkF = new MenuItem();
			mi_ChkF.Header = "確認除外False";
			mi_ChkF.Click += Mi_ChkF_Click;
			mi_ChkF.CommandParameter = selected;
			cm.Items.Add(mi_ChkF);

			var mi_Del = new MenuItem();
			mi_Del.Header = "削除";
			mi_Del.Click += Mi_Del_Click;
			mi_Del.CommandParameter = selected;
			cm.Items.Add(mi_Del);

			ContextMenuService.SetContextMenu(dgcell, cm);
		}

		private void Mi_Del_Click(object sender, RoutedEventArgs e) {
			var data = ((sender as MenuItem).CommandParameter) as List<LogData>;

			if (MessageBox.Show("選択したデータを削除してよろしいでしょうか。", "確認", MessageBoxButton.YesNo) == MessageBoxResult.No) {
				return;
			}

			foreach(var d in data) {
				LogGrid.Remove(d);
			}

			DgLog.Items.Refresh();
		}

		private void Mi_ChkF_Click(object sender, RoutedEventArgs e) {
			var data = ((sender as MenuItem).CommandParameter) as List<LogData>;

			foreach (var d in data) {
				d.Exclude = false;
			}

			DgLog.Items.Refresh();
		}

		private void Mi_ChkT_Click(object sender, RoutedEventArgs e) {
			var data = ((sender as MenuItem).CommandParameter) as List<LogData>;

			foreach(var d in data) {
				d.Exclude = true;
			}

			DgLog.Items.Refresh();
		}

		private void Mi_MultiCheck(object sender, RoutedEventArgs e) {
			var data = ((sender as MenuItem).CommandParameter) as List<LogData>;
			var send = new CheckData {
				Logs = data,
				Gens = se.Gen,
				Ches = se.Che,
				ConGen = UcConfigTab.GetGeneral(),
				Area1 = se.Area1,
				Area2 = se.Area2
			};

			var cw = new CheckWindow(send);
			cw.ShowDialog();

			DgLog.Items.Refresh();
		}

		private void Mi_SingleCheck(object sender, RoutedEventArgs e) {
			var data = ((sender as MenuItem).CommandParameter) as LogData;
			var send = new CheckData {
				Logs = new List<LogData>() { data },
				Gens = se.Gen,
				Ches = se.Che,
				ConGen = UcConfigTab.GetGeneral(),
				Area1 = se.Area1,
				Area2 = se.Area2
			};

			var cw = new CheckWindow(send);
			cw.ShowDialog();
			
			DgLog.Items.Refresh();
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

		private void MiAllCheck_Click(object sender, RoutedEventArgs e) {
			var send = new CheckData {
				Logs = LogGrid.ToList(),
				Gens = se.Gen,
				Ches = se.Che,
				ConGen = UcConfigTab.GetGeneral(),
				Area1 = se.Area1,
				Area2 = se.Area2
			};

			var cw = new CheckWindow(send);
			cw.ShowDialog();

			DgLog.Items.Refresh();
		}

		private void MiLogClear_Click(object sender, RoutedEventArgs e) {
			LogGrid.Clear();
		}
	}
}
