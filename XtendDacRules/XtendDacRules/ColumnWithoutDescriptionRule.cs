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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xtend.Dac.Rules
{
    [XtendExportCodeAnalysisRuleAttribute(
        ColumnWithoutDescriptionRule.RuleId,
        RuleConstants.ColumnWithoutDescription_RuleName,              // ID used to look up the display name inside the resources file
        RuleConstants.ColumnWithoutDescription_ProblemDescription,    // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryDesign,            // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)]                  // This rule targets specific elements rather than the whole model

    public sealed class ColumnWithoutDescriptionRule : SqlCodeAnalysisRule
    {
        /// <summary>
        /// This rule= will be grouped by "Xtend.Dac.Rules.Design", with the rule
        /// shown as "XR0003: All columns should have a description"
        /// </summary>
        public const string RuleId = "Xtend.Rules.Data.XR0003";

        public ColumnWithoutDescriptionRule()
        {
            // This rule supports Tables. Only those objects will be passed to the Analyze method
            SupportedElementTypes = new[]
            {
                ModelSchema.Table
            };

        }

        public override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();

            TSqlObject modelElement = ruleExecutionContext.ModelElement;
            string elementName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(modelElement, ElementNameStyle.EscapedFullyQualifiedName);
            RuleDescriptor ruleDescriptor = ruleExecutionContext.RuleDescriptor;

            List<TSqlObject> columns = modelElement.GetReferenced(Table.Columns).ToList();
            foreach (TSqlObject column in modelElement.GetReferenced(Table.Columns))
            {
                bool hasDescription = false;

                // Check if it has an extended property
                List<TSqlObject> extendedProperties = column.GetReferencing(ExtendedProperty.Host).ToList();

                if (extendedProperties.Count > 0)
                {
                    foreach (TSqlObject prop in extendedProperties)
                    {
                        if (ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(prop, ElementNameStyle.SimpleName) == "MS_Description")
                        {
                            hasDescription = true;
                            break;
                        }
                    }
                }

                if (!hasDescription)
                {
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription,
                                                    ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(column, ElementNameStyle.EscapedFullyQualifiedName)),
                                                column);
                    problems.Add(problem);

                }
            }

            return problems;
        }
    }
}
