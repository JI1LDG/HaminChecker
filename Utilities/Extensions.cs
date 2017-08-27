using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities {
	/// <summary>
	/// 拡張メソッド群クラス
	/// </summary>
	public static class Extensions {
		/// <summary>
		/// 文字列コレクションを数字化して返却します。
		/// </summary>
		/// <param name="src">対象コレクション</param>
		public static IEnumerable<int> IntParse(this IEnumerable<string> src) {
			foreach(var s in src) {
				if(int.TryParse(s, out int itp)) {
					yield return itp;
				}
			}
		}
	}
}
