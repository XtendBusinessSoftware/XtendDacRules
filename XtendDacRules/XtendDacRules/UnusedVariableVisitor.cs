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
        public Dictionary<string, DeclareVariableElement> DeclareVariableElements { get; private set; }
        public Dictionary<string, TSqlFragment> AssignedVariableElements { get; private set; }

        private int index;
        private readonly bool checkParameters;
        private readonly bool checkAssigned;

        public UnusedVariableVisitor(bool checkParameters = false, bool checkAssigned = false)
        {
            DeclareVariableElements = new Dictionary<string, DeclareVariableElement>();
            AssignedVariableElements = new Dictionary<string, TSqlFragment>();
            index = 0;
            this.checkParameters = checkParameters;
            this.checkAssigned = checkAssigned;
        }

        public override void Visit(TSqlFragment node)
        {
            base.Visit(node);

            if ((DeclareVariableElements.Count > 0 || AssignedVariableElements.Count > 0)
                && node.StartOffset >= index)
            {
                // Get the token stream for the fragment and try to match the variables from our list
                IList<TSqlParserToken> stream = node.ScriptTokenStream;
                for (int i = node.FirstTokenIndex; i <= node.LastTokenIndex; i++)
                {
                    TSqlParserToken token = stream[i];
                    if (token.TokenType == TSqlTokenType.Variable)
                    {
                        // Declared variable matches variable in other text. Remove it from the list.
                        if (!checkAssigned && DeclareVariableElements.ContainsKey(token.Text))
                        {
                            DeclareVariableElements.Remove(token.Text);
                        }
                        if (checkAssigned && AssignedVariableElements.ContainsKey(token.Text))
                        {
                            AssignedVariableElements.Remove(token.Text);
                        }
                    }
                }
            }

            if (node is DeclareVariableElement && (checkParameters && node is ProcedureParameter || !checkParameters && !(node is ProcedureParameter)))
            {
                if (!checkAssigned || !checkParameters || (checkParameters && ((ProcedureParameter)node).Modifier != ParameterModifier.Output))
                {
                    DeclareVariableElement element = (DeclareVariableElement)node;
                    DeclareVariableElements[element.VariableName.Value] = element;
                    if (index < node.StartOffset + node.FragmentLength)
                        index = node.StartOffset + node.FragmentLength;
                }
            }

            if (node is WhileStatement)
            {
                //WhileStatement element = (WhileStatement)node;
                IList<TSqlParserToken> stream = node.ScriptTokenStream;
                for (int i = node.FirstTokenIndex; i <= node.LastTokenIndex; i++)
                {
                    TSqlParserToken token = stream[i];
                    if (token.TokenType == TSqlTokenType.Variable)
                    {
                        // ignore variables in a loop predicate (they are used here)
                        if (DeclareVariableElements.ContainsKey(token.Text))
                        {
                            DeclareVariableElements.Remove(token.Text);
                        }
                    }
                }
            }

            if (checkAssigned)
            {
                if (node is SetVariableStatement)
                {
                    SetVariableStatement element = (SetVariableStatement)node;
                    if (DeclareVariableElements.ContainsKey(element.Variable.Name)) // account for checkParameters value
                    {
                        AssignedVariableElements[element.Variable.Name] = element.Variable;
                        if (index < node.StartOffset + node.FragmentLength)
                            index = node.StartOffset + node.FragmentLength;
                    }
                }
                else if (node is SelectSetVariable)
                {
                    SelectSetVariable element = (SelectSetVariable)node;
                    if (DeclareVariableElements.ContainsKey(element.Variable.Name)) // account for checkParameters value
                    {
                        AssignedVariableElements[element.Variable.Name] = element.Variable;
                        if (index < node.StartOffset + node.FragmentLength)
                            index = node.StartOffset + node.FragmentLength;
                    }
                }
            }
        }
    }
}