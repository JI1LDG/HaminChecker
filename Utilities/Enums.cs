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

	public class Enums {
		private static readonly string[] freqNames = {
			"1.9MHz", "3.5MHz", "7MHz", "10MHz", "14MHz", "18MHz", "21MHz",
			"24MHz", "28MHz", "50MHz", "144MHz", "430MHz", "1200MHz", "2400MHz", "5600MHz"
		};

		public static string FreqToString(Freqs freq) {
			return freqNames[(int)freq];
		}

		public static Freqs FreqFromString(string str) {
			var idx = freqNames.ToList().FindIndex(x => x == str + "MHz");
			if(idx == -1) {
				return Freqs.None;
			} else {
				return (Freqs)idx;
			}
		}
	}
}
