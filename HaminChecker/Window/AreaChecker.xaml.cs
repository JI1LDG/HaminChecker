using System.Collections.Generic;
using System.Windows;

using HaminChecker.Declares;

namespace HaminChecker {
	/// <summary>
	/// AreaChecker.xaml の相互作用ロジック
	/// </summary>
	public partial class AreaChecker : System.Windows.Window {
		public AreaChecker(IEnumerable<SingleAreaData> data) {
			InitializeComponent();

			DgArea.ItemsSource = data;
		}
	}
}
