using System;
using System.Collections.Generic;
using System.Linq;

using HaminChecker.Utils;

namespace HaminChecker.Declares {
	/// <summary>
	/// コンテスト設定データ
	/// </summary>
	public class CheckInfo : NotifyChanged {
		/// <summary>
		/// 設定名
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 説明
		/// </summary>
		public string Rem { get; private set; }

		/// <summary>
		/// 設定モード
		/// </summary>
		public SetMode Mode { get; private set; }

		private string _data;
		/// <summary>
		/// 文字列データ
		/// </summary>
		public string Data {
			get { return _data; }
			set { _data = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">項目名</param>
		/// <param name="rem">説明</param>
		/// <param name="mode">設定モード</param>
		/// <param name="data">データ</param>
		public CheckInfo(string name, string rem, SetMode mode, string data = "<Empty>") {
			Name = name;
			Rem = rem;
			Mode = mode;
			if (data != null) {
				Data = data;
			}
		}
	}

	/// <summary>
	/// コンテスト設定用拡張メソッド
	/// </summary>
	public static class CheckInfoExtensions {
		/// <summary>
		/// リストの対象項目のデータを設定します。
		/// </summary>
		/// <param name="list">追加対象リスト</param>
		/// <param name="name">項目名</param>
		/// <param name="data">データ</param>
		public static void Set(this List<CheckInfo> list, string name, string data) {
			if (list.Exists(x => x.Name == name) && data != null) {
				list.Find(x => x.Name == name).Data = data;
			}
		}

		/// <summary>
		/// 設定データを文字列化します。
		/// </summary>
		/// <param name="src">対象項目</param>
		/// <returns>設定データ文字列</returns>
		public static string ToPrint(this CheckInfo src) {
			switch (src.Mode) {
				case SetMode.Terms:
					var dts = Date.FromTerm(src.Data, 2017);
					return (dts == null) ? "" : string.Join(" to ", dts.Where(x => x != DateTime.MinValue && x != DateTime.MaxValue).Select(x => x.ToString("yyyy/MM/dd HH:mm")));
				case SetMode.Frequency:
					var fqs = Frequency.Split(src.Data);
					return (fqs == null) ? "" : fqs.Join();
				case SetMode.PowerMode:
					var pms = PowerMode.ToArray(src.Data);
					return (pms == null) ? "" : string.Join(", ", pms.Select(x => x.ToString()));
				case SetMode.Sector:
					var data = src.Data.Replace("\r\n", "`");
					var scs = Sectors.ToArray(data)?.ToArray();
					return (scs == null) ? "" : string.Join("\r\n", scs.Select(x => x.ToString()));
				default:
					return src.Data.ToString();
			}
		}

		/// <summary>
		/// 設定データリストから指定した名前に一致する設定データを取得します。
		/// </summary>
		/// <param name="src">設定データリスト</param>
		/// <param name="name">対象設定名</param>
		/// <returns>設定データ文字列</returns>
		public static string GetByName(this IEnumerable<CheckInfo> src, string name) {
			return src.Where(x => x.Name == name).FirstOrDefault()?.Data;
		}
	}
}
