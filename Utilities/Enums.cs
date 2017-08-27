using System;
using System.Linq;

namespace Utilities {
	/// <summary>
	/// モード一覧
	/// </summary>
	public enum Modes {
		CW, RTTY, SSB, FM, AM,
	}

	/// <summary>
	/// 周波数一覧
	/// </summary>
	public enum Freqs {
		f1p9M, f3p5M, f7M, f10M, f14M, f18M, f21M, f24M, f28M, f50M, f144M, f430M, f1200M, f2400M, f5600M, None
	}

	/// <summary>
	/// 列挙体用クラス
	/// </summary>
	public class Enums {
		public static readonly string[] freqNames = {
			"1.9MHz", "3.5MHz", "7MHz", "10MHz", "14MHz", "18MHz", "21MHz",
			"24MHz", "28MHz", "50MHz", "144MHz", "430MHz", "1200MHz", "2400MHz", "5600MHz"
		};

		/// <summary>
		/// 周波数列挙体を文字列に変換します。
		/// </summary>
		/// <param name="freq">周波数列挙体</param>
		public static string FreqToString(Freqs freq) {
			try {
				return freqNames[(int)freq];
			} catch {
				return "";
			}
		}

		/// <summary>
		/// 文字列から周波数列挙体に変換します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		public static Freqs FreqFromString(string str) {
			var idx = freqNames.ToList().FindIndex(x => x == str + "MHz");
			if(idx == -1) {
				return Freqs.None;
			} else {
				return (Freqs)idx;
			}
		}
	}

	/// <summary>
	/// 周波数用拡張メソッドクラス
	/// </summary>
	public static class FreqExtensions {
		/// <summary>
		/// 周波数配列をカンマ区切りの文字列に変換します。
		/// </summary>
		/// <param name="src"></param>
		public static string JoinString(this Freqs[] src) {
			return JoinStringWithUnits(src).Replace("MHz", "");
		}

		public static string JoinStringWithUnits(this Freqs[] src) {
			return (src == null) ? "" : string.Join(",", src.Select(x => Enums.FreqToString(x)).Where(y => y != ""));
		}
	}
}
