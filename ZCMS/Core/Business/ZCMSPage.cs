using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    public class ZCMSPage
    {        
        private DateTime? _startPublish;
        private DateTime? _endPublish;
        private PageStatus _status;
        private int _pageId;
        private string _pageName;
        private bool _showInMenu;
        private bool _allowComments;
        private DateTime _created;

        private List<ZCMSProperty> _properties = new List<ZCMSProperty>();

        public ZCMSPage(params ZCMSProperty[] properties)
        {
            _created = DateTime.Now;
            foreach (ZCMSProperty item in properties)
                _properties.Add(item);

            Sort();
        }

        public ZCMSPage()
        {
            _created = DateTime.Now;
            if (_properties == null)
                _properties = new List<ZCMSProperty>();
        }

        public ZCMSPage(dynamic bpt)
        {
            _created = DateTime.Now;
            this._properties = bpt.Properties;
            Sort();
        }

        private void Sort()
        {
            this._properties = this.Properties.OrderBy(o => o.Order).ToList();
        }

        public List<ZCMSProperty> Properties
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

        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "StartPublish")]
        [Required(ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationStartPublishRequired")]
        public DateTime? StartPublish { 
            get
            {
                return _startPublish;
            } 
            set
            {
                _startPublish = value;
            }
        }

        [DataType(DataType.Date)]
        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "EndPublish")]        
        public DateTime? EndPublish
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
        [Required(ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageNameRequired")]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(CMS_i18n.BackendResources), ErrorMessageResourceName = "ValidationPageNameLength")]
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

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "ShowInMenus")]
        public bool ShowInMenus
        {
            get
            {
                return _showInMenu;
            }
            set
            {
                _showInMenu = value;
            }
        }

        [Display(ResourceType = typeof(CMS_i18n.BackendResources), Name = "AllowComments")]
        public bool AllowComments
        {
            get
            {
                return _allowComments;
            }
            set
            {
                _allowComments = value;
            }
        }

    }
}