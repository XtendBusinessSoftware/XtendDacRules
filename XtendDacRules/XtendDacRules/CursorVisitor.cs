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
    internal class CursorVisitor : TSqlConcreteFragmentVisitor
    {
        public Dictionary<FetchCursorStatement, string> IncorrectFetchCursorStatements { get; private set; }
        public List<DeallocateCursorStatement> IncorrectDeallocateCursorStatements { get; private set; }
        public List<OpenCursorStatement> IncorrectOpenCursorStatements { get; private set; }
        public List<DeclareCursorStatement> DeclareCursorStatementsMissingDeallocate { get; private set; }
        public List<DeclareCursorStatement> DeclareCursorStatementsMissingOpen { get; private set; }

        private readonly bool checkMissing;
        private readonly bool checkFetch;
        private Dictionary<string, DeclareCursorStatement> declareCursorStatements;

        private T GetCursorStatement<T>(Dictionary<string, T> dict, string cursorkey) where T : TSqlStatement
        {
            if (!string.IsNullOrEmpty(cursorkey) && dict.ContainsKey(cursorkey))
                return dict[cursorkey];
            return null;
        }

        public CursorVisitor(bool checkMissing, bool checkFetch)
        {
            this.checkMissing = checkMissing;
            this.checkFetch = checkFetch;
            declareCursorStatements = new Dictionary<string, DeclareCursorStatement>();
            if(checkFetch)
                IncorrectFetchCursorStatements = new Dictionary<FetchCursorStatement, string>();
            if (checkMissing)
            {
                IncorrectDeallocateCursorStatements = new List<DeallocateCursorStatement>();
                IncorrectOpenCursorStatements = new List<OpenCursorStatement>();
                DeclareCursorStatementsMissingDeallocate = new List<DeclareCursorStatement>();
                DeclareCursorStatementsMissingOpen = new List<DeclareCursorStatement>();
            }
        }

        public override void Visit(TSqlFragment node)
        {
            base.Visit(node);

            if (node is DeclareCursorStatement declareCursor)
            {
                string keysrc = declareCursor.Name.Value;
                declareCursorStatements[keysrc] = declareCursor;
                if (checkMissing)
                {
                    DeclareCursorStatementsMissingDeallocate.Add(declareCursor);
                    DeclareCursorStatementsMissingOpen.Add(declareCursor);
                }
            }

            if (checkMissing && node is OpenCursorStatement open)
            {
                string cursorkey = open.Cursor?.Name.Value;
                var cursor = GetCursorStatement(declareCursorStatements, cursorkey);
                if (cursor == null)
                    IncorrectOpenCursorStatements.Add(open);
                else
                    DeclareCursorStatementsMissingOpen.Remove(cursor);
            }

            if (checkFetch && node is FetchCursorStatement fetch)
            {
                string cursorkey = (fetch.Cursor?.Name.Value);
                var cursor = GetCursorStatement(declareCursorStatements, cursorkey);
                int fetchCount = fetch.IntoVariables.Count;
                int selectCount = 0;
                if (cursor != null)
                {
                    var query = cursor.CursorDefinition.Select.QueryExpression;
                    if (query is QuerySpecification)
                        selectCount = ((QuerySpecification)query).SelectElements.Count;
                    else
                        selectCount = fetchCount;
                }
                if (fetchCount != selectCount)
                {
                    string cursorName = string.Format("{0} ({1})", cursor == null ? "NULL" : cursor.Name.Value, selectCount);
                    IncorrectFetchCursorStatements[fetch] = cursorName;
                }
            }

            if (checkMissing && node is DeallocateCursorStatement deallocateCursor)
            {
                string cursorkey = deallocateCursor.Cursor?.Name.Value;
                var cursor = GetCursorStatement(declareCursorStatements, cursorkey);
                if (cursor == null)
                    IncorrectDeallocateCursorStatements.Add(deallocateCursor);
                else
                    DeclareCursorStatementsMissingDeallocate.Remove(cursor);
                declareCursorStatements.Remove(cursorkey);
            }
        }
    }
}