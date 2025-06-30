// <copyright file="MaxCmsController.cs" company="Lakstins Family, LLC">
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
// <change date="6/19/2014" author="Brian A. Lakstins" description="Making controller thinner.">
// <change date="8/13/2014" author="Brian A. Lakstins" description="Added logging.">
// <change date="8/21/2014" author="Brian A. Lakstins" description="Update CMS editing and storage.">
// <change date="9/30/2014" author="Brian A. Lakstins" description="Update based on OutputCache testing.">
// <change date="10/6/2014" author="Brian A. Lakstins" description="Update index to handle any request method (GET, POST, HEAD)">
// <change date="7/29/2015" author="Brian A. Lakstins" description="Added logging errors when urls without content are accessed anonymously.">
// <change date="12/20/2015" author="Brian A. Lakstins" description="Fix for showing a view when a file for the view exists.">
// <change date="5/18/2016" author="Brian A. Lakstins" description="Limit page not found error emails to once per url.">
// <change date="10/8/2018" author="Brian A. Lakstins" description="Change 404 email to send every time, but not send for bots.">
// <change date="10/10/2018" author="Brian A. Lakstins" description="Change 404 email to require at least 3 hits to same page to send 404 error email.">
// <change date="10/19/2018" author="Brian A. Lakstins" description="Change 404 email to have more information.">
// <change date="5/19/2020" author="Brian A. Lakstins" description="Update sending of 404 email.  Send every 3 hits that are from different ip addresses.">
// <change date="5/25/2020" author="Brian A. Lakstins" description="Fix RawURL Exceptions.  Add logging to 404 notifications.">
// <change date="5/26/2020" author="Brian A. Lakstins" description="Fix logging 404 notifications.">
// <change date="5/27/2020" author="Brian A. Lakstins" description="Add referrer to logging 404 notifications.">
// <change date="11/18/2020" author="Brian A. Lakstins" description="Add handling of dist folder with static files.">
// <change date="11/18/2020" author="Brian A. Lakstins" description="Update handling of dist folder with static files and folder names.">
// <change date="2/5/2021" author="Brian A. Lakstins" description="Add handling of dist folder under views folder.">
// <change date="6/17/2025" author="Brian A. Lakstins" description="Updated logging.">
// <change date="6/21/2025" author="Brian A. Lakstins" description="Updates handling of virtual files">
// <change date="6/30/2025" author="Brian A. Lakstins" description="Use zip file(s) for static content. Process static content files before any View files.">
// </changelog>
#endregion

namespace MaxFactry.Module.Cms.Mvc4.PresentationLayer
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Hosting;
    using MaxFactry.Core;
    using MaxFactry.Module.Cms.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;
    using System.IO.Compression;

    [MaxAuthorize(Order = 2)]
    public class MaxCmsController : MaxBaseController
    {
        private string _sDefaultView = "MaxCms";

        private static MaxIndex _oCmsPageNotFoundIndex = new MaxIndex();

        private static MaxIndex _oCmsPageNotFoundClientIndex = new MaxIndex();

        private static object _oLock = new object();

        private static MaxIndex _oCmsPageNotFoundLogIndex = new MaxIndex();

        private static bool _bZipContentIntialized = false; 

        private static Dictionary<string, byte[]> _oZipContentFileIndex = new Dictionary<string, byte[]>();

        protected string GetUrl(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5)
        {
            return MaxHtmlHelperLibrary.GetCmsUrl(lsName1, lsName2, lsName3, lsName4, lsName5);
        }

        protected string GetViewFileName(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5)
        {
            string lsR = this.GetUrl(lsName1, lsName2, lsName3, lsName4, lsName5);
            if (string.IsNullOrEmpty(lsName1))
            {
                lsR = "MaxCmsHome";
            }

            return lsR;
        }

        /// <summary>
        /// CMS Pages should cached for 600 seconds (10 minutes) based on Url and MaxStorageKey.
        /// Cookies cannot be sent with the page, or it will not store in the cache.
        /// Cache the output for 1 hour so that it can be used by other requests.  It's either static files or dynamic content that does not change often.
        /// </summary>
        /// <param name="lsName1"></param>
        /// <param name="lsName2"></param>
        /// <param name="lsName3"></param>
        /// <param name="lsName4"></param>
        /// <param name="lsName5"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [OutputCache(Duration = 3600, Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult MaxCms(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5)
        {
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxCms", MaxEnumGroup.LogInfo, "{Url} is not in output cache and is being processed.", Request.Url.ToString()));
            //// This works to find the file page because of the rewrite rule <action type="Rewrite" url="/{R:1}/?f={R:0}" /> for 
            //// for these file types: <match url="(.*)((\.txt)|(\.htm)(l?)|(\.json)|(\.js)|(\.png)|(\.css)|(\.jpg)|(\.gif))($|\??)" />
            //// If other file types need to come through they will need to be added to the web.config file.
            string lsFilePath = this.Request.QueryString["f"];
            if (string.IsNullOrEmpty(lsFilePath))
            {
                lsFilePath = this.Request.Url.AbsolutePath;
                if (!lsFilePath.EndsWith("/"))
                {
                    lsFilePath += "/";
                }

                lsFilePath += "index.html";
                if (lsFilePath.StartsWith("/") && lsFilePath.Length > 1)
                {
                    lsFilePath = lsFilePath.Substring(1);
                }
            }

            string lsDataDirectory = MaxConvertLibrary.ConvertToString(typeof(object), MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxDataDirectory"));
            if (!string.IsNullOrEmpty(lsDataDirectory))
            {
                if (!_bZipContentIntialized)
                {
                    lock (_oLock)
                    {
                        if (!_bZipContentIntialized)
                        {
                            _bZipContentIntialized = true;
                            string[] laFile = Directory.GetFiles(lsDataDirectory);
                            foreach (string lsFile in laFile)
                            {
                                if (lsFile.EndsWith(".zip"))
                                {
                                    FileInfo loFile = new FileInfo(lsFile);
                                    string lsFileBase = loFile.Name.Replace(".zip", string.Empty) + "/";
                                    FileStream loZipFileStream = System.IO.File.OpenRead(lsFile);
                                    try
                                    {
                                        ZipArchive loArchive = new ZipArchive(loZipFileStream, ZipArchiveMode.Read, true);
                                        try
                                        {
                                            for (int lnF = 0; lnF < loArchive.Entries.Count; lnF++)
                                            {
                                                ZipArchiveEntry loEntry = loArchive.Entries[lnF];
                                                if (loEntry.Length > 0)
                                                {
                                                    Stream loFileStream = loEntry.Open();
                                                    try
                                                    {
                                                        string lsFileName = loEntry.FullName;
                                                        if (lsFileName.StartsWith(lsFileBase))
                                                        {
                                                            lsFileName = lsFileName.Substring(lsFileBase.Length);
                                                        }

                                                        if (!_oZipContentFileIndex.ContainsKey(lsFileName))
                                                        {
                                                            if (loFileStream is MemoryStream)
                                                            {
                                                                _oZipContentFileIndex.Add(lsFileName, ((MemoryStream)loFileStream).ToArray());
                                                            }
                                                            else
                                                            {
                                                                MemoryStream loMemoryStream = new MemoryStream();
                                                                try
                                                                {
                                                                    loFileStream.CopyTo(loMemoryStream);
                                                                    _oZipContentFileIndex.Add(lsFileName, loMemoryStream.ToArray());
                                                                }
                                                                finally
                                                                {
                                                                    loMemoryStream.Close();
                                                                    loMemoryStream.Dispose();
                                                                }
                                                            }
                                                        }
                                                    }
                                                    finally
                                                    {
                                                        loFileStream.Close();
                                                        loFileStream.Dispose();
                                                    }
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            loArchive.Dispose();
                                        }
                                    }
                                    finally
                                    {
                                        if (null != loZipFileStream)
                                        {
                                            loZipFileStream.Close();
                                            loZipFileStream.Dispose();
                                        }
                                    }

                                }
                            }
                        }
                    }
                }

                if (_oZipContentFileIndex.Count > 0 && _oZipContentFileIndex.ContainsKey(lsFilePath))
                {
                    Response.Clear();
                    Response.ContentType = MaxFactry.General.BusinessLayer.MaxFileEntity.Create().GetMimeType(lsFilePath);
                    Response.OutputStream.Write(_oZipContentFileIndex[lsFilePath], 0, _oZipContentFileIndex[lsFilePath].Length);
                    return null;
                }
            }

            string[] laPath = new string[] { "~/views/dist/", "~/dist/" };
            foreach (string lsPath in laPath)
            {
                if (HostingEnvironment.VirtualPathProvider.FileExists(lsPath + lsFilePath))
                {
                    VirtualFile loFile = HostingEnvironment.VirtualPathProvider.GetFile(lsPath + lsFilePath);
                    Response.Clear();
                    Response.ContentType = MaxFactry.General.BusinessLayer.MaxFileEntity.Create().GetMimeType(loFile.Name);
                    Stream loStream = loFile.Open();
                    loStream.CopyTo(Response.OutputStream);
                    return null;
                }
            }

            string lsPage = this.GetUrl(lsName1, lsName2, lsName3, lsName4, lsName5);
            MaxFactry.Core.MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxCms", MaxEnumGroup.LogDebug, "Processing Page {Page}", lsPage));
            MaxWebPageContentViewModel loModel = new MaxWebPageContentViewModel(lsPage, string.Empty);
            string lsView = this.GetView(this.GetViewFileName(lsName1, lsName2, lsName3, lsName4, lsName5));

            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxHtmlContent-MetaKeyWords", loModel.GetContentPublic("MetaKeyWords"));
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxHtmlContent-MetaDescription", loModel.GetContentPublic("MetaDescription"));
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, "MaxHtmlContent-MetaTitle", loModel.GetContentPublic("MetaTitle"));

            loModel.View = lsView;
            if (loModel.View != this._sDefaultView || loModel.HasContent || MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer.MaxHtmlHelperLibrary.MaxIsInRole("Admin,Admin - App") || loModel.Url == string.Empty)
            {
                return View(loModel.View, loModel);
            }

            string lsUrl = "MaxCms";
            try
            {
                lsUrl = this.Request.Url.Host + this.Request.RawUrl;
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(this.GetType(), "MaxCms", MaxEnumGroup.LogWarning, "Error Getting RawURL in MaxCms {loE.ToString()}", loE.ToString()));
            }

            string lsUserAgent = Request.UserAgent;
            bool lbIsBot = true;
            if (!string.IsNullOrEmpty(lsUserAgent))
            {
                lbIsBot = Regex.IsMatch(Request.UserAgent, @"bot|crawler|baiduspider|80legs|ia_archiver|voyager|curl|wget|yahoo! slurp|mediapartners-google", RegexOptions.IgnoreCase);
            }

            if (!lbIsBot)
            {
                string lsKey = lsUrl + this.Request.UserHostAddress;

                bool lbSendEmail = false;
                int lnHitCount = 1;
                string lsAccessLog = string.Empty;
                if (!_oCmsPageNotFoundClientIndex.Contains(lsKey))
                {
                    lock (_oLock)
                    {
                        if (!_oCmsPageNotFoundClientIndex.Contains(lsKey))
                        {
                            _oCmsPageNotFoundClientIndex.Add(lsKey, lsUrl);
                            if (!_oCmsPageNotFoundIndex.Contains(lsUrl))
                            {
                                _oCmsPageNotFoundIndex.Add(lsUrl, 1);
                            }
                            else
                            {
                                _oCmsPageNotFoundIndex[lsUrl] = MaxConvertLibrary.ConvertToInt(typeof(object), _oCmsPageNotFoundIndex[lsUrl]) + 1;
                            }

                            lnHitCount = MaxConvertLibrary.ConvertToInt(typeof(object), _oCmsPageNotFoundIndex[lsUrl]);
                            if (lnHitCount % 3 == 0)
                            {
                                lbSendEmail = true;
                            }

                            MaxIndex loIndex = _oCmsPageNotFoundLogIndex[lsUrl] as MaxIndex;
                            if (null == loIndex)
                            {
                                loIndex = new MaxIndex();
                            }

                            MaxIndex loLogIndex = new MaxIndex();
                            loLogIndex.Add("UserHostAddress", this.Request.UserHostAddress);
                            loLogIndex.Add("UserAgent", this.Request.UserAgent);
                            loLogIndex.Add("Referrer", "null");
                            if (null != this.Request.UrlReferrer)
                            {
                                loLogIndex.Add("Referrer", this.Request.UrlReferrer.ToString());
                            }

                            loIndex.Add(MaxConvertLibrary.ConvertToSortString(typeof(object), DateTime.UtcNow), loLogIndex);
                            _oCmsPageNotFoundLogIndex.Add(lsUrl, loIndex);
                            string[] laKey = loIndex.GetSortedKeyList();
                            foreach (string lsLogKey in laKey)
                            {
                                loLogIndex = loIndex[lsLogKey] as MaxIndex;
                                lsAccessLog += lsLogKey + "|" + loLogIndex["UserHostAddress"] + "|" + loLogIndex["UserAgent"] + "|" + loLogIndex["Referrer"] + "\r\n";
                            }
                        }
                    }
                }

                //// Send an email about page being access, but having no content.
                string lsVersion = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeApplication, "MaxAssemblyFileVersion").ToString();
                if (null == lsVersion)
                {
                    lsVersion = "0.0.0.0";
                }

                string lsContent = "Host+RawUrl: " + lsUrl + "\r\n";
                lsContent += "Hit Count: " + lnHitCount + "\r\n";
                lsContent += "UTC Time: " + DateTime.UtcNow.ToString() + "\r\n";
                lsContent += "Version: " + lsVersion + "\r\n";
                lsContent += "Server: " + Request.Url.Scheme + "://" + Request.Url.Authority + "\r\n";
                lsContent += "Page: " + lsPage + "\r\n";
                lsContent += "Agent: " + lsUserAgent + "\r\n";
                lsContent += "RequestUrl: " + Request.Url.ToString() + "\r\n";
                lsContent += "Encoded Url: " + Server.UrlEncode(Request.Url.ToString()) + "\r\n\r\n";
                lsContent += "Access Log:\r\n";
                lsContent += lsAccessLog;

                lsContent += MaxFactry.Core.MaxLogLibrary.GetEnvironmentInformation() + "\r\n\r\n";
                MaxLogLibrary.Log(new MaxLogEntryStructure("404", MaxEnumGroup.LogWarning, "404 notification from MaxCms {lsContent}", lsContent));

                if (lbSendEmail)
                {
                    MaxEmailEntity loEmail = MaxEmailEntity.Create();
                    loEmail.RelationType = "MaxApplicationHostNotification";
                    loEmail.Subject = "[MaxCms404." + lsVersion + "-" + lnHitCount.ToString() + "] " + lsPage;
                    loEmail.Content = lsContent;
                    loEmail.Send();
                }
            }

            //// Redirecting to home page.
            HttpContext.Response.AddHeader("Location", "/");
            return new HttpStatusCodeResult(307);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult MaxCms(MaxWebPageContentViewModel loModel)
        {
            ActionResult loR = this.View(loModel);
            if (!string.IsNullOrEmpty(loModel.View))
            {
                ViewEngineResult loViewEngine = ViewEngines.Engines.FindView(ControllerContext, loModel.View, null);
                if (loViewEngine.View != null)
                {
                    loR = this.View(loModel.View, loModel);
                }
            }

            return loR;
        }

        [MaxRequireHttpsAttribute(Order = 1)]
        [MaxAuthorize(Roles = "Admin,Admin - App")]
        [HttpGet]
        public ActionResult MaxCmsEdit(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Index(string lsName) start", "MaxCmsController");
            string lsPage = this.GetUrl(lsName1, lsName2, lsName3, lsName4, lsName5);

            MaxFactry.Core.MaxLogLibrary.Log(MaxFactry.Core.MaxEnumGroup.LogDebug, "Index(string lsName) [" + lsPage + "]", "MaxCmsController");

            if (!string.IsNullOrEmpty(Request.QueryString["name"]))
            {
                MaxFactry.Core.MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Index([" + lsPage + "]) end", "MaxCmsController");
                return View("MaxCmsEdit", new MaxWebPageContentViewModel(lsPage, Request.QueryString["name"]));
            }

            MaxFactry.Core.MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Index([" + lsPage + "]) end", "MaxCmsController");
            MaxWebPageContentViewModel loModel = new MaxWebPageContentViewModel(lsPage, string.Empty);
            return View(loModel);
        }

        [HttpPost]
        [MaxRequireHttps(Order = 1)]
        [MaxAuthorize(Roles = "Admin,Admin - App")]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult MaxCmsEdit(string lsName1, string lsName2, string lsName3, string lsName4, string lsName5, MaxWebPageContentViewModel loModel, string uoProcess)
        {
            MaxFactry.Core.MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Index([" + lsName1 + "],[" + loModel.Url + "]) start", "MaxCmsController");
            if (null == loModel.Url)
            {
                loModel.Url = string.Empty;
            }

            int lnLastVersion = loModel.Save(uoProcess);
            ViewBag.Message = "Content was saved as version " + lnLastVersion.ToString() + ".";
            MaxFactry.Core.MaxLogLibrary.Log(MaxEnumGroup.LogDebug, "Index([" + lsName1 + "],[" + loModel.Url + "]) end", "MaxCmsController");
            HttpResponse.RemoveOutputCacheItem("/" + loModel.Url);
            return View(loModel);
        }

        public string GetView(string lsPage)
        {
            if (HostingEnvironment.VirtualPathProvider.FileExists("~/views/maxcms/" + lsPage + ".cshtml"))
            {
                return lsPage;
            }

            return this._sDefaultView;
        }
    }
}
