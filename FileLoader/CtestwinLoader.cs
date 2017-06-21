using System.Collections.Generic;
using NLog;

namespace LogLoader {
	/// <summary>
	/// CTESTWINログファイル読み込みクラス
	/// </summary>
	public class CtestwinLoader {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private FileReader.BinaryReader br;
		public List<Do.CtestwinData> LoadData { get; private set; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">ファイルパス</param>
		public CtestwinLoader(string path) {
			logger.Debug("Start(path: " + path + ")");
			try {
				//ファイル読み込み
				br = new FileReader.BinaryReader(path);

				//リスト準備
				LoadData = new List<Do.CtestwinData>();
			} catch(System.Exception e) {
				logger.Error(e, e.Message);
			}
			logger.Debug("End");
		}

		/// <summary>
		/// CTESTWINログの読み込みをします。
		/// </summary>
		public bool Execute() {
			logger.Debug("Start");
			//読み込み失敗なら終了
			if(br == null) {
				logger.Error("読み込み失敗");
				return false;
			}

			ushort i = ushort.MaxValue;
			ushort logCnts = ushort.MaxValue;
			try {
				logger.Info("CTESTWINログ読み込み開始");
				//ログ件数
				logCnts = br.ReadWord();
				logger.Info("全" + logCnts + "件");
				br.ReadBytes(14);

				for(i = 0;i < logCnts;i++) {
					logger.Debug((i + 1) + "件目");
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
					var data = new Do.CtestwinData();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

					data.Callsign = br.ReadString(20);
					logger.Debug("コールサイン: " + data.Callsign);

					data.ReceivedContestNo = br.ReadString(30);
					logger.Debug("受信コンテストナンバ: " + data.ReceivedContestNo);

					data.SentContestNo = br.ReadString(30);
					logger.Debug("送信コンテストナンバ: " + data.SentContestNo);

					data.Mode = (Utilities.Modes)br.ReadWord();
					logger.Debug("モード: " + data.Mode);

					data.Frequency = (Utilities.Freqs)br.ReadWord();
					logger.Debug("周波数: " + data.Frequency);

					br.ReadBytes(4);

					data.Date = new System.DateTime(1970, 1, 1).AddSeconds(br.ReadLong()).ToLocalTime();
					logger.Debug("日時: " + data.Date.ToString());

					data.Operator = br.ReadString(20);
					logger.Debug("オペレータ: " + data.Operator);

					br.ReadBytes(2);

					data.Rem = br.ReadString(50 + 2);
					logger.Debug("コメント: " + data.Rem);

					LoadData.Add(data);
				}
			} catch(System.Exception e) {
				logger.Error(e, i + "件中" + logCnts + "件目 / " + e.Message);
				return false;
			}

			logger.Debug("End");
			return true;
		}
	}
}
