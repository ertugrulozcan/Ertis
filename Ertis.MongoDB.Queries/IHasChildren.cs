using System.Collections.Generic;

namespace Ertis.MongoDB.Queries
{
    internal interface IHasChildren
    {
        List<IQuery> Children { get; }

        void AddQuery(IQuery query);
    }
}