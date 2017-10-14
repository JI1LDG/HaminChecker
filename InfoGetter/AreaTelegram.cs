using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoGetter.Do {
	public enum AreaFunc {
		None, Load, Add, Addarg, Addattel, Addatarg, Delno, Delstr, Sort, Movat, Movto, Argnox
	}

	public class AreaTelegram {
		public AreaFunc Function { get; set; }
		public string Arg1 { get; set; }
		public string Arg2 { get; set; }
		public string Arg3 { get; set; }

		public AreaTelegram() {
			Arg1 = Arg2 = Arg3 = "";
		}

		public override string ToString() {
			return string.Join(" ", new string[] { Function.ToString(), Arg1, Arg2, Arg3 });
		}

		public static AreaTelegram GetObject(string data) {
			var at = new AreaTelegram();
			AreaFunc af = AreaFunc.None;

			var split = data.Split(' ');
			if (Enum.TryParse<AreaFunc>(split[0], out af) == false) {
				return null;
			}

			at.Function = af;
			switch (af) {
				case AreaFunc.Add:
					for (int i = 1; i < split.Length; i++) {
						at.Arg1 += split[i] + " ";
					}
					break;
				case AreaFunc.Addattel:
					at.Arg1 = split[1];
					at.Arg2 = split[2];
					for (int i = 3; i < split.Length; i++) {
						at.Arg3 += split[i] + " ";
					}
					break;
				case AreaFunc.Load:
				case AreaFunc.Movto:
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
