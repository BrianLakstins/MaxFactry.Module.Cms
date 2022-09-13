// <copyright file="MaxAppLibraryCmsProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/5/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Rename from MaxAppLibraryProvider.  Add default global configuration.">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.Mvc4
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Module.Cms.BusinessLayer;

    /// <summary>
    /// Provider for MaxApplicationLibrary used when running this project as an application
    /// </summary>
    public class MaxAppLibraryCmsProvider : MaxFactry.General.AspNet.IIS.Mvc4.Provider.MaxAppLibraryDefaultProvider
    {

        public override void RegisterProviders()
        {
            base.RegisterProviders();
            MaxFactry.Module.Cms.Mvc4.MaxStartup.Instance.RegisterProviders();
        }

        public override void SetProviderConfiguration(MaxIndex loConfig)
        {
            MaxIndex loConfigurationIndex = loConfig["MaxConfigurationLibraryDefaultProviderIndex"] as MaxIndex;
            if (null == loConfigurationIndex)
            {
                loConfigurationIndex = new MaxIndex();
            }

            //// Set default Storage Key
            loConfigurationIndex.Add(MaxEnumGroup.ScopeApplication.ToString() + "-" + MaxFactryLibrary.MaxStorageKeyName, "b20748e0-9a16-4103-84ae-6c006a843eb7");
            loConfigurationIndex.Add(MaxEnumGroup.ScopeProcess.ToString() + "-" + MaxFactryLibrary.MaxStorageKeyName, "b20748e0-9a16-4103-84ae-6c006a843eb7");
            loConfig.Add("MaxConfigurationLibraryDefaultProviderIndex", loConfigurationIndex);

            base.SetProviderConfiguration(loConfig);
            MaxFactry.Module.Cms.Mvc4.MaxStartup.Instance.SetProviderConfiguration(loConfig);
        }

        public override void ApplicationStartup()
        {
            string lsDefaultId = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, MaxFactryLibrary.MaxStorageKeyName) as string;
            if (null == lsDefaultId || lsDefaultId.Length == 0)
            {
                lsDefaultId = "b20748e0-9a16-4103-84ae-6c006a843eb7";
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeApplication, MaxFactryLibrary.MaxStorageKeyName, lsDefaultId);
            }

            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxFactryLibrary.MaxStorageKeyName, lsDefaultId);

            base.ApplicationStartup();
            MaxFactry.General.AspNet.IIS.Mvc4.MaxAppLibrary.AddValidStorageKey(lsDefaultId);

            MaxFactry.Module.Cms.Mvc4.MaxStartup.Instance.ApplicationStartup();

            //// Add a default route last for any routes that have not been added, but will still match controllers and actions already loaded.
            RouteTable.Routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { id = UrlParameter.Optional }
            );
        }
    }
}