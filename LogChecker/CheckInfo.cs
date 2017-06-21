using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogChecker {	
	/// <summary>
	/// 
	/// </summary>
	public static class CheckInfoExtensions {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="name"></param>
		/// <param name="data"></param>
		public static void Set(this List<Do.CheckInfo> list, string name, string data) {
			if(list.Exists(x => x.Name == name) && data != null) {
				list.Find(x => x.Name == name).Data = data;
			}
		}
	}
}

namespace LogChecker.Do {
	/// <summary>
	/// 
	/// </summary>
	public class CheckInfo {
		public string Name { get; private set; }
		public string Rem { get; private set; }
		public SetMode Mode { get; private set; }
		public string Data { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rem"></param>
		/// <param name="mode"></param>
		public CheckInfo(string name, string rem, SetMode mode, string data = null) {
			Name = name;
			Rem = rem;
			Mode = mode;
			if(data != null) {
				Data = data;
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class PowerMode {
		public string SuffixPowerSign { get; private set; }
		public string Name { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="suffixPowerSign"></param>
		/// <param name="name"></param>
		public PowerMode(string suffixPowerSign, string name) {
			SuffixPowerSign = suffixPowerSign;
			Name = name;
		}

		public static PowerMode[] GetByScript(string[] suffixPowers, string[] names) {
			var cnt = Math.Min(suffixPowers.Length, names.Length);
			var pma = new PowerMode[cnt];

			for(int i = 0;i < cnt;i++) {
				pma[i] = new PowerMode(suffixPowers[i], names[i]);
			}

			return pma;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class Sectors {
		public string Name { get; private set; }
		public string WrittenName { get; private set; }
		public string Code { get; private set; }
		public int[] Modes { get; private set; }
		public Utilities.Freqs[] EnabledFreqs { get; private set; }


	}

	/// <summary>
	/// 
	/// </summary>
	public enum SetMode {
		Bool, String, Number,
		Terms, Frequency, Sector, PowerSign, PowerMode,
	}
}