using System;
using System.Collections.Generic;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// CTESTWINログファイル読み込み
	/// </summary>
	public class CtestwinLoad {
		private BinaryRead br;

		/// <param name="path">ファイルパス</param>
		public CtestwinLoad(string path) {
			try {
				//ファイル読み込み
				br = new BinaryRead();
				br.ChangeEncoding("shift-jis");
				br.ReadOpen(path);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// CTESTWINログの読み込みをします。
		/// </summary>
		/// <returns>ログリスト</returns>
		public IEnumerable<CtestwinData> Execute() {
			//読み込み失敗なら終了
			if (br == null) {
				yield break;
			}

			ushort i = 0;
			ushort logCnts = ushort.MaxValue;
			//ログ件数
			logCnts = br.ReadWord();
			br.ReadString(14);

			for (i = 0; i < logCnts; i++) {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
				var data = new CtestwinData();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します
				try {

					data.Callsign = br.ReadString(20);

					data.SentContestNo = br.ReadString(30);

					data.ReceivedContestNo = br.ReadString(30);

					data.Mode = (Modes)br.ReadWord();

					data.Frequency = (Freqs)br.ReadWord();

					br.ReadString(4);

					long num = br.ReadLong();
					data.Date = new System.DateTime(1970, 1, 1).AddSeconds(num).ToLocalTime();

					data.Operator = br.ReadString(20);

					br.ReadString(2);

					data.Rem = br.ReadString(50 + 2);
				} catch (Exception e) {
					Console.WriteLine(i + "件中" + logCnts + "件目 / " + e.Message);
					continue;
				}

				yield return data;
			}
			yield break;
		}
	}
}
