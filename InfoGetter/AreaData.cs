using System.Collections.Generic;

namespace InfoGetter.Do {
	/// <summary>
	/// 地域番号と相当する住所の一覧クラス
	/// </summary>
	public class AreaData {
		public string No { get; set; }
		public string Name { get; set; }
		public List<string> Addresses { get; set; }

		public AreaData() {
			Addresses = new List<string>();
		}
	}
}
