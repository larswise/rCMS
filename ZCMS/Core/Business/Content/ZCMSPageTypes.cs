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
            _properties.Add(new DisplayOnlyTextProperty() { DisplayType = Business.DisplayType.Editor, Order = 0, PropertyName = CMS_i18n.BackendResources.PageUrlSlug, PropertyValue = "", PropertyValidator = typeof(TextPropertyNotNullEmptyValidator).FullName, DisplayInTab = Tab.Tab1 });

            _properties.Add(new BooleanProperty() { DisplayType = Business.DisplayType.Editor, Order = 3, PropertyName = CMS_i18n.BackendResources.AllowComments, PropertyValue = false, DisplayInTab = Tab.Tab1 });
            _properties.Add(new BooleanProperty() { DisplayType = Business.DisplayType.Editor, Order = 4, PropertyName = CMS_i18n.BackendResources.ShowInMenus, PropertyValue = false, DisplayInTab = Tab.Tab1 });

            _properties.Add(new TextProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 5, PropertyName = CMS_i18n.BackendResources.ArticleHeading, PropertyValue = " ", DisplayInTab = Tab.Tab2 });
            _properties.Add(new MultiLineTextProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 6, PropertyName = CMS_i18n.BackendResources.ArticleIntro, PropertyValue = " ", DisplayInTab = Tab.Tab2 });
            _properties.Add(new RichTextProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 7, PropertyName = CMS_i18n.BackendResources.PageDefaultEditor, PropertyValue = "", DisplayInTab = Tab.Tab2 });

            _properties.Add(new TagsProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 8, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>(), DisplayInTab = Tab.Tab3 });
            _properties.Add(new ImageListProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 9, PropertyName = CMS_i18n.BackendResources.ImageCarousel, PropertyValue = new List<string>(), DisplayInTab = Tab.Tab3 });

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
            _properties.Add(new DisplayOnlyTextProperty() { DisplayType = Business.DisplayType.Editor, Order = 0, PropertyName = CMS_i18n.BackendResources.PageUrlSlug, PropertyValue = "", DisplayInTab = Tab.Tab1 });

            _properties.Add(new TextProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 3, PropertyName = CMS_i18n.BackendResources.ContainerHeading, PropertyValue = " ", DisplayInTab = Tab.Tab2 });
            _properties.Add(new TagsProperty() { DisplayType = Business.DisplayType.Everywhere, Order = 4, PropertyName = CMS_i18n.BackendResources.Tags, PropertyValue = new List<string>(), DisplayInTab = Tab.Tab2 });

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