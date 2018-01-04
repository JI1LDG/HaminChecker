using System;

namespace HaminChecker.Declares {
	/// <summary>
	/// モード一覧
	/// </summary>
	public enum Modes {
		CW, RTTY, SSB, FM, AM,
	}

	/// <summary>
	/// 周波数一覧
	/// </summary>
	public enum Freqs {
		f1p9M, f3p5M, f7M, f10M, f14M, f18M, f21M,
		f24M, f28M, f50M, f144M, f430M, f1200M, f2400M, f5600M,
		None
	}

	/// <summary>
	/// ログデータ
	/// </summary>
	public class LogData : NotifyChanged {
		/// <summary>
		/// 交信日時
		/// </summary>
		public DateTime Date { get; set; }

		private string callsign;
		/// <summary>
		/// コールサイン
		/// </summary>
		public string Callsign {
			get { return callsign; }
			set {
				if (callsign != value.ToUpper()) {
					callsign = value.ToUpper();
					OnPropertyChanged();
				}
			}
		}

		private string sentCn;
		/// <summary>
		/// 自局CN
		/// </summary>
		public string SentCn {
			get { return sentCn; }
			set {
				if (sentCn != value.ToUpper().Trim()) {
					sentCn = value.ToUpper().Trim();
					OnPropertyChanged();
				}
			}
		}

		private string recvCn;
		/// <summary>
		/// 相手CN
		/// </summary>
		public string RecvCn {
			get { return recvCn; }
			set {
				if (recvCn != value.ToUpper().Trim()) {
					recvCn = value.ToUpper().Trim();
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// モード
		/// </summary>
		public string Mode { get; set; }

		/// <summary>
		/// 周波数
		/// </summary>
		public string Freq { get; set; }

		private string _operator;
		/// <summary>
		/// オペレータ
		/// </summary>
		public string Operator {
			get { return _operator; }
			set {
				if (_operator != value) {
					_operator = value;
					OnPropertyChanged();
				}
			}
		}

		private string rem;
		/// <summary>
		/// コメント
		/// </summary>
		public string Rem {
			get { return rem; }
			set {
				if (rem != value) {
					rem = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// 得点
		/// </summary>
		public int Point { get; set; }

		private string failedStr;
		/// <summary>
		/// エラー文字列
		/// </summary>
		public string FailedStr {
			get { return failedStr; }
			set {
				if (failedStr != value) {
					failedStr = value;
					OnPropertyChanged();
				}
			}
		}

		private int errLv;
		/// <summary>
		/// エラーレベル
		/// </summary>
		public int ErrLv {
			get { return errLv; }
			set {
				if (errLv != value) {
					errLv = value;
					OnPropertyChanged();
				}
			}
		}

		private bool exclude;
		/// <summary>
		/// 確認除外
		/// </summary>
		public bool Exclude {
			get { return exclude; }
			set {
				if (exclude != value) {
					exclude = value;
					OnPropertyChanged();
				}
			}
		}

		public LogData() {
			callsign = sentCn = recvCn = "";
			ErrLv = 0;
			Exclude = false;
		}

		public bool IsRate0 { get { return ErrLv == 0; } }
		public bool IsRate1 { get { return ErrLv == 1; } }
		public bool IsRate2 { get { return ErrLv == 2; } }
		public bool IsRate3 { get { return ErrLv == 3; } }
		public bool IsRate4 { get { return ErrLv == 4; } }
		public bool IsRate5 { get { return ErrLv == 5; } }

		/// <summary>
		/// 初期設定をします。
		/// </summary>
		public void SetInit() {
			ErrLv = 0;
			FailedStr = "";
			Point = 1;
		}

		/// <summary>
		/// エラーレベルに最大値を設定します。
		/// </summary>
		/// <param name="setval">設定値</param>
		public void SetMax(int setval) {
			if (ErrLv < setval) ErrLv = setval;
		}
	}
}
