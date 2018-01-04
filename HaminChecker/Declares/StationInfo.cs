namespace HaminChecker.Declares {
	/// <summary>
	/// 無線局情報
	/// </summary>
	public class StationInfo {
		/// <summary>
		/// コールサイン
		/// </summary>
		public string Callsign { get; set; }

		/// <summary>
		/// 住所リスト
		/// </summary>
		public System.Collections.Generic.List<string> Address { get; set; }

		public StationInfo() {
			Address = new System.Collections.Generic.List<string>();
		}

		/// <summary>
		/// "Callsign: [Callsign], Addresses: [Address](, [Address])"
		/// </summary>
		public override string ToString() {
			return "Callsign: " + Callsign + ", Addresses: " + string.Join(", ", Address);
		}
	}

}
