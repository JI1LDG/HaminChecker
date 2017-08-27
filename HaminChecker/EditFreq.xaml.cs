using System;
using System.Collections.Generic;
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

using LogChecker.Do;
using LogChecker;
using Utilities;

namespace HaminChecker {
	/// <summary>
	/// EditFreq.xaml の相互作用ロジック
	/// </summary>
	public partial class EditFreq : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }
		private List<CheckBox> checkBoxes;

		public EditFreq(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();

			checkBoxes = new List<CheckBox>();
			for(int i = 0;Enums.FreqToString((Freqs)i) != ""; i++) {
				checkBoxes.Add(new CheckBox() { Content = Enums.FreqToString((Freqs)i) });
				Grid.SetRow(checkBoxes.Last(), i / 2);
				Grid.SetColumn(checkBoxes.Last(), i % 2);
				GdFreqs.Children.Add(checkBoxes.Last());
			}

			var fqs = Definer.Frequencies(src.Data);

			for(int i = 0; i < checkBoxes.Count / 2 + (checkBoxes.Count % 2); i++) {
				GdFreqs.RowDefinitions.Add(new RowDefinition());
			}

			foreach(var f in fqs) {
				checkBoxes[(int)f].IsChecked = true;
			}
		}

		public bool IsNotInvalid() {
			ErrorStr = "";

			if(checkBoxes.Exists(x => x.IsChecked == true)) {
				return true;
			}

			ErrorStr = "チェックされてません。";

			return false;
		}

		public void Update() {
			var list = new List<Freqs>();
			for(int i = 0;i < checkBoxes.Count; i++) {
				if(checkBoxes[i].IsChecked == true) {
					list.Add((Freqs)i);
				}
			}

			tmp.Data = list.ToArray().JoinString();
			Result = tmp;
		}
	}
}
