using NLog;

namespace Utilities {
	/// <summary>
	/// 日付処理クラス
	/// </summary>
	public class DateUtils {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 現在の日付を文字列に変換します。
		/// </summary>
		public static string NowToString() {
			logger.Debug("Execute");
			return System.DateTime.Now.ToString("yyyy-MM-dd");
		}

		/// <summary>
		/// "yyyy-MM-dd"形式の文字列を日付クラスに変換します。
		/// </summary>
		/// <param name="updated">日付文字列</param>
		public static System.DateTime FromString(string updated) {
			logger.Debug("Execute(updated: " + updated + ")");
			var date = updated.Split('-');
			return new System.DateTime(int.Parse(date[0]) + 2_000, int.Parse(date[1]), int.Parse(date[2]));
		}

		/// <summary>
		/// 月初日を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		private static System.DateTime GetFirstDayOfMonth(int year, int month) {
			return new System.DateTime(year, month, 1);
		}

		/// <summary>
		/// 月末日を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		private static System.DateTime GetLastDayOfMonth(int year, int month) {
			return GetFirstDayOfMonth(year, month).AddMonths(1).AddDays(-1);
		}

		/// <summary>
		/// 指定した月の第[weekNumber][weekDay]曜日の日付を取得します。
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <param name="weekNumber">指定週番号</param>
		/// <param name="weekDay">指定曜日</param>
		public static System.DateTime GetDateByWeekNumber(int year, int month, int weekNumber, System.DayOfWeek weekDay) {
			var firstDay = GetFirstDayOfMonth(year, month);
			var lastDay = GetLastDayOfMonth(year, month);

			var week = System.Math.Max(1, weekNumber);
			var day = 0;

			do {
				if(weekDay >= firstDay.DayOfWeek) {
					day = 7 * (week - 1) + ((int)weekDay - (int)firstDay.DayOfWeek) + 1;
				} else {
					day = 7 * week + ((int)weekDay - (int)firstDay.DayOfWeek) + 1;
				}

				week--;
			} while(day > lastDay.Day);

			return new System.DateTime(year, month, day);
		}
	}
}
