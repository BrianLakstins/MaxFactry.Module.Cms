﻿// <copyright file="MaxContentUrlDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="8/21/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="7/6/2020" author="Brian A. Lakstins" description="Allow some nulls">
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
    public class MaxContentUrlDataModel : MaxFactry.Base.DataLayer.MaxBaseIdDataModel
	{
        /// <summary>
        /// The url access to get content related to.
        /// </summary>
        public readonly string Url = "Url";

        /// <summary>
        /// Id of content group, so all content for a page can be queried at one time.
        /// </summary>
        public readonly string ContentGroupId = "ContentGroupId";

        /// <summary>
        /// Title related to the Url.
        /// </summary>
        public readonly string Title = "Title";

        /// <summary>
        /// Meta description of the page.
        /// </summary>
        public readonly string MetaDescription = "MetaDescription";

        /// <summary>
        /// Extra header content for this url.
        /// </summary>
        public readonly string HtmlHeader = "HtmlHeader";

        /// <summary>
        /// Extra footer content for this url.
        /// </summary>
        public readonly string HtmlFooter = "HtmlFooter";
        
        /// <summary>
        /// Initializes a new instance of the MaxContentDataModel class.
		/// </summary>
        public MaxContentUrlDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.Module.Cms.DataLayer.Provider.MaxContentRepositoryProvider);
            this.RepositoryType = typeof(MaxContentRepository);
            this.SetDataStorageName("MaxCmsContentUrl");
            this.AddType(this.Url, typeof(string));
            this.AddType(this.ContentGroupId, typeof(Guid));
            this.AddNullable(this.Title, typeof(string));
            this.AddNullable(this.MetaDescription, typeof(string));
            this.AddNullable(this.HtmlHeader, typeof(string));
            this.AddNullable(this.HtmlFooter, typeof(string));
        }
	}
}
