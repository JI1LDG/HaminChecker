using System;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// 無線局情報検索
	/// </summary>
	public class SearchStation {
		private DbSearch ds;

		/// <param name="dbPath">DBパス</param>
		/// <param name="standardTime">無線局情報再取得基準時間</param>
		public SearchStation(string dbPath, DateTime standardTime) {
			ds = new DbSearch(dbPath, standardTime);
		}

		~SearchStation() {
			ds.Dispose();
		}

		/// <summary>
		/// 無線局情報を検索します。
		/// </summary>
		/// <param name="callsign">コールサイン</param>
		/// <param name="isTimeCheck">基準時間確認するか</param>
		/// <returns></returns>
		public StationInfo Execute(string callsign, bool isTimeCheck = true) {
			StationInfo si;

			si = ds.Execute(callsign, isTimeCheck);
			if (si == null) {
				si = WebSearch.Execute(callsign);
				if (si != null) {
					ds.Update(si);
				}
			}

			return si;
		}
	}
}
