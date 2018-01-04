using System.Linq;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// 周波数
	/// </summary>
	public class Frequency {
		public static readonly string[] FreqNames = {
			"1.9MHz", "3.5MHz", "7MHz", "10MHz", "14MHz", "18MHz", "21MHz",
			"24MHz", "28MHz", "50MHz", "144MHz", "430MHz", "1200MHz", "2400MHz", "5600MHz"
		};

		/// <summary>
		/// 周波数列挙体を文字列に変換します。
		/// </summary>
		/// <param name="freq">周波数列挙体</param>
		public static string ToString(Freqs freq) {
			try {
				return FreqNames[(int)freq];
			} catch {
				return "";
			}
		}

		/// <summary>
		/// 文字列から周波数列挙体に変換します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		public static Freqs FromString(string str) {
			var idx = FreqNames.ToList().FindIndex(x => x == str + "MHz");
			if (idx == -1) {
				return Freqs.None;
			} else {
				return (Freqs)idx;
			}
		}

		/// <summary>
		/// カンマ区切りの文字列から周波数配列に変換します。
		/// </summary>
		/// <param name="str">カンマ区切り周波数文字列</param>
		/// <returns>周波数配列</returns>
		public static Freqs[] Split(string str) {
			if (str.IsEmpty()) return new Freqs[0];
			return str.Split(',').Select(x => FromString(x)).Where(x => x != Freqs.None).ToArray();
		}

	}

	/// <summary>
	/// 周波数用拡張メソッドクラス
	/// </summary>
	public static class FreqExtensions {
		/// <summary>
		/// 周波数配列をカンマ区切りの文字列(MHz除外)に変換します。
		/// </summary>
		/// <param name="src">周波数配列</param>
		public static string Join(this Freqs[] src) {
			return JoinWithUnits(src).Replace("MHz", "");
		}

		/// <summary>
		/// 周波数配列をカンマ区切りの文字列に変換します。
		/// </summary>
		/// <param name="src">周波数配列</param>
		public static string JoinWithUnits(this Freqs[] src) {
			return (src == null) ? "" : string.Join(",", src.Select(x => Frequency.ToString(x)).Where(y => y != ""));
		}
	}
}
