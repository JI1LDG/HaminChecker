using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HaminChecker.Utils;

namespace HaminChecker.Declares {
	/// <summary>
	/// 部門
	/// </summary>
	public class Sectors : NotifyChanged {
		private string _name;
		/// <summary>
		/// 部門名
		/// </summary>
		public string Name {
			get { return _name; }
			set { _name = value; OnPropertyChanged(); }
		}

		private string _writtenName;
		/// <summary>
		/// 表示用部門名
		/// </summary>
		public string WrittenName {
			get { return _writtenName; }
			set { _writtenName = value; OnPropertyChanged(); }
		}

		private string _code;
		/// <summary>
		/// 部門コード
		/// </summary>
		public string Code {
			get { return _code; }
			set { _code = value; OnPropertyChanged(); }
		}

		private int[] _modes;
		/// <summary>
		/// 対象モード
		/// </summary>
		public int[] Modes {
			get { return _modes; }
			set { _modes = value; OnPropertyChanged(); }
		}
		/// <summary>
		/// 対象モード文字列
		/// </summary>
		public string ModeStr {
			get { return (Modes == null) ? "" : string.Join(",", Modes.Select(x => x.ToString())); }
			set { Modes = value.Split(',').IntParse().ToArray(); }
		}

		/// <summary>
		/// 対象周波数リスト
		/// </summary>
		public Freqs[] EnabledFreqs { get; set; }
		/// <summary>
		/// 対象周波数リスト文字列
		/// </summary>
		public string EnabledFreqStr {
			get { return (EnabledFreqs == null) ? "" : EnabledFreqs.Join(); }
			set { EnabledFreqs = Frequency.Split(value); }
		}

		/// <summary>
		/// 非対象周波数リスト
		/// </summary>
		public Freqs[] UnabledFreqs { get; set; }
		/// <summary>
		/// 非対象周波数リスト文字列
		/// </summary>
		public string UnabledFreqStr {
			get { return (UnabledFreqs == null) ? "" : UnabledFreqs.Join(); }
			set { UnabledFreqs = Frequency.Split(value); }
		}

		/// <summary>
		/// `で連結した部門定義文字列を部門配列に変換します。
		/// </summary>
		/// <param name="str"></param>
		public static IEnumerable<Sectors> ToArray(string str) {
			if (str.IsEmpty()) yield return null;

			var spl = str.Split('`');
			var pattern = @"([^;]*;)+"; //g1 c0-5
			Sectors ret;
			foreach (var s in spl) {
				ret = null;
				if (!Regex.IsMatch(s, pattern)) {
					continue;
				}
				var m = Regex.Match(s, pattern)?.Groups[1]?.Captures.Cast<Capture>()?.Select(x => x.ToString().TrimEnd(';'))?.ToArray();
				if (m == null) {
					continue;
				}
				try {
					ret = new Sectors()
					{
						Name = m[0],
						WrittenName = (m[1] == "") ? m[0] : m[1],
						Code = m[2],
						Modes = m[3].Split(',').IntParse().ToArray(),
						EnabledFreqs = m[4].Split(',').Select(y => Frequency.FromString(y)).ToArray(),
						UnabledFreqs = m[5].Split(',').Select(y => Frequency.FromString(y)).ToArray(),
					};
				} catch (Exception e) {
					Console.WriteLine(e.Message);
					continue;
				}

				yield return ret;
			}
		}
	}
}
