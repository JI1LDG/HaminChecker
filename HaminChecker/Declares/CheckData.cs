using System.Collections.Generic;

namespace HaminChecker.Declares {
	/// <summary>
	/// ログ確認用データ
	/// </summary>
	public class CheckData {
		/// <summary>
		/// ログリスト
		/// </summary>
		public List<LogData> Logs;

		/// <summary>
		/// 設定タブ (一般)
		/// </summary>
		public ConfigGeneral ConGen;

		/// <summary>
		/// 設定タブ (固定)
		/// </summary>
		public ConfigSolid ConSol;

		/// <summary>
		/// コンテスト設定 (一般)
		/// </summary>
		public List<CheckInfo> Gens;

		/// <summary>
		/// コンテスト設定 (確認)
		/// </summary>
		public List<CheckInfo> Ches;

		/// <summary>
		/// コンテスト設定 (地域1)
		/// </summary>
		public List<AreaData> Area1;

		/// <summary>
		/// コンテスト設定 (地域2)
		/// </summary>
		public List<AreaData> Area2;
	}
}
