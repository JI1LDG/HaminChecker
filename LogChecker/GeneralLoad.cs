﻿using NLog;
using System.Collections.Generic;

namespace LogChecker {
	/// <summary>
	/// 
	/// </summary>
	public class GeneralLoad {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		private List<Do.CheckInfo> checks;

		/// <summary>
		/// 
		/// </summary>
		public GeneralLoad() {
			checks = new List<Do.CheckInfo>() {
				new Do.CheckInfo("ContestName", "コンテスト名", Do.SetMode.String, "<ContestName>"),
				new Do.CheckInfo("Terms1", "期間", Do.SetMode.Terms, "<yy/MM/dd-hh:mm:ss> <Term>"),
				new Do.CheckInfo("Terms2", "期間", Do.SetMode.Terms),
				new Do.CheckInfo("Terms3", "期間", Do.SetMode.Terms),
				new Do.CheckInfo("Freq1", "対象周波数", Do.SetMode.Frequency, "<Frequencies>"),
				new Do.CheckInfo("Freq2", "対象周波数", Do.SetMode.Frequency),
				new Do.CheckInfo("Freq3", "対象周波数", Do.SetMode.Frequency),
				new Do.CheckInfo("PowerMode", "最大電力・部門", Do.SetMode.PowerMode, "[<PowerSign>]<PowerName>"),
				new Do.CheckInfo("PowerSign", "空中線電力記号", Do.SetMode.PowerSign, "<PowerSigns>"),
				new Do.CheckInfo("Sectors", "部門", Do.SetMode.Sector, "<SectorName>;<WrittenName>;<SectorCode>;<PowerModes>;<EnableFreqs>;<UnableFreqs>"),
			};
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
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

						checks.Set(item, tmp);
						break;
					default:
						checks.Set(item, GetData(line));
						break;
				}
			}
			logger.Debug("End");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="str"></param>
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
