using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ZCMS.Core.Business
{
    
    public class ZCMSMenu
    {
        private List<ZCMSMenuItem> _menuItems;
        private string _menuName;
        public ZCMSMenu()
        {
            _menuItems = new List<ZCMSMenuItem>();
        }

        public List<ZCMSMenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value.ToList();
            }
        }

        public string MenuName
        {
            get
            {
                return _menuName;
            }
            set
            {
                _menuName = value;
            }
        }
    }

    public class ZCMSMenuItem
    {
        public string ItemName { get; set; }
        public string ItemAction { get; set; }
    }
}