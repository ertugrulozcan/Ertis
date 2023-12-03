using Ertis.Security.Cryptography;
using NUnit.Framework;
using HashParser = Ertis.Security.Helpers.HashParser;

namespace Ertis.Tests.Ertis.Security.Tests.Helpers
{
	public class HashParserTests
	{
		/*
		MD5,
		SHA0,
		SHA1,
		SHA2_224,
		SHA2_256,
		SHA2_384,
		SHA2_512,
		SHA2_512_224,
		SHA2_512_256,
		SHA3_224,
		SHA3_256,
		SHA3_384,
		SHA3_512,
		*/
		
		#region Methods

		[Test]
		public void MD5_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("MD5", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.MD5 == algorithm);
		}
		
		[Test]
		public void MD5_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("md5", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.MD5 == algorithm);
		}
		
		[Test]
		public void SHA0_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA0", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA0 == algorithm);
		}
		
		[Test]
		public void SHA0_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha0", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA0 == algorithm);
		}
		
		[Test]
		public void SHA1_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA1", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA1 == algorithm);
		}
		
		[Test]
		public void SHA1_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha1", out var algorithm, out _, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA1 == algorithm);
		}
		
		[Test]
		public void SHA224_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA_224_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA-224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA256_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA_256_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA-256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA384_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA_384_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA-384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA512_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		[Test]
		public void SHA_512_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA-512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		[Test]
		public void SHA2_224_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA2_224_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA2_256_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA2_256_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA2_384_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA2_384_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA2_512_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		[Test]
		public void SHA2_512_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		[Test]
		public void SHA2_512_224_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-512-224", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_224 == algorithm);
			Assert.That(224 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA2_512_224_Slash_Separator_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-512/224", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_224 == algorithm);
			Assert.That(224 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA2_512_224_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-512-224", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_224 == algorithm);
			Assert.That(224 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA2_512_256_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-512-256", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_256 == algorithm);
			Assert.That(256 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA2_512_256_Slash_Separator_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA2-512/256", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_256 == algorithm);
			Assert.That(256 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA2_512_256_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha2-512-256", out var algorithm, out var outputBitSize, out var stateSize);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA2_512_256 == algorithm);
			Assert.That(256 == outputBitSize);
			Assert.That(512 == stateSize);
		}
		
		[Test]
		public void SHA3_224_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA3-224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA3_224_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha3-224", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_224 == algorithm);
			Assert.That(224 == outputBitSize);
		}
		
		[Test]
		public void SHA3_256_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA3-256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA3_256_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha3-256", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_256 == algorithm);
			Assert.That(256 == outputBitSize);
		}
		
		[Test]
		public void SHA3_384_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA3-384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA3_384_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha3-384", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_384 == algorithm);
			Assert.That(384 == outputBitSize);
		}
		
		[Test]
		public void SHA3_512_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("SHA3-512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		[Test]
		public void SHA3_512_Case_Sensitive_Test()
		{
			var isParsed = HashParser.TryParseHashAlgorithm("sha3-512", out var algorithm, out var outputBitSize, out _);
			Assert.That(isParsed);
			Assert.That(HashAlgorithms.SHA3_512 == algorithm);
			Assert.That(512 == outputBitSize);
		}
		
		#endregion
	}
}