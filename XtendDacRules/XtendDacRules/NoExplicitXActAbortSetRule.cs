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
        NoExplicitXActAbortSetRule.RuleId,
        RuleConstants.NoExplicitXActAbortSet_RuleName,              // ID used to look up the display name inside the resources file
        RuleConstants.NoExplicitXActAbortSet_ProblemDescription,    // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryPerformance,            // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)]                  // This rule targets specific elements rather than the whole model

    public sealed class NoExplicitXActAbortSetRule : XtendSqlProcedureAnalysisRule
    {
        /// <summary>
        /// This rule= will be grouped by "Xtend.Dac.Rules.Performance", with the rule
        /// shown as "XR0009: All procedures must set xact_abort"
        /// </summary>
        public const string RuleId = "Xtend.Rules.Data.XR0009";

        public NoExplicitXActAbortSetRule()
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
        public override IList<SqlRuleProblem> Analyze(XtendSqlRuleExecutionContext context)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();

            // Use a visitor to see if the procedure has a nocount set
            SetXActAbortVisitor visitor = new SetXActAbortVisitor();
            context.ScriptFragment.Accept(visitor);
            if (!visitor.SetXActAbortFound)
            {
                SqlRuleProblem problem = new SqlRuleProblem(
                                            String.Format(
                                                CultureInfo.CurrentCulture,
                                                context.RuleDescriptor.DisplayDescription,
                                                context.ElementName),
                                            context.ModelElement);
                problems.Add(problem);
            }

            return problems;
        }
    }
}
