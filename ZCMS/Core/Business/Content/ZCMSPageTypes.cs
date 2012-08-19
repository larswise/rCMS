using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public interface IZCMSPageType
    {
        string PageTypeName { get; }
        List<ZCMSProperty> Properties { get; set; }
        string FriendlyPageTypeName { get; }
        string PageTypeDisplayName { get; set; }
    }

    public class ArticlePage : IZCMSPageType
    {
        private List<ZCMSProperty> _properties;
        private string _pageTypeDisplayName;

        public ArticlePage()
        {
            _properties = new List<ZCMSProperty>();

            _properties.Add(new TextProperty() { Order = 1, PropertyName = CMS_i18n.BackendResources.ArticleHeading, PropertyValue = " " });
            _properties.Add(new MultiLineTextProperty() { Order = 2, PropertyName = CMS_i18n.BackendResources.ArticleIntro, PropertyValue = " " });
            _properties.Add(new RichTextProperty() { Order = 3, PropertyName = CMS_i18n.BackendResources.PageDefaultEditor, PropertyValue = "" });
            _properties.Add(new BooleanProperty() { Order = 6, PropertyName = CMS_i18n.BackendResources.SelectedOrNot, PropertyValue = false });
            _properties.Add(new TagsProperty() { Order = 4, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>() });
            _properties.Add(new ImageListProperty() { Order = 5, PropertyName = CMS_i18n.BackendResources.ImageCarousel, PropertyValue = new List<string>() });

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

        public List<ZCMSProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = (List<ZCMSProperty>)value;
            }
        }
    }

    public class ContainerPage : IZCMSPageType
    {
        private List<ZCMSProperty> _properties;
        private string _pageTypeDisplayName;

        public ContainerPage()
        {
            _properties = new List<ZCMSProperty>();

            _properties.Add(new TextProperty() { Order = 1, PropertyName = CMS_i18n.BackendResources.ContainerHeading, PropertyValue = " " });
            _properties.Add(new TagsProperty() { Order = 3, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>() });

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

        public List<ZCMSProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {                
                _properties = (List<ZCMSProperty>)value;                
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