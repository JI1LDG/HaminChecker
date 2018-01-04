namespace HaminChecker.Declares {
	/// <summary>
	/// ItemEditor用インタフェース
	/// </summary>
	interface IItemEditor {
		/// <summary>
		/// 設定データ
		/// </summary>
		CheckInfo Result { get; set; }

		/// <summary>
		/// エラー文字列
		/// </summary>
		string ErrorStr { get; set; }

		/// <summary>
		/// 整合性確認
		/// </summary>
		bool IsNotInvalid();

		/// <summary>
		/// データ更新
		/// </summary>
		void Update();
	}
}
