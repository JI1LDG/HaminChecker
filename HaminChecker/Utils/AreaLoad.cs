using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// 地域情報読み込み
	/// </summary>
	public class AreaLoad {
		/// <summary>
		/// ファイルから地域情報を読み込みます。
		/// </summary>
		/// <param name="filePath">ファイル名</param>
		/// <returns>地域情報リスト</returns>
		public static IEnumerable<AreaData> FromFile(string filePath) {
			using (var sr = new System.IO.StreamReader("Areas\\" + filePath + ".area.txt", System.Text.Encoding.GetEncoding("shift-jis"))) {
				while (sr.Peek() > 0) {
					var item = GetAreaByLine(sr.ReadLine());
					if (item != null) yield return item;
				}
			}
		}

		/// <summary>
		/// 地域フォーマットから地域情報を取得します。
		/// </summary>
		/// <param name="line">取得元文字列</param>
		private static AreaData GetAreaByLine(string line) {
			var buf = line;
			var spl = buf.Split(' ');
			switch (buf[0]) {
				//A [No] [Name/Address]
				case 'A':
					return new AreaData() { No = spl[1], Name = spl[2], Addresses = new List<string>() { spl[2] } };
				//E [No] [[Address-Preffix]Name] [Address-Suffix]{,[AS]}
				case 'E':
				case 'I':
					return new AreaData() { No = spl[1], Name = spl[2], Addresses = spl[3].Split(',').Select(x => spl[2].Split('(')[0] + x).ToList() };
				//N [No] [Name] [Address]{,[A]}
				case 'N':
					return new AreaData() { No = spl[1], Name = spl[2], Addresses = spl[3].Split(',').ToList() };
				default:
					return null;
			}
		}

		/// <summary>
		/// 電文を解析します。
		/// </summary>
		/// <param name="telegram">電文配列</param>
		/// <returns>地域情報リスト</returns>
		public static List<AreaData> AnalyzeTelegram(AreaTelegram[] telegram) {
			var list = new List<AreaData>();
			var load = new Dictionary<string, List<AreaData>>();
			int idx;

			foreach (var tg in telegram) {
				switch (tg.Function) {
					case TelegramFuncMode.Load:
						load.Add(tg.Arg2, FromFile(tg.Arg1).ToList());
						break;
					case TelegramFuncMode.Add:
						list.Add(GetAreaByLine(tg.Arg1));
						break;
					case TelegramFuncMode.Addarg:
						list.AddRange(load[tg.Arg1]);
						break;
					case TelegramFuncMode.Addattel:
						idx = list.FindIndex(x => x.No == tg.Arg2);
						if (idx < 0) break;

						if (tg.Arg1 != "bef") idx++;

						list.Insert(idx, GetAreaByLine(tg.Arg3));
						break;
					case TelegramFuncMode.Addatarg:
						idx = list.FindIndex(x => x.No == tg.Arg2);
						if (idx < 0) break;

						if (tg.Arg1 != "bef") idx++;

						list.InsertRange(idx, load[tg.Arg1]);
						break;
					case TelegramFuncMode.Delno:
						if (list.Exists(x => x.No == tg.Arg1)) {
							list.Remove(list.First(x => x.No == tg.Arg1));
						}
						break;
					case TelegramFuncMode.Movat:
						//後日実装
						break;
					case TelegramFuncMode.Movto:
						//後日実装
						break;
					case TelegramFuncMode.Argnox:
						AreaData[] rgx;

						if (tg.Arg2 == "get") {
							rgx = load[tg.Arg1].Where(x => !Regex.IsMatch(x.No, tg.Arg3)).ToArray();
						} else {
							rgx = load[tg.Arg1].Where(x => Regex.IsMatch(x.No, tg.Arg3)).ToArray();
						}

						foreach (var r in rgx) {
							load[tg.Arg1].Remove(r);
						}

						break;
					case TelegramFuncMode.Delstr:
					case TelegramFuncMode.Sort:
						//不使用
						break;
				}
			}

			return list;
		}
	}
}
