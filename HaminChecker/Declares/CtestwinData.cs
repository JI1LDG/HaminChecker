using System;

namespace HaminChecker.Declares {
	/// <summary>
	/// CTESTWINログデータ
	/// </summary>
	public class CtestwinData {
		/// <summary>
		/// コールサイン
		/// </summary>
		public string Callsign { get; set; }

		/// <summary>
		/// My/ 自局CN
		/// </summary>
		public string SentContestNo { get; set; }

		/// <summary>
		/// Ur/ 相手CN
		/// </summary>
		public string ReceivedContestNo { get; set; }

		/// <summary>
		/// モード
		/// </summary>
		public Modes Mode { get; set; }

		/// <summary>
		/// 周波数
		/// </summary>
		public Freqs Frequency { get; set; }

		/// <summary>
		/// 交信日時
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// オペレータ
		/// </summary>
		public string Operator { get; set; }

		/// <summary>
		/// コメント
		/// </summary>
		public string Rem { get; set; }
	}
}
