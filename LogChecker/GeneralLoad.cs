using NLog;
using System.Collections.Generic;

namespace LogChecker {
	/// <summary>
	/// 基本設定読み込みクラス
	/// </summary>
	public class GeneralLoad {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		public List<Do.CheckInfo> Checks;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GeneralLoad() {
			Checks = new List<Do.CheckInfo>() {
				new Do.CheckInfo("ContestName", "コンテスト名", Do.SetMode.String, "<ContestName>"),
				new Do.CheckInfo("Terms1", "期間", Do.SetMode.Terms, "<yy/MM/dd-hh:mm:ss> <Term>"),
				new Do.CheckInfo("Terms2", "期間", Do.SetMode.Terms),
				new Do.CheckInfo("Terms3", "期間", Do.SetMode.Terms),
				new Do.CheckInfo("Freq1", "対象周波数", Do.SetMode.Frequency, "<Frequencies>"),
				new Do.CheckInfo("Freq2", "対象周波数", Do.SetMode.Frequency),
				new Do.CheckInfo("Freq3", "対象周波数", Do.SetMode.Frequency),
				new Do.CheckInfo("PowerMode", "最大電力・部門", Do.SetMode.PowerMode, "[<PowerSign>]<PowerName>"),
				new Do.CheckInfo("PowerSign", "空中線電力記号", Do.SetMode.CommaEd, "<PowerSigns>"),
				new Do.CheckInfo("Sectors", "部門", Do.SetMode.Sector, "<SectorName>;<WrittenName>;<SectorCode>;<PowerModes>;<EnableFreqs>;<UnableFreqs>"),
			};
		}

		/// <summary>
		/// 項目とその設定情報を取得し、チェックリストに格納します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		public void Execute(string str) {
			logger.Debug("Start");
			while(str.Length > 0) {
				var line = str.Substring(0, str.IndexOf("\r\n") - 1);
				str = str.Substring(0, line.Length + 2);
				if(line.Substring(0, 2) == "//") {
					continue;
				}

				var item = line.Substring(0, line.IndexOf(":"));
				switch(item) {
					case "Sectors":
						var tmp = "";
						while(line != "}") {
							line = str.Substring(0, str.IndexOf("\r\n") - 1);
							str = str.Substring(0, line.Length + 2);
							if(line.Substring(0, 2) == "//") {
								continue;
							}

							tmp += line + "`";
						}

						Checks.Set(item, tmp);
						break;
					default:
						Checks.Set(item, GetData(line));
						break;
				}
			}
			logger.Debug("End");
		}

		/// <summary>
		/// 文字列から項目名を取得します。
		/// </summary>
		/// <param name="str">対象文字列</param>
		private string GetData(string str) {
			logger.Debug("Execute(str: " + str + ")");
			try {
				return str.Substring(str.IndexOf(": ") + 2);
			} catch {
				return null;
			}
		}
	}
}
