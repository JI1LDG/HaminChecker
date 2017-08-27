using NLog;
using System;

namespace FileReader {
	/// <summary>
	/// バイナリデータ読み込みクラス
	/// </summary>
	public class BinaryReader {
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private System.IO.FileStream fStream;
		private System.IO.BinaryReader bReader;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path">ファイルパス</param>
		public BinaryReader(string path) {
			logger.Debug("Start(path: " + path + ")");
			try {
				fStream = new System.IO.FileStream(path, System.IO.FileMode.Open);
				bReader = new System.IO.BinaryReader(fStream);
			} catch(Exception e) {
				logger.Error(e, e.Message);
			}
			logger.Debug("End");
		}

		/// <summary>
		/// デストラクタ
		/// </summary>
		~BinaryReader() {
			bReader?.Dispose();
			fStream?.Dispose();
		}

		/// <summary>
		/// 1バイト符号なし整数をストリームから読み込みます。
		/// </summary>
		public byte ReadByte() {
			return bReader.ReadByte();
		}

		/// <summary>
		/// 指定されたバイト数分を1バイト符号なし整数配列としてストリームから読み込みます。
		/// </summary>
		/// <param name="cnts">バイト数</param>
		public byte[] ReadBytes(int cnts) {
			return bReader.ReadBytes(cnts);
		}

		/// <summary>
		/// 2バイト符号なし整数をストリームから読み込みます。
		/// </summary>
		public ushort ReadWord() {
			return bReader.ReadUInt16();
		}

		/// <summary>
		/// 8バイト符号付き整数をストリームから読み込みます。
		/// </summary>
		public long ReadLong() {
			return bReader.ReadInt64();
		}

		/// <summary>
		/// 指定されたバイト数分を文字列としてストリームから読み込みます。
		/// </summary>
		/// <param name="cnt">バイト数</param>
		public string ReadString(int cnt) {
			return new string(bReader.ReadChars(cnt)).Split('\0')[0];
		}
	}
}
