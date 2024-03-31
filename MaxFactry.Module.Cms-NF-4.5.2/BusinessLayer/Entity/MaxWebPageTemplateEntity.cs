// <copyright file="MaxWebPageTemplateEntity.cs" company="Lakstins Family, LLC">
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
// <change date="1/13/2021" author="Brian A. Lakstins" description="Add logic to update name to prevent duplicate names">
// <change date="7/8/2021" author="Brian A. Lakstins" description="Add mapindex default for mapping for a selection list">
// <change date="3/31/2024" author="Brian A. Lakstins" description="Updated for changes to dependency namespace">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Module.Cms.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxWebPageTemplateEntity : MaxFactry.Base.BusinessLayer.MaxBaseIdEntity
    {
        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxWebPageTemplateEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxContentEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxWebPageTemplateEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetString(this.DataModel.Name);
            }

            set
            {
                this.Set(this.DataModel.Name, value);
            }
        }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description
        {
            get
            {
                return this.GetString(this.DataModel.Description);
            }

            set
            {
                this.Set(this.DataModel.Description, value);
            }
        }

        /// <summary>
        /// Gets or sets the Json used for an Unlayer template
        /// </summary>
        public string JsonUnlayer
        {
            get
            {
                return this.GetString(this.DataModel.JsonUnlayer);
            }

            set
            {
                this.Set(this.DataModel.JsonUnlayer, value);
            }
        }

        /// <summary>
        /// Gets or sets the Html based on the Json or instead of the json
        /// </summary>
        public string Html
        {
            get
            {
                return this.GetString(this.DataModel.Html);
            }

            set
            {
                this.Set(this.DataModel.Html, value);
            }
        }

        /// <summary>
        /// Gets or sets the named parts of the html
        /// </summary>
        public MaxIndex HtmlChunkList
        {
            get
            {
                MaxIndex loR = this.GetObject(this.DataModel.HtmlChunkListText, typeof(MaxIndex)) as MaxIndex;
                if (null == loR)
                {
                    loR = new MaxIndex();
                    this.SetObject(this.DataModel.HtmlChunkListText, loR);
                }

                return loR;
            }

            set
            {
                this.SetObject(this.DataModel.HtmlChunkListText, value);
            }
        }

        public override string GetDefaultSortString()
        {
            return this.Name + base.GetDefaultSortString();
        }

        public override bool Insert()
        {
            //// Check Name to make sure not using the same name
            List<string> loCurrentNameList = new List<string>();
            MaxEntityList loList = MaxWebPageTemplateEntity.Create().LoadAllCache();
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                if (!loCurrentNameList.Contains(((MaxWebPageTemplateEntity)loList[lnE]).Name))
                {
                    loCurrentNameList.Add(((MaxWebPageTemplateEntity)loList[lnE]).Name);
                }
            }

            string lsBase = this.Name;
            if (loCurrentNameList.Contains(lsBase))
            {
                int lnIndex = 1;
                if (lsBase.EndsWith(")") && lsBase.Contains("("))
                {
                    string lsIndex = lsBase.Substring(lsBase.LastIndexOf("(") + 1, lsBase.LastIndexOf(")") - lsBase.LastIndexOf("(") - 1);
                    int lnIndexTest = 0;
                    if (int.TryParse(lsIndex, out lnIndexTest))
                    {
                        lnIndex = lnIndexTest;
                        lsBase = lsBase.Substring(0, lsBase.LastIndexOf("("));
                    }
                }

                string lsName = lsBase + "(" + lnIndex + ")";
                while (loCurrentNameList.Contains(lsName))
                {
                    lnIndex++;
                    lsName = lsBase + "(" + lnIndex + ")";
                }

                this.Name = lsName;
            }

            return base.Insert();
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxWebPageTemplateDataModel DataModel
        {
            get
            {
                return (MaxWebPageTemplateDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new instance of the MaxContentEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxContentEntity class.</returns>
        public static MaxWebPageTemplateEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxWebPageTemplateEntity),
                typeof(MaxWebPageTemplateDataModel)) as MaxWebPageTemplateEntity;
        }

        public override MaxIndex MapIndex(params string[] laPropertyNames)
        {
            MaxIndex loR = base.MapIndex(laPropertyNames);
            if (null == laPropertyNames || laPropertyNames.Length == 0 || loR.Count == 0)
            {
                loR = base.MapIndex(
                    this.GetType() + ".Id",
                    this.GetType() + ".Name",
                    this.GetType() + ".Description",
                    this.GetType() + ".JsonUnlayer",
                    this.GetType() + ".Html",
                    this.GetType() + ".HtmlChunkList",
                    this.GetType() + ".IsActive");
            }

            return loR;
        }
    }
}
