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
// <change date="4/20/2016" author="Brian A. Lakstins" description="Added method to send form content through email.">
// <change date="7/20/2016" author="Brian A. Lakstins" description="Update recaptcha handling so that the field does not need to be required.">
// <change date="3/8/2020" author="Brian A. Lakstins" description="Updated error handling so emails get sent even if one field fails.">
// <change date="5/24/2020" author="Brian A. Lakstins" description="Update exception logging for a common exception type">
// <change date="8/25/2020" author="Brian A. Lakstins" description="Update for change to refernce">
// <change date="2/15/2021" author="Brian A. Lakstins" description="Add domain for local testing.">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.Module.Cms.BusinessLayer;
    using MaxFactry.Module.Cms.Mvc4.PresentationLayer;

    /// <summary>
    /// Helper library for producing HTML
    /// </summary>
    public static class MaxHtmlHelperCmsExtension
    {
        public static IHtmlString MaxCmsGetPublicContent<T>(this HtmlHelper<T> helper, string lsContentName)
        {
            string lsR = null;
            if (!string.IsNullOrEmpty(helper.ViewData[lsContentName] as string))
            {
                lsR = helper.ViewData[lsContentName] as string;
            }
            else
            {

                string lsUrl = MaxFactry.Module.Cms.Mvc4.PresentationLayer.MaxHtmlHelperLibrary.GetCmsUrl(helper.ViewContext.RequestContext.HttpContext.Request.Url);
                Guid loContentGroupId = MaxContentUrlEntity.GetContentGroupByUrl(lsUrl);
                lsR = MaxWebPageContentEntity.GetContent(loContentGroupId, lsContentName);
            }

            if (!string.IsNullOrEmpty(lsR))
            {
                lsR = MaxFactry.General.PresentationLayer.MaxShortCodeLibrary.GetContentShortCode(lsR);
            }

            if (null == lsR)
            {
                if (lsContentName.StartsWith("Meta"))
                {
                    lsR = string.Empty;
                }
                else
                {
                    lsR = "<div><a href='javascript:void(0);' class='btn btn-default'>Double click to add content.</a></div>";
                }
            }

            return MvcHtmlString.Create(lsR);
        }

        public static bool MaxCmsEmailForm<T>(this HtmlHelper<T> helper, string lsEmailToAddressList, string lsEmailSubject, string lsEmailFrom, string lsRelationId, string lsCaptchaSecret, string lsRequiredFieldList)
        {
            HttpRequestBase loRequest = helper.ViewContext.RequestContext.HttpContext.Request;
            MaxBaseEntityViewModel loModel = helper.ViewData.Model as MaxBaseEntityViewModel;
            bool lbR = true;
            var lsTest = string.Empty;
            if (loRequest.HttpMethod.Equals("post", StringComparison.InvariantCultureIgnoreCase))
            {
                MaxIndex loFormNameValue = new MaxIndex();
                foreach (string lsKey in loRequest.Form.Keys)
                {
                    string lsValue = "MULTI";
                    try
                    {
                        string[] laValue = loRequest.Form.GetValues(lsKey);
                        if (laValue.Length > 1)
                        {
                            loModel.OriginalValues.Add(lsKey, lsValue);
                            for (int lnV = 0; lnV < laValue.Length; lnV++)
                            {
                                loFormNameValue.Add(lsKey + "|" + lnV.ToString(), laValue[lnV]);
                                loModel.OriginalValues.Add(lsKey + "|" + lnV.ToString(), laValue[lnV]);
                            }
                        }
                        else
                        {
                            lsValue = loRequest.Form[lsKey];
                            loModel.OriginalValues.Add(lsKey, lsValue);
                        }
                    }
                    catch (Exception loE)
                    {
                        loModel.SetText(lsKey + "-style", "background-color:#F0CACA;");
                        lsValue = "ERROR:" + HttpUtility.HtmlEncode(loE.Message);
                        if (loE is HttpRequestValidationException)
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("Request.Form", MaxEnumGroup.LogWarning, "Getting value for {lsKey} from form field caused HttpValidationException", lsKey));
                        }
                        else
                        {
                            MaxLogLibrary.Log(new MaxLogEntryStructure("Request.Form", MaxEnumGroup.LogError, "Getting value for {lsKey} from form field caused exception", loE, lsKey));
                        }
                    }

                    loFormNameValue.Add(lsKey, lsValue);
                }

                string lsReplyEmail = string.Empty;
                string[] laFieldNameList = lsRequiredFieldList.Split(new char[] { ',' });
                foreach (string lsName in laFieldNameList)
                {
                    string lsValue = loFormNameValue[lsName] as string;
                    if (string.IsNullOrEmpty(lsValue))
                    {
                        loModel.SetText(lsName + "-style", "background-color:#F0CACA;");
                        lbR = false;
                    }
                    else if (lsName == "email")
                    {
                        if (!MaxFactry.General.BusinessLayer.MaxEmailEntity.IsValidEmail(lsValue))
                        {
                            loModel.SetText(lsName + "-style", "background-color:#F0CACA;");
                            lbR = false;
                        }
                        else
                        {
                            lsReplyEmail = lsValue;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(lsCaptchaSecret) && lbR && loRequest.Url.DnsSafeHost.ToLower() != "localhost" && !loRequest.Url.DnsSafeHost.ToLower().EndsWith(".local"))
                {
                    string lsRemoteIP = loRequest.ServerVariables["REMOTE_ADDR"];
                    var lsResponse = loFormNameValue["g-recaptcha-response"] as string;
                    try
                    {
                        lbR = MaxFactry.General.AspNet.PresentationLayer.MaxOwinLibrary.IsRecaptchaVerified(lsCaptchaSecret, lsResponse, lsRemoteIP);
                    }
                    catch (Exception loE)
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "Error validating Recaptcha", loE));
                        lbR = false;
                    }
                }

                //// Fake field that is hidden and would only be filled in by bots
                string lsEmailConfirm = loFormNameValue["emailconfirm"] as string;
                if (!string.IsNullOrEmpty(lsEmailConfirm) && lbR)
                {
                    lbR = false;
                }

                var loForm = MaxFactry.General.AspNet.BusinessLayer.MaxFormEntity.Create();
                loForm.FormRelationId = new Guid(lsRelationId);
                var loIndex = new MaxFactry.Core.MaxIndex();
                string lsContent = "From Url=[" + loRequest.Url.ToString() + "]\r\n";
                loForm.Insert();
                string[] laKey = loFormNameValue.GetSortedKeyList();
                foreach (string lsKey in laKey)
                {
                    if (lsKey != "g-recaptcha-response" && lsKey != "View" && lsKey != "emailconfirm")
                    {
                        string lsValue = loFormNameValue[lsKey] as string;
                        loIndex.Add(lsKey, lsValue);
                        lsContent += lsKey + ": " + lsValue + "\r\n";
                        var loFormValue = MaxFactry.General.AspNet.BusinessLayer.MaxFormValueEntity.Create();
                        loFormValue.FormId = loForm.Id;
                        loFormValue.Name = lsKey;
                        loFormValue.Value = lsValue;
                        loFormValue.Insert();
                    }
                }

                if (lbR)
                {
                    loIndex.Add("FormContentText", lsContent);
                    loForm.FormContent = loIndex;
                    loForm.IsActive = true;
                    loForm.Update();

                    var loEmail = MaxFactry.General.BusinessLayer.MaxEmailEntity.Create();
                    loEmail.Subject = lsEmailSubject;

                    loEmail.Content = lsContent;
                    loEmail.FromAddress = "web@efactorysolutions.com";
                    loEmail.FromName = lsEmailFrom;
                    loEmail.ToAddressListText = lsEmailToAddressList;
                    if (!string.IsNullOrEmpty(lsReplyEmail))
                    {
                        loEmail.ReplyAddress = lsReplyEmail;
                    }

                    loEmail.Insert();
                    loEmail.Send();
                    loEmail.Update();
                }
            }

            return lbR;
        }
    }
}
