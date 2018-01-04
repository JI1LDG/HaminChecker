using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using HaminChecker.Declares;
using HaminChecker.Utils;

namespace HaminChecker {
	/// <summary>
	/// ConfigTab.xaml の相互作用ロジック
	/// </summary>
	public partial class ConfigTab : UserControl {
		private List<CheckInfo> General;
		private List<CheckInfo> Check;

        public ConfigTab() {
            InitializeComponent();
        }

		public void Set(List<CheckInfo> gen, List<CheckInfo> chk) {
			General = gen;
			Check = chk;

			TbContestName.Text = General.GetByName("ContestName");

			CbCategory.Items.Clear();
			var sects = Sectors.ToArray(General.GetByName("Sectors")).ToList();
			foreach (var cat in sects) {
				if(cat != null) CbCategory.Items.Add("(" + cat.Code + ")" + cat.WrittenName);
			}
			CbCategory.IsEnabled = CbCategory.Items.Count > 0 ? true : false;

			WpPower.Children.Clear();
			var pows = PowerMode.ToArray(General.GetByName("PowerMode")).ToList();
			foreach(var mod in pows) {
				WpPower.Children.Add(new RadioButton() { Content = mod.Name, GroupName = "PowerMode" });
			}

			TbCoeff.IsEnabled = chk.GetByName("CoeffSetting").ToBool();

			TbMainContestNo.IsEnabled = chk.GetByName("AreaNoExists").ToBool();
			TbSubContestNo.IsEnabled = !chk.GetByName("AreaByFreq").IsEmpty();
		}

		public ConfigGeneral GetGeneral() {
#pragma warning disable IDE0017 // オブジェクトの初期化を簡略化します
			var gen = new ConfigGeneral();
#pragma warning restore IDE0017 // オブジェクトの初期化を簡略化します

			if (General == null || Check == null) return gen;

			gen.ContestName = TbContestName.Text;
			gen.Category = CbCategory.SelectedValue?.ToString();
			for (int i = 0;i < WpPower.Children.Count; i++) {
				var rb = WpPower.Children[i] as RadioButton;
				if(rb.IsChecked == true) {
					gen.PowerModeArg = i;
					break;
				}
			}
			if(TbCoeff.Text.Length > 0) {
				gen.Coeff = int.Parse(TbCoeff.Text);
			}
			gen.MainCn = TbMainContestNo.Text;
			gen.SubCn = TbSubContestNo.Text;
			gen.PowerValue = int.Parse(CbPowerValue.Text);
			gen.Operators = TbOperator.Text;
			gen.AutoOperator = (
				CbAutoOperator.IsChecked == true);

			return gen;
		}

		public void SetGeneral(ConfigGeneral gen) {
			if(gen.ContestName == General?.GetByName("ContestName")) {
				TbContestName.Text = gen.ContestName;
				var catidx = ConfigGeneral.GetCategoryIndex(gen.Category, General);
				if (catidx != -1) {
					CbCategory.SelectedIndex = catidx;
				}

				if(gen.PowerModeArg < WpPower.Children.Count && 0 <= gen.PowerModeArg) {
					(WpPower.Children[gen.PowerModeArg] as RadioButton).IsChecked = true;
				} else {
					for(int i = 0;i < WpPower.Children.Count; i++) {
						(WpPower.Children[i] as RadioButton).IsChecked = false;
					}
				}

				if(0 <= gen.Coeff) TbCoeff.Text = gen.Coeff.ToString();
				TbMainContestNo.Text = gen.MainCn;
				TbSubContestNo.Text = gen.SubCn;
				CbPowerValue.Text = gen.PowerValue.ToString();

				CbAutoOperator.IsChecked = gen.AutoOperator;
				TbOperator.Text = gen.Operators;
			}
		}

		public ConfigSolid GetSolid() {
			var con = new ConfigSolid {
				CallSign = TbCallSign.Text,
				ZipCode = TbZipCode.Text,
				Address = TbAddress.Text,
				PhoneNumber = TbPhone.Text,
				Name = TbName.Text,
				MailAddress = TbMail.Text,
				LicenserName = TbLicenserName.Text,

				GestOp = TbGestOp.Text,
				Place = TbPlace.Text,
				Supply = TbSupply.Text,
				UsedLogType = TbUseType.Text,
				Comment = TbComment.Text,
				Oath = TbOath.Text
			};

			return con;
		}

		public void SetSolid(ConfigSolid con) {
			TbCallSign.Text = con.CallSign;
			TbZipCode.Text = con.ZipCode;
			TbAddress.Text = con.Address;
			TbPhone.Text = con.PhoneNumber;
			TbName.Text = con.Name;
			TbMail.Text = con.MailAddress;
			TbLicenserName.Text = con.LicenserName;

			TbGestOp.Text = con.GestOp;
			TbPlace.Text = con.Place;
			TbSupply.Text = con.Supply;
			TbUseType.Text = con.UsedLogType;
			TbComment.Text = con.Comment;
			TbOath.Text = con.Oath;
		}

		private void Tbox_PreviewTextInputNumber(object sender, TextCompositionEventArgs e) {
			char ch = e.Text[0];

			if ('0' <= ch && ch <= '9') {
				e.Handled = false;
				return;
			}

			e.Handled = true;
		}

		private void Tbox_PreviewTextInputCallSign(object sender, TextCompositionEventArgs e) {
			char ch = e.Text[0];

			if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z') || ch == '/') {
				e.Handled = false;
				return;
			}

			e.Handled = true;
		}

		private void Tbox_PreviewTextInputTelecom(object sender, TextCompositionEventArgs e) {
			char ch = e.Text[0];

			if (('0' <= ch && ch <= '9') || ch == '-') {
				e.Handled = false;
				return;
			}

			e.Handled = true;
		}

		private void Tbox_PreviewTextInputMail(object sender, TextCompositionEventArgs e) {
			char ch = e.Text[0];

			if (('0' <= ch && ch <= '9') || ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z') || ch == '@' || ch == '-' || ch == '_' || ch == '.' || ch == '+') {
				e.Handled = false;
				return;
			}

			e.Handled = true;
		}

		private void CbAutoOperator_Checked(object sender, RoutedEventArgs e) {
			TbOperator.IsEnabled = ((CheckBox)sender).IsChecked == true ? false : true;
		}

		private void CbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			if (General == null || CbCategory.SelectedValue == null) return;
			var sec = ConfigGeneral.GetSectorFromCategory(CbCategory.SelectedValue.ToString(), General);

			for(int i = 0;i < WpPower.Children.Count; i++) {
				var rb = WpPower.Children[i] as RadioButton;
				if (sec.Modes?.Contains(i) != true) {
					rb.IsEnabled = false;
					rb.IsChecked = false;
				} else {
					rb.IsEnabled = true;
				}
			}
		}
	}
}
