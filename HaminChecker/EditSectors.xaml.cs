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
	/// EditSectors.xaml の相互作用ロジック
	/// </summary>
	public partial class EditSectors : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }
		private ObservableCollection<Sectors> sectorData;

		public EditSectors(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();

			sectorData = new ObservableCollection<Sectors>(Definer.Sectors(src.Data));

			DgSectors.ItemsSource = sectorData;
		}

		//右クリックでWrittenNameにNameをコピー追加

		public bool IsNotInvalid() {
			ErrorStr = "";

			return false;
		}

		public void Update() {
			tmp.Data = null;
			Result = tmp;
		}
	}
}
