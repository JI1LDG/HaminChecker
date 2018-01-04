using System.Collections.Generic;
using System.Linq;

using HaminChecker.Declares;

namespace HaminChecker.Utils {
	/// <summary>
	/// Redo/Undo
	/// </summary>
	class Sequence {
		private List<SeqData> Seq;
		private int NowIdx;

		public Sequence() {
			Seq = new List<SeqData>();
			NowIdx = 0;
		}

		/// <summary>
		/// シーケンスに追加します。
		/// </summary>
		/// <param name="mode">追加モード</param>
		public void Add(SeqMode mode, string data1, string data2) {
			Seq.Add(new SeqData(mode, data1, data2));
			NowIdx++;

			Seq = Seq.GetRange(0, NowIdx);
		}

		/// <summary>
		/// Undoします。
		/// </summary>
		/// <returns>シーケンス</returns>
		public SeqData Undo() {
			NowIdx--;
			if (NowIdx < 0) {
				NowIdx = 0;
				return null;
			}

			return Seq[NowIdx];
		}

		/// <summary>
		/// Redoします。
		/// </summary>
		/// <returns>シーケンス</returns>
		public SeqData Redo() {
			NowIdx++;
			if (Seq.Count <= NowIdx) {
				NowIdx--;
				return null;
			}

			return Seq[NowIdx];
		}

		/// <summary>
		/// 最後のシーケンスを取得します。
		/// </summary>
		/// <param name="mode">モード</param>
		/// <returns>シーケンス</returns>
		public SeqData Last(SeqMode mode) {
			return Seq.Last(x => x.Mode == mode);
		}
	}
}
