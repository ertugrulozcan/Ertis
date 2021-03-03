using System;
using System.IO;
using Ertis.Core.Dynamics;
using Ertis.Core.Dynamics.Primitives;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.Shared.Core.Tests.Dynamics
{
	public class DynamicObjectTests
	{
		#region Properties

		private DynamicObjectSchema TestSchema { get; set; }

		#endregion
		
		#region Setup

		[SetUp]
		public void InitializeUnitTest()
		{
			var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
			var json = File.ReadAllText($"{currentDirectory.Parent?.Parent?.Parent?.FullName}/Ertis.Shared.Core.Tests/Dynamics/schema.json");
			this.TestSchema = DynamicObjectSchema.Parse(json);
		}

		#endregion
		
		#region Methods

		[Test]
		public void DynamicObjectSchema_ParsingFromJson_Id_Test()
		{
			var idField = this.TestSchema["_id"];
			Assert.NotNull(idField);
			Assert.AreEqual(DynamicObjectFieldType.String, idField.Type);
			Assert.AreEqual(true, idField.IsRequired);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_FirstName_Test()
		{
			var firstNameField = this.TestSchema["firstname"];
			Assert.NotNull(firstNameField);
			Assert.AreEqual(DynamicObjectFieldType.String, firstNameField.Type);
			Assert.AreEqual(false, firstNameField.IsRequired);
			Assert.AreEqual(string.Empty, firstNameField.Default);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_LastName_Test()
		{
			var lastNameField = this.TestSchema["lastname"];
			Assert.NotNull(lastNameField);
			Assert.AreEqual(DynamicObjectFieldType.String, lastNameField.Type);
			Assert.AreEqual(false, lastNameField.IsRequired);
			Assert.AreEqual(string.Empty, lastNameField.Default);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_Username_Test()
		{
			var userNameField = this.TestSchema["username"];
			Assert.NotNull(userNameField);
			Assert.AreEqual(DynamicObjectFieldType.String, userNameField.Type);
			Assert.AreEqual(true, userNameField.IsRequired);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_EmailAddress_Test()
		{
			var emailAddressField = this.TestSchema["email_address"];
			Assert.NotNull(emailAddressField);
			Assert.AreEqual(DynamicObjectFieldType.String, emailAddressField.Type);
			Assert.AreEqual(true, emailAddressField.IsRequired);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_Age_Test()
		{
			var ageField = this.TestSchema["age"];
			Assert.NotNull(ageField);
			Assert.AreEqual(DynamicObjectFieldType.Integer, ageField.Type);
			Assert.AreEqual(false, ageField.IsRequired);
			Assert.AreEqual(21, ageField.Default);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_Weight_Test()
		{
			var weightField = this.TestSchema["weight"];
			Assert.NotNull(weightField);
			Assert.AreEqual(DynamicObjectFieldType.Double, weightField.Type);
			Assert.AreEqual(false, weightField.IsRequired);
			Assert.AreEqual(70.5d, weightField.Default);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_BirthDate_Test()
		{
			var birthDateField = this.TestSchema["birth_date"];
			Assert.NotNull(birthDateField);
			Assert.AreEqual(DynamicObjectFieldType.Date, birthDateField.Type);
			Assert.AreEqual(true, birthDateField.IsRequired);
			Assert.AreEqual(DateTime.Parse("2021-01-29T16:31:58"), birthDateField.Default);
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_NestedObject_Test()
		{
			var nestedObjectField = this.TestSchema["complex_type"] as DynamicObject;
			Assert.NotNull(nestedObjectField);
			Assert.AreEqual(DynamicObjectFieldType.Object, nestedObjectField.Type);
			Assert.AreEqual(false, nestedObjectField.IsRequired);

			dynamic test = new
			{
				string_field = "test", 
				integer_field = 9716, 
				test_object = new
				{
					double_field = 9716.0d, 
					boolean_field = true,
					array_field = new []
					{
						new { string_field = "test_1", double_field = 9716.0d },
						new { string_field = "test_2", double_field = 9716.1d },
						new { string_field = "test_3", double_field = 9716.2d }
					}
				}
			};
			
			//Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(test), Newtonsoft.Json.JsonConvert.SerializeObject(nestedObjectField.DefaultAs(test.GetType())));
			Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(test), Newtonsoft.Json.JsonConvert.SerializeObject(nestedObjectField.Default));
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_ComplexArray_Test()
		{
			var arrayField = this.TestSchema["array"] as DynamicArray;
			Assert.NotNull(arrayField);
			Assert.AreEqual(DynamicObjectFieldType.Array, arrayField.Type);
			Assert.AreEqual(false, arrayField.IsRequired);

			//dynamic test_type_scheme = new { string_field = "test", double_field = 9716.0d };
			dynamic test = new[]
			{
				new { string_field = "test_1", double_field = 9716.0d },
				new { string_field = "test_2", double_field = 9716.1d },
				new { string_field = "test_3", double_field = 9716.2d }
			};
			
			//Assert.AreEqual(test, arrayField.DefaultAs(test_type_scheme.GetType()));
			Assert.AreEqual(Newtonsoft.Json.JsonConvert.SerializeObject(test), Newtonsoft.Json.JsonConvert.SerializeObject(arrayField.Default));
		}
		
		[Test]
		public void DynamicObjectSchema_ParsingFromJson_PrimitiveArray_Test()
		{
			var arrayField = this.TestSchema["primitive_array"] as DynamicArray;
			Assert.NotNull(arrayField);
			Assert.AreEqual(DynamicObjectFieldType.Array, arrayField.Type);
			Assert.AreEqual(false, arrayField.IsRequired);

			Assert.AreEqual(new [] { "test_1", "test_2", "test_3" }, arrayField.Default);
		}

		#endregion
	}
}