using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utilities;

namespace InfoGetter {
	/// <summary>
	/// 無線局の検索クラス
	/// </summary>
	public class WebSearchStation {
		public List<Do.StationInfo> GottenData { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public WebSearchStation() {
			GottenData = new List<Do.StationInfo>();
		}

		/// <summary>
		/// コールサインから無線機常置場所と詳細情報を閲覧できるURLの取得をします。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		public void Execute(string callsign) {
			System.Net.WebClient wc = new System.Net.WebClient();

			var url = "http://www.tele.soumu.go.jp/musen/SearchServlet?SC=1&pageID=3&SelectID=1&CONFIRM=0&SelectOW=01&HZ=3&MA=" + callsign + "&SK=4&DC=500";
			var source = wc.DownloadString(url);
			//結果なし
			if(source.Contains("検索結果が0件です。")) {
				return;
			}

			//件数表示確認
			int idx = source.IndexOf("<B>検索結果件数</B>&nbsp;&nbsp;");
			if(idx <= 0) {
				return;
			}

			try {
				do {
					//件数取得
					source = source.Substring(idx);
					string cnt = source.Substring(0, source.IndexOf("\n"));

					//データ開始まで
					source = source.Substring(source.IndexOf("免許の年月日"));

					//テーブルの最後まで
					string table = source.Substring(0, source.IndexOf("</table>"));

					//テーブル以降
					source = source.Substring(table.Length);

					while(table.IndexOf("<tr>") >= 0) {
						//行開始
						table = table.Substring(table.IndexOf("<tr>"));

						//アドレス取得
						table = table.Substring(table.IndexOf("<a href=\".") + 10);
						string address = table.Substring(0, table.IndexOf("\""));

						//コールサイン取得
						table = table.Substring(table.IndexOf(">") + 1);
						string name = table.Substring(0, table.IndexOf("</a>"));
						name = name.Substring(0, name.LastIndexOf("）"));
						name = name.Substring(name.LastIndexOf("（") + 1);

						//常置場所取得
						table = table.Substring(table.IndexOf("<td >") + 5).Trim();
						string at = table.Substring(0, table.IndexOf("<br>"));

						//次
						table = table.Substring(table.IndexOf("</tr>"));

						//発見時
						if(name == callsign) {
							//リストに既にある場合
							if(GottenData.Exists(x => x.Callsign == name)) {
								GottenData.Find(x => x.Callsign == name).Address.Add(at);
							} else {
								var wsi = new Do.StationInfo() {
									Callsign = name,
									Address = new List<string>() { at }
								};
								GottenData.Add(wsi);
							}
						}
					}

					//「次へ」リンクあるか
					if(source.IndexOf("alt=\"次へ\">次へ</a>") > 0) {
						//移動テーブルのみ抜き出し
						source = source.Substring(source.IndexOf(">前へ"));
						source = source.Substring(0, source.IndexOf("</table>"));

						//リンクを辿る
						while(source.IndexOf("<a ") > 0) {
							//リンクの1つを抽出
							source = source.Substring(source.IndexOf("<a "));
							string link = source.Substring(0, source.IndexOf("</a>"));
							source = source.Substring(link.Length);

							//次じゃなかったらやり直し
							if(!link.Contains("次へ")) continue;

							//アドレス抜き出し
							link = link.Substring(link.IndexOf("href=\"") + 7);
							link = link.Substring(0, link.IndexOf("\"><img src="));
							Console.WriteLine("next: " + link);

							//次ページ取得
							source = wc.DownloadString("http://www.tele.soumu.go.jp/musen" + link);
							idx = source.IndexOf("<B>検索結果件数</B>&nbsp;&nbsp;");
							break;
						}
					}
				} while(idx > 0);
			} catch(Exception e) {
				if(!(e is ArgumentOutOfRangeException)) {
				}
			}

		}

		/// <summary>
		/// 無線局情報リスト内の住所リストから重複するデータを削除します。
		/// </summary>
		public void Distinct() {
			GottenData.ForEach(x => x.Address = x.Address.Distinct().ToList());
		}
	}
}
