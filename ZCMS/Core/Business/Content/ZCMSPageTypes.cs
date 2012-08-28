using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZCMS.Core.Business.Validators;

namespace ZCMS.Core.Business.Content
{
    public interface IZCMSPageType
    {
        string PageTypeName { get; }
        List<IZCMSProperty> Properties { get; set; }
        string FriendlyPageTypeName { get; }
        string PageTypeDisplayName { get; set; }
    }

    public class ArticlePage : IZCMSPageType
    {
        private List<IZCMSProperty> _properties;
        private string _pageTypeDisplayName;

        public ArticlePage()
        {
            _properties = new List<IZCMSProperty>();
            _properties.Add(new DisplayOnlyTextProperty() { Order = 0, PropertyName = CMS_i18n.BackendResources.PageUrlSlug, PropertyValue = "", PropertyValidator = typeof(TextPropertyNotNullEmptyValidator).FullName });
            _properties.Add(new DateProperty() { Order = 1, PropertyName = CMS_i18n.BackendResources.StartPublish, PropertyValue = DateTime.Now, PropertyValidator = typeof(DatePropertyNotEmptyValidator).FullName });
            _properties.Add(new DateProperty() { Order = 2, PropertyName = CMS_i18n.BackendResources.EndPublish });

            _properties.Add(new BooleanProperty() { Order = 3, PropertyName = CMS_i18n.BackendResources.AllowComments, PropertyValue = false });
            _properties.Add(new BooleanProperty() { Order = 4, PropertyName = CMS_i18n.BackendResources.ShowInMenus, PropertyValue = false });

            _properties.Add(new TextProperty() { Order = 5, PropertyName = CMS_i18n.BackendResources.ArticleHeading, PropertyValue = " ", PropertyValidator = typeof(TextPropertyStringLengthNotNullEmptyValidator).FullName });
            _properties.Add(new MultiLineTextProperty() { Order = 6, PropertyName = CMS_i18n.BackendResources.ArticleIntro, PropertyValue = " ", PropertyValidator = typeof(TextPropertyNotNullEmptyValidator).FullName });
            _properties.Add(new RichTextProperty() { Order = 7, PropertyName = CMS_i18n.BackendResources.PageDefaultEditor, PropertyValue = "" });
            
            _properties.Add(new TagsProperty() { Order = 8, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>() });
            _properties.Add(new ImageListProperty() { Order = 9, PropertyName = CMS_i18n.BackendResources.ImageCarousel, PropertyValue = new List<string>() });

            _pageTypeDisplayName = CMS_i18n.BackendResources.PageTypeDisplayArticle;
        }

        public string PageTypeName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string FriendlyPageTypeName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public string PageTypeDisplayName 
        {
            get
            {
                return _pageTypeDisplayName;
            }
            set
            {
                _pageTypeDisplayName = value;
            }
        }

        public List<IZCMSProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = (List<IZCMSProperty>)value;
            }
        }
    }

    public class ContainerPage : IZCMSPageType
    {
        private List<IZCMSProperty> _properties;
        private string _pageTypeDisplayName;

        public ContainerPage()
        {
            _properties = new List<IZCMSProperty>();
            _properties.Add(new DisplayOnlyTextProperty() { Order = 0, PropertyName = CMS_i18n.BackendResources.PageUrlSlug, PropertyValue = "" });
            _properties.Add(new DateProperty() { Order = 1, PropertyName = CMS_i18n.BackendResources.StartPublish, PropertyValue = DateTime.Now });
            _properties.Add(new DateProperty() { Order = 2, PropertyName = CMS_i18n.BackendResources.EndPublish, PropertyValue = null });

            _properties.Add(new TextProperty() { Order = 3, PropertyName = CMS_i18n.BackendResources.ContainerHeading, PropertyValue = " " });
            _properties.Add(new TagsProperty() { Order = 4, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>() });

            _pageTypeDisplayName = CMS_i18n.BackendResources.PageTypeDisplayContainer;
        }

        public string PageTypeName
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string FriendlyPageTypeName
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public List<IZCMSProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {                
                _properties = (List<IZCMSProperty>)value;                
            }
        }

        public string PageTypeDisplayName
        {
            get
            {
                return _pageTypeDisplayName;
            }
            set
            {
                _pageTypeDisplayName = value;
            }
        }
    }

}