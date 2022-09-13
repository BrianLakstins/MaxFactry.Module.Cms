// <copyright file="MaxHtmlHelperLibrary.cs" company="Lakstins Family, LLC">
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
// <change date="12/31/2014" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Module.Cms.BusinessLayer;

    /// <summary>
    /// Helper library for producing HTML
    /// </summary>
    public static class MaxHtmlHelperLibrary
    {
        public static string GetCmsUrl(Uri loUrl)
        {
            string lsR = loUrl.PathAndQuery;
            if (lsR.Contains("?"))
            {
                lsR = lsR.Substring(0, lsR.IndexOf('?'));
            }

            if (lsR.StartsWith("/"))
            {
                lsR = lsR.Substring(1);
            }

            if (lsR.EndsWith("/"))
            {
                lsR = lsR.Substring(0, lsR.Length - 1);
            }

            return lsR;
        }

        public static string GetCmsUrl(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5)
        {
            string lsR = string.Empty;
            if (!string.IsNullOrEmpty(lsName1))
            {
                lsR = lsName1;
            }

            if (!string.IsNullOrEmpty(lsName2))
            {
                lsR += "/" + lsName2;
            }

            if (!string.IsNullOrEmpty(lsName3))
            {
                lsR += "/" + lsName3;
            }

            if (!string.IsNullOrEmpty(lsName4))
            {
                lsR += "/" + lsName4;
            }

            if (!string.IsNullOrEmpty(lsName5))
            {
                lsR += "/" + lsName5;
            }

            return lsR;
        }

    }
}