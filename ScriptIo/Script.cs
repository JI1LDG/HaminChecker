using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogChecker.Do;
using Newtonsoft.Json;

namespace ScriptIo {
	public class ScriptIo {
		public static string Write(List<CheckInfo> General, List<CheckInfo> Area, List<CheckInfo> Settings) {
			string generalJson = JsonConvert.SerializeObject(General, Formatting.Indented);
			return generalJson;
		}

		public static List<CheckInfo> Read(string jsonStr) {
			return JsonConvert.DeserializeObject<List<CheckInfo>>(jsonStr);
		}
	}
}
