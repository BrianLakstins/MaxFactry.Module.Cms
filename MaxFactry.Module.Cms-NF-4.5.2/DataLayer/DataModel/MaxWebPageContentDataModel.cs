// <copyright file="MaxWebPageContentDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/12/2025" author="Brian A. Lakstins" description="Add ContentId to DataKey for unique identification">
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
    public class MaxWebPageContentDataModel : MaxFactry.Base.DataLayer.MaxBaseIdVersionedDataModel
	{
        /// <summary>
        /// Id of content group, so all content for a page can be queried at one time.
        /// </summary>
        public readonly string ContentGroupId = "ContentGroupId";  

        /// <summary>
        /// The value of the content to be stored.
        /// </summary>
        public readonly string Value = "Value";	

		/// <summary>
        /// Initializes a new instance of the MaxContentDataModel class.
		/// </summary>
        public MaxWebPageContentDataModel()
            : base()
		{
            this.RepositoryProviderType = typeof(MaxFactry.Module.Cms.DataLayer.Provider.MaxContentRepositoryProvider);
            this.RepositoryType = typeof(MaxContentRepository);
            this.SetDataStorageName("MaxCmsWebPageContent");
            this.AddDataKey(this.ContentGroupId, typeof(Guid));
            this.AddType(this.Value, typeof(string));
        }
	}
}
