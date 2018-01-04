using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// DB検索
	/// </summary>
	public class DbSearch : IDisposable {
		SQLiteConnection conn;
		DateTime StandardTime;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">DBパス</param>
		/// <param name="standard">無線局情報を再取得する基準時間</param>
		public DbSearch(string path, DateTime standard) {
			try {
				conn = new SQLiteConnection("Data Source=" + path);
			} catch (Exception e) {
				throw e;
			}
			conn.Open();

			StandardTime = standard;
		}

		public void Dispose() {
			try {
				conn?.Close();
				conn?.Dispose();
			} catch {
				;
			}
		}

		/// <summary>
		/// DBから無線局情報を検索します。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		/// <param name="isTimeCheck">基準時間確認するか</param>
		public StationInfo Execute(string callsign, bool isTimeCheck) {
			using (SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + callsign + "';";
				using (var reader = cmd.ExecuteReader()) {
					if (reader.Read()) {
						var updated = Date.FromString(reader["Updated"].ToString());
						if (!isTimeCheck || (isTimeCheck && updated > StandardTime)) {
							return new StationInfo() {
								Callsign = reader["Callsign"].ToString(),
								Address = reader["Addresses"].ToString().Split(',').ToList()
							};
						} else {
							return null;
						}
					}
				}
			}

			return null;
		}

		/// <summary>
		/// DBに無線局情報を登録します。
		/// </summary>
		/// <param name="station">登録する無線局情報</param>
		public void Update(StationInfo station) {
			bool hasRows;
			using (SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + station.Callsign + "';";
				using (var reader = cmd.ExecuteReader()) {
					hasRows = reader.HasRows;
				}
				if (hasRows) {
					cmd.CommandText = "update Station set Addresses = '" + string.Join(",", station.Address.ToArray()) + "', Updated = '" + Date.NowToString() + "' where Callsign = '" + station.Callsign + "';";
					cmd.ExecuteNonQuery();
				} else {
					cmd.CommandText = "insert into Station(Callsign, Addresses, Updated) values('" + station.Callsign + "', '" + string.Join(",", station.Address.ToArray()) + "', '" + Date.NowToString() + "');";
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
