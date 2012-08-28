using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business.Content
{
    public class ZCMSBasePage
    {
        private DateTime _created;
        private DateTime _lastModified;
        private PageStatus _status;
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
        //[Required(ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageNameRequired")]
        //[StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageNameLength")]
        //[RegularExpression(@"^[A-z0-9\s]+", ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageNameAllowedChars")]
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


    }
}