using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// JSONファイル読み書き
	/// </summary>
	public class Json {
		/// <summary>
		/// リストをJSON形式に変換します。
		/// </summary>
		/// <param name="data">変換元リスト</param>
		/// <returns>JSON文字列</returns>
		public static string Get<T>(IEnumerable<T> data) {
			return JsonConvert.SerializeObject(data, Formatting.Indented);
		}

		/// <summary>
		/// JSON文字列からリストに変換します。
		/// </summary>
		/// <param name="jsonStr">JSON文字列</param>
		/// <returns>リスト</returns>
		public static IEnumerable<T> Read<T>(string jsonStr) {
			return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonStr);
		}

		/// <summary>
		/// JSONファイルからオブジェクトに変換します。
		/// </summary>
		/// <param name="filename">JSONファイル名</param>
		/// <returns>オブジェクト</returns>
		public static T GetSingle<T>(string filename) {
			return Read<T>(Get(filename)[""]).First();
		}

		/// <summary>
		/// JSONファイルからリストに変換します。
		/// </summary>
		/// <param name="filename">JSONファイル名</param>
		/// <returns>リスト</returns>
		public static IEnumerable<T> GetEnumerable<T>(string filename) {
			return Read<T>(Get(filename)[""]);
		}

		/// <summary>
		/// スクリプトファイルを取得します。
		/// </summary>
		/// <param name="filename">スクリプトファイル名</param>
		/// <returns>スクリプトリスト</returns>
		public static Dictionary<string, string> Get(string filename) {
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

		/// <summary>
		/// JSONデータを保存します。
		/// </summary>
		/// <param name="filepath">保存ファイル名</param>
		/// <param name="data">JSONデータ</param>
		public static void Save(string filepath, string data) {
			using (var sw = new System.IO.StreamWriter(filepath, false, System.Text.Encoding.GetEncoding("shift-jis"))) {
				sw.WriteLine(data);
			}
		}

		/// <summary>
		/// スクリプトを保存します。
		/// </summary>
		/// <param name="filepath">保存ファイル名</param>
		/// <param name="data">スクリプト文字列</param>
		public static void Save(string filepath, Dictionary<string, string> data) {
			using (var sw = new System.IO.StreamWriter(filepath, false, System.Text.Encoding.GetEncoding("shift-jis"))) {
				foreach (var kdt in data) {
					sw.WriteLine("##" + kdt.Key);
					sw.WriteLine(kdt.Value);
				}
			}
		}

		/// <summary>
		/// スクリプトを保存します。
		/// </summary>
		/// <param name="filepath">保存ファイル名</param>
		/// <param name="General">コンテスト設定 (一般)</param>
		/// <param name="Area1">コンテスト設定 (地域1)</param>
		/// <param name="Area2">コンテスト設定 (地域2)</param>
		/// <param name="Checks">コンテスト設定 (確認)</param>
		public static void Save(string filepath, IEnumerable<CheckInfo> General, IEnumerable<AreaTelegram> Area1, IEnumerable<AreaTelegram> Area2, IEnumerable<CheckInfo> Checks) {
			var dic = new Dictionary<string, string> {
				{ "General", Get(General) },
				{ "Area1", Get(Area1) },
				{ "Area2", Get(Area2) },
				{ "Checks", Get(Checks) }
			};

			Save(filepath, dic);
		}
	}
}
