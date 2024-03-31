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
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Module.Cms.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxWebPageContentEntity : MaxFactry.Base.BusinessLayer.MaxBaseIdVersionedEntity
    {
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

        public override MaxFactry.Base.BusinessLayer.MaxBaseIdVersionedEntity GetCurrent(string lsName)
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

        public static List<MaxWebPageContentEntity> GetContentList(Guid loContentGroupId)
        {
            List<MaxWebPageContentEntity> loR = new List<MaxWebPageContentEntity>();
            MaxEntityList loList = MaxWebPageContentEntity.Create().LoadAllByContentGroupIdCache(loContentGroupId);
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxWebPageContentEntity loEntity = loList[lnE] as MaxWebPageContentEntity;
                if (!string.IsNullOrEmpty(loEntity.Value) && loEntity.Value.Trim().Length > 0)
                {
                    loR.Add(loEntity);
                }
            }

            return loR;
        }

        public static string GetContent(Guid loContentGroupId, string lsContentName)
        {
            string lsR = null;
            if (!Guid.Empty.Equals(loContentGroupId))
            {
                List<MaxWebPageContentEntity> loList = GetContentList(loContentGroupId);
                int lnVersionMax = 0;
                foreach (MaxWebPageContentEntity loEntity in loList)
                {
                    if (loEntity.Name.Equals(lsContentName, StringComparison.InvariantCultureIgnoreCase) && loEntity.Version > lnVersionMax)
                    {
                        lsR = loEntity.Value;
                        lnVersionMax = loEntity.Version;
                    }
                }
            }

            return lsR;
        }

        /// <summary>
        /// Loads all content for a group
        /// </summary>
        /// <param name="loContentGroupId"></param>
        /// <returns></returns>
        public MaxEntityList LoadAllByContentGroupIdCache(Guid loContentGroupId)
        {
            MaxEntityList loR = new MaxEntityList(this.GetType());
            //// Using LoadAllCache because there are not many records and pulling with too many calls is slower
            MaxEntityList loEntityList = this.LoadAllCache();
            for (int lnE = 0; lnE < loEntityList.Count; lnE++)
            {
                MaxWebPageContentEntity loEntity = loEntityList[lnE] as MaxWebPageContentEntity;
                if (loEntity.ContentGroupId == loContentGroupId && loEntity.IsActive)
                {
                    loR.Add(loEntity);
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
            MaxDataList loDataList = MaxFactry.Base.DataLayer.MaxBaseIdVersionedRepository.SelectAllByName(this.Data, this.Name);
            //// Get the largest version number of any entity regardless of active or deleted.
            int lnVersionCurrent = 0;
            for (int lnD = 0; lnD < loDataList.Count; lnD++)
            {
                if (MaxFactry.Core.MaxConvertLibrary.ConvertToGuid(typeof(object), loDataList[lnD].Get(this.DataModel.ContentGroupId)) == this.ContentGroupId)
                {
                    int lnVersion = MaxFactry.Core.MaxConvertLibrary.ConvertToInt(typeof(object), loDataList[lnD].Get(this.MaxBaseIdVersionedDataModel.Version));
                    if (lnVersion > lnVersionCurrent)
                    {
                        lnVersionCurrent = lnVersion;
                    }
                }
            }

            return lnVersionCurrent + 1;
        }

        /// <summary>
        /// Inserts a new record using the next available version id.
        /// </summary>
        /// <returns>true if a record was inserted.</returns>
        public override bool Insert()
        {
            bool lbR = base.Insert();
            if (lbR)
            {
                MaxEntityList loList = this.LoadAllByContentGroupIdCache(this.ContentGroupId);
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    MaxWebPageContentEntity loEntity = loList[lnE] as MaxWebPageContentEntity;
                    if (loEntity.Name == this.Name && loEntity.Version < this.Version)
                    {
                        loEntity.IsActive = false;
                        loEntity.Update();
                    }
                }
            }

            return lbR;
        }
    }
}
