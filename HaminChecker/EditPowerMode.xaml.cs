using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using LogChecker;
using LogChecker.Do;

namespace HaminChecker {
	/// <summary>
	/// EditPowerMode.xaml の相互作用ロジック
	/// </summary>
	public partial class EditPowerMode : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }
		private ObservableCollection<PowerMode> modes;

		public EditPowerMode(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();

			var apm = Definer.Modes(src.Data);
			if(apm != null) {
				modes = new ObservableCollection<PowerMode>(Definer.Modes(src.Data));
			} else {
				modes = new ObservableCollection<PowerMode>();
			}
			DgModes.ItemsSource = modes;
		}

		public bool IsNotInvalid() {
			ErrorStr = "";

			if(modes.Where(x => x.Name == "" && (x.SuffixPowerSign != "" || x.SuffixContestNo != "")).Count() == 0) {
				return true;
			}

			ErrorStr = "部門名が空欄です。";

			return false;
		}

		public void Update() {
			tmp.Data = PowerMode.ToString(modes);
			Result = tmp;
		}

		private void BtAddRow_Click(object sender, RoutedEventArgs e) {
			modes.Add(new PowerMode("", "", ""));
		}
	}
}
