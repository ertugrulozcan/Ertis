using System.Globalization;
using System.Text.Json;

namespace Ertis.Extensions.AspNetCore.Helpers;

public static class QueryHelper
{
	#region Methods
	
	/// <summary>
	/// Returns the raw json of the 'where' node of the query body, or null when the body has no 'where' node
	/// </summary>
	/// <exception cref="JsonException">The body is not a valid json</exception>
	public static string? ExtractWhereQuery(dynamic body)
	{
		using JsonDocument? document = ParseBody((object?) body);
		if (document?.RootElement is { ValueKind: JsonValueKind.Object } root && root.TryGetProperty("where", out JsonElement whereNode))
		{
			return whereNode.GetRawText();
		}
		
		return null;
	}
	
	/// <summary>
	/// Returns the fields of the 'select' node of the query body (1, true, "true" or a non-zero number includes a field, 0 or false excludes it)
	/// </summary>
	/// <exception cref="JsonException">The body is not a valid json</exception>
	public static Dictionary<string, bool> ExtractSelectFields(dynamic body)
	{
		var fieldDictionary = new Dictionary<string, bool>();
		
		using JsonDocument? document = ParseBody((object?) body);
		if (document?.RootElement is { ValueKind: JsonValueKind.Object } root && root.TryGetProperty("select", out JsonElement selectNode) && selectNode.ValueKind == JsonValueKind.Object)
		{
			foreach (var property in selectNode.EnumerateObject())
			{
				if (TryGetSelection(property.Value, out var isSelected))
				{
					// A repeated field overrides the previous one
					fieldDictionary[property.Name] = isSelected;
				}
			}
		}
		
		return fieldDictionary;
	}
	
	private static JsonDocument? ParseBody(object? body)
	{
		var json = body as string ?? body?.ToString();
		if (string.IsNullOrWhiteSpace(json))
		{
			return null;
		}
		
		return JsonDocument.Parse(json, new JsonDocumentOptions
		{
			AllowTrailingCommas = true,
			CommentHandling = JsonCommentHandling.Skip
		});
	}
	
	private static bool TryGetSelection(JsonElement value, out bool isSelected)
	{
		switch (value.ValueKind)
		{
			case JsonValueKind.True:
				isSelected = true;
				return true;
			case JsonValueKind.False:
				isSelected = false;
				return true;
			case JsonValueKind.Number:
				isSelected = value.GetDouble() != 0;
				return true;
			case JsonValueKind.String:
			{
				var text = value.GetString();
				if (bool.TryParse(text, out isSelected))
				{
					return true;
				}
				
				if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number))
				{
					isSelected = number == 1;
					return true;
				}
				
				isSelected = false;
				return false;
			}
			default:
				isSelected = false;
				return false;
		}
	}
	
	#endregion
}