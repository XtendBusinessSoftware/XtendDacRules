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
using System;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace Xtend.Dac.Rules
{
    class XtendExportCodeAnalysisRuleAttribute : ExportCodeAnalysisRuleAttribute
    {
        private readonly string fDisplayNameResourceId;
        private readonly string fDescriptionResourceId;
        private ResourceManager fResourceManager;
        private string fDisplayName;
        private string fDescription;

        public XtendExportCodeAnalysisRuleAttribute(
            string id,
            string displayNameResourceId,
            string descriptionResourceId)
            : base(id, null)
        {
            fDisplayNameResourceId = displayNameResourceId;
            fDescriptionResourceId = descriptionResourceId;
            fResourceManager = new ResourceManager(RuleConstants.ResourceBaseName, GetType().Assembly);
        }

        public override string DisplayName
        {
            get
            {
                if (fDisplayName == null)
                {
                    fDisplayName = fResourceManager.GetString(fDisplayNameResourceId, CultureInfo.CurrentUICulture);
                }
                return fDisplayName;
            }
        }

        public override string Description
        {
            get
            {
                if (fDescription == null)
                {
                    fDescription = fResourceManager.GetString(fDescriptionResourceId, CultureInfo.CurrentUICulture);
                }
                return fDescription;
            }
        }
    }
}
