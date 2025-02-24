using Ertis.MongoDB.Helpers;
using NUnit.Framework;

namespace Ertis.Tests.Ertis.MongoDB.Tests;

public class ObjectIdTests
{
    #region Methods
    
    [Test]
    public void ObjectIdParseNinQuery_Test()
    {
        const string query = "{\"categories.slug\":\"futbol\",\"sys.state\":\"published\",\"_id\":{\"$nin\":[\"66c8c0a7a54a186fb2e05158\",\"66c8c0a9a54a186fb2e0515b\",\"66c8c0aca54a186fb2e05160\",\"66c8c0b1a54a186fb2e05165\",\"66c8c0b3a54a186fb2e05168\",\"66c8c0b6a54a186fb2e0516d\",\"66c8c0b7a54a186fb2e05172\",\"66c8c0bda54a186fb2e0517a\",\"66c8c0bfa54a186fb2e0517f\",\"66c8c0c1a54a186fb2e05184\"]}}";
        const string expected = "{\"categories.slug\":\"futbol\",\"sys.state\":\"published\",\"_id\":{\"$nin\":[ObjectId(\"66c8c0a7a54a186fb2e05158\"),ObjectId(\"66c8c0a9a54a186fb2e0515b\"),ObjectId(\"66c8c0aca54a186fb2e05160\"),ObjectId(\"66c8c0b1a54a186fb2e05165\"),ObjectId(\"66c8c0b3a54a186fb2e05168\"),ObjectId(\"66c8c0b6a54a186fb2e0516d\"),ObjectId(\"66c8c0b7a54a186fb2e05172\"),ObjectId(\"66c8c0bda54a186fb2e0517a\"),ObjectId(\"66c8c0bfa54a186fb2e0517f\"),ObjectId(\"66c8c0c1a54a186fb2e05184\")]}}";
        var ensured = QueryHelper.EnsureObjectIdsAndISODates(query);
        var anan1 = expected.Replace(" ", string.Empty).Replace("\n", string.Empty).Trim();
        var anan2 = ensured.Replace(" ", string.Empty).Replace("\n", string.Empty).Trim();
        Assert.That(expected.Replace(" ", string.Empty).Replace("\n", string.Empty).Trim() == ensured.Replace(" ", string.Empty).Replace("\n", string.Empty).Trim());
    }
    
    #endregion
}