using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogLoader.Tests {
	[TestClass()]
	public class CtestwinLoaderTests {
		[TestMethod()]
		public void ExecuteTest() {
			var cl = new CtestwinLoader("../Release/6ad.lg8");
			Assert.AreEqual(cl.Execute(), true);
			Assert.AreEqual(cl.LoadData.Count, 2);
		}
	}
}