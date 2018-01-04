using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// Output.xaml の相互作用ロジック
	/// </summary>
	public partial class Output : System.Windows.Window {
		int SumPoint = 0;
		public Output(CheckData check) {
			InitializeComponent();

			string sheet = MakeSheet(check);
			string summery = MakeSummery(check);

			TbOutput.Text = summery + sheet;
		}

		private string MakeSheet(CheckData check) {
			string sheet = "<LOGSHEET TYPE=" + check.ConSol.UsedLogType + ">\r\n";
			int calls = 0;
			int multis = 0;
			List<string>[] multCnt = Enumerable.Range(0, Frequency.FreqNames.Length).Select(x => new List<string>()).ToArray();

			sheet += "DATE(JST) TIME BAND MODE CALLSIGN SENTNo RCVNo\r\n";

			foreach(var l in check.Logs.OrderBy(x => x.Date)) {
				if (l.Point == 0) continue;

				string tmp;
				if (check.Ches.GetByName("RSTExists").ToBool()) {
					tmp = @"\d\d";
					if (l.Mode == "CW") {
						tmp += @"\d";
					}
				} else {
					tmp = "";
				}

				if (check.Ches.GetByName("AreaNoExists").ToBool()) {
					tmp += @"(\d+)";
				} else {
					tmp += @"(.*)";
				}

				if (check.Ches.GetByName("PowerSignExists").ToBool()) {
					if (check.Ches.GetByName("NonSignAllowed").ToBool()) {
						tmp += @"([A-Z]*)";
					} else {
						tmp += @"([A-Z])";
					}
				} else {
					tmp += "()";
				}


				try {
					string nos = Regex.Matches(l.RecvCn, tmp)[0].Groups[1].Value;
					if (!multCnt[(int)Frequency.FromStringWithUnits(l.Freq)].Exists(x => x == nos)) {
						multCnt[(int)Frequency.FromStringWithUnits(l.Freq)].Add(nos);
					}
				} catch {
					continue;
				}
				calls++;

				sheet += l.Date.ToString("yyyy-MM-dd hh:mm ") + l.Freq.Replace("MHz", " ") + l.Mode + " " + l.Callsign + " ";
				int sep = 2;
				if (l.Mode == "CW") {
					sep = 3;
				}
				sheet += l.SentCn.Substring(0, sep) + " " + l.SentCn.Substring(sep) + " ";
				sheet += l.RecvCn.Substring(0, sep) + " " + l.RecvCn.Substring(sep) + "\r\n";
			}

			multis = multCnt.Sum(x => x.Count);

			sheet += "</LOGSHEET>\r\n";

			SumPoint = calls * multis;
			return sheet;
		}

		private string MakeSummery(CheckData check) {
			string log = "<SUMMARYSHEET VERSION=R2.0>";

			try {
				if (check.Gens.GetByName("ContestName").IsEmpty()) {
					return null;
				}
				log += "<CONTESTNAME>" + check.Gens.GetByName("ContestName") + "</CONTESTNAME>\r\n";

				log += "<CATEGORYCODE>";
				var catidx = ConfigGeneral.GetCategoryIndex(check.ConGen.Category, check.Gens);
				if (catidx != -1) {
					log += Sectors.ToArray(check.Gens.GetByName("Sectors")).ToArray()[catidx].Code;
				} else {
					return null;
				}

				if (check.ConGen.PowerModeArg != -1) {
					log += PowerMode.ToArray(check.Gens.GetByName("PowerMode"))[check.ConGen.PowerModeArg].SuffixPowerSign;
				}
				log += "</CATEGORYCODE>\r\n";

				log += "<CALLSIGN>" + check.ConSol.CallSign + "</CALLSIGN>\r\n";

				if (check.ConSol.GestOp != "") {
					log += "<OPCALLSIGN>" + check.ConSol.GestOp + "</OPCALLSIGN>\r\n";
				}

				log += "<TOTALSCORE>" + SumPoint + "</TOTALSCORE>\r\n";

				log += "<ADDRESS>" + check.ConSol.Address + "</ADDRESS>\r\n";

				log += "<NAME>" + check.ConSol.Name + "</NAME>\r\n";

				log += "<TEL>" + check.ConSol.PhoneNumber + "</TEL>\r\n";

				log += "<EMAIL>" + check.ConSol.MailAddress + "</EMAIL>\r\n";

				log += "<POWER>" + check.ConGen.PowerValue + "</POWER>\r\n";

				if (check.Ches.GetByName("CoeffSetting").ToBool()) {
					log += "<FDCOEFF>" + check.ConGen.Coeff + "</FDCOEFF>\r\n";
				}

				log += "<OPPLACE>" + check.ConSol.Place + "</OPPLACE>\r\n";

				log += "<POWERSUPPLY>" + check.ConSol.Supply + "</POWERSUPPLY>\r\n";

				log += "<COMMENTS>" + check.ConSol.Comment + "</COMMENTS>\r\n";

				if (check.ConGen.AutoOperator) {
					log += "<MULTIOPLIST>" + string.Join(",", check.Logs.Select(x => x.Operator).Distinct()) + "</MULTIOPLIST>\r\n";
				} else {
					log += "<MULTIOPLIST>" + check.ConGen.Operators + "</MULTIOPLIST>\r\n";
				}

				log += "<OATH>" + check.ConSol.Oath + "</OATH>\r\n";

				log += "<DATE>" + System.DateTime.Now.ToLocalTime().ToString("yyyy年MM月dd日") + "</DATE>\r\n";

				log += "<SIGNATURE>" + check.ConSol.LicenserName + "</SIGNATURE>\r\n";
			} catch {
				return null;
			}

			log += "</SUMMARYSHEET>\r\n";

			return log;
		}
	}
}
