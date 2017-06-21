using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

using Utilities;
using NLog;
using System;

namespace InfoGetter {
	/// <summary>
	/// 無線局検索クラス
	/// </summary>
	public class DbSearchStation {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public List<Do.StationInfo> GottenData { get; private set; }
		SQLiteConnection conn;
		DateTime StandardTime;
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">DBパス</param>
		/// <param name="standard">無線局情報を再取得する基準時間</param>
		public DbSearchStation(string path, DateTime standard) {
			logger.Debug("Start(path: " + path + ", standard: " + standard.ToString() + ")");
			try {
				conn = new SQLiteConnection("Data Source=" + path);
			} catch(Exception e) {
				logger.Error(e, e.Message);
				throw e;
			}
			conn.Open();
			logger.Debug("DB接続開始");

			StandardTime = standard;
			logger.Debug("End");
		}

		/// <summary>
		/// 後処理
		/// </summary>
		public void Dispose() {
			logger.Debug("Execute");
			conn?.Close();
			conn?.Dispose();
			logger.Info("後処理: 切断");
		}

		/// <summary>
		/// DBから無線局情報を検索します。
		/// </summary>
		/// <param name="callsign">検索コールサイン</param>
		public bool Get(string callsign) {
			logger.Debug("Execute(callsign: " + callsign + ")");
			using(SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + callsign + "';";
				using(var reader = cmd.ExecuteReader()) {
					if(reader.Read()) {
						var updated = DateUtils.FromString(reader["Updated"].ToString());
						if(updated > StandardTime) {
							logger.Debug("登録あり(期限内)");
							return true;
						} else {
							logger.Debug("登録あり(期限切れ)");
							return false;
						}
					}
				}
			}

			logger.Debug("登録なし");
			return false;
		}

		/// <summary>
		/// DBに無線局情報を登録します。
		/// </summary>
		/// <param name="station">登録する無線局情報</param>
		public void Update(Do.StationInfo station) {
			logger.Debug("Start(station: [" + station.ToString() + "])");
			bool hasRows;
			using(SQLiteCommand cmd = conn.CreateCommand()) {
				cmd.CommandText = "select * from Station where Callsign = '" + station.Callsign + "';";
				using(var reader = cmd.ExecuteReader()) {
					hasRows = reader.HasRows;
				}
				if(hasRows) {
					cmd.CommandText = "update Station set Addresses = '" + string.Join(",", station.Address.ToArray()) + "', Updated = '" + DateUtils.NowToString() + "' where Callsign = '" + station.Callsign + "';";
					cmd.ExecuteNonQuery();
					logger.Debug("住所追加");
				} else {
					cmd.CommandText = "insert into Station(Callsign, Addresses, Updated) values('" + station.Callsign + "', '" + string.Join(",", station.Address.ToArray()) + "', '" + DateUtils.NowToString() + "');";
					cmd.ExecuteNonQuery();
					logger.Debug("新規追加");
				}
			}
			logger.Debug("End");
		}

		/// <summary>
		/// DBから全無線局の情報を取得します。
		/// </summary>
		public void GetAll() {
			logger.Debug("Start");
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
			logger.Info("後処理: 無線局情報リスト");
			logger.Debug("End");
		}
	}
}
