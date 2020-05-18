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
    /// <summary>
    /// Base class for SQL static code analysis rules. An analysis rule analyzes a model
    /// / model element and returns a list of problems found during analysis. Implementing
    /// classes must have a Microsoft.SqlServer.Dac.CodeAnalysis.ExportCodeAnalysisRuleAttribute
    /// defined on the class definition to be discovered and used during code analysis.
    /// </summary>
    public abstract class XtendSqlProcedureAnalysisRule : SqlCodeAnalysisRule
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected XtendSqlProcedureAnalysisRule() : base() { }

        /// <summary>
        /// Whether to SKIP analysis for problems.
        /// </summary>
        public static bool IgnoreScriptFragment(Microsoft.SqlServer.TransactSql.ScriptDom.TSqlFragment fragment)
        {
            // external (generated)
            if (fragment is ProcedureStatementBody psb && psb.MethodSpecifier != null)
                return true;
            return false;
        }

        /// <summary>
        /// For element-scoped rules the Analyze method is executed once for every matching object in the model. 
        /// </summary>
        /// <param name="context">The context object contains the TSqlObject being analyzed, a TSqlFragment
        /// that's the AST representation of the object, the current rule's descriptor, and a reference to the model being
        /// analyzed.
        /// </param>
        /// <returns>A list of problems should be returned. These will be displayed in the Visual Studio error list</returns>
        public abstract IList<SqlRuleProblem> Analyze(XtendSqlRuleExecutionContext context);

        /// <summary>
        /// For element-scoped rules the Analyze method is executed once for every matching object in the model. 
        /// </summary>
        /// <param name="ruleExecutionContext">The context object contains the TSqlObject being analyzed, a TSqlFragment
        /// that's the AST representation of the object, the current rule's descriptor, and a reference to the model being
        /// analyzed.
        /// </param>
        /// <returns>A list of problems should be returned. These will be displayed in the Visual Studio error list</returns>
        public sealed override IList<SqlRuleProblem> Analyze(SqlRuleExecutionContext ruleExecutionContext)
        {
            var context = new XtendSqlRuleExecutionContext(ruleExecutionContext);
            if (context.Schema != null && context.ScriptFragment.FragmentLength > 0  // valid procedure
                && !IgnoreScriptFragment(context.ScriptFragment))
            {
                return Analyze(context);
            }
            return new List<SqlRuleProblem>();
        }

        /// <summary>
        /// Returns list of tokens of given TSqlFragment
        /// </summary>
        public static List<TSqlParserToken> SelectTokens(TSqlFragment node, bool ignoreWhiteSpace = true)
        {
            List<TSqlParserToken> tokens = new List<TSqlParserToken>();
            for (int i = System.Math.Max(0, node.FirstTokenIndex); i <= System.Math.Min(node.ScriptTokenStream.Count - 1, node.LastTokenIndex); i++)
                if (!ignoreWhiteSpace || node.ScriptTokenStream[i].TokenType != TSqlTokenType.WhiteSpace)
                    tokens.Add(node.ScriptTokenStream[i]);
            return tokens;
        }

#if DEBUG
        public static string DebugNodeStr(TSqlFragment node, bool ignoreWhiteSpace = true)
        {
            List<TSqlParserToken> tokens = SelectTokens(node, false);
            string text2 = string.Join("", tokens.Where(x => !ignoreWhiteSpace || x.TokenType != TSqlTokenType.WhiteSpace)
                .Select(x => "{" + "[" + x.TokenType + "]" + x.Text + "}").ToArray());
            return node.GetType() + ": '" + string.Join("", tokens.Select(x => x.Text).ToArray()) + "'\r\n(" + text2 + ")";
        }
#endif
    }

    /// <summary>
    /// Defines the fields necessary for analysis, including the schema model and model element to analyze.
    /// </summary>
    public sealed class XtendSqlRuleExecutionContext
    {
        internal XtendSqlRuleExecutionContext(SqlRuleExecutionContext ruleExecutionContext)
        {
            this.SchemaModel = ruleExecutionContext.SchemaModel;
            this.ModelElement = ruleExecutionContext.ModelElement;
            this.ScriptFragment = ruleExecutionContext.ScriptFragment;
            this.RuleDescriptor = ruleExecutionContext.RuleDescriptor;

            this.ElementName = ruleExecutionContext.SchemaModel.DisplayServices.GetElementName(ModelElement, ElementNameStyle.EscapedFullyQualifiedName);
            this.Schema = ModelElement.GetReferenced(Procedure.Schema).SingleOrDefault(); // schema of the procedure
        }

        /// <summary>
        /// the EscapedFullyQualifiedName
        /// </summary>
        public string ElementName { get; }

        /// <summary>
        /// The schema of the procedure
        /// </summary>
        public TSqlObject Schema { get; }

        #region SqlRuleExecutionContext

        /// <summary>The <see cref="Microsoft.SqlServer.Dac.Model.TSqlModel"/> being analyzed.</summary>
        public TSqlModel SchemaModel { get; }

        /// <summary>The <see cref="Microsoft.SqlServer.Dac.Model.TSqlObject"/> being analyzed.</summary>
        public TSqlObject ModelElement { get; }

        /// <summary>Describes the rule being executed.</summary>
        public RuleDescriptor RuleDescriptor { get; }

        /// <summary>
        /// <para>Gets the script fragment which defines the element being analyzed, if this is available. May be null.</para>
        ///
        /// Tries to get the most suitable <see cref="T:Microsoft.SqlServer.TransactSql.ScriptDom.TSqlFragment" /> for use during the rule analysis process.
        /// If the TSqlObject was originally built from a scripted source then the original source fragment will be returned.
        /// Otherwise a new AST will be generated from the <see cref="P:Microsoft.SqlServer.Dac.CodeAnalysis.SqlRuleExecutionContext.ModelElement" />/&gt;.
        /// This ensures that when reporting <see cref="T:Microsoft.SqlServer.Dac.CodeAnalysis.SqlRuleProblem" />s the most accurate source information can be included
        /// in the error messages. 
        /// </summary>
        /// <remarks>
        /// See <see cref="M:Microsoft.SqlServer.Dac.TSqlModelUtils.TryGetFragmentForAnalysis(Microsoft.SqlServer.Dac.Model.TSqlObject,Microsoft.SqlServer.TransactSql.ScriptDom.TSqlFragment@)" /> for implementation details
        /// </remarks>
        public TSqlFragment ScriptFragment { get; }

        #endregion SqlRuleExecutionContext
    }
}
