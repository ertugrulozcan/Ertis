using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Ertis.Schema.Exceptions;
using Ertis.Schema.Types.Primitives;
using Ertis.Schema.Validation;

namespace Ertis.Schema.Types.CustomTypes;

public partial class EmailAddressFieldInfo : StringFieldInfo
{
	#region Properties
	
	[JsonPropertyName("type")]
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public override FieldType Type => FieldType.email;
	
	#endregion
	
	#region Methods
	
	protected internal override bool Validate(object? obj, IValidationContext validationContext)
	{
		var isValid = base.Validate(obj, validationContext);
		
		if (obj is string emailAddress)
		{
			if (!IsValidEmail(emailAddress))
			{
				isValid = false;
				validationContext.Errors.Add(new FieldValidationException("Email address is not valid", this));
			}
		}
		
		return isValid;
	}
	
	private static bool IsValidEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return false;
		}
		
		try
		{
			// Normalize the domain
			email = DomainRegex().Replace(email, DomainMapper);
			
			// Examines the domain part of the email and normalizes it.
			string DomainMapper(Match match)
			{
				// Use IdnMapping class to convert Unicode domain names.
				var idn = new IdnMapping();
				
				// Pull out and process domain name (throws ArgumentException on invalid)
				var domainName = idn.GetAscii(match.Groups[2].Value);
				
				return match.Groups[1].Value + domainName;
			}
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
		catch (ArgumentException)
		{
			return false;
		}
		
		try
		{
			return EmailRegex().IsMatch(email);
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
	}
	
	[GeneratedRegex("(@)(.+)$", RegexOptions.None, matchTimeoutMilliseconds: 200)]
	private static partial Regex DomainRegex();
	
	[GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 250)]
	private static partial Regex EmailRegex();
	
	#endregion
}