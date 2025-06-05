// <copyright file="IMaxContentRepositoryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="6/27/2014" author="Brian A. Lakstins" description="Change base from AppId to BaseId.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Update CMS editing and storage.">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Change base class.">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.DataLayer
{
    using System;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Interface for MaxContentRepository
    /// </summary>
    public interface IMaxContentRepositoryProvider : MaxFactry.Base.DataLayer.IMaxBaseRepositoryProvider
    {
    }
}
