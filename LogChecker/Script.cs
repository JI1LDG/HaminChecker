using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogChecker.Do;
using Newtonsoft.Json;

namespace Utilities {
	public class ScriptIo {
		public static string GetJsonCi<T>(List<T> data) {
			return JsonConvert.SerializeObject(data, Formatting.Indented);
		}

		public static List<T> ReadJsonCi<T>(string jsonStr) {
			return JsonConvert.DeserializeObject<List<T>>(jsonStr);
		}

		public static Dictionary<string, string> GetJsons(string filename) {
			string column = "";
			string data = "";
			Dictionary<string, string> dicJson = new Dictionary<string, string>();
			
			using (var sr = new System.IO.StreamReader(filename, System.Text.Encoding.GetEncoding("shift-jis"))) {
				while (sr.Peek() > 0) {
					string line = sr.ReadLine();

					if (line.Contains("##")) {
						var newcol = line.Substring(2);

						if (data != "") {
							dicJson.Add(column, data);

							data = "";
						}

						column = newcol;
					} else {
						data += line;
					}
				}

				if (data != "") {
					dicJson.Add(column, data);
				}
			}

			return dicJson;
		}

		public static void SaveJsons(string filename, Dictionary<string, string> data) {
			using (var sw = new System.IO.StreamWriter(filename, false, System.Text.Encoding.GetEncoding("shift-jis"))) {
				foreach(var kdt in data) {
					sw.WriteLine("##" + kdt.Key);
					sw.WriteLine(kdt.Value);
				}
			}
		}
	}
}
