using System.Text.Json.Serialization;
using Ertis.Schema.Extensions;
using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

using DynamicObject = Ertis.Schema.Dynamics.DynamicObject;

namespace Ertis.Schema.Benchmarks;

/// <summary>
/// A member-like user type and document (shaped like the ErtisAuth user types)
/// </summary>
public static class SampleData
{
	#region Constants
	
	public const string MEMBER_JSON =
		"""
		{
			"_id": "65a0f0c2e4b0a1b2c3d4e5f6",
			"firstname": "Jane",
			"lastname": "Doe",
			"email": "jane@example.com",
			"age": 30,
			"birthday": "1990-01-31T10:00:00Z",
			"gender": "f",
			"address": { "city": "Istanbul", "zip": 34000 },
			"phones": [{ "number": "1", "label": "home" }, { "number": "2" }, { "number": "3" }],
			"tags": ["a", "b", "c"]
		}
		""";
	
	#endregion
	
	#region Methods
	
	public static MemberSchema CreateSchema()
	{
		return new MemberSchema
		{
			Properties =
			[
				new StringFieldInfo { Name = "_id" },
				new StringFieldInfo { Name = "firstname", IsRequired = true, MaxLength = 50 },
				new StringFieldInfo { Name = "lastname", MaxLength = 50, RegexPattern = "^[A-Za-z]+$" },
				new EmailAddressFieldInfo { Name = "email" },
				new IntegerFieldInfo { Name = "age", Minimum = 0, Maximum = 150 },
				new DateTimeFieldInfo { Name = "birthday" },
				new EnumFieldInfo { Name = "gender", Items = [new EnumFieldInfo.EnumItem { DisplayName = "F", Value = "f" }, new EnumFieldInfo.EnumItem { DisplayName = "M", Value = "m" }] },
				new ObjectFieldInfo([new StringFieldInfo { Name = "city" }, new IntegerFieldInfo { Name = "zip" }, new StringFieldInfo { Name = "country", DefaultValue = "TR" }]) { Name = "address" },
				new ArrayFieldInfo
				{
					Name = "phones",
					UniqueBy = ["number"],
					ItemSchema = new ObjectFieldInfo([new StringFieldInfo { Name = "number", IsRequired = true }, new StringFieldInfo { Name = "label" }]) { Name = "$schema" }
				},
				new TagsFieldInfo { Name = "tags" },
				new StringFieldInfo { Name = "fullname", FormatPattern = "{firstname} {lastname}" }
			]
		};
	}
	
	public static Member CreateMember()
	{
		return new Member
		{
			Id = "65a0f0c2e4b0a1b2c3d4e5f6",
			Firstname = "Jane",
			Lastname = "Doe",
			Email = "jane@example.com",
			Age = 30,
			Birthday = new DateTime(1990, 1, 31, 10, 0, 0, DateTimeKind.Utc),
			Address = new Address { City = "Istanbul", Zip = 34000 },
			Tags = ["a", "b", "c"]
		};
	}
	
	#endregion
}

public sealed class MemberSchema : ISchema
{
	public string Slug => "member";
	
	public required IReadOnlyCollection<IFieldInfo> Properties { get; init; }
	
	public bool AllowAdditionalProperties => false;
	
	public bool ValidateSchema(out Exception? exception)
	{
		this.Validate(out exception);
		return exception == null;
	}
	
	public bool ValidateContent(DynamicObject obj, IValidationContext validationContext)
	{
		return this.ValidateData(obj, validationContext);
	}
}

public sealed class Member
{
	[JsonPropertyName("_id")]
	public string? Id { get; set; }
	
	[JsonPropertyName("firstname")]
	public string? Firstname { get; set; }
	
	[JsonPropertyName("lastname")]
	public string? Lastname { get; set; }
	
	[JsonPropertyName("email")]
	public string? Email { get; set; }
	
	[JsonPropertyName("age")]
	public int Age { get; set; }
	
	[JsonPropertyName("birthday")]
	public DateTime Birthday { get; set; }
	
	[JsonPropertyName("address")]
	public Address? Address { get; set; }
	
	[JsonPropertyName("tags")]
	public string[]? Tags { get; set; }
}

public sealed class Address
{
	[JsonPropertyName("city")]
	public string? City { get; set; }
	
	[JsonPropertyName("zip")]
	public int Zip { get; set; }
}
