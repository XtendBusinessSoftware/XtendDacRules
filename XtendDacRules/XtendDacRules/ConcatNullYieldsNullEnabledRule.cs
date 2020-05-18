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
        ConcatNullYieldsNullEnabledRule.RuleId,
        RuleConstants.ConcatNullYieldsNullEnabled_RuleName,              // ID used to look up the display name inside the resources file
        RuleConstants.ConcatNullYieldsNullEnabled_ProblemDescription,    // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryDesign,            // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)]                  // This rule targets specific elements rather than the whole model

    public sealed class ConcatNullYieldsNullEnabledRule : XtendSqlProcedureAnalysisRule
    {
        /// <summary>
        /// This rule= will be grouped by "Xtend.Dac.Rules.Design", with the rule
        /// shown as "XR0008: Concat_null_yields_null option should not be used
        /// </summary>
        public const string RuleId = "Xtend.Rules.Data.XR0008";

        public ConcatNullYieldsNullEnabledRule()
        {
            // This rule supports Procedures. Only those objects will be passed to the Analyze method
            SupportedElementTypes = new[]
            {
                ModelSchema.Procedure
            };
        }

        public override IList<SqlRuleProblem> Analyze(XtendSqlRuleExecutionContext context)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();

            // Use a visitor to see if the procedure has a nocount set
            SetConcatNullYieldsNullVisitor visitor = new SetConcatNullYieldsNullVisitor();
            context.ScriptFragment.Accept(visitor);
            if (visitor.SetConcatNullYieldsNullEnabled)
            {
                foreach (PredicateSetStatement element in visitor.SetCalls)
                {
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    context.RuleDescriptor.DisplayDescription,
                                                    context.ElementName),
                                                context.ModelElement,
                                                element);
                    problems.Add(problem);
                }
            }

            return problems;
        }
    }
}
