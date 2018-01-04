using System.Collections.Generic;

namespace HaminChecker.Declares {
	/// <summary>
	/// 地域情報 (複数住所)
	/// </summary>
	public class AreaData {
		/// <summary>
		/// 地域番号
		/// </summary>
		public string No { get; set; }

		/// <summary>
		/// 地域名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 住所 (リスト)
		/// </summary>
		public List<string> Addresses { get; set; }

		public AreaData() {
			Addresses = new List<string>();
		}
	}
}
