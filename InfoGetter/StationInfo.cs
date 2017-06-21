namespace InfoGetter.Do {
	/// <summary>
	/// Webから取得した無線局情報クラス
	/// </summary>
	public class StationInfo {
		public string Callsign { get; set; }
		public System.Collections.Generic.List<string> Address { get; set; }

		public StationInfo() {
			Address = new System.Collections.Generic.List<string>();
		}

		/// <summary>
		/// 文字列に変換します。
		/// </summary>
		public override string ToString() {
			return "Callsign: " + Callsign + ", Addresses: " + string.Join(", ", Address);
		}
	}

}
