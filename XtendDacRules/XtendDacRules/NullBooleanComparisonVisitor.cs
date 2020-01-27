//------------------------------------------------------------------------------
// <copyright company="Xtend Business Software">
//   Copyright 2016 Xtend Business Software
//
//   Licensed under the Lesser General Public License, Version 2.1 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    internal class NullBooleanComparisonVisitor : TSqlConcreteFragmentVisitor
    {
        public List<BooleanComparisonExpression> InvalidBooleanComparisons { get; private set; }
        
        public NullBooleanComparisonVisitor()
        {
            InvalidBooleanComparisons = new List<BooleanComparisonExpression>();
        }

        public override void Visit(TSqlFragment node)
        {
            base.Visit(node);

            if (node is BooleanComparisonExpression expr)
            {
                if (expr.SecondExpression is NullLiteral)
                    InvalidBooleanComparisons.Add(expr);
            }
            //else if (node is BooleanIsNullExpression) // {Variable} is [not] null 
        }
    }
}