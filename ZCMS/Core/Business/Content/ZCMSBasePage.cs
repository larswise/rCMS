using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSBasePage
    {
        private DateTime _created;
        private DateTime _lastModified;
        private PageStatus _status;
        private PageType _pageContentType;
        private DateTime _startPublish;
        private DateTime _endPublish;
        private int _pageId;
        private string _pageName;
        private string _writtenBy;
        private string _lastChangedBy;
        private string _pageType;

        private List<IZCMSProperty> _properties = new List<IZCMSProperty>();

        public ZCMSBasePage(IZCMSProperty []properties)
        {
            foreach (IZCMSProperty item in properties)
                _properties.Add(item);

            Sort();
        }

        public ZCMSBasePage()
        {
            if (_properties == null)
                _properties = new List<IZCMSProperty>();
        }

        public ZCMSBasePage(dynamic bpt)
        {
            
            this._properties = bpt.Properties;
            Sort();
            this._pageType = ((IZCMSPageType)bpt).PageTypeDisplayName;
        }

        private void Sort()
        {
            this._properties = this.Properties.OrderBy(o => o.Order).ToList();
        }

        public List<IZCMSProperty> Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "PageID")]
        public int PageID
        {
            get
            {
                return _pageId;
            }
            set
            {
                _pageId = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "PageName")]
        [AdditionalMetadata("PropName", "PageName")]
        [Required]
        public string PageName
        {
            get
            {
                return _pageName;
            }
            set
            {
                _pageName = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "PageStatus")]
        public PageStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public PageType PublishType
        {
            get
            {
                return _pageContentType;
            }
            set
            {
                _pageContentType = value;
            }
        }

        public string PageType
        {
            get
            {
                return _pageType;
            }
            set
            {
                _pageType = value;
            }
        }

        public string WrittenBy
        {
            get
            {
                return _writtenBy;
            }
            set
            {
                _writtenBy = value;
            }
        }

        public string LastChangedBy
        {
            get
            {
                return _lastChangedBy;
            }
            set
            {
                _lastChangedBy = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return _created;
            }
            set
            {
                _created = value;
            }
        }

        public DateTime LastModified
        {
            get
            {
                return _lastModified;
            }
            set
            {
                _lastModified = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "StartPublish", GroupName = "RequiredDate")]
        [DataType(DataType.Date, ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageDateTime")]
        [AdditionalMetadata("PropName", "StartPublish")]
        [Required]
        public DateTime StartPublish
        {
            get
            {
                return _startPublish;
            }
            set
            {
                _startPublish = value;
            }
        }

        [DataType(DataType.Date, ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageDateTime")]        
        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "EndPublish", ShortName="EndPublish")]
        public DateTime EndPublish
        {
            get
            {
                return _endPublish;
            }
            set
            {
                _endPublish = value;
            }
        }

    }
}
