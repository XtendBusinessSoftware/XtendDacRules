using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    public class CollectJoinVisitor : TSqlFragmentVisitor
    {
        public List<JoinTableReference> Joins { get; } = new List<JoinTableReference>();

        public override void Visit(JoinTableReference joinTableReference)
        {
            Joins.Add(joinTableReference);
            base.Visit(joinTableReference);
        }
    }
}
