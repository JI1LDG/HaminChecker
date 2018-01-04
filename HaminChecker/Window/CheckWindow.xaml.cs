using System.Windows;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// CheckWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class CheckWindow : System.Windows.Window {
		private Worker work;
		private int dataCount;
		private bool isEnded;

		public CheckWindow(CheckData Data) {
			InitializeComponent();
			isEnded = false;
			work = new Worker(Data);
			LbNum.DataContext = work;
			dataCount = Data.Logs.Count;
			if (dataCount == 0) {
				TbProgress.Text = "ログを読み込ませてください。";
				BtCancel.Content = "終了";
				isEnded = true;
				return;
			}

			work.RunAsync(
				percentage => {
					PbExecute.Value = (double)percentage / (double)dataCount * 100;
					TbProgress.Text = (PbExecute.Value / 100).ToString("0.00%");
				},
				isCancel => {
					PbExecute.Value = (double)work.NowCount / (double)dataCount * 100;
					TbProgress.Text = (PbExecute.Value / 100).ToString("0.00%");
					BtCancel.Content = "終了";
					isEnded = true;
				}
			);
		}

		private void BtCancel_Click(object sender, RoutedEventArgs e) {
			work.Cancel();
			if (isEnded) {
				this.Close();
			} else {
				BtCancel.Content = "処理の終了待機中...";
			}
		}
	}
}
