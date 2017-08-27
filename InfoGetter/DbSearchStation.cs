using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

using Utilities;
using System;

namespace InfoGetter {
	/// <summary>
	/// 無線局検索クラス
	/// </summary>
	public class DbSearchStation {

		public List<Do.StationInfo> GottenData { get; private set; }
		SQLiteConnection conn;
		DateTime StandardTime;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">DBパス</param>
		/// <param name="standard">無線局情報を再取得する基準時間</param>
		public DbSearchStation(string path, DateTime standard) {
			try {
				conn = new SQLiteConnection("Data Source=" + path);
			} catch(Exception e) {
				throw e;
			}
			conn.Open();

			StandardTime = standard;
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void Dispose() {
			conn?.Close();
			conn?.Dispose();
		}

		/// <summary>
		/// DBから無線局情報を検索します。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		public bool Get(string callsign) {
			using(SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + callsign + "';";
				using(var reader = cmd.ExecuteReader()) {
					if(reader.Read()) {
						var updated = DateUtils.FromString(reader["Updated"].ToString());
						if(updated > StandardTime) {
							return true;
						} else {
							return false;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// DBに無線局情報を登録します。
		/// </summary>
		/// <param name="station">登録する無線局情報</param>
		public void Update(Do.StationInfo station) {
			bool hasRows;
			using(SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + station.Callsign + "';";
				using(var reader = cmd.ExecuteReader()) {
					hasRows = reader.HasRows;
				}
				if(hasRows) {
					cmd.CommandText = "update Station set Addresses = '" + string.Join(",", station.Address.ToArray()) + "', Updated = '" + DateUtils.NowToString() + "' where Callsign = '" + station.Callsign + "';";
					cmd.ExecuteNonQuery();
				} else {
					cmd.CommandText = "insert into Station(Callsign, Addresses, Updated) values('" + station.Callsign + "', '" + string.Join(",", station.Address.ToArray()) + "', '" + DateUtils.NowToString() + "');";
					cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// DBから全無線局の情報を取得します。
		/// </summary>
		public void GetAll() {
			GottenData = new List<Do.StationInfo>();

			using(SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station;";
				using(var reader = cmd.ExecuteReader()) {
					while(reader.Read()) {
						GottenData.Add(new Do.StationInfo() {
							Callsign = reader["Callsign"].ToString(),
							Address = reader["Addresses"].ToString().Split(',').ToList()
						});
					}
				}
			}
		}
	}
}
