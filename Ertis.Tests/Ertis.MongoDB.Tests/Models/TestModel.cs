using System;
using System.Collections.Generic;
using Ertis.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Models
{
	public class TestModel : IEntity<string>
	{
		#region Properties

		[BsonId]
		[BsonIgnoreIfDefault]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		
		[BsonElement("string_field")]
		public string Text { get; set; }
		
		[BsonElement("int_field")]
		public int Integer { get; set; }
		
		[BsonElement("double_field")]
		public double Double { get; set; }
		
		[BsonElement("datetime_field")]
		public DateTime? NullableDate { get; set; }
		
		[BsonElement("enum_field")]
		public TestEnum Enum { get; set; }
		
		[BsonElement("array_field")]
		public TestModel[] Array { get; set; }
		
		#endregion

		#region Methods

		public static IEnumerable<TestModel> GenerateRandom(int count)
		{
			List<TestModel> list = new List<TestModel>();
			for (int i = 0; i < count; i++)
			{
				list.Add(GenerateRandom(i + 1, 2));
			}

			return list;
		}
		
		public static TestModel GenerateRandom(int no, int childCount)
		{
			var children = new List<TestModel>();
			for (int i = 0; i < childCount; i++)
			{
				children.Add(GenerateRandom(no + 10, 0));
			}
			
			var random = new Random((int)DateTime.Now.Ticks);
			return new TestModel
			{
				Text = $"Entity - {no}",
				Integer = no,
				Double = no * random.NextDouble(),
				NullableDate = DateTime.Now.AddDays(no),
				Enum = (TestEnum) (no % 4),
				Array = children.ToArray()
			};
		}

		#endregion
	}

	public enum TestEnum
	{
		EnumValue1,
		EnumValue2,
		EnumValue3,
		EnumValue4,
	}
}