namespace HaminChecker.Declares {
	/// <summary>
	/// シーケンスモード
	/// </summary>
	public enum SeqMode {
		Log,
	}

	/// <summary>
	/// シーケンスデータ
	/// </summary>
	public class SeqData {
		/// <summary>
		/// シーケンスモード
		/// </summary>
		public SeqMode Mode;
		public string Data1;
		public string Data2;

		public SeqData(SeqMode mode, string data1, string data2) {
			Mode = mode;
			Data1 = data1;
			Data2 = data2;
		}
	}
}
