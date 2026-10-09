using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.MongoDB.Exceptions;

public class IndexException(string message) : ErtisException(HttpStatusCode.BadRequest, message, "MongoIndexException");