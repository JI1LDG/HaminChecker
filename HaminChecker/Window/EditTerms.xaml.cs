using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// EditTerms.xaml の相互作用ロジック
	/// </summary>
	public partial class EditTerms : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }
		private string[] weeks = new string[] {
			"日", "月", "火", "水", "木", "金", "土"
		};

		public EditTerms(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();
			DpDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
			for(int i = 1;i <= 12; i++) {
				CbMonth.Items.Add(i + "月");
				if(i <= 5) {
					CbCount.Items.Add("第" + i);
				}
			}
			CbCount.Items.Add("最終");
			foreach(var w in weeks) {
				CbWeekday.Items.Add(w + "曜日");
			}
			for(int i = 0;i < 24; i++) {
				CbHour.Items.Add(i + "時");
			}
			for(int i = 0;i < 60; i++) {
				CbMinute.Items.Add(i + "分");
			}

			var dt = Date.FromTerm(src.Data, DateTime.Now.Year);
			if(dt == null) return;
			if(dt[2] == DateTime.MinValue) {
				RbDatePick.IsChecked = true;
				DpDate.Text = dt[0].ToString("yyyy/MM/dd");
			} else {
				RbDatePoint.IsChecked = true;
				var p = Regex.Match(src.Data, @"@\[(\s?[A-Za-z0-9]+)*\]");
				var weekNumber = int.TryParse(p.Groups[1].Captures[0].ToString(), out int res) ? res : 6;
				var weekDay = new System.Globalization.DateTimeFormatInfo().DayNames.ToList()
					.FindIndex(x => x == p.Groups[1].Captures[1].ToString().Trim());
				var monthNumber = new System.Globalization.DateTimeFormatInfo().MonthNames.ToList()
					.FindIndex(x => x == p.Groups[1].Captures[2].ToString().Trim());

				CbCount.SelectedIndex = weekNumber;
				CbWeekday.SelectedIndex = weekDay + 1;
				CbMonth.SelectedIndex = monthNumber + 1;

				var startDate = Date.FromWeekNumber(DateTime.Now.Year, monthNumber + 1, weekNumber, (DayOfWeek)weekDay);
				LbExample.Content = startDate.ToString("yyyy/MM/dd");
			}
			CbHour.SelectedIndex = dt[0].Hour + 1;
			CbMinute.SelectedIndex = dt[0].Minute + 1;
			var m = Regex.Match(src.Data, @"(\d{1,3})(Ds|Hs|Ms)$");
			TbTime.Text = m.Groups[1].ToString();
			switch(m.Groups[2].ToString()) {
				case "Ms":
					CbTimeKind.SelectedIndex = 0;
					break;
				case "Hs":
					CbTimeKind.SelectedIndex = 1;
					break;
				case "Ds":
					CbTimeKind.SelectedIndex = 2;
					break;
			}
		}

		public bool IsNotInvalid() {
			ErrorStr = "";

			if(RbDatePick.IsChecked == true) {
				if(!Regex.IsMatch(DpDate.Text, @"^\d\d\d\d/\d\d/\d\d$")) {
					ErrorStr = "日付が指定されていません";
					return false;
				}
			} else if(RbDatePoint.IsChecked == true) {
				if(CbCount.SelectedIndex * CbMonth.SelectedIndex * CbWeekday.SelectedIndex == 0) {
					ErrorStr = "開始日を指定してください";
					return false;
				}
			} else {
				ErrorStr = "チェックされてません";
				return false;
			}

			if(CbHour.SelectedIndex * CbMinute.SelectedIndex == 0) {
				ErrorStr = "時間を指定してください";
				return false;
			}

			if(!int.TryParse(TbTime.Text, out int res)) {
				ErrorStr = "期間を指定してください";
				return false;
			}

			return GetDateToError();
		}

		public void Update() {
			tmp.Data = GetData();
			Result = tmp;
		}

		private void CbMonth_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(CbMonth == null || CbWeekday == null || CbCount == null) return;
			var monthNumber = CbMonth.SelectedIndex;
			var weekNumber = CbCount.SelectedIndex;
			var weekDay = CbWeekday.SelectedIndex - 1;

			if(monthNumber < 1 || weekNumber < 1 || weekDay < 1) return;

			var startDate = Date.FromWeekNumber(DateTime.Now.Year, monthNumber, weekNumber, (DayOfWeek)weekDay);
			LbExample.Content = startDate.ToString("yyyy/MM/dd");
		}

		private string GetData() {
			string ret = "";

			if(RbDatePick.IsChecked == true) {
				ret += DpDate.Text.Substring(2) + "-";
			} else {
				ret += "@[";
				ret += CbCount.SelectedIndex + " ";
				ret += new System.Globalization.DateTimeFormatInfo().DayNames.ToList()[CbWeekday.SelectedIndex - 1] + " ";
				ret += new System.Globalization.DateTimeFormatInfo().MonthNames.ToList()[CbMonth.SelectedIndex - 1] + "]-";
			}
			ret += (CbHour.SelectedIndex - 1).ToString("00") + ":";
			ret += (CbMinute.SelectedIndex - 1).ToString("00") + " ";
			ret += TbTime.Text;
			switch(CbTimeKind.SelectedIndex) {
				case 0: ret += "Ms"; break;
				case 1: ret += "Hs"; break;
				case 2: ret += "Ds"; break;
			}

			return ret;
		}

		private bool GetDateToError() {
			var data = GetData();
			var dt = Date.FromTerm(data, DateTime.Now.Year);
			if(dt == null) {
				ErrorStr = "不明なエラー(文字列から日付への変換失敗)";
				return false;
			} else {
				ErrorStr = dt[0].ToString("yyyy/MM/dd-HH:mm") + " - " + dt[1].ToString("yyyy/MM/dd-HH:mm");
				return true;
			}
		}

		private void BtCheckTerms_Click(object sender, RoutedEventArgs e) {
			LbTerms.Content = "";

			IsNotInvalid();
			LbTerms.Content = ErrorStr;
		}
	}
}
