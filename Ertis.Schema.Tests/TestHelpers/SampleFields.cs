using Ertis.Schema.Types;
using Ertis.Schema.Types.CustomTypes;
using Ertis.Schema.Types.Primitives;

namespace Ertis.Schema.Tests.TestHelpers;

/// <summary>
/// One field of each field type, with their optional rules set
/// </summary>
public static class SampleFields
{
	#region Methods
	
	public static IFieldInfo[] All()
	{
		return
		[
			new StringFieldInfo { Name = "title", DisplayName = "Title", Description = "The title", IsRequired = true, MinLength = 2, MaxLength = 50, RegexPattern = "^[A-Z]", IsSearchable = true, SearchWeight = 2 },
			new StringFieldInfo { Name = "slug", FormatPattern = "{title}", IsReadonly = true },
			new IntegerFieldInfo { Name = "age", Minimum = 0, Maximum = 150, MultipleOf = 1, DefaultValue = 18, IsUnique = true },
			new FloatFieldInfo { Name = "ratio", ExclusiveMinimum = 0, ExclusiveMaximum = 1 },
			new BooleanFieldInfo { Name = "active", DefaultValue = true },
			new ArrayFieldInfo
			{
				Name = "phones",
				MinCount = 1,
				MaxCount = 3,
				UniqueBy = ["number"],
				ItemSchema = new ObjectFieldInfo([new StringFieldInfo { Name = "number", IsRequired = true }, new StringFieldInfo { Name = "label" }]) { Name = "$schema" }
			},
			new ArrayFieldInfo { Name = "scores", UniqueItems = true, ItemSchema = new IntegerFieldInfo { Name = "$schema", Minimum = 0 } },
			new EnumFieldInfo { Name = "country", IsMultiple = true, Items = [new EnumFieldInfo.EnumItem { DisplayName = "Türkiye", Value = "tr" }, new EnumFieldInfo.EnumItem { DisplayName = "Germany", Value = "de" }] },
			new ConstantFieldInfo { Name = "kind", ValueType = ConstantFieldInfo.ConstantType.@string, Value = "member" },
			new TagsFieldInfo { Name = "tags", MaxCount = 5, MaxLength = 20 },
			new JsonFieldInfo { Name = "data" },
			new DateFieldInfo { Name = "birthday", MinValue = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
			new DateTimeFieldInfo { Name = "startsAt" },
			new LongTextFieldInfo { Name = "bio", MaxLength = 1000 },
			new RichTextFieldInfo { Name = "body" },
			new EmailAddressFieldInfo { Name = "email", IsUnique = true },
			new UriFieldInfo { Name = "website" },
			new HostNameFieldInfo { Name = "host" },
			new ColorFieldInfo { Name = "color" },
			new LocationFieldInfo { Name = "position" },
			new ReferenceFieldInfo { Name = "author", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single, ContentType = "authors" },
			new CodeFieldInfo { Name = "snippet" },
			new ImageFieldInfo { Name = "photos", Multiple = true, MaxCount = 4 },
			new VideoFieldInfo { Name = "clip" },
			new ObjectFieldInfo([new StringFieldInfo { Name = "city", IsRequired = true }, new IntegerFieldInfo { Name = "zip" }]) { Name = "address", AllowAdditionalProperties = true }
		];
	}
	
	#endregion
}
