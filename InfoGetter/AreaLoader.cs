using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace InfoGetter {
	public class AreaLoader {
		public static IEnumerable<Do.AreaData> GetAreaDataFromFile(string filePath) {
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
		private static Do.AreaData GetAreaByLine(string line) {
			var buf = line;
			var spl = buf.Split(' ');
			switch (buf[0]) {
				//A [No] [Name/Address]
				case 'A':
					return new Do.AreaData() { No = spl[1], Name = spl[2], Addresses = new List<string>() { spl[2] } };
				//E [No] [[Address-Preffix]Name] [Address-Suffix]{,[AS]}
				case 'E':
				case 'I':
					return new Do.AreaData() { No = spl[1], Name = spl[2], Addresses = spl[3].Split(',').Select(x => spl[2].Split('(')[0] + x).ToList() };
				//N [No] [Name] [Address]{,[A]}
				case 'N':
					return new Do.AreaData() { No = spl[1], Name = spl[2], Addresses = spl[3].Split(',').ToList() };
				default:
					return null;
			}
		}

		public static List<Do.AreaData> AnalyzeTelegram(Do.AreaTelegram[] telegram) {
			var list = new List<Do.AreaData>();
			var load = new Dictionary<string, List<Do.AreaData>>();
			int idx;

			foreach (var tg in telegram) {
				switch (tg.Function) {
					case Do.AreaFunc.Load:
						load.Add(tg.Arg2, GetAreaDataFromFile(tg.Arg1).ToList());
						break;
					case Do.AreaFunc.Add:
						list.Add(GetAreaByLine(tg.Arg1));
						break;
					case Do.AreaFunc.Addarg:
						list.AddRange(load[tg.Arg1]);
						break;
					case Do.AreaFunc.Addattel:
						idx = list.FindIndex(x => x.No == tg.Arg2);
						if (idx < 0) break;

						if (tg.Arg1 != "bef") idx++;

						list.Insert(idx, GetAreaByLine(tg.Arg3));
						break;
					case Do.AreaFunc.Addatarg:
						idx = list.FindIndex(x => x.No == tg.Arg2);
						if (idx < 0) break;

						if (tg.Arg1 != "bef") idx++;

						list.InsertRange(idx, load[tg.Arg1]);
						break;
					case Do.AreaFunc.Delno:
						if (list.Exists(x => x.No == tg.Arg1)) {
							list.Remove(list.First(x => x.No == tg.Arg1));
						}
						break;
					case Do.AreaFunc.Movat:
						//後日実装
						break;
					case Do.AreaFunc.Movto:
						//後日実装
						break;
					case Do.AreaFunc.Argnox:
						Do.AreaData[] rgx;

						if (tg.Arg2 == "get") {
							rgx = load[tg.Arg1].Where(x => !Regex.IsMatch(x.No, tg.Arg3)).ToArray();
						} else {
							rgx = load[tg.Arg1].Where(x => Regex.IsMatch(x.No, tg.Arg3)).ToArray();
						}

						foreach (var r in rgx) {
							load[tg.Arg1].Remove(r);
						}

						break;
					case Do.AreaFunc.Delstr:
					case Do.AreaFunc.Sort:
						//不使用
						break;
				}
			}

			return list;
		}
	}
}
