using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    public class GetConstraintsVisitor : TSqlFragmentVisitor
    {
        public List<BooleanExpression> Constraints { get; } = new List<BooleanExpression>();
        
        public override void Visit(BooleanExpression expression)
        {
            Constraints.Add(expression);
            base.Visit(expression);
        }
    }
}
