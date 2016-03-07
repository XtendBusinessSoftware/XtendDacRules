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
    internal class UnusedVariableVisitor : TSqlConcreteFragmentVisitor
    {
        public Dictionary<string, DeclareVariableElement> DeclareVariableElements{ get; private set; }

        private int index;
        private bool checkParameters;

        public UnusedVariableVisitor(bool checkParameters = false)
        {
            DeclareVariableElements = new Dictionary<string, DeclareVariableElement>();
            index = 0;
            this.checkParameters = checkParameters;
        }

        public override void Visit(TSqlFragment node)
        {
            base.Visit(node);
            if (DeclareVariableElements.Count > 0 && node.StartOffset >= index)
            {
                // Get the token stream for the fragment and try to match the variables from our list
                IList<TSqlParserToken> stream = node.ScriptTokenStream;
                for (int i = node.FirstTokenIndex; i <= node.LastTokenIndex; i++)
                { 
                    TSqlParserToken token = stream[i];
                    if (token.TokenType == TSqlTokenType.Variable)
                    {
                        if (DeclareVariableElements.ContainsKey(token.Text))
                        {
                            // Declared variable matches variable in other text. Remove it from the list.
                            DeclareVariableElements.Remove(token.Text);
                        }
                    }
                }
            }
            if (node is DeclareVariableElement && (checkParameters && node is ProcedureParameter || !checkParameters && !(node is ProcedureParameter)))
            {
                DeclareVariableElement element = (DeclareVariableElement)node;
                DeclareVariableElements[element.VariableName.Value] = element;
                if (index < node.StartOffset + node.FragmentLength)
                    index = node.StartOffset + node.FragmentLength;
            }
        }
    }
}