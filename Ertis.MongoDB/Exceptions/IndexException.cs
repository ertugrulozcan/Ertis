using System.Net;
using Ertis.Core.Exceptions;

namespace Ertis.MongoDB.Exceptions;

// ReSharper disable once UnusedType.Global
public class IndexException(string message) : ErtisException(HttpStatusCode.BadRequest, message, "MongoIndexException");