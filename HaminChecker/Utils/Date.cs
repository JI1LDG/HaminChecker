using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HaminChecker.Utils {
	/// <summary>
	/// 日時
	/// </summary>
	public class Date {
		/// <summary>
		/// 現在の日付を文字列に変換します。
		/// </summary>
		public static string NowToString() {
			return DateTime.Now.ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// "yyyy-MM-dd"形式の文字列を日付クラスに変換します。
		/// </summary>
		/// <param name="updated">日付文字列</param>
		public static DateTime FromString(string updated) {
			var date = updated.Split('-');
			return new DateTime(int.Parse(date[0]) + 2_000, int.Parse(date[1]), int.Parse(date[2]));
		}

		/// <summary>
		/// 月初日を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		private static DateTime FirstDayOfMonth(int year, int month) {
			return new DateTime(year, month, 1);
		}

		/// <summary>
		/// 月末日を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		private static DateTime LastDayOfMonth(int year, int month) {
			return FirstDayOfMonth(year, month).AddMonths(1).AddDays(-1);
		}

		/// <summary>
		/// 指定した月の第[weekNumber][weekDay]曜日の日付を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <param name="weekNumber">指定週番号</param>
		/// <param name="weekDay">指定曜日</param>
		public static DateTime FromWeekNumber(int year, int month, int weekNumber, DayOfWeek weekDay) {
			var firstDay = FirstDayOfMonth(year, month);
			var lastDay = LastDayOfMonth(year, month);

			var week = Math.Max(1, weekNumber);
			var day = 0;
			do {
				if (weekDay >= firstDay.DayOfWeek) {
					day = 7 * (week - 1) + ((int)weekDay - (int)firstDay.DayOfWeek) + 1;
				} else {
					day = 7 * week + ((int)weekDay - (int)firstDay.DayOfWeek) + 1;
				}

				week--;
			} while (day > lastDay.Day);

			return new DateTime(year, month, day);
		}

		/// <summary>
		/// 期間定義文字列から開始・終了日時を取得します。
		/// </summary>
		/// <param name="terms">期間定義文字列</param>
		/// <param name="year">対象年</param>
		/// <returns>DateTime型配列 / [0]: 開始日時, [1]: 終了日時, [2]: MinValue(日付指定) / MaxValue(相対指定)</returns>
		public static DateTime[] FromTerm(string terms, int year) {
			var dt = new DateTime[3];

			if (terms == "<Empty>") return dt;

			var daterm = @"^(\d\d)/(\d\d)/(\d\d)-(\d\d):(\d\d) (\d{1,3})(Ds|Hs|Ms)$";
			var targeterm = @"^@\[(\s?[A-Za-z0-9]+)*\]-(\d\d):(\d\d) (\d{1,3})(Ds|Hs|Ms)$";
			try {
				if (Regex.IsMatch(terms, daterm)) {
					var m = Regex.Match(terms, daterm);
					dt[0] = new DateTime(
						int.Parse(m.Groups[1].ToString()) + 2_000, int.Parse(m.Groups[2].ToString()), int.Parse(m.Groups[3].ToString()),
						int.Parse(m.Groups[4].ToString()), int.Parse(m.Groups[5].ToString()), 0
						);

					var timeVal = int.Parse(m.Groups[6].ToString());
					switch (m.Groups[7].ToString()) {
						case "Ds":
							dt[1] = dt[0].AddDays(timeVal);
							break;
						case "Hs":
							dt[1] = dt[0].AddHours(timeVal);
							break;
						case "Ms":
							dt[1] = dt[0].AddMinutes(timeVal);
							break;
						default:
							return null;
					}
					dt[2] = DateTime.MinValue;

					return dt;
				} else if (Regex.IsMatch(terms, targeterm)) {
					var m = Regex.Match(terms, targeterm);
					var target = m.Groups[1].Captures.Cast<Capture>().Select(x => x.ToString().Trim()).ToArray();
					var timeVal = int.Parse(m.Groups[4].ToString());
					var weekNumber = int.TryParse(target[0], out int res) ? res : 5;
					var weekDay = new System.Globalization.DateTimeFormatInfo().DayNames.ToList()
						.FindIndex(x => x == target[1]);
					var monthNumber = new System.Globalization.DateTimeFormatInfo().MonthNames.ToList()
						.FindIndex(x => x == target[2]);

					if (weekDay == -1 || monthNumber == -1) return null;

					dt[0] = FromWeekNumber(year, monthNumber + 1, weekNumber, (DayOfWeek)weekDay);
					dt[0] = dt[0].AddHours(int.Parse(m.Groups[2].ToString())).AddMinutes(int.Parse(m.Groups[3].ToString()));

					switch (m.Groups[5].ToString()) {
						case "Ds":
							dt[1] = dt[0].AddDays(timeVal);
							break;
						case "Hs":
							dt[1] = dt[0].AddHours(timeVal);
							break;
						case "Ms":
							dt[1] = dt[0].AddMinutes(timeVal);
							break;
						default:
							return null;
					}
					dt[2] = DateTime.MaxValue;

					return dt;
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}

			return null;
		}
	}
}
