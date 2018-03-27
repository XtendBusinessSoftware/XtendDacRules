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
using Microsoft.SqlServer.Dac.Model;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Xtend.Dac.Rules
{
    internal class MandatoryParameterVisitor : TSqlConcreteFragmentVisitor
    {
        public IList<ExecutableProcedureReference> ProcedureCalls { get; private set; }
        public Dictionary<ExecutableProcedureReference, string> MissingParameters { get; private set; }

        private TSqlModel model;
        public MandatoryParameterVisitor(TSqlModel model)
        {
            ProcedureCalls = new List<ExecutableProcedureReference>();
            MissingParameters = new Dictionary<ExecutableProcedureReference, string>();
            this.model = model;
        }

        public override void ExplicitVisit(ExecutableProcedureReference node)
        {
            SchemaObjectName schemaObject = node.ProcedureReference.ProcedureReference.Name;
            Identifier id = node.ProcedureReference.ProcedureReference.Name.SchemaIdentifier;
            string schema;
            if (schemaObject.SchemaIdentifier != null)
            {
                schema = schemaObject.SchemaIdentifier.Value;
            }
            else
            {
                // Assuming schema is dbo
                schema = "dbo";
            }
            string procedure = node.ProcedureReference.ProcedureReference.Name.BaseIdentifier.Value;
            TSqlObject procObject = model.GetObject(Procedure.TypeClass, new ObjectIdentifier(schema, procedure), DacQueryScopes.UserDefined);
            if (procObject != null)
            {
                bool hasMissingMandatoryParams = false;

                // Get the list of supplied parameters to match against.
                IList<ExecuteParameter> suppliedParams = node.Parameters;

                string missingParamString = "";
                int paramIndex = 0;
                // Get the parameters of the procedure
                foreach (TSqlObject param in procObject.GetReferenced(Procedure.Parameters))
                {
                    // Check if it has an extended property
                    object paramDefault = param.GetProperty(Parameter.DefaultExpression);
                    // Check if it is readonly
                    bool paramReadOnly = (bool)param.GetProperty(Parameter.ReadOnly);

                    if (paramDefault == null && !paramReadOnly)
                    {
                        // Check if we supplied it
                        bool suppliedMandatoryParam = false;
                        string paramName = param.Name.Parts[param.Name.Parts.Count - 1];
                        int paramsFound = 0;
                        foreach (ExecuteParameter suppliedParam in suppliedParams.ToList())
                        {
                            if (suppliedParam.Variable != null)
                            {
                                if (suppliedParam.Variable.Name == paramName)
                                    suppliedMandatoryParam = true;
                            }
                            else
                            {
                                if (paramsFound == paramIndex)
                                    suppliedMandatoryParam = true;
                            }

                            paramsFound++;
                        }
                        if (!suppliedMandatoryParam)
                        {
                            hasMissingMandatoryParams = true;
                            if (missingParamString == "")
                                missingParamString = paramName;
                            else
                                missingParamString += ", " + paramName;
                        }
                    }
                    paramIndex++;
                }

                if (hasMissingMandatoryParams)
                {
                    ProcedureCalls.Add(node);
                    MissingParameters[node] = missingParamString;
                }
            }
        }
    }
}