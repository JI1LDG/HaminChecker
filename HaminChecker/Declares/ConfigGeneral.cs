using System.Collections.Generic;
using System.Linq;

namespace HaminChecker.Declares {
	/// <summary>
	/// コンテスト設定 (一般)
	/// </summary>
	public class ConfigGeneral {
		/// <summary>
		/// コンテスト名
		/// </summary>
		public string ContestName;

		/// <summary>
		/// カテゴリ
		/// </summary>
		public string Category;

		/// <summary>
		/// 電力記号番号
		/// </summary>
		public int PowerModeArg;

		/// <summary>
		/// 局種係数
		/// </summary>
		public int Coeff;

		/// <summary>
		/// 通常コンテストナンバ
		/// </summary>
		public string MainCn;

		/// <summary>
		/// 追加コンテストナンバ
		/// </summary>
		public string SubCn;

		/// <summary>
		/// 電力値
		/// </summary>
		public int PowerValue;

		/// <summary>
		/// オペレータ
		/// </summary>
		public string Operators;

		/// <summary>
		/// オペレータリストを自動的に作成するか
		/// </summary>
		public bool AutoOperator;

		public ConfigGeneral() {
			PowerModeArg = -1;
			Coeff = -1;
			PowerValue = 0;
		}

		/// <summary>
		/// 対象の部門データを取得
		/// </summary>
		/// <param name="Category">対象カテゴリ</param>
		/// <param name="General">コンテスト設定 (一般)リスト</param>
		/// <returns>部門データ</returns>
		public static Sectors GetSectorFromCategory(string Category, List<CheckInfo> General) {
			return Sectors.ToArray(General.GetByName("Sectors"))?.FirstOrDefault(x => Category == "(" + x.Code + ")" + x.WrittenName);
		}

		/// <summary>
		/// 0から始まる対象の部門番号を取得
		/// </summary>
		/// <param name="Category">対象カテゴリ</param>
		/// <param name="General">コンテスト設定 (一般)リスト</param>
		/// <returns>部門番号</returns>
		public static int GetCategoryIndex(string Category, List<CheckInfo> General) {
			int ret = -1;

			var sec = Sectors.ToArray(General.GetByName("Sectors")).ToList();

			for (int i = 0; i < sec.Count; i++) {
				if (Category == "(" + sec[i].Code + ")" + sec[i].WrittenName) {
					ret = i;
					break;
				}
			}

			return ret;
		}
	}
}
