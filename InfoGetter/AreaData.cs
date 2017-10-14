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

	public class SingleAreaData {
		public string No { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }

		public static IEnumerable<SingleAreaData> GetFromLists(IEnumerable<AreaData> data) {
			foreach (var listdata in data) {
				yield return new SingleAreaData() { No = listdata.No, Name = listdata.Name, Address = string.Join(", ", listdata.Addresses) };
			}
		}
	}
}
