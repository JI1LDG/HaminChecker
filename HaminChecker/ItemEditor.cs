using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LogChecker.Do;

namespace HaminChecker {
	interface IItemEditor {
		CheckInfo Result { get; set; }
		string ErrorStr { get; set; }
		bool IsNotInvalid();
		void Update();
	}
}
