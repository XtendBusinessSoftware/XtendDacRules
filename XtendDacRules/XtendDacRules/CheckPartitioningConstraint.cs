using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.SqlServer.Dac.CodeAnalysis;
using Microsoft.SqlServer.Dac.Model;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace Xtend.Dac.Rules
{
    [XtendExportCodeAnalysisRuleAttribute(
        RuleId,
        RuleConstants
            .MissingPartitioningColumn_RuleName, // ID used to look up the display name inside the resources file
        RuleConstants
            .MissingPartitioningColumn_ProblemDescription, // ID used to look up the description inside the resources file
        Category = RuleConstants.CategoryPerformance, // Rule category (e.g. "Design", "Naming")
        RuleScope = SqlRuleScope.Element)] // This rule targets specific elements rather than the whole model
    public class CheckPartitioningConstraint : SqlCodeAnalysisRule
    {
        public const string RuleId = "Xtend.Rules.Data.XR0013";

        public CheckPartitioningConstraint()
        {
            // This rule supports Procedures. Only those objects will be passed to the Analyze method
            SupportedElementTypes = new[]
            {
                // Note: can use the ModelSchema definitions, or access the TypeClass for any of these types
                ModelSchema.Procedure
            };
        }

        private TSqlObject ResolveTable(TSqlModel model, SchemaObjectName node)
        {
            string schema;
            if (node.SchemaIdentifier != null)
            {
                schema = node.SchemaIdentifier.Value;
            }
            else
            {
                // Assuming schema is dbo
                schema = "dbo";
            }

            string table = node.BaseIdentifier.Value;
            return model.GetObject(Table.TypeClass, new ObjectIdentifier(schema, table),
                DacQueryScopes.All);
        }

        private class PartitionedColumn
        {
            public PartitionedColumn(HashSet<string> tableNames, string columnName, TSqlFragment o)
            {
                TableNames = tableNames;
                ColumnName = columnName;
                Fragment = o;
            }

            public HashSet<string> TableNames { get; }
            public string ColumnName { get; }
            public TSqlFragment Fragment { get; }
        }

        private int IsValidColumnReference(ColumnReferenceExpression columnReference,
            List<PartitionedColumn> unspecifiedPartitions)
        {
            var identifier = columnReference.MultiPartIdentifier.Identifiers;
            for (var index = 0; index < unspecifiedPartitions.Count; index++)
            {
                var unspecifiedPartition = unspecifiedPartitions[index];
                var columnName = identifier.Last();
                if (string.Equals(unspecifiedPartition.ColumnName, columnName.Value))
                {
                    if (identifier.Count > 1)
                    {
                        var tableName = identifier[identifier.Count - 2];
                        if (!unspecifiedPartition.TableNames.Contains(tableName.Value))
                        {
                            continue;
                        }
                    }

                    return index;
                }
            }

            return -1;
        }

        private IEnumerable<PartitionedColumn> CollectFromTableReference(SqlRuleExecutionContext ruleExecutionContext,
            TSqlFragment fragment, TableReference tableReference)
        {
            if (!(tableReference is NamedTableReference namedTableReference))
                return new List<PartitionedColumn>();

            // Resolve the table from its name.
            var table =
                ResolveTable(ruleExecutionContext.SchemaModel, namedTableReference.SchemaObject);
            if (table == null)
                return new List<PartitionedColumn>();
            // In the tabe object, find all columns that are a partition column, and save them.
            var partitionColumns = table.GetReferenced(Table.PartitionColumn, DacQueryScopes.All);

            var allowedTableNames = new HashSet<string>();
            allowedTableNames.Add(namedTableReference.SchemaObject.BaseIdentifier.Value);
            if (namedTableReference.Alias != null)
                allowedTableNames.Add(namedTableReference.Alias.Value);

            return partitionColumns
                .Select(x => new PartitionedColumn(
                    allowedTableNames,
                    x.Name.Parts.Last(), fragment));
        }

        private List<PartitionedColumn> CollectFromClause(SqlRuleExecutionContext ruleExecutionContext,
            QuerySpecification querySpecification)
        {
            if (querySpecification.FromClause == null)
                return new List<PartitionedColumn>();
            var specifiedTables = querySpecification.FromClause.TableReferences;
            var unspecifiedPartitions = new List<PartitionedColumn>();
            foreach (var tableReference in specifiedTables)
            {
                unspecifiedPartitions.AddRange(CollectFromTableReference(ruleExecutionContext,
                    tableReference,
                    tableReference));
            }

            return unspecifiedPartitions;
        }

        private SqlRuleProblem CreateProblem(SqlRuleExecutionContext ruleExecutionContext,
            PartitionedColumn partitionedColumn)
        {
            return new SqlRuleProblem(
                String.Format(
                    CultureInfo.CurrentCulture,
                    ruleExecutionContext.RuleDescriptor.DisplayDescription,
                    ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(
                        ruleExecutionContext.ModelElement,
                        ElementNameStyle.EscapedFullyQualifiedName),
                    string.Join(", ", partitionedColumn.TableNames),
                    partitionedColumn.ColumnName
                ),
                ruleExecutionContext.ModelElement, partitionedColumn.Fragment);
        }

        private void SearchConstraints(TSqlFragment fragment, List<PartitionedColumn> unspecifiedPartitions)
        {
            if (unspecifiedPartitions.Count == 0) return;
            var constraintsVisitor = new GetConstraintsVisitor();
            fragment.Accept(constraintsVisitor);
            foreach (var constraint in constraintsVisitor.Constraints)
            {
                if (unspecifiedPartitions.Count == 0) return;
                // Check if the constraint is a boolean comparison.
                if (!(constraint is BooleanComparisonExpression comparison))
                    continue;

                var left = comparison.FirstExpression;
                var right = comparison.SecondExpression;

                // See if left or right side is a partitioned column, if so, mark it as handled.
                if (left is ColumnReferenceExpression columnReference)
                {
                    var r = IsValidColumnReference(columnReference, unspecifiedPartitions);
                    if (r != -1)
                        unspecifiedPartitions.RemoveAt(r);
                }

                if (right is ColumnReferenceExpression columnReferenceRight)
                {
                    var r = IsValidColumnReference(columnReferenceRight, unspecifiedPartitions);
                    if (r != -1)
                        unspecifiedPartitions.RemoveAt(r);
                }
            }
        }

        private void AnalyzeJoins(IList<SqlRuleProblem> problems, SqlRuleExecutionContext ruleExecutionContext,
            QuerySpecification querySpecification)
        {
            var visitor = new CollectJoinVisitor();
            querySpecification.Accept(visitor);
            foreach (var join in visitor.Joins)
            {
                // We should have already handled the first table reference here, as it's included from an earlier part
                // of the query. As such, we now handle the second table only.
                var tablePartitions = CollectFromTableReference(ruleExecutionContext, join.SecondTableReference,
                    join.SecondTableReference).ToList();

                if (tablePartitions.Count == 0) continue;

                if (join is QualifiedJoin qualifiedJoin)
                {
                    var condition = qualifiedJoin.SearchCondition;
                    SearchConstraints(condition, tablePartitions);
                }

                foreach (var partitionedColumn in tablePartitions)
                {
                    SqlRuleProblem problem = CreateProblem(ruleExecutionContext, partitionedColumn);
                    problems.Add(problem);
                }
            }
        }

        private void AnalyzeQueries(IList<SqlRuleProblem> problems, TSqlFragment fragment,
            SqlRuleExecutionContext ruleExecutionContext)
        {
            // Find all queries in current checking fragment.
            var visitor = new GetQuerySpecificationsVisitor();
            fragment.Accept(visitor);
            foreach (var querySpecification in visitor.Queries)
            {
                // Get the specified tables in the query.
                var unspecifiedPartitions = CollectFromClause(ruleExecutionContext, querySpecification);

                // If the query isn't querying a partition table, we don't need to check it.
                if (unspecifiedPartitions.Count > 0)
                {
                    // Get all constraints in the query.
                    if (querySpecification.WhereClause != null)
                    {
                        SearchConstraints(querySpecification.WhereClause, unspecifiedPartitions);
                    }
                }

                // If there are any unspecified partitions left, these are considered problems.
                foreach (var unspecifiedPartition in unspecifiedPartitions)
                {
                    SqlRuleProblem problem = CreateProblem(ruleExecutionContext, unspecifiedPartition);
                    problems.Add(problem);
                }

                AnalyzeJoins(problems, ruleExecutionContext, querySpecification);
            }
        }

        public override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            IList<SqlRuleProblem> problems = new List<SqlRuleProblem>();
            TSqlFragment fragment = ruleExecutionContext.ScriptFragment;

            try
            {
                AnalyzeQueries(problems, fragment, ruleExecutionContext);
            }
            catch (Exception e)
            {
                problems.Add(new SqlRuleProblem($"Rule encountered an exception:\n{e}",
                    ruleExecutionContext.ModelElement));
            }

            return problems;
        }
    }
}
