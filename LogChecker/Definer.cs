using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NLog;

using Utilities;

namespace LogChecker {
	/// <summary>
	/// スクリプト解析クラス
	/// </summary>
	public class Definer {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// 期間定義文字列から開始・終了日時を取得します。
		/// </summary>
		/// <param name="terms">期間定義文字列</param>
		/// <param name="year">対象年</param>
		/// <returns>System.DateTime型配列 / [0]: 開始日時, [1]: 終了日時, [2]: MinValue(日付指定) / MaxValue(相対指定)</returns>
		public static DateTime[] TermToDate(string terms, int year) {
			var dt = new DateTime[3];

			var daterm = @"^(\d\d)/(\d\d)/(\d\d)-(\d\d):(\d\d) (\d{1,3})(Ds|Hs|Ms)$";
			var targeterm = @"^@\[(\s?[A-Za-z0-9]+)*\]-(\d\d):(\d\d) (\d{1,3})(Ds|Hs|Ms)$";
			try {
				if(Regex.IsMatch(terms, daterm)) {
					var m = Regex.Match(terms, daterm);
					dt[0] = new DateTime(
						int.Parse(m.Groups[1].ToString()) + 2_000, int.Parse(m.Groups[2].ToString()), int.Parse(m.Groups[3].ToString()),
						int.Parse(m.Groups[4].ToString()), int.Parse(m.Groups[5].ToString()), 0
						);

					var timeVal = int.Parse(m.Groups[6].ToString());
					switch(m.Groups[7].ToString()) {
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
				} else if(Regex.IsMatch(terms, targeterm)) {
					var m = Regex.Match(terms, targeterm);
					var target = m.Groups[1].Captures.Cast<Capture>().Select(x => x.ToString().Trim()).ToArray();
					var timeVal = int.Parse(m.Groups[4].ToString());
					var weekNumber = int.TryParse(target[0], out int res) ? res : 5;
					var weekDay = new System.Globalization.DateTimeFormatInfo().DayNames.ToList()
						.FindIndex(x => x == target[1]);
					var monthNumber = new System.Globalization.DateTimeFormatInfo().MonthNames.ToList()
						.FindIndex(x => x == target[2]);

					if(weekDay == -1 || monthNumber == -1) return null;

					dt[0] = DateUtils.GetDateByWeekNumber(year, monthNumber + 1, weekNumber, (DayOfWeek)weekDay);
					dt[0] = dt[0].AddHours(int.Parse(m.Groups[2].ToString())).AddMinutes(int.Parse(m.Groups[3].ToString()));

					switch(m.Groups[5].ToString()) {
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
			} catch(Exception e) {
				logger.Error(e, e.Message);
			}

			return null;
		}

		/// <summary>
		/// カンマ区切りの文字列から周波数列挙体配列を取得します。
		/// </summary>
		/// <param name="freqs">対象文字列</param>
		public static Freqs[] Frequencies(string freqs) {
			try {
				var spl = freqs.Split(',');
				return spl.Select(x => Enums.FreqFromString(x)).Where(x => x != Freqs.None).ToArray();
			} catch(Exception e) {
				logger.Error(e, e.Message);
				return null;
			}
		}

		/// <summary>
		/// モード定義文字列からモード定義配列を取得します。
		/// </summary>
		/// <param name="modes">モード定義文字列</param>
		public static Do.PowerMode[] Modes(string modes) {
			var pattern = @"(\[(\w?),(\w?)\]([^\[\]:]*);)+";
			try {
				if(Regex.IsMatch(modes, pattern)) {
					var m = Regex.Match(modes, pattern);
					var spows = m.Groups[2].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();
					var conno = m.Groups[3].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();
					var names = m.Groups[4].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();

					return Do.PowerMode.GetByScript(spows, conno, names);
				}
			} catch(Exception e) {
				logger.Error(e, e.Message);
			}
			return null;
		}

		/// <summary>
		/// カンマ区切りの文字列から出力記号文字列配列を取得します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		public static string[] SplitWithComma(string str) {
			return str.Split(',');
		}

		/// <summary>
		/// `で連結した部門定義文字列を部門配列に変換します。
		/// </summary>
		/// <param name="str"></param>
		public static IEnumerable<Do.Sectors> Sectors(string str) {
			var spl = str.Split('`');
			var pattern = @"([^;]*;)+"; //g1 c0-5
			Do.Sectors ret;
			foreach(var s in spl) {
				ret = null;
				if(!Regex.IsMatch(s, pattern)) {
					continue;
				}
				var m = Regex.Match(s, pattern)?.Groups[1]?.Captures.Cast<Capture>()?.Select(x => x.ToString().TrimEnd(';'))?.ToArray();
				if(m == null) {
					continue;
				}
				try {
					ret = new Do.Sectors() {
						Name = m[0],
						WrittenName = (m[1] == "") ? m[0] : m[1],
						Code = m[2],
						Modes = m[3].Split(',').IntParse().ToArray(),
						EnabledFreqs = m[4].Split(',').Select(y => Enums.FreqFromString(y)).ToArray(),
						UnenabledFreqs = m[5].Split(',').Select(y => Enums.FreqFromString(y)).ToArray(),
					};
				} catch(Exception e) {
					logger.Error(e, e.Message);
					continue;
				}

				yield return ret;
			}
		}
	}
}
