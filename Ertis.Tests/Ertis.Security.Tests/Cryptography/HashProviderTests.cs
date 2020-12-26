using System.Text;
using Ertis.Security.Cryptography;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Security.Tests.Cryptography
{
	public class HashProviderTests
	{
		#region Services

		private HashProvider hashProvider;

		#endregion
		
		#region Initialize

		[SetUp]
		public void Setup()
		{
			this.hashProvider = new HashProvider();
		}

		#endregion

		#region Methods

		[Test]
		public void Sha256Test()
		{
			string hash = this.hashProvider.Hash(".Abcd1234!", HashAlgorithms.SHA2_256, Encoding.UTF8);
			Assert.AreEqual("6f2dd598adb2cb6eeebe6c94b52616c5337305639db5fd7fa82f8a0ba210786a", hash);
		}

		#endregion
	}
}