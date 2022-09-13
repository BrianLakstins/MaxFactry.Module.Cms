// <copyright file="MaxWebPageContentViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/3/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="6/19/2014" author="Brian A. Lakstins" description="Moving controller functionality to viewmodel.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added logging.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Update CMS editing and storage.">
// <change date="11/4/2014" author="Brian A. Lakstins" description="Updated to not use 'action' variable because it conflicts with form action on the client.">
// <change date="11/13/2018" author="Brian A. Lakstins" description="Move code to entity">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Module.Cms.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;

    /// <summary>
    /// View model for content.
    /// </summary>
    public class MaxWebPageContentViewModel : MaxBaseEntityViewModel
    {
        private string _sContent = null;

        private string _sMetaKeyWords = null;

        private string _sMetaDescription = null;

        private string _sMetaTitle = null;

        private MaxContentUrlEntity _oContentUrlEntity = null;

        private List<MaxWebPageContentEntity> _oContentList = null;

        private string _sName = null;

        public MaxWebPageContentViewModel()
        {
            MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Creating view model");
        }

        public MaxWebPageContentViewModel(string lsUrl, string lsControl)
        {
            if (lsUrl == "MaxCmsHome")
            {
                this.Url = string.Empty;
            }
            else
            {
                this.Url = lsUrl;
            }
            
            this.Name = lsControl;
        }

        public bool HasContent
        {
            get
            {
                bool lbR = false;
                foreach (MaxWebPageContentEntity loEntity in this.ContentList)
                {
                    if (loEntity.Name != "MetaKeyWords" && loEntity.Name != "MetaDescription" && loEntity.Name != "MetaTitle" && !string.IsNullOrEmpty(loEntity.Value))
                    {
                        lbR = true;
                        if (loEntity.Value.Length > 7)
                        {
                            if (loEntity.Value.StartsWith("<!--") && loEntity.Value.EndsWith("-->"))
                            {
                                if (!loEntity.Value.Substring(4, loEntity.Value.Length - 7).Contains("-->"))
                                {
                                    lbR = false;
                                }
                            }
                        }
                    }
                }

                return lbR;
            }

        }



        /// <summary>
        /// Gets or sets the content
        /// </summary>
        public string Content
        {
            get
            {
                if (null == this._sContent)
                {
                    if (null != this.Url && null != this.Name)
                    {
                        this._sContent = this.GetContent(this.Name).Replace("<MaxBlank />", string.Empty);
                    }
                }

                if (string.IsNullOrEmpty(this._sContent))
                {
                    return "Enter content here, then click the 'Save' button.";
                }

                return this._sContent;
            }
            set
            {
                this._sContent = value;
            }
        }

        public string MetaKeyWords
        {
            get
            {
                if (null == this._sMetaKeyWords)
                {
                    if (null != this.Url)
                    {
                        this._sMetaKeyWords = this.GetContent("MetaKeyWords").Replace("<MaxBlank />", string.Empty);
                    }
                }

                if (string.IsNullOrEmpty(this._sMetaKeyWords))
                {
                    return "Enter content here, then click the 'Save' button.";
                }

                return this._sMetaKeyWords;
            }
            set
            {
                this._sMetaKeyWords = value;
            }
        }

        public string MetaDescription
        {
            get
            {
                if (null == this._sMetaDescription)
                {
                    if (null != this.Url)
                    {
                        this._sMetaDescription = this.GetContent("MetaDescription").Replace("<MaxBlank />", string.Empty);
                    }
                }

                if (string.IsNullOrEmpty(this._sMetaDescription))
                {
                    return "Enter content here, then click the 'Save' button.";
                }

                return this._sMetaDescription;
            }
            set
            {
                this._sMetaDescription = value;
            }
        }

        public string MetaTitle
        {
            get
            {
                if (null == this._sMetaTitle)
                {
                    if (null != this.Url)
                    {
                        this._sMetaTitle = this.GetContent("MetaTitle").Replace("<MaxBlank />", string.Empty);
                    }
                }

                if (string.IsNullOrEmpty(this._sMetaTitle))
                {
                    return "Enter content here, then click the 'Save' button.";
                }

                return this._sMetaTitle;
            }
            set
            {
                this._sMetaTitle = value;
            }
        }

        /// <summary>
        /// Gets or sets the key for the content
        /// </summary>
        public string Key
        {
            get
            {
                return this.GetKey(this.Url, this.Name);
            }
        }

        public string View
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the key for the content
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the key for the content
        /// </summary>
        public string Name
        {
            get
            {
                return this._sName;
            }

            set
            {
                if (null != this._sName)
                {
                    this._sContent = null;
                }

                this._sName = value;
            }
        }

        protected MaxContentUrlEntity ContentUrlEntity
        {
            get
            {
                if (null == this._oContentUrlEntity)
                {
                    if (null != this.Url)
                    {
                        this._oContentUrlEntity = MaxContentUrlEntity.Create();
                        this._oContentUrlEntity.LoadCurrentByUrlCache(this.Url);
                    }
                    else
                    {
                        return MaxContentUrlEntity.Create();
                    }
                }

                return this._oContentUrlEntity;
            }
        }

        protected List<MaxWebPageContentEntity> ContentList
        {
            get
            {
                if (null == _oContentList)
                {
                    this._oContentList = MaxWebPageContentEntity.GetContentList(this.ContentUrlEntity.ContentGroupId);
                }

                return _oContentList;
            }
        }

        protected string GetKey(string lsUrl, string lsName)
        {
            if (!String.IsNullOrEmpty(lsName))
            {
                return lsUrl + "." + lsName;
            }

            return lsUrl + ".udContent";
        }

        public string GetContentPublic(string lsName)
        {
            string lsR = this.GetContent(lsName);
            if (lsName != "MetaKeyWords" && lsName != "MetaDescription" && lsName != "MetaTitle")
            {
                lsR = lsR.Replace("<MaxBlank />", "<div><a href='javascript:void(0);' class='btn btn-default'>Double click to add content.</a></div>");
                // Run any code to replace dynamic content.


            }
            else
            {
                lsR = lsR.Replace("<MaxBlank />", string.Empty);
            }

            return lsR;
        }

        protected string GetContent(string lsName)
        {
            string lsR = "<MaxBlank />";
            int lnVersionMax = 0;
            foreach (MaxWebPageContentEntity loEntity in this.ContentList)
            {
                if (loEntity.Name.Equals(lsName, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (loEntity.Version > lnVersionMax)
                    {
                        lsR = loEntity.Value;
                        lnVersionMax = loEntity.Version;
                    }
                }
            }

            return lsR;
        }

        public int Save(string lsAction)
        {
            if (this.ContentUrlEntity.Id.Equals(Guid.Empty))
            {
                this.ContentUrlEntity.Url = this.Url;
                this.ContentUrlEntity.ContentGroupId = Guid.NewGuid();
                this.ContentUrlEntity.IsActive = true;
                this.ContentUrlEntity.Insert();
            }


            MaxWebPageContentEntity loEntity = MaxWebPageContentEntity.Create();
            loEntity.ContentGroupId = this.ContentUrlEntity.ContentGroupId;
            loEntity.Name = this.Name;
            loEntity.Value = this.Content;
            if (null == lsAction)
            {
                loEntity.Insert();
            }
            else
            {
                if (null != this._sMetaKeyWords && lsAction.Equals("Save Keywords", StringComparison.InvariantCultureIgnoreCase))
                {
                    loEntity = MaxWebPageContentEntity.Create();
                    loEntity.ContentGroupId = this.ContentUrlEntity.ContentGroupId;
                    loEntity.Name = "MetaKeyWords";
                    loEntity.Value = this._sMetaKeyWords;
                    loEntity.Insert();
                }

                if (null != this._sMetaDescription && lsAction.Equals("Save Description", StringComparison.InvariantCultureIgnoreCase))
                {
                    loEntity = MaxWebPageContentEntity.Create();
                    loEntity.ContentGroupId = this.ContentUrlEntity.ContentGroupId;
                    loEntity.Name = "MetaDescription";
                    loEntity.Value = this._sMetaDescription;
                    loEntity.Insert();
                }

                if (null != this._sMetaTitle && lsAction.Equals("Save Title", StringComparison.InvariantCultureIgnoreCase))
                {
                    loEntity = MaxWebPageContentEntity.Create();
                    loEntity.ContentGroupId = this.ContentUrlEntity.ContentGroupId;
                    loEntity.Name = "MetaTitle";
                    loEntity.Value = this._sMetaTitle;
                    loEntity.Insert();
                }
            }

            return loEntity.Version;
        }


    }
}
