// <copyright file="MaxContentUrlEntity.cs" company="Lakstins Family, LLC">
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
// <change date="12/18/2014" author="Brian A. Lakstins" description="Updates to follow core data access patterns">
// <change date="4/20/2016" author="Brian A. Lakstins" description="Updated to use centralized caching.">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.BusinessLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Module.Cms.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxContentUrlEntity : MaxFactry.Base.BusinessLayer.MaxBaseIdEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxContentUrlEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxContentUrlEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public Guid ContentGroupId
        {
            get
            {
                return this.GetGuid(this.DataModel.ContentGroupId);
            }

            set
            {
                this.Set(this.DataModel.ContentGroupId, value);
            }
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetString(this.DataModel.Url);
            }

            set
            {
                this.Set(this.DataModel.Url, value);
            }
        }


        /// <summary>
        /// Gets the name
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetString(this.DataModel.Title);
            }

            set
            {
                this.Set(this.DataModel.Title, value);
            }
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string MetaDescription
        {
            get
            {
                return this.GetString(this.DataModel.MetaDescription);
            }

            set
            {
                this.Set(this.DataModel.MetaDescription, value);
            }
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string HtmlHeader
        {
            get
            {
                return this.GetString(this.DataModel.HtmlHeader);
            }

            set
            {
                this.Set(this.DataModel.HtmlHeader, value);
            }
        }

        /// <summary>
        /// Gets the name
        /// </summary>
        public string HtmlFooter
        {
            get
            {
                return this.GetString(this.DataModel.HtmlFooter);
            }

            set
            {
                this.Set(this.DataModel.HtmlFooter, value);
            }
        }


        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxContentUrlDataModel DataModel
        {
            get
            {
                return (MaxContentUrlDataModel)MaxDataLibrary.GetDataModel(typeof(MaxContentUrlDataModel));
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxContentEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxContentEntity class.</returns>
        public static MaxContentUrlEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxContentUrlEntity), 
                typeof(MaxContentUrlDataModel)) as MaxContentUrlEntity;
        }

        /// <summary>
        /// Loads the latest content based on the url
        /// </summary>
        /// <param name="lsName">Name of the content.</param>
        /// <returns>true if found. False if no data found.</returns>
        public bool LoadCurrentByUrlCache(string lsUrl)
        {
            MaxEntityList loR = new MaxEntityList(this.GetType());
            //// Using LoadAllCache because there are not many records and pulling with too many calls is slower
            MaxEntityList loEntityList = this.LoadAllCache();
            for (int lnE = 0; lnE < loEntityList.Count; lnE++)
            {
                MaxContentUrlEntity loEntity = loEntityList[lnE] as MaxContentUrlEntity;
                if (loEntity.Url == lsUrl && loEntity.IsActive)
                {
                    return this.Load(loEntity.GetData());
                }
            }

            return false;
        }

        public static Guid GetContentGroupByUrl(string lsUrl)
        {
            Guid loR = Guid.Empty;
            MaxContentUrlEntity loEntity = MaxContentUrlEntity.Create();
            if (loEntity.LoadCurrentByUrlCache(lsUrl))
            {
                loR = loEntity.ContentGroupId;
            }

            return loR;
        }
    }
}
