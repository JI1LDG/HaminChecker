using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using HaminChecker.Declares;

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

			var apm = PowerMode.ToArray(src.Data);
			if(apm != null) {
				modes = new ObservableCollection<PowerMode>(apm);
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
