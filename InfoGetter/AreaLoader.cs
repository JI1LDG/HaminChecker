using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using NLog;

namespace InfoGetter {
	/// <summary>
	/// 地域情報取得クラス
	/// </summary>
	public class AreaLoader {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		//地域リスト
		public List<Do.AreaData> Areas { get; set; }

		public AreaLoader() {
			logger.Debug("Start");
			Areas = new List<Do.AreaData>();
			logger.Debug("End");
		}

		/// <summary>
		/// 地域リストを定義する電文を解析します。
		/// </summary>
		/// <param name="data">電文定義ファイル文字列</param>
		public void AnalyzeTelegram(string data) {
			logger.Debug("Start");
			var lines = data.Replace("\r\n", "\n").Split('\n');
			for(int i = 0;i < lines.Length; i++) {
				var spl = lines[i].Split(' ');
				switch(spl[0]) {
					case "load":
						logger.Debug("Command: load");
						//load Prefectures
						if(spl.Length > 2 - 1) LoadCmd(spl[1]);
						break;
					case "remove":
						logger.Debug("Command: remove");
						//remove no 01
						if(spl.Length > 3 - 1 && spl[1] == "no") {
							RemoveAreaWithNo(spl[2]);
						}
						break;
					case "add":
						logger.Debug("Command: add");
						//add on top/bottom
						if(spl.Length > 3 - 1 && spl[1] == "on") {
							AddOn(lines, ref i, spl[2]);
						//add no bef 10
						} else if(spl.Length > 4 - 1 && spl[1] == "no") {
							AddAt(lines, ref i, spl[2], spl[3]);
						}
						break;
					case "addpre":
						logger.Debug("Command: addpre");
						//addpre on top/bottom st 11 of JCC
						if(spl.Length > 7 - 1 && spl[1] == "on") {
							AddpreOn(spl[2], spl[3], spl[4], spl[6]);
						//addpre no aft 48 st 11 of JCC
						} else if(spl.Length > 7 - 1 && spl[1] == "no") {
							AddpreAt(spl[2], spl[3], spl[4], spl[5], spl[7]);
						}
						break;
				}
			}
			logger.Debug("End");
		}

		/// <summary>
		/// loadコマンドを実行します。
		/// </summary>
		/// <param name="name">ファイルパス</param>
		private void LoadCmd(string name) {
			logger.Debug("Start(name: " + name + ")");
			if(System.IO.File.Exists(name + ".area.txt")) {
				logger.Debug("ファイル(" + name + ".area.txt)存在");
				Areas.AddRange(FromFile(name + ".area.txt"));
			} else {
				logger.Info(name + ".area.txtは存在しません");
			}
			logger.Debug("End");
		}

		/// <summary>
		/// end句まで地域情報の読み込み処理をします。
		/// </summary>
		/// <param name="lines">定義ファイル文字列</param>
		/// <param name="at">現在行番号</param>
		private List<Do.AreaData> GetTillEnd(string[] lines, ref int at) {
			logger.Debug("Start");
			var adds = new List<Do.AreaData>();

			while(at < lines.Length) {
				var line = lines[at];
				if(line == "end") break;

				var item = GetAreaByLine(line);
				if(item != null) adds.Add(item);
				at++;
			}

			logger.Debug("End");
			return adds;
		}

		/// <summary>
		/// 地域リストから指定した地域番号に対応する地域リストの要素番号を取得します。
		/// </summary>
		/// <param name="no">地域番号</param>
		private int GetAtByNo(string no) {
			logger.Debug("Execute(no: " + no + ")");
			for(int i = 0; i < Areas.Count; i++) {
				if(Areas[i].No == no) {
					return i;
				}
			}

			logger.Info("地域番号は存在しませんでした");
			return -1;
		}

		/// <summary>
		/// 地域フォーマットから作成した地域情報を地域リストに挿入します。
		/// </summary>
		/// <param name="lines">定義ファイル文字列</param>
		/// <param name="at">現在行番号</param>
		/// <param name="slTopBottom">リストの始めか終りか</param>
		private void AddOn(string[] lines, ref int at, string slTopBottom) {
			logger.Debug("Start");
			var adds = GetTillEnd(lines, ref at);

			if(slTopBottom == "top") Areas.InsertRange(0, adds);
			else if(slTopBottom == "bottom") Areas.AddRange(adds);
			logger.Debug("End");
		}

		/// <summary>
		/// 地域フォーマットから作成した地域情報を地域リストの指定した位置に挿入します。
		/// </summary>
		/// <param name="lines">定義ファイル文字列</param>
		/// <param name="at">現在行番号</param>
		/// <param name="slBefAft">前か後ろか</param>
		/// <param name="no">挿入位置番号</param>
		private void AddAt(string[] lines, ref int at, string slBefAft, string no) {
			logger.Debug("Start");
			var addAt = GetAtByNo(no);
			if(addAt >= 0) {
				var adds = GetTillEnd(lines, ref at);
				if(slBefAft == "bef") Areas.InsertRange(addAt, adds);
				else if(slBefAft == "aft") Areas.InsertRange(addAt + 1, adds);
			} else {
				if(slBefAft == "bef") AddOn(lines, ref at, "top");
				else if(slBefAft == "aft") AddOn(lines, ref at, "bottom");
			}
			logger.Debug("End");
		}

		/// <summary>
		/// 指定したファイルの指定した条件に一致する地域情報を取得します。
		/// </summary>
		/// <param name="slStEd">検索条件(開始/終了)</param>
		/// <param name="whereNo">検索番号</param>
		/// <param name="file">ファイルパス</param>
		/// <returns></returns>
		private List<Do.AreaData> GetPreMatchData(string slStEd, string whereNo, string file) {
			logger.Debug("Start(slStEd: " + slStEd + ", whereNo: " + whereNo + ", file: " + file + ")");
			var pre = FromFile(file + ".area.txt");
			List<Do.AreaData> found = null;
			switch(slStEd) {
				case "st":
					found = pre.Where(x => Regex.IsMatch(x.No, "^" + whereNo)).ToList();
					break;
				case "ed":
					found = pre.Where(x => Regex.IsMatch(x.No, whereNo + "$")).ToList();
					break;
			}

			logger.Debug("End");
			return pre;
		}

		/// <summary>
		/// 指定したファイルの指定した条件に一致する地域情報を地域リストに挿入します。
		/// </summary>
		/// <param name="slTopBottom">リストの始めか終りか</param>
		/// <param name="slStEd">検索条件(開始/終了)</param>
		/// <param name="whereNo">検索番号</param>
		/// <param name="file">ファイルパス</param>
		private void AddpreOn(string slTopBottom, string slStEd, string whereNo, string file) {
			logger.Debug("Start(slTopBottom: " + slTopBottom + ", slStEd: " + slStEd + ", whereNo: " + whereNo + ", file: " + file + ")");
			var found = GetPreMatchData(slStEd, whereNo, file);

			if(found != null) {
				if(slTopBottom == "top") Areas.InsertRange(0, found);
				else if(slTopBottom == "bottom") Areas.AddRange(found);
			}

			logger.Debug("End");
		}

		/// <summary>
		/// 指定したファイルの指定した条件に一致する地域情報を地域リストの指定した位置に挿入します。
		/// </summary>
		/// <param name="slBefAft">前か後ろか</param>
		/// <param name="no">挿入位置番号</param>
		/// <param name="slStEd">検索条件(開始/終了)</param>
		/// <param name="whereNo">検索番号</param>
		/// <param name="file">ファイルパス</param>
		private void AddpreAt(string slBefAft, string no, string slStEd, string whereNo, string file) {
			logger.Debug("Start(slBefAft: " + slBefAft + ", no: " + no + ", slStEd: " + slStEd + ", whereNo: " + whereNo + ", file: " + file + ")");
			var addAt = GetAtByNo(no);

			if(addAt >= 0) {
				var found = GetPreMatchData(slStEd, whereNo, file);

				if(found != null) {
					if(slBefAft == "bef") Areas.InsertRange(addAt, found);
					else if(slBefAft == "aft") Areas.InsertRange(addAt + 1, found);
				}
			} else {
				if(slBefAft == "bef") AddpreOn("top", slStEd, whereNo, file);
				else if(slBefAft == "aft") AddpreOn("bottom", slStEd, whereNo, file);
			}

			logger.Debug("End");
		}

		/// <summary>
		/// ファイルから地域情報を取得します。
		/// </summary>
		/// <param name="path">ファイルパス</param>
		private List<Do.AreaData> FromFile(string path) {
			logger.Debug("Start(path: " + path + ")");
			var ret = new List<Do.AreaData>();
			using(var sr = new System.IO.StreamReader(path, System.Text.Encoding.GetEncoding("shift-jis"))) {
				while(sr.Peek() > 0) {
					var item = GetAreaByLine(sr.ReadLine());
					if(item != null) ret.Add(item);
				}
			}

			logger.Debug("End");
			return ret;
		}

		/// <summary>
		/// 地域フォーマットから地域情報を取得します。
		/// </summary>
		/// <param name="line">取得元文字列</param>
		private Do.AreaData GetAreaByLine(string line) {
			logger.Debug("Execute(line: " + line + ")");
			var buf = line;
			var spl = buf.Split(' ');
			switch(buf[0]) {
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

		/// <summary>
		/// 地域リストから地域番号を指定して削除します。
		/// </summary>
		/// <param name="removeNo">削除する地域番号</param>
		public void RemoveAreaWithNo(string removeNo) {
			logger.Debug("Start(removeNo: " + removeNo + ")");
			Areas = Areas.Where(x => x.No != removeNo).ToList();
			logger.Debug("End");
		}

		/// <summary>
		/// 地域リストの情報を出力します。
		/// </summary>
		public void PrintAll() {
			Areas.ForEach(x => System.Console.WriteLine(string.Join(" ", new string[] { x.Name, x.No, string.Join(",", x.Addresses) })));
		}
	}
}
