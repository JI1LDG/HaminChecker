namespace LogLoader.Do {
	/// <summary>
	/// CTESTWINログデータクラス
	/// </summary>
	public class CtestwinData {
		public string Callsign { get; set; }
		/// <summary>
		/// My/ 送信コンテストナンバ
		/// </summary>
		public string SentContestNo { get; set; }
		/// <summary>
		/// Ur/ 受信コンテストナンバ
		/// </summary>
		public string ReceivedContestNo { get; set; }
		public Utilities.Modes Mode { get; set; }
		public Utilities.Freqs Frequency { get; set; }
		public System.DateTime Date { get; set; }
		public string Operator { get; set; }
		public string Rem { get; set; }
	}
}
