using System.Collections.Generic;

namespace HaminChecker.Declares {
	/// <summary>
	/// 地域情報 (住所カンマ区切り)
	/// </summary>
	public class SingleAreaData {
		/// <summary>
		/// 地域番号
		/// </summary>
		public string No { get; set; }

		/// <summary>
		/// 地域名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 住所 (カンマ区切り)
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// AreaDataリストからSingleAreaDataリストへ変換
		/// </summary>
		/// <param name="data">変換元AreaDataリスト</param>
		/// <returns>SingleAreaDataリスト</returns>
		public static IEnumerable<SingleAreaData> FromLists(IEnumerable<AreaData> data) {
			foreach (var listdata in data) {
				yield return new SingleAreaData() { No = listdata.No, Name = listdata.Name, Address = string.Join(", ", listdata.Addresses) };
			}
		}
	}
}
