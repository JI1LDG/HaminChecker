using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utilities;

namespace LogChecker {	
	/// <summary>
	/// 設定リスト用拡張メソッドクラス
	/// </summary>
	public static class CheckInfoExtensions {
		/// <summary>
		/// リストの対象項目のデータを設定します。
		/// </summary>
		/// <param name="list">追加対象リスト</param>
		/// <param name="name">項目名</param>
		/// <param name="data">データ</param>
		public static void Set(this List<Do.CheckInfo> list, string name, string data) {
			if(list.Exists(x => x.Name == name) && data != null) {
				list.Find(x => x.Name == name).Data = data;
			}
		}

		/// <summary>
		/// 設定データを文字列化します。
		/// </summary>
		/// <param name="src">対象項目</param>
		public static string ToPrint(this Do.CheckInfo src) {
			switch(src.Mode) {
				case Do.SetMode.Terms:
					var dts = Definer.TermToDate(src.Data, 2017);
					return (dts == null) ? "" : string.Join(" to ", dts.Where(x => x != DateTime.MinValue && x != DateTime.MaxValue).Select(x => x.ToString("yyyy/MM/dd HH:mm")));
				case Do.SetMode.Frequency:
					var fqs = Definer.Frequencies(src.Data);
					return(fqs == null) ? "" : fqs.JoinString();
				case Do.SetMode.PowerMode:
					var pms = Definer.Modes(src.Data);
					return (pms == null) ? "" : string.Join(", ", pms.Select(x => x.ToString()));
				case Do.SetMode.Sector:
					var data = src.Data.Replace("\r\n", "`");
					var scs = Definer.Sectors(data)?.ToArray();
					return (scs == null) ? "" : string.Join("\r\n", scs.Select(x => x.ToString()));
				default:
					return src.Data.ToString();
			}
		}
	}
}

namespace LogChecker.Do {
	/// <summary>
	/// 設定リストクラス
	/// </summary>
	public class CheckInfo {
		public string Name { get; private set; }
		public string Rem { get; private set; }
		public SetMode Mode { get; private set; }
		public string Data { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">項目名</param>
		/// <param name="rem">説明</param>
		/// <param name="mode">設定モード</param>
		/// <param name="data">データ</param>
		public CheckInfo(string name, string rem, SetMode mode, string data = "<Empty>") {
			Name = name;
			Rem = rem;
			Mode = mode;
			if(data != null) {
				Data = data;
			}
		}
	}

	/// <summary>
	/// 電力部門クラス
	/// </summary>
	public class PowerMode {
		public string SuffixPowerSign { get; set; }
		public string SuffixContestNo { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="suffixPowerSign">電力記号(Suffix)</param>
		/// <param name="name">部門名</param>
		public PowerMode(string suffixPowerSign, string conno, string name) {
			SuffixPowerSign = suffixPowerSign;
			SuffixContestNo = conno;
			Name = name;
		}

		/// <summary>
		/// [+記号]名前のフォーマットで出力します。
		/// </summary>
		public override string ToString() {
			return "[" + SuffixPowerSign + "," + SuffixContestNo + "]" + Name;
		}

		/// <summary>
		/// 電力部門配列を作成します。
		/// </summary>
		/// <param name="suffixPowers">電力記号(Suffix)</param>
		/// <param name="conno">コンテストナンバ末尾</param>
		/// <param name="names">部門名</param>
		public static PowerMode[] GetByScript(string[] suffixPowers, string[] conno, string[] names) {
			var cnt = Math.Min(suffixPowers.Length, names.Length);
			var pma = new PowerMode[cnt];

			for(int i = 0;i < cnt;i++) {
				pma[i] = new PowerMode(suffixPowers[i], conno[i], names[i]);
			}

			return pma;
		}

		public static string ToString(IEnumerable<PowerMode> src) {
			return string.Join(";", src.Select(x => x.ToString())) + ";";
		}
	}

	/// <summary>
	/// 部門クラス
	/// </summary>
	public class Sectors {
		public string Name { get;  set; }
		public string WrittenName { get;  set; }
		public string Code { get;  set; }
		private int[] modes;
		public int[] Modes {
			get { return modes; }
			set { modes = value; }
		}
		public string ModeStr {
			get { return (modes == null) ? "" : string.Join(",", modes.Select(x => x.ToString())); }
			set { modes = Definer.SplitWithComma(value).IntParse().ToArray(); }
		}
		public Freqs[] EnabledFreqs { get; set; }
		public string EnabledFreqStr {
            get { return (EnabledFreqs == null) ? "" : EnabledFreqs.JoinString(); }
            set { EnabledFreqs = value.Split(',').Select(x => Enums.FreqFromString(x)).Where(x => x != Freqs.None).ToArray(); }
        }
		public Freqs[] UnabledFreqs { get; set; }
		public string UnabledFreqStr {
            get { return (UnabledFreqs == null) ? "" : UnabledFreqs.JoinString(); }
            set { UnabledFreqs = value.Split(',').Select(x => Enums.FreqFromString(x)).Where(x => x != Freqs.None).ToArray(); }
        }

        public override string ToString() {
			var mds = Modes.Select(x => x.ToString())?.ToArray();
			return Name + "(" + WrittenName + "): Code/" + Code + ", Mode/" + ((mds == null) ? "" : string.Join(", ", mds)) + ", EF/" + EnabledFreqs.JoinString() + ", UF/" + UnabledFreqs.JoinString();
		}
	}

	/// <summary>
	/// 設定モード列挙体
	/// </summary>
	public enum SetMode {
		Bool, String, Number, CommaEd,
		Terms, Frequency, Sector, PowerMode,
	}
}