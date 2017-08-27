using System;
using System.Collections.Generic;

namespace LogLoader {
	/// <summary>
	/// CTESTWINログファイル読み込みクラス
	/// </summary>
	public class CtestwinLoader {

		private FileReader.BinaryReader br;
		public List<Do.CtestwinData> LoadData { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">ファイルパス</param>
		public CtestwinLoader(string path) {
			try {
				//ファイル読み込み
				br = new FileReader.BinaryReader(path);

				//リスト準備
				LoadData = new List<Do.CtestwinData>();
			} catch(System.Exception e) {
				Console.WriteLine(e.Message);
			}
		}

		/// <summary>
		/// CTESTWINログの読み込みをします。
		/// </summary>
		public bool Execute() {
			//読み込み失敗なら終了
			if(br == null) {
				return false;
			}

			ushort i = ushort.MaxValue;
			ushort logCnts = ushort.MaxValue;
			try {
				//ログ件数
				logCnts = br.ReadWord();
				br.ReadBytes(14);

				for(i = 0;i < logCnts;i++) {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
					var data = new Do.CtestwinData();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

					data.Callsign = br.ReadString(20);

					data.ReceivedContestNo = br.ReadString(30);

					data.SentContestNo = br.ReadString(30);

					data.Mode = (Utilities.Modes)br.ReadWord();

					data.Frequency = (Utilities.Freqs)br.ReadWord();

					br.ReadBytes(4);

					data.Date = new System.DateTime(1970, 1, 1).AddSeconds(br.ReadLong()).ToLocalTime();

					data.Operator = br.ReadString(20);

					br.ReadBytes(2);

					data.Rem = br.ReadString(50 + 2);

					LoadData.Add(data);
				}
			} catch(System.Exception e) {
				Console.WriteLine(i + "件中" + logCnts + "件目 / " + e.Message);
				return false;
			}
			return true;
		}
	}
}
