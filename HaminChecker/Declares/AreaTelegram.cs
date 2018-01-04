using System;

namespace HaminChecker.Declares {
	/// <summary>
	/// 電文用モード列挙体
	/// </summary>
	public enum TelegramFuncMode {
		None, Load, Add, Addarg, Addattel, Addatarg, Delno, Delstr, Sort, Movat, Movto, Argnox
	}

	/// <summary>
	/// 地域用電文
	/// </summary>
	public class AreaTelegram {
		/// <summary>
		/// 電文モード
		/// </summary>
		public TelegramFuncMode Function { get; set; }
		public string Arg1 { get; set; }
		public string Arg2 { get; set; }
		public string Arg3 { get; set; }

		public AreaTelegram() {
			Arg1 = Arg2 = Arg3 = "";
		}

		/// <summary>
		/// 電文を出力します。
		/// </summary>
		/// <returns>電文</returns>
		public override string ToString() {
			return string.Join(" ", new string[] { Function.ToString(), Arg1, Arg2, Arg3 });
		}

		/// <summary>
		/// 電文文字列をオブジェクトに変換します。
		/// </summary>
		/// <param name="data">電文</param>
		/// <returns>電文オブジェクト</returns>
		public static AreaTelegram GetObject(string data) {
			var at = new AreaTelegram();
			TelegramFuncMode af = TelegramFuncMode.None;

			var split = data.Split(' ');
			if (Enum.TryParse(split[0], out af) == false) {
				return null;
			}

			at.Function = af;
			switch (af) {
				case TelegramFuncMode.Add:
					for (int i = 1; i < split.Length; i++) {
						at.Arg1 += split[i] + " ";
					}
					break;
				case TelegramFuncMode.Addattel:
					at.Arg1 = split[1];
					at.Arg2 = split[2];
					for (int i = 3; i < split.Length; i++) {
						at.Arg3 += split[i] + " ";
					}
					break;
				case TelegramFuncMode.Addarg:
					at.Arg1 = split[1];
					break;
				case TelegramFuncMode.Load:
				case TelegramFuncMode.Movto:
					at.Arg1 = split[1];
					at.Arg2 = split[2];
					break;
				default:
					at.Arg1 = split[1];
					at.Arg2 = split[2];
					at.Arg3 = split[3];
					break;
			}

			return at;
		}
	}
}
