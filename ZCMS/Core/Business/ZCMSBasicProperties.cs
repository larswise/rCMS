using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZCMS.Core.Business
{
    public abstract class ZCMSProperty
    {
        public abstract int Order { get; set; }
        public abstract object PropertyValue { get; set; }
        public abstract string PropertyName { get; set; }
        public abstract string PropertyType { get; }
    }

    public class BooleanProperty : ZCMSProperty
    {
        public BooleanProperty() { }
        private object _isTrue;
        private string _propertyName;
        private int? _order;

        public override object PropertyValue 
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

        public override string PropertyName 
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

    }

    public class TagsProperty : ZCMSProperty
    {
        public TagsProperty() { }
        private List<string> _tags;
        private string _propertyName;
        private int? _order;

        [UIHint("TagsProperty")]
        public override object PropertyValue
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

        public override string PropertyName
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }

    }

    public class RichTextProperty : ZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;

        public RichTextProperty() { } 

        [UIHint("tinymce_full"), AllowHtml]
        public override object PropertyValue
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

        public override string PropertyName
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }

    public class TextProperty : ZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;

        public TextProperty() { }


        public override object PropertyValue
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

        public override string PropertyName
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }

    public class MultiLineTextProperty : ZCMSProperty
    {
        private object _textValue;
        private string _propertyName;
        private int? _order;

        public MultiLineTextProperty() { }


        [DataType(DataType.MultilineText)]
        public override object PropertyValue
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

        public override string PropertyName
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }
    
    public class ImageListProperty : ZCMSProperty
    {        
        private string _propertyName;
        private int? _order;
        private List<string> _imageVirtualPaths = new List<string>();

        public ImageListProperty() { }

        [UIHint("ImageListProperty")]
        public override object PropertyValue 
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

        public override string PropertyName
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

        public override int Order
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

        public override string PropertyType
        {
            get
            {
                return this.GetType().FullName;
            }
        }
    }
}