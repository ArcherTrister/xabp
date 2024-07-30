// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.Localization;

namespace X.Abp.AuditLogging.Web.Pages.AuditLogging
{
    public class IndexModel : AuditLoggingPageModel
    {
        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public List<SelectListItem> SelectListHttpStatusCode { get; set; }

        public List<SelectListItem> SelectListHttpMethod { get; set; }

        public List<SelectListItem> SelectListHasException { get; set; }

        [SelectItems("SelectListHttpMethod")]
        public string HttpMethod { get; set; }

        public string UserName { get; set; }

        public string ApplicationName { get; set; }

        public string ClientIpAddress { get; set; }

        public string CorrelationId { get; set; }

        public string UrlFilter { get; set; }

        [SelectItems("SelectListHttpStatusCode")]
        public int? HttpStatusCode { get; set; }

        [SelectItems("SelectListHasException")]
        public string HasException { get; set; }

        public int? MaxExecutionDuration { get; set; }

        public int? MinExecutionDuration { get; set; }

        public string EntityChangeStartTime { get; set; }

        public string EntityChangeEndTime { get; set; }

        public string EntityChangeEntityTypeFullName { get; set; }

        public int? EntityChangeChangeType { get; set; }

        public List<SelectListItem> EntityChangeChangeTypeList { get; set; }

        [RazorInject]
        public IHtmlLocalizer<AuditLoggingResource> HtmlLocalizer { get; set; }

        protected IHtmlHelper HtmlHelper { get; }

        public IndexModel(IHtmlHelper htmlHelper)
        {
            HtmlHelper = htmlHelper;
        }

        public virtual void OnGet()
        {
            FillHttpStatusCodeSelectList();
            FillHttpMethodSelectList();
            FillHasExceptionSelectList();
            FillEntityChangeTypeSelectList();
        }

        public virtual Task<IActionResult> OnPostAsync()
        {
            return Task.FromResult<IActionResult>(Page());
        }

        protected virtual void FillHttpStatusCodeSelectList()
        {
            SelectListHttpStatusCode = HtmlHelper.GetEnumSelectList(typeof(System.Net.HttpStatusCode)).ToList();
            foreach (SelectListItem selectListItem in SelectListHttpStatusCode)
            {
                selectListItem.Text = !(selectListItem.Value == "200") ? selectListItem.Value + " - " + Regex.Replace(selectListItem.Text, "([A-Z])", " $1").Trim() : selectListItem.Value + " - " + selectListItem.Text;
            }

            SelectListHttpStatusCode.AddFirst(new SelectListItem("", ""));
        }

        protected virtual void FillHttpMethodSelectList() => SelectListHttpMethod = new List<SelectListItem>()
        {
          new SelectListItem("", ""),
          new SelectListItem("GET", "GET"),
          new SelectListItem("POST", "POST"),
          new SelectListItem("DELETE", "DELETE"),
          new SelectListItem("PUT", "PUT"),
          new SelectListItem("HEAD", "HEAD"),
          new SelectListItem("CONNECT", "CONNECT"),
          new SelectListItem("OPTIONS", "OPTIONS"),
          new SelectListItem("TRACE", "TRACE")
        };

        protected virtual void FillHasExceptionSelectList() => SelectListHasException = new List<SelectListItem>()
        {
          new SelectListItem("", ""),
          new SelectListItem(HtmlLocalizer["Yes"].Value, "true"),
          new SelectListItem(HtmlLocalizer["No"].Value, "false")
        };

        protected virtual void FillEntityChangeTypeSelectList()
        {
            IEnumerable<SelectListItem> collection = Enum.GetValues(typeof(EntityChangeType)).Cast<EntityChangeType>().Select(x => new SelectListItem(x.ToString(), x.ToString()));
            List<SelectListItem> selectListItemList = new List<SelectListItem>()
            {
              new SelectListItem("", "")
            };
            selectListItemList.AddRange(collection);
            EntityChangeChangeTypeList = selectListItemList;
        }
    }
}
