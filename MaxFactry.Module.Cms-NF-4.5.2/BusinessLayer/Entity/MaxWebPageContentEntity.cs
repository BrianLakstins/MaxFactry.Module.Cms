// <copyright file="MaxWebPageContentEntity.cs" company="Lakstins Family, LLC">
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
// <change date="11/14/2014" author="Brian A. Lakstins" description="Update to insert new version based on GroupId">
// <change date="12/18/2014" author="Brian A. Lakstins" description="Updates to follow core data access patterns">
// <change date="4/20/2016" author="Brian A. Lakstins" description="Updated to use centralized caching.">
// <change date="7/12/2016" author="Brian A. Lakstins" description="Fix issue with assinging next version not taking group into account.">
// <change date="11/13/2016" author="Brian A. Lakstins" description="Add a way to remove content and speed up finding current content.">
// <change date="3/31/2024" author="Brian A. Lakstins" description="Updated for changes to dependency namespace">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Update for change in repository">
// <change date="6/12/2025" author="Brian A. Lakstins" description="Filter content by active only.">
// <change date="6/21/2025" author="Brian A. Lakstins" description="Update base class.">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Core;
    using MaxFactry.Module.Cms.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxWebPageContentEntity : MaxFactry.Base.BusinessLayer.MaxBaseVersionedEntity
    {
        private static readonly object _oLock = new object();

        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxWebPageContentEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxWebPageContentEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Value
        {
            get
            {
                return this.GetString(this.DataModel.Value);
            }

            set
            {
                this.Set(this.DataModel.Value, value);
            }
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
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxWebPageContentDataModel DataModel
        {
            get
            {
                return (MaxWebPageContentDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        public override MaxFactry.Base.BusinessLayer.MaxBaseVersionedEntity GetCurrent(string lsName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new instance of the MaxContentEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxContentEntity class.</returns>
        public static MaxWebPageContentEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxWebPageContentEntity),
                typeof(MaxWebPageContentDataModel)) as MaxWebPageContentEntity;
        }

        public static string GetContent(string lsName, Guid loContentGroupId)
        {
            string lsR = null;
            MaxWebPageContentEntity loEntity = MaxWebPageContentEntity.Create().GetCurrent(lsName, loContentGroupId);
            if (null != loEntity)
            {
                lsR = loEntity.Value;
            }

            return lsR;
        }

        /// <summary>
        /// Gets the current entity for the virtual path.
        /// </summary>
        /// <param name="lsName">Key used for the versioned information.</param>
        /// <returns>Current entity.</returns>
        public virtual MaxWebPageContentEntity GetCurrent(string lsName, Guid loContentGroupId)
        {
            MaxWebPageContentEntity loR = null;
            MaxDataQuery loDataQuery = this.GetDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(new MaxDataFilter(this.MaxBaseVersionedDataModel.IsActive, "=", true));
            loDataQuery.EndGroup();

            MaxData loData = new MaxData(this.Data.DataModel);
            loData.Set(this.MaxBaseVersionedDataModel.Name, lsName);
            loData.Set(this.DataModel.ContentGroupId, loContentGroupId);
            MaxEntityList loList = this.LoadAllByPageCache(loData, 0, 0, string.Empty, loDataQuery);
            if (loList.Count == 0)
            {
                loDataQuery = this.GetDataQuery();
                loList = this.LoadAllByPageCache(loData, 0, 0, string.Empty, loDataQuery);
                if (loList.Count == 0)
                {
                    MaxEntityList loAllList = this.LoadAllCache();
                    for (int lnE = 0; lnE < loAllList.Count; lnE++)
                    {
                        MaxWebPageContentEntity loEntity = loAllList[lnE] as MaxWebPageContentEntity;
                        if (loEntity.Name.Equals(lsName, StringComparison.InvariantCultureIgnoreCase) &&
                            loEntity.ContentGroupId.Equals(loContentGroupId))
                        {
                            loList.Add(loEntity);
                        }
                    }
                }
            }

            if (loList.Count == 1)
            {
                loR = loList[0] as MaxWebPageContentEntity;
            }
            else if (loList.Count > 1)
            {
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    MaxWebPageContentEntity loEntity = loList[lnE] as MaxWebPageContentEntity;
                    if (null == loR)
                    {
                        loR = loEntity;
                    }
                    else if (loR.Version < loEntity.Version)
                    {
                        if (loR.IsActive)
                        {
                            loR.IsActive = false;
                            loR.Update();
                        }

                        loR = loEntity;
                    }
                    else if (loEntity.IsActive)
                    {
                        loEntity.IsActive = false;
                        loEntity.Update();
                    }
                }

                if (!loR.IsActive)
                {
                    loR.IsActive = true;
                    loR.Update();
                }
            }

            return loR;
        }

        /// <summary>
        /// Gets the next version based on however the data is stored.
        /// </summary>
        /// <returns></returns>
        public override int GetNextVersion()
        {
            int lnR = 1;
            lock (_oLock)
            {
                MaxBaseVersionedEntity loEntity = this.GetCurrent(this.Name, this.ContentGroupId);
                if (null != loEntity)
                {
                    lnR = loEntity.Version + 1;
                }
            }

            return lnR;
        }

        public MaxEntityList LoadAllByContentGroupIdCache(Guid loContentGroupId)
        {
            MaxDataQuery loDataQuery = this.GetDataQuery();
            loDataQuery.StartGroup();
            loDataQuery.AddFilter(new MaxDataFilter(this.MaxBaseVersionedDataModel.IsActive, "=", true));
            loDataQuery.EndGroup();
            MaxData loData = new MaxData(this.Data.DataModel);
            loData.Set(this.DataModel.ContentGroupId, loContentGroupId);
            MaxEntityList loR = this.LoadAllByPageCache(loData, 0, 0, string.Empty, loDataQuery);
            if (loR.Count == 0)
            {
                MaxEntityList loAllList = this.LoadAllCache();
                for (int lnE = 0; lnE < loAllList.Count; lnE++)
                {
                    MaxWebPageContentEntity loEntity = loAllList[lnE] as MaxWebPageContentEntity;
                    if (loEntity.ContentGroupId.Equals(loContentGroupId))
                    {
                        loR.Add(loEntity);
                    }
                }
            }

            return loR;
        }
    }
}
