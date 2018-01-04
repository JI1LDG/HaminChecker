using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// AreaPane.xaml の相互作用ロジック
	/// </summary>
	public partial class AreaPane : UserControl {
		public AreaPane() {
			InitializeComponent();
		}

		private void BtAreaChk_Click(object sender, RoutedEventArgs e) {
			var dlg = new AreaChecker(SingleAreaData.FromLists(GetAreaList()));
			dlg.ShowDialog();
		}

		/// <summary>
		/// スクリプト削除
		/// </summary>
		public void DeleteScript() {
			TbArea.Text = "";
		}

		/// <summary>
		/// 電文設定
		/// </summary>
		/// <param name="telegram">電文</param>
		public void SetTelegram(List<AreaTelegram> telegram) {
			foreach (var a in telegram) {
				TbArea.Text += a.ToString() + "\r\n";
			}
		}

		/// <summary>
		/// 電文取得
		/// </summary>
		/// <returns>電文リスト</returns>
		public List<AreaTelegram> GetTelegrams() {
			return TbArea.Text.Replace("\r\n", "`").Split('`').Select(x => AreaTelegram.GetObject(x)).Where(x => x != null).ToList();
		}

		/// <summary>
		/// 地域情報取得
		/// </summary>
		/// <returns>地域情報リスト</returns>
		public List<AreaData> GetAreaList() {
			var TelgObj = GetTelegrams().ToArray();

			return AreaLoad.AnalyzeTelegram(TelgObj);
		}
	}
}
