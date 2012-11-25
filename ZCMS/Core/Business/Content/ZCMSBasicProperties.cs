using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation;

namespace ZCMS.Core.Business.Content
{
    public interface IZCMSProperty
    {
        int Order { get; set; }
        object PropertyValue { get; set; }
        string PropertyName { get; set; }
        string PropertyType { get; }
        string PropertyValidator { get; set; }
        Tab DisplayInTab { get; set; }
        DisplayType DisplayType { get; set; }
    }

    public class BooleanProperty : IZCMSProperty
    {
        public BooleanProperty() { }
        private object _isTrue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;


        public object PropertyValue 
        {
            get
            {
                return _isTrue;
            }
            set
            {
                _isTrue = value;
            }
        }

        public string PropertyName 
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }

    }

    public class TagsProperty : IZCMSProperty
    {
        public TagsProperty() { }
        private List<string> _tags;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        [UIHint("TagsProperty")]
        public object PropertyValue
        {
            get
            {
                return _tags;
            }
            set
            {
                if (value is Raven.Abstractions.Linq.DynamicList)
                {
                    _tags = new List<string>();
                    foreach (var item in (Raven.Abstractions.Linq.DynamicList)value)
                    {
                        _tags.Add(item.ToString());
                    }
                }
                else
                {
                    _tags = (List<string>)value;
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }

    public class DateProperty : IZCMSProperty
    {
        private DateTime _textValue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public DateProperty() { }

        [DataType(DataType.Date)]
        public object PropertyValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                _textValue = Convert.ToDateTime(value);
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }

    public class RichTextProperty : IZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public RichTextProperty() { } 

        [UIHint("tinymce_full"), AllowHtml]
        public object PropertyValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                _textValue = value;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }

    public class TextProperty : IZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public TextProperty() { }


        public object PropertyValue
        {
            get 
            {
                return _textValue;
            }
            set
            {
                _textValue = value;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }

    public class DisplayOnlyTextProperty : IZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public DisplayOnlyTextProperty() { }


        public object PropertyValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                _textValue = value;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }

    public class MultiLineTextProperty : IZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public MultiLineTextProperty() { }


        [DataType(DataType.MultilineText)]
        public object PropertyValue
        {
            get
            {
                return _textValue;
            }
            set
            {
                _textValue = value;
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator
        {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }
    
    public class ImageListProperty : IZCMSProperty
    {        
        private string _propertyName;
        private int? _order;
        private List<string> _imageVirtualPaths = new List<string>();
        private string _validator;
        private Tab _displayInTab;
        private DisplayType _displayType;

        public ImageListProperty() { }

        [UIHint("ImageListProperty")]
        public object PropertyValue 
        {
            get
            {
                return _imageVirtualPaths;
            }
            set
            {
                if (value is Raven.Abstractions.Linq.DynamicList)
                {
                    _imageVirtualPaths = new List<string>();
                    foreach (var item in (Raven.Abstractions.Linq.DynamicList)value)
                    {
                        _imageVirtualPaths.Add(item.ToString());
                    }
                }
                else
                {
                    _imageVirtualPaths = (List<string>)value;
                }
            }
        }

        public string PropertyName
        {
            get
            {
                return _propertyName;
            }
            set
            {
                _propertyName = value;
            }
        }

        public int Order
        {
            get
            {
                return _order.HasValue ? _order.Value : 0;
            }
            set
            {
                _order = value;
            }
        }

        public string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

        public string PropertyValidator {
            get
            {
                return _validator;
            }
            set
            {
                _validator = value;
            }
        }

        public Tab DisplayInTab
        {
            get
            {
                return _displayInTab;
            }
            set
            {
                _displayInTab = value;
            }
        }

        public DisplayType DisplayType
        {
            get
            {
                return _displayType;
            }
            set
            {
                _displayType = value;
            }
        }
    }
}