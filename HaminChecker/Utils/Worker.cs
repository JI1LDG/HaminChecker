using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker.Utils {
	/// <summary>
	/// 
	/// </summary>
	public class Worker : NotifyChanged {
		private BackgroundWorker bw = new BackgroundWorker();
		private SearchStation ss = new SearchStation("RadioStation.db", DateTime.Now.ToLocalTime().AddMonths(-3));
		private Action<int> progress;
		private Action<bool> isCompleted;
		private bool isCancel;

		private CheckData data;
		public int NowCount;

		private string status;
		public string Status {
			get { return status; }
			set {
				if (status != value) {
					status = value;
					OnPropertyChanged();
				}
			}
		}

		public Worker(CheckData logs) {
			bw.DoWork += new DoWorkEventHandler(Bw_DoWork);
			bw.ProgressChanged += new ProgressChangedEventHandler(Bw_ProgressChanged);
			bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Bw_RunWorkerCompleted);
			bw.WorkerReportsProgress = true;

			data = logs;
		}

		private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			isCompleted(isCancel);
		}

		private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			progress(e.ProgressPercentage);
		}

		private void Bw_DoWork(object sender, DoWorkEventArgs e) {
			var logs = data.Logs;
			var failed = new List<string>();
			string tmp;

			NowCount = 0;
			Status = 0 + "/" + logs.Count;
			bw.ReportProgress(NowCount);

			for (int i = 0; i < logs.Count; i++) {
				if (isCancel) break;

				var l = logs[i];

				var callsign = Callsign.RemoveStroke(l.Callsign);
				l.SetInit();

				if (l.Exclude) continue;
				failed = new List<string>();

				//Dupe
				if (logs.Count(x => x.Callsign == l.Callsign && x.Freq == l.Freq) > 1) {
					l.SetMax(5);
					failed.Add("データが重複しています。");
					if (data.Ches.GetByName("ZeroByDupe").ToBool()) {
						l.Point = 0;
					}
				}

				Status = "無線局情報検索中 (" + NowCount + "/" + logs.Count + ")";
				var si = ss.Execute(callsign);
				//DbNothing
				if (si == null) {
					l.SetMax(1);
					failed.Add("無線局情報の取得に失敗しました。");
				}

				Status = "ログ確認中 (" + NowCount + "/" + logs.Count + ")";

				//SentCNCheck
				tmp = l.SentCn;
				if (data.Ches.GetByName("RSTExists").ToBool()) {
					if (l.Mode == Modes.CW.ToString()) {
						tmp = tmp.Substring(3);
					} else {
						tmp = tmp.Substring(2);
					}
				}
				var frq = Frequency.Split(data.Ches.GetByName("AreaByFreq"));
				string cn;
				if (frq.Count(x => Frequency.ToString(x) == l.Freq) > 0) {
					cn = data.ConGen.SubCn;
				} else {
					cn = data.ConGen.MainCn;
				}
				if (cn != tmp) {
					l.SetMax(5);
					failed.Add("自局CN不一致");
				} else {
					if (data.Ches.GetByName("PowerSignExists").ToBool()) {
						if (data.Ches.GetByName("NonSignAllowed").ToBool() == false) {
							if (!Regex.IsMatch(tmp, @"\d+[A-Z]")) {
								l.SetMax(5);
								failed.Add("自局CNに記号なし");
							}
						}
					}
				}

				//RecvCNCheck
				if (data.Ches.GetByName("CheckRecvCn").ToBool() && si != null) {
					if (data.Ches.GetByName("RSTExists").ToBool()) {
						tmp = @"\d\d";
						if (l.Mode == "CW") {
							tmp += @"\d";
						}
					} else {
						tmp = "";
					}

					if (data.Ches.GetByName("AreaNoExists").ToBool()) {
						tmp += @"(\d+)";
					} else {
						tmp += @"(.*)";
					}

					if (data.Ches.GetByName("PowerSignExists").ToBool()) {
						if (data.Ches.GetByName("NonSignAllowed").ToBool()) {
							tmp += @"([A-Z]*)";
						} else {
							tmp += @"([A-Z])";
						}
					} else {
						tmp += "()";
					}

					var m = Regex.Matches(l.RecvCn, tmp);
					if (m.Count > 0 && m[0].Groups.Count > 2) {
						var nos = m[0].Groups[1].Value;
						var sign = m[0].Groups[2].Value;

						var pbs = Regex.Matches(data.Ches.GetByName("PointBySign"), @"(([A-Z])(\d))*");
						if (pbs.Count > 0 && pbs[0].Groups.Count > 2) {
							for (int j = 0;j < pbs[0].Groups[2].Captures.Count - 2;j++) {
								if (sign == pbs[0].Groups[2].Captures[j].Value) {
									l.Point = int.Parse(pbs[0].Groups[3].Captures[j].Value);
									break;
								}
							}
						}

						if (data.Ches.GetByName("ExcludedAreaNos").Split(',').Count(x => x == nos) == 0) {
							var fr2q = Frequency.Split(data.Ches.GetByName("AreaByFreq"));
							List<AreaData> area;
							if (fr2q.Count(x => Frequency.ToString(x) == l.Freq) > 0) {
								area = data.Area2;
							} else {
								area = data.Area1;
							}

							if (Callsign.GetPortableNo(l.Callsign) != -1) {
								failed.Add("移動局: " + Callsign.Areas[Callsign.GetPortableNo(l.Callsign)] + "地方");
							} else {

								var addr = area.FirstOrDefault(x => x.No == nos);

								if (addr == null) {
									l.SetMax(5);
									failed.Add("地域番号が存在しません");
									if (Callsign.GetPortableNo(l.Callsign) == -1) {
										string suggest = "";
										for (int k = 0; k < si.Address.Count; k++) {
											foreach (var aw in area.Where(x => x.Addresses.Exists(y => si.Address[k].Contains(y)))) {
												suggest += aw.No + ", ";
												break;
											}
										}
										failed.Add("もしかして: " + suggest);
									}
								}
							}
						}
					} else {
						l.SetMax(5);
						failed.Add("相手CNが不正です。");
					}
				}

				//IsCallSignCheck
				if (data.Ches.GetByName("CheckCallsign").ToBool()) {
					//OnlyJA
					if (data.Ches.GetByName("OnlyJAStation").ToBool()) {
						if (!Callsign.IsJapanCall(callsign)) {
							l.SetMax(5);
							failed.Add("日本の無線局ではありません。");
						}
					}
				}

				//DateCheck
				for (int j = 1; j <= 4; j++) {
					if (j == 4) {
						l.SetMax(2);
						failed.Add("コンテスト期間外もしくは周波数が対象外です。");
						break;
					}

					//SectorCheck
					if (data.Ches.GetByName("CheckBySection").ToBool()) {
						var cat = ConfigGeneral.GetSectorFromCategory(data.ConGen.Category, data.Gens);
						if (cat == null) {
							l.SetMax(5);
							failed.Add("部門が見つかりません。");
						} else {
							if (cat.EnabledFreqStr != "") {
								var enq = cat.EnabledFreqs.Select(x => Frequency.ToString(x)).ToList();
								if (enq.Count(x => x == l.Freq) == 0) {
									l.SetMax(2);
									failed.Add("周波数が対象外です。");
								}
							} else if (cat.UnabledFreqStr != "") {
								var unq = cat.UnabledFreqs.Select(x => Frequency.ToString(x)).ToList();
								if (unq.Count(x => x == l.Freq) > 0) {
									l.SetMax(2);
									failed.Add("周波数が対象外です。");
								}
							}
						}
					}

					//DateCheck
					var frq3 = Frequency.Split(data.Gens.GetByName("Freq" + j))?.Select(x => Frequency.ToString(x)).ToList();
					if (frq3.Count(x => x == l.Freq) > 0) {
						var dt = Date.FromTerm(data.Gens.GetByName("Terms" + j), l.Date.Year);
						if (!(dt[0] <= l.Date && l.Date < dt[1])) {
							l.SetMax(2);
							failed.Add("コンテスト期間外です。");
						}
						break;
					}
				}

				l.FailedStr = string.Join("\r\n", failed.Where(x => x != "")); ;

				if (data.Ches.GetByName("CoeffSetting").ToBool()) {
					l.Point *= data.ConGen.Coeff;
				}

				NowCount++;

				Status = NowCount + "/" + logs.Count;
				bw.ReportProgress(NowCount);
			}

			Status = "ログ確認終了 (" + NowCount + "/" + logs.Count + ")";
			bw.ReportProgress(NowCount);
		}

		public void RunAsync(Action<int> percentage, Action<bool> flag) {
			isCancel = false;
			progress = percentage;
			isCompleted = flag;
			bw.RunWorkerAsync();
		}

		public void Cancel() {
			isCancel = true;
		}
	}
}
