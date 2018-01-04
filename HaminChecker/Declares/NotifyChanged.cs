using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HaminChecker.Declares {
	/// <summary>
	/// 更新通知
	/// </summary>
	public class NotifyChanged : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// 更新通知
		/// </summary>
		/// <param name="propertyName">呼び出しメンバ名</param>
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "") {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
