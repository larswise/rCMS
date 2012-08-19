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
        private string _menuController;
        private string _menuArea;
        private int _prio;

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

        public int Priority
        {
            get
            {
                return _prio;
            }
            set
            {
                _prio = value;
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

        public string MenuController
        {
            get
            {
                return _menuController;
            }
            set
            {
                _menuController = value;
            }
        }

        public string MenuArea
        {
            get
            {
                return _menuArea;
            }
            set
            {
                _menuArea = value;
            }
        }

    }

    public class ZCMSMenuItem
    {
        public string ItemName { get; set; }
        public string ItemDisplay { get; set; }
        public string ItemAction { get; set; }        
    }
}