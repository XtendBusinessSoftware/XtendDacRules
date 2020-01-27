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

using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtend.Dac.Rules
{
    [XtendExportCodeAnalysisRuleAttribute(
        CursorMissingStatementRule.RuleId,
        RuleConstants.CursorMissingStatement_RuleName,              // ID used to look up the display name inside the resources file
        RuleConstants.CursorMissingStatement_ProblemDescription,    // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryDesign,            // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)]                  // This rule targets specific elements rather than the whole model
    public sealed class CursorMissingStatementRule : SqlCodeAnalysisRule
    {
        /// <summary>
        /// This rule= will be grouped by "Xtend.Dac.Rules.Naming", with the rule
        /// shown as "XR0011: Missing deallocate of cursor"
        /// </summary>
        public const string RuleId = "Xtend.Rules.Data.XR0011";

        public CursorMissingStatementRule()
        {
            // This rule supports Procedures. Only those objects will be passed to the Analyze method
            SupportedElementTypes = new[]
            {
                // Note: can use the ModelSchema definitions, or access the TypeClass for any of these types
                ModelSchema.Procedure
            };
        }

        /// <summary>
        /// For element-scoped rules the Analyze method is executed once for every matching object in the model. 
        /// </summary>
        /// <param name="ruleExecutionContext">The context object contains the TSqlObject being analyzed, a TSqlFragment
        /// that's the AST representation of the object, the current rule's descriptor, and a reference to the model being
        /// analyzed.
        /// </param>
        /// <returns>A list of problems should be returned. These will be displayed in the Visual Studio error list</returns>
        public override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();

            TSqlObject modelElement = ruleExecutionContext.ModelElement;
            string elementName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(modelElement, ElementNameStyle.EscapedFullyQualifiedName);
            TSqlFragment fragment = ruleExecutionContext.ScriptFragment;
            RuleDescriptor ruleDescriptor = ruleExecutionContext.RuleDescriptor;

            // Get schema of the procedure.
            TSqlObject schema = modelElement.GetReferenced(Procedure.Schema).SingleOrDefault();

            if (schema != null && fragment.FragmentLength > 0)
            {
                CursorVisitor visitor = new CursorVisitor(true, false);
                fragment.Accept(visitor);

                foreach (var element in visitor.DeclareCursorStatementsMissingDeallocate)
                {
                    string cursorName = element.Name.Value;
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription, "Missing deallocate",
                                                    cursorName,
                                                    elementName),
                                                modelElement,
                                                element.CursorDefinition);
                    problems.Add(problem);
                }

                foreach (var element in visitor.IncorrectDeallocateCursorStatements)
                {
                    string cursorName = element.Cursor?.Name.Value ?? "NULL";
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription, "Invalid deallocate",
                                                    cursorName,
                                                    elementName),
                                                modelElement,
                                                element);
                    problems.Add(problem);
                }

                foreach (var element in visitor.DeclareCursorStatementsMissingOpen)
                {
                    string cursorName = element.Name.Value;
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription, "Missing open",
                                                    cursorName,
                                                    elementName),
                                                modelElement,
                                                element.CursorDefinition);
                    problems.Add(problem);
                }

                foreach (var element in visitor.IncorrectOpenCursorStatements)
                {
                    string cursorName = element.Cursor?.Name.Value ?? "NULL";
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription, "Invalid open",
                                                    cursorName,
                                                    elementName),
                                                modelElement,
                                                element);
                    problems.Add(problem);
                }
            }
            return problems;
        }
    }
}
