using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    public class GetQuerySpecificationsVisitor : TSqlFragmentVisitor
    {
        public List<QuerySpecification> Queries { get; } = new List<QuerySpecification>();
        
        public override void Visit(QueryExpression expression)
        {
            var query = expression as QuerySpecification;
            if (query != null)
            {
                Queries.Add(query);
            }
            base.Visit(expression);
        }
    }
}
