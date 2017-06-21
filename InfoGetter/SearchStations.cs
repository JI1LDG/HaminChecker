using System;
using System.Collections.Generic;

using NLog;

namespace InfoGetter {
	/// <summary>
	/// 無線局をDBとWebから検索します。
	/// </summary>
	public class SearchStations {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public List<Do.StationInfo> GottenData { get; private set; }

		private string dbPath;
		private DateTime stdTime;
		private DbSearchStation dss;
		private WebSearchStation wss;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dbPath">DBパス</param>
		/// <param name="standardTime">無線局情報を再取得する基準時間</param>
		public SearchStations(string dbPath, DateTime standardTime) {
			logger.Debug("Start(dbPath: " + dbPath + ", standardTime: " + standardTime.ToString() + ")");
			this.dbPath = dbPath;
			stdTime = standardTime;

			dss = new DbSearchStation(this.dbPath, stdTime);
			wss = new WebSearchStation();
			logger.Debug("End");
		}

		/// <summary>
		/// DBとWebから検索します。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		public void Execute(string callsign) {
			logger.Debug("Start");
			logger.Debug("DBから取得開始");
			if(!dss.Get(callsign)) {
				logger.Debug("Webから取得開始");
				wss.Execute(callsign);
			}
			logger.Debug("End");
		}

		/// <summary>
		/// Webから取得した情報をDBに反映します。
		/// </summary>
		public void DbUpdate() {
			logger.Debug("Start");
			wss.GottenData.ForEach(x => dss.Update(x));
			dss.GetAll();
			GottenData = dss.GottenData;
			logger.Debug("End");
			logger.Info("取得無線局情報反映");
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void Dispose() {
			logger.Debug("Execute");
			dss?.Dispose();
			logger.Info("後処理: DB切断");
		}
	}
}
