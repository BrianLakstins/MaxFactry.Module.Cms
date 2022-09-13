// <copyright file="MaxWebPageTemplateDataModel.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="12/17/2020" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
    /// Data model for the configuration information associated with web site content.
	/// </summary>
    public class MaxWebPageTemplateDataModel : MaxFactry.Base.DataLayer.MaxBaseIdDataModel
	{
        /// <summary>
        /// Description of the template
        /// </summary>
        public readonly string Name = "Name";

        /// <summary>
        /// Description of the template
        /// </summary>
        public readonly string Description = "Description";

        /// <summary>
        /// Unlayer Json design
        /// </summary>
        public readonly string JsonUnlayer = "JsonUnlayer";

        /// <summary>
        /// Html for template
        /// </summary>
        public readonly string Html = "Html";

        /// <summary>
        /// Text for template
        /// </summary>
        public readonly string Text = "Text";

        /// <summary>
        /// Named portions of the HTML
        /// </summary>
        public readonly string HtmlChunkListText = "HtmlChunkListText";

        /// <summary>
        /// Initializes a new instance of the MaxWebPageTemplateDataModel class.
        /// </summary>
        public MaxWebPageTemplateDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.Module.Cms.DataLayer.Provider.MaxContentRepositoryProvider);
            this.RepositoryType = typeof(MaxContentRepository);
            this.SetDataStorageName("MaxCmsWebPageTemplate");
            this.AddType(this.Name, typeof(MaxShortString));
            this.AddNullable(this.Description, typeof(string));
            this.AddType(this.JsonUnlayer, typeof(MaxLongString));
            this.AddNullable(this.Html, typeof(MaxLongString));
            this.AddNullable(this.Text, typeof(MaxLongString));
            this.AddNullable(this.HtmlChunkListText, typeof(MaxLongString));
        }
    }
}
