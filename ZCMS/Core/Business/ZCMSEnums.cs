using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public enum PageStatus
    {
        [Display(Name = "Published")]
        Published,
        [Display(Name = "Draft")]
        Draft,
        [Display(Name = "New")]
        New,
        [Display(Name = "Any")]
        Any
    }

    public enum Tab
    {
        [Display(Name = "Tab1")]
        Tab1,
        [Display(Name = "Tab2")]
        Tab2,
        [Display(Name = "Tab3")]
        Tab3

    }

    public enum PageType
    {
        [Display(Name = "Contentpage")]
        Contentpage,
        [Display(Name = "Email")]
        Email,
        [Display(Name = "Form")]
        Formpage
    }

    public enum FileType
    {
        ImageFile,
        DocumentFile,
        VideoFile,
        AudioFile,
        Unknown
    }

    public enum ContentViewStatus
    {
        Authorized,
        NotAuthorized,
        NotAuthenticated
    }
}