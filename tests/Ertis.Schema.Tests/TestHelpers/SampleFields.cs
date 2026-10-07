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
			new StringFieldInfo { Name = "title", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", DisplayName = "Title", Description = "The title", IsRequired = true, MinLength = 2, MaxLength = 50, RegexPattern = "^[A-Z]" },
			new StringFieldInfo { Name = "slug", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", FormatPattern = "{title}", IsReadonly = true },
			new IntegerFieldInfo { Name = "age", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", Minimum = 0, Maximum = 150, MultipleOf = 1, DefaultValue = 18, IsUnique = true },
			new FloatFieldInfo { Name = "ratio", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", ExclusiveMinimum = 0, ExclusiveMaximum = 1 },
			new BooleanFieldInfo { Name = "active", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", DefaultValue = true },
			new ArrayFieldInfo
			{
				Name = "phones", IsSearchable = true, SearchWeight = 1.5, Appearance = "default",
				MinCount = 1,
				MaxCount = 3,
				UniqueBy = ["number"],
				ItemSchema = new ObjectFieldInfo([new StringFieldInfo { Name = "number", IsRequired = true }, new StringFieldInfo { Name = "label" }]) { Name = "$schema" }
			},
			new ArrayFieldInfo { Name = "scores", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", UniqueItems = true, ItemSchema = new IntegerFieldInfo { Name = "$schema", Minimum = 0 } },
			new EnumFieldInfo { Name = "country", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", IsMultiple = true, Items = [new EnumFieldInfo.EnumItem { DisplayName = "Türkiye", Value = "tr" }, new EnumFieldInfo.EnumItem { DisplayName = "Germany", Value = "de" }] },
			new ConstantFieldInfo { Name = "kind", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", ValueType = ConstantFieldInfo.ConstantType.@string, Value = "member" },
			new TagsFieldInfo { Name = "tags", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", MaxCount = 5, MaxLength = 20 },
			new JsonFieldInfo { Name = "data", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new DateFieldInfo { Name = "birthday", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", MinValue = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
			new DateTimeFieldInfo { Name = "startsAt", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new LongTextFieldInfo { Name = "bio", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", MaxLength = 1000 },
			new RichTextFieldInfo { Name = "body", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new EmailAddressFieldInfo { Name = "email", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", IsUnique = true },
			new UriFieldInfo { Name = "website", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new HostNameFieldInfo { Name = "host", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new ColorFieldInfo { Name = "color", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new LocationFieldInfo { Name = "position", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new ReferenceFieldInfo { Name = "author", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", ReferenceType = ReferenceFieldInfo.ReferenceTypes.single, ContentType = "authors" },
			new CodeFieldInfo { Name = "snippet", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new ImageFieldInfo { Name = "photos", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", Multiple = true, MaxCount = 4 },
			new VideoFieldInfo { Name = "clip", IsSearchable = true, SearchWeight = 1.5, Appearance = "default" },
			new ObjectFieldInfo([new StringFieldInfo { Name = "city", IsRequired = true }, new IntegerFieldInfo { Name = "zip" }]) { Name = "address", IsSearchable = true, SearchWeight = 1.5, Appearance = "default", AllowAdditionalProperties = true }
		];
	}
	
	#endregion
}
