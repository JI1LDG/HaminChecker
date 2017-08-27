using System;
using System.Collections.Generic;

namespace InfoGetter {
	/// <summary>
	/// 無線局をDBとWebから検索します。
	/// </summary>
	public class SearchStations {
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
			this.dbPath = dbPath;
			stdTime = standardTime;

			dss = new DbSearchStation(this.dbPath, stdTime);
			wss = new WebSearchStation();
		}

		/// <summary>
		/// DBとWebから検索します。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		public void Execute(string callsign) {
			if(!dss.Get(callsign)) {
				wss.Execute(callsign);
			}
		}

		/// <summary>
		/// Webから取得した情報をDBに反映します。
		/// </summary>
		public void DbUpdate() {
			wss.GottenData.ForEach(x => dss.Update(x));
			dss.GetAll();
			GottenData = dss.GottenData;
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void Dispose() {
			dss?.Dispose();
		}
	}
}
