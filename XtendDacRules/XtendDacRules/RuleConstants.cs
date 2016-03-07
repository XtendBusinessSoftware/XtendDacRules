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

namespace Xtend.Dac.Rules
{
    internal static class RuleConstants
    {
        /// <summary>
        /// The name of the resources file to use when looking up rule resources
        /// </summary>
        public const string ResourceBaseName = "Xtend.Dac.Rules.RuleResources";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string ForeignKeyWithoutIndex_RuleName = "ForeignKeyWithoutIndex_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string ForeignKeyWithoutIndex_ProblemDescription = "ForeignKeyWithoutIndex_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string ColumnWithoutDescription_RuleName = "ColumnWithoutDescription_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string ColumnWithoutDescription_ProblemDescription = "ColumnWithoutDescription_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string TableWithoutDescription_RuleName = "TableWithoutDescription_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string TableWithoutDescription_ProblemDescription = "TableWithoutDescription_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string NoExplicitNoCountSet_RuleName = "NoExplicitNoCountSet_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string NoExplicitNoCountSet_ProblemDescription = "NoExplicitNoCountSet_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string UnusedVariable_RuleName = "UnusedVariable_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string UnusedVariable_ProblemDescription = "UnusedVariable_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string UnusedParameter_RuleName = "UnusedParameter_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string UnusedParameter_ProblemDescription = "UnusedParameter_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string MissingMandatoryParameter_RuleName = "MissingMandatoryParameter_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string MissingMandatoryParameter_ProblemDescription = "MissingMandatoryParameter_ProblemDescription";

        /// <summary>
        /// Lookup name inside the resources file for the rule name
        /// </summary>
        public const string ConcatNullYieldsNullEnabled_RuleName = "ConcatNullYieldsNullEnabled_RuleName";
        /// <summary>
        /// Lookup ID inside the resources file for the description
        /// </summary>
        public const string ConcatNullYieldsNullEnabled_ProblemDescription = "ConcatNullYieldsNullEnabled_ProblemDescription";

        /// <summary>
        /// The design category (should not be localized)
        /// </summary>
        public const string CategoryDesign = "Design";

        /// <summary>
        /// The performance category (should not be localized)
        /// </summary>
        public const string CategoryPerformance = "Performance";

        /// <summary>
        /// The naming category (should not be localized)
        /// </summary>
        public const string CategoryNaming = "Naming";
    }
}
