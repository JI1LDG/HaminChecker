using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HaminChecker.Declares {
	/// <summary>
	/// 電力記号
	/// </summary>
	public class PowerMode {
		/// <summary>
		/// 部門コード末尾記号
		/// </summary>
		public string SuffixPowerSign { get; set; }
		/// <summary>
		/// コンテストナンバ末尾記号
		/// </summary>
		public string SuffixContestNo { get; set; }
		/// <summary>
		/// 電力記号名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="suffixPowerSign">部門コード末尾記号</param>
		/// <param name="conno">コンテストナンバ末尾記号</param>
		/// <param name="name">電力記号名</param>
		public PowerMode(string suffixPowerSign, string conno, string name) {
			SuffixPowerSign = suffixPowerSign;
			SuffixContestNo = conno;
			Name = name;
		}

		/// <summary>
		/// "[+記号]名前"
		/// </summary>
		public override string ToString() {
			return "[" + SuffixPowerSign + "," + SuffixContestNo + "]" + Name;
		}

		/// <summary>
		/// 電力記号配列を作成します。
		/// </summary>
		/// <param name="suffixPowers">部門コード末尾記号</param>
		/// <param name="conno">コンテストナンバ末尾記号</param>
		/// <param name="names">電力記号名</param>
		public static PowerMode[] GetByScript(string[] suffixPowers, string[] conno, string[] names) {
			var cnt = Math.Min(suffixPowers.Length, names.Length);
			var pma = new PowerMode[cnt];

			for (int i = 0; i < cnt; i++) {
				pma[i] = new PowerMode(suffixPowers[i], conno[i], names[i]);
			}

			return pma;
		}

		/// <summary>
		/// 電力記号リスト文字列
		/// </summary>
		/// <param name="src">電力記号リスト</param>
		/// <returns>電力記号リスト文字列</returns>
		public static string ToString(IEnumerable<PowerMode> src) {
			return string.Join(";", src.Select(x => x.ToString())) + ";";
		}

		/// <summary>
		/// モード定義文字列からモード定義配列を取得します。
		/// </summary>
		/// <param name="modes">モード定義文字列</param>
		public static PowerMode[] ToArray(string modes) {
			if (modes == "<Empty>") return new PowerMode[0];

			var pattern = @"(\[(\w?),(\w?)\]([^\[\]:]*);)+";
			try {
				if (Regex.IsMatch(modes, pattern)) {
					var m = Regex.Match(modes, pattern);
					var spows = m.Groups[2].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();
					var conno = m.Groups[3].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();
					var names = m.Groups[4].Captures.Cast<Capture>().Select(x => x.ToString()).ToArray();

					return PowerMode.GetByScript(spows, conno, names);
				}
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
			return new PowerMode[0];
		}
	}
}
