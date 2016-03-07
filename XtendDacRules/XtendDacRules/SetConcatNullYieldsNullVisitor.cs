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
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    internal class SetConcatNullYieldsNullVisitor : TSqlConcreteFragmentVisitor
    {
        public bool SetConcatNullYieldsNullEnabled { get; private set; }
        public IList<PredicateSetStatement> SetCalls { get; private set; }

        public SetConcatNullYieldsNullVisitor()
        {
            SetConcatNullYieldsNullEnabled = false;

            SetCalls = new List<PredicateSetStatement>();
        }

        public override void ExplicitVisit(PredicateSetStatement node)
        {
            if (node.Options == SetOptions.ConcatNullYieldsNull && !node.IsOn)
            {
                SetConcatNullYieldsNullEnabled = true;
                SetCalls.Add(node);
            }
        }
    }
}