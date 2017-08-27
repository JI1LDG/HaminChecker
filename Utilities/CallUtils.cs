using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Utilities {
	/// <summary>
	/// コールサイン処理クラス
	/// </summary>
	public class CallUtils {
		public static readonly string[][] Prefectures = new string[][] {
			new string[] { "長野県", "新潟県" },
			new string[] { "東京都", "神奈川県", "千葉県", "埼玉県", "群馬県", "栃木県", "茨城県", "山梨県" },
			new string[] { "愛知県", "静岡県", "岐阜県", "三重県" },
			new string[] { "大阪府", "兵庫県", "京都府", "奈良県", "滋賀県", "和歌山県" },
			new string[] { "岡山県", "広島県", "山口県", "鳥取県", "島根県" },
			new string[] { "香川県", "愛媛県", "高知県", "徳島県" },
			new string[] { "沖縄県", "福岡県", "佐賀県", "長崎県", "熊本県", "大分県", "宮崎県", "鹿児島県" },
			new string[] { "宮城県", "福島県", "岩手県", "青森県", "秋田県", "山形県" },
			new string[] { "北海道" },
			new string[] { "石川県", "福井県", "富山県" }
		};

		/// <summary>
		/// 移動エリアを除去したコールサインを返します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static string RemoveStroke(string callsign) {
			return callsign.Split('/')[0];
		}

		/// <summary>
		/// コールサインから移動エリア番号を取得します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		/// <returns>移動局の場合は移動エリア番号 非移動局の場合はnull</returns>
		public static string GetPortableNo(string callsign) {
			if(callsign.Split('/').Length > 1) {
				return callsign.Split('/')[1];
			}

			return "";
		}

		/// <summary>
		/// コールサインからエリア番号を取得します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static string GetAreaNo(string callsign) {
			return IsKanto(callsign) ? "1" : callsign.Substring(2, 1);
		}

		/// <summary>
		/// 関東の無線局であるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool IsKanto(string callsign) {
			if((IsJaCall(callsign) || Is8JaCall(callsign)) && callsign[2] == '1') return true;
			if(Is7JaCall(callsign) && 'K' <= callsign[1] && callsign[1] <= 'N') return true;
			return false;
		}

		/// <summary>
		/// コールサインのエリアナンバから都道府県のリストを取得します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static string[] GetPrefectures(string callsign) {
			return int.TryParse(GetAreaNo(callsign), out int res) ? Prefectures[res] : null;
		}

		/// <summary>
		/// 日本の局であるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool IsJapanCall(string callsign) {
			return IsJaCall(callsign) || Is7JaCall(callsign) || Is8JaCall(callsign) ? true : false;
		}

		/// <summary>
		/// J*#コールであるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool IsJaCall(string callsign) {
			return Regex.IsMatch(callsign, @"^J([AE-S]\d|D1)[A-Z]{1,3}(/\d)?") ? true : false;
		}

		/// <summary>
		/// 7コールであるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool Is7JaCall(string callsign) {
			return Regex.IsMatch(callsign, @"^7(J\d|[K-N][1-4])[A-Z]{1,3}(/\d)?") ? true : false;
		}

		/// <summary>
		/// 8コールであるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool Is8JaCall(string callsign) {
			return Regex.IsMatch(callsign, @"^8[J-N]\d[0-9A-Z]*(/\d)?") ? true : false;
		}

		/// <summary>
		/// 記念局であるか判断します。
		/// </summary>
		/// <param name="callsign">処理コールサイン</param>
		public static bool IsAnivCall(string callsign) {
			return Regex.IsMatch(callsign, @"^8J\d[0-9A-Z]*(/\d)?") ? true : false;
		}
	}
}
