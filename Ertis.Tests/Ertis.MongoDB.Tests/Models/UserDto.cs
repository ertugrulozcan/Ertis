using System;
using System.Collections.Generic;
using Ertis.MongoDB.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ertis.Tests.Ertis.MongoDB.Tests.Models
{
    public class UserDto
    {
        #region Properties
		
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [Searchable]
        [BsonElement("firstname")]
        public string FirstName { get; set; }
		
        [Searchable]
        [BsonElement("lastname")]
        public string LastName { get; set; }
		
        [Searchable]
        [BsonElement("username")]
        public string Username { get; set; }
		
        [Searchable]
        [BsonElement("email_address")]
        public string EmailAddress { get; set; }
		
        [BsonElement("membership_id")]
        public string MembershipId { get; set; }
		
        [BsonElement("role")]
        public string Role { get; set; }
		
        [BsonElement("permissions")]
        public IEnumerable<string> Permissions { get; set; }
		
        [BsonElement("forbidden")]
        public IEnumerable<string> Forbidden { get; set; }
		
        [BsonElement("password_hash")]
        public string PasswordHash { get; set; }
		
        [BsonElement("sys")]
        public SysModelDto Sys { get; set; }
		
        [BsonElement("additional_properties")]
        public BsonDocument AdditionalProperties { get; set; }
		
        #endregion
    }
    
    public class SysModelDto
    {
	    #region Properties

	    [BsonIgnoreIfNull]
	    [BsonElement("created_at")]
	    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
	    public DateTime? CreatedAt { get; set; }
		
	    [BsonIgnoreIfNull]
	    [BsonElement("created_by")]
	    public string CreatedBy { get; set; }
		
	    [BsonIgnoreIfNull]
	    [BsonElement("modified_at")]
	    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
	    public DateTime? ModifiedAt { get; set; }
		
	    [BsonIgnoreIfNull]
	    [BsonElement("modified_by")]
	    public string ModifiedBy { get; set; }

	    #endregion
    }
}