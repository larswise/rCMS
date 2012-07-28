using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;

namespace ZCMS.Core.Business
{
    [ModelBinderType(typeof(ZCMSPage))]
    public class ZCMSPageModelBinder : IModelBinder
    {

        public ZCMSPageModelBinder()
        {
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {

            ZCMSPage model = (ZCMSPage)Activator.CreateInstance(bindingContext.ModelType);
            model.StartPublish = null;
            model.EndPublish = null;

            //var items = bindingContext.ValueProvider.GetValue("Properties").AttemptedValue;
            model.PageName = bindingContext.ValueProvider.GetValue("PageName").AttemptedValue;
            var valMenue = bindingContext.ValueProvider.GetValue("ShowInMenus").AttemptedValue;
            model.ShowInMenus = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("ShowInMenus").AttemptedValue.Split(',')[0]);

            model.AllowComments = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("AllowComments").AttemptedValue.Split(',')[0]);
            try
            {
                model.PageID = Int32.Parse(bindingContext.ValueProvider.GetValue("PageID").AttemptedValue);
            }
            catch
            {
                model.PageID = new Random().Next(100, 100000);
            }
            DateTime spd, epd;
            bool sp = DateTime.TryParse(bindingContext.ValueProvider.GetValue("StartPublish").AttemptedValue, out spd);
            if (sp)
                model.StartPublish = spd;

            bool ep = DateTime.TryParse(bindingContext.ValueProvider.GetValue("EndPublish").AttemptedValue, out epd);
            if (ep)
                model.EndPublish = epd;

            var items2 = bindingContext.ValueProvider.GetValue("Properties[0].PropertyName").AttemptedValue;

            // limiting number of properties to 8 on purpose
            for (int i = 0; i < 8; i++)
            {
                try
                {

                    var type = bindingContext.ValueProvider.GetValue("Properties["+ i +"].PropertyType").AttemptedValue;
                    if (type == "ZCMS.Core.Business.RichTextProperty")
                    {
                        RichTextProperty prop = new RichTextProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        prop.PropertyValue = bindingContext.ValueProvider.GetValue("PropertyValue").AttemptedValue;
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.BooleanProperty")
                    {
                        BooleanProperty prop = new BooleanProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        var val = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue;
                        if(val.Split(',').Length>1)
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue.Split(',')[0]);
                        else
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyValue").AttemptedValue);
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.TagsProperty")
                    {
                        TagsProperty tp = new TagsProperty();
                        tp.PropertyName = bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyName").AttemptedValue;
                        tp.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("Properties[" + i + "].Order").AttemptedValue);
                        tp.PropertyValue = new List<string>();
                        try
                        {
                            string[] tagvalues = bindingContext.ValueProvider.GetValue("PropertyValue.PageTagValues").AttemptedValue.Split(',');
                            List<string> tags = new List<string>();
                            foreach (string s in tagvalues)
                            {
                                if(s!=null && s!=string.Empty)
                                    tags.Add(s);
                            }
                            tp.PropertyValue = tags;

                        }
                        catch
                        {
                        }
                        model.Properties.Add(tp);
                    }
                    else
                    {
                        var propInstance = Activator.CreateInstance(Type.GetType(bindingContext.ValueProvider.GetValue("Properties[" + i + "].PropertyType").AttemptedValue));

                        var propInstanceProperties = propInstance.GetType().GetProperties();

                        foreach (PropertyInfo propInfo in propInstanceProperties)
                        {
                            try
                            {
                                var value = bindingContext.ValueProvider.GetValue("Properties[" + i + "]." + propInfo.Name).AttemptedValue;
                                if (propInfo.PropertyType.FullName == "System.Int32")
                                    propInfo.SetValue(propInstance, Int32.Parse(value));
                                else
                                    propInfo.SetValue(propInstance, value);
                            }
                            catch
                            {
                            }
                        }

                        model.Properties.Add((ZCMSProperty)propInstance);
                    }
                }
                catch
                {
                    break;
                }
            }
            model.Properties.OrderBy(o => o.Order);
            return model;
        }
    }
}