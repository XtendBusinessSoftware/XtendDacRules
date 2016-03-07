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
        ForeignKeyWithoutIndexRule.RuleId,
        RuleConstants.ForeignKeyWithoutIndex_RuleName,              // ID used to look up the display name inside the resources file
        RuleConstants.ForeignKeyWithoutIndex_ProblemDescription,    // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryPerformance,            // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)]                  // This rule targets specific elements rather than the whole model

    public sealed class ForeignKeyWithoutIndexRule : SqlCodeAnalysisRule
    {
        /// <summary>
        /// This rule= will be grouped by "Xtend.Dac.Rules.Design", with the rule
        /// shown as "XR0001: All foreign keys should have an index that covers the foreign key"
        /// </summary>
        public const string RuleId = "Xtend.Rules.Data.XR0001";

        public ForeignKeyWithoutIndexRule()
        {
            // This rule supports Foreign keys. Only those objects will be passed to the Analyze method
            SupportedElementTypes = new[]
            {
                ModelSchema.ForeignKeyConstraint
            };

        }

        /// <summary>
        /// Compares two sets of columns to each other. Returns true if the first set is a subset of the second set.
        /// The order of the columns matters.
        /// </summary>
        /// <param name="firstColumns">The first set of columns to check. It has to be a subset of the second one to get a match</param>
        /// <param name="secondColumns">The second set of columns to check against</param>
        /// <param name="ruleExecutionContext">The context object which is used to get the right name of a column</param>
        /// <returns>Returns true if the first set is an ordered subset of the second set and false otherwise</returns>
        private bool CompareColumns(List<TSqlObject> firstColumns, List<TSqlObject> secondColumns, SqlRuleExecutionContext ruleExecutionContext)
        {
            bool foundMatch = false;

            // Try to match all the columns in the first set of columns to the second
            // If it works we have a match and can stop.
            // First check we have to do is make sure there are not more columns in the foreign key than the index
            // If not, there is no point in comparing as the first set will never fit in the second set
            if (firstColumns.Count <= secondColumns.Count)
            {
                // Assume we have a match and try to invalidate it.
                foundMatch = true;

                for (int i = 0; i < firstColumns.Count; i++)
                {
                    TSqlObject firstColumn = firstColumns[i];
                    TSqlObject secondColumn = secondColumns[i];

                    string firstColumnName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(firstColumn, ElementNameStyle.EscapedFullyQualifiedName);
                    string secondColumnName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(secondColumn, ElementNameStyle.EscapedFullyQualifiedName);

                    if (firstColumnName != secondColumnName)
                    {
                        foundMatch = false;
                        break;
                    }
                }
            }

            return foundMatch;

        }

        public override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();

            TSqlObject modelElement = ruleExecutionContext.ModelElement;
            string elementName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(modelElement, ElementNameStyle.EscapedFullyQualifiedName);
            RuleDescriptor ruleDescriptor = ruleExecutionContext.RuleDescriptor;

            // Get columns of the foreign key
            List<TSqlObject> fkColumns = modelElement.GetReferenced(ForeignKeyConstraint.Columns).ToList();

            // Get table of the foreign key.
            TSqlObject table = modelElement.GetReferenced(ForeignKeyConstraint.Host).SingleOrDefault();

            bool foundMatch = false;

            if (table != null)
            {
                // Check the indexes of the table
                foreach (TSqlObject index in table.GetReferencing(Index.IndexedObject))
                {
                    // Get columns of the index
                    List<TSqlObject> indexColumns = index.GetReferenced(Index.Columns).ToList();

                    foundMatch = CompareColumns(fkColumns, indexColumns, ruleExecutionContext);

                    if (foundMatch)
                        break;
                }

                if (!foundMatch)
                {
                    // Check the primary keys as well
                    foreach (TSqlObject pk in table.GetReferencing(PrimaryKeyConstraint.Host))
                    {
                        // Get columns of the primary key
                        List<TSqlObject> pkColumns = pk.GetReferenced(PrimaryKeyConstraint.Columns).ToList();

                        foundMatch = CompareColumns(fkColumns, pkColumns, ruleExecutionContext);

                        if (foundMatch)
                            break;
                    }
                }

                if (!foundMatch)
                {
                    // Check the unique constraints as well
                    foreach (TSqlObject uq in table.GetReferencing(UniqueConstraint.Host))
                    {
                        // Get columns of the primary key
                        List<TSqlObject> uqColumns = uq.GetReferenced(UniqueConstraint.Columns).ToList();

                        // Try to match all the columns in the foreign key to the index.
                        foundMatch = CompareColumns(fkColumns, uqColumns, ruleExecutionContext);

                        if (foundMatch)
                            break;
                    }
                }

                if (!foundMatch)
                {
                    SqlRuleProblem problem = new SqlRuleProblem(
                                                String.Format(
                                                    CultureInfo.CurrentCulture,
                                                    ruleDescriptor.DisplayDescription,
                                                    elementName,
                                                    ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(table, ElementNameStyle.EscapedFullyQualifiedName)),
                                                modelElement);
                    problems.Add(problem);

                }
            }

            return problems;
        }
    }
}
