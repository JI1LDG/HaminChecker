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

using LogChecker;
using LogChecker.Do;

namespace HaminChecker {
	/// <summary>
	/// EditString.xaml の相互作用ロジック
	/// </summary>
	public partial class EditString : UserControl, IItemEditor {
		public CheckInfo Result { get; set; }
		private CheckInfo tmp;
		public string ErrorStr { get; set; }

		public EditString(CheckInfo src) {
			Result = null;
			tmp = src;
			InitializeComponent();
			LbName.Content = src.Name + ": ";
			TbString.Text = src.Data;
		}

		public bool IsNotInvalid() {
			ErrorStr = "";

			switch(tmp.Mode) {
				case SetMode.String:
					if(TbString.Text.Length > 0) {
						return true;
					} else {
						ErrorStr = "文字列が入力されていません";
						return false;
					}
				case SetMode.CommaEd:
					if(TbString.Text.Length > 0 && Definer.SplitWithComma(TbString.Text).Length > 0) {
						return true;
					} else {
						ErrorStr = "文字列が入力されていません";
						return false;
					}
				default:
					ErrorStr = "不明なエラーです";
					return false;
			}
		}

		public void Update() {
			tmp.Data = TbString.Text;
			Result = tmp;
		}
	}
}
