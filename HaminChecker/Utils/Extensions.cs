using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaminChecker.Utils {
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

		/// <summary>
		/// 文字列の真偽判定
		/// </summary>
		/// <param name="src">文字列</param>
		/// <returns>真偽</returns>
		public static bool ToBool(this string src) {
			if (src == true.ToString()) {
				return true;
			} else {
				return false;
			}
		}

		/// <summary>
		/// 空か判定する
		/// </summary>
		/// <param name="src">文字列</param>
		/// <returns>空か否か</returns>
		public static bool IsEmpty(this string src) {
			if (src == null || src.Trim() == "" || src == "<Empty>") {
				return true;
			}

			return false;
		}
	}
}
