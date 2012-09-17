using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using ZCMS.Core.Business.Content;

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
            model.PageName = bindingContext.ValueProvider.GetValue("Instance.PageName").AttemptedValue;
            model.PageType = bindingContext.ValueProvider.GetValue("PageType").AttemptedValue;
            try
            {
                model.PageID = Int32.Parse(bindingContext.ValueProvider.GetValue("PageID").AttemptedValue);
            }
            catch
            {
                model.PageID = new Random().Next(100, 100000);
            }

            DateTime start, end;

            if (DateTime.TryParse(bindingContext.ValueProvider.GetValue("Instance.StartPublish").AttemptedValue, out start))
                model.StartPublish = start;
            else
                model.StartPublish = DateTime.MinValue;

            if (DateTime.TryParse(bindingContext.ValueProvider.GetValue("Instance.EndPublish").AttemptedValue, out end))
                model.EndPublish = end;
            else 
                model.EndPublish = DateTime.MinValue;
            
            if (bindingContext.ValueProvider.GetValue("save-draft") != null)
                model.Status = PageStatus.Draft;
            else
                model.Status = PageStatus.Published;

            // limiting number of properties to 8 on purpose
            for (int i = 0; i < 12; i++)
            {
                try
                {

                    var type = bindingContext.ValueProvider.GetValue("["+ i +"].PropertyType").AttemptedValue;
                    if (type == "ZCMS.Core.Business.Content.RichTextProperty")
                    {
                        RichTextProperty prop = new RichTextProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyName").AttemptedValue;
                        prop.PropertyValue = bindingContext.ValueProvider.GetValue("PropertyValue").AttemptedValue;
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("[" + i + "].Order").AttemptedValue);
                        prop.DisplayInTab = (Tab)Enum.Parse(typeof(Tab), bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue);

                        if (bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValidator")!=null)
                            prop.PropertyValidator = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValidator").AttemptedValue;
                        else
                            prop.PropertyValidator = string.Empty;
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.BooleanProperty")
                    {
                        BooleanProperty prop = new BooleanProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyName").AttemptedValue;
                        var val = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue;
                        if(val.Split(',').Length>1)
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue.Split(',')[0]);
                        else
                            prop.PropertyValue = Convert.ToBoolean(bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue);
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("[" + i + "].Order").AttemptedValue);
                        prop.DisplayInTab = (Tab)Enum.Parse(typeof(Tab), bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.DisplayOnlyTextProperty")
                    {
                        DisplayOnlyTextProperty prop = new DisplayOnlyTextProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyName").AttemptedValue;
                        prop.PropertyValue = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue;
                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("[" + i + "].Order").AttemptedValue);

                        var test = bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue;
                        prop.DisplayInTab = (Tab)Enum.Parse(typeof(Tab), bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue);
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.DateProperty")
                    {
                        DateProperty prop = new DateProperty();
                        prop.PropertyName = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyName").AttemptedValue;
                        string val = string.Empty;
                        if(!String.IsNullOrEmpty(bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue))
                            prop.PropertyValue = Convert.ToDateTime(bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValue").AttemptedValue);
                        else 
                            prop.PropertyValue = null;

                        prop.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("[" + i + "].Order").AttemptedValue);
                        prop.DisplayInTab = (Tab)Enum.Parse(typeof(Tab), bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue);
                        if (bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValidator")!=null)
                            prop.PropertyValidator = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyValidator").AttemptedValue;
                        else
                            prop.PropertyValidator = string.Empty;
                        model.Properties.Add(prop);
                    }
                    else if (type == "ZCMS.Core.Business.Content.TagsProperty")
                    {
                        TagsProperty tp = new TagsProperty();
                        tp.PropertyName = bindingContext.ValueProvider.GetValue("[" + i + "].PropertyName").AttemptedValue;
                        tp.Order = Int32.Parse(bindingContext.ValueProvider.GetValue("[" + i + "].Order").AttemptedValue);
                        tp.DisplayInTab = (Tab)Enum.Parse(typeof(Tab), bindingContext.ValueProvider.GetValue("[" + i + "].DisplayInTab").AttemptedValue);
                        tp.PropertyValue = new List<string>();
                        try
                        {
                            string[] tagvalues = bindingContext.ValueProvider.GetValue("PropertyValue.PageTagValues").AttemptedValue.Split(',');
                            List<string> tags = new List<string>();
                            foreach (string s in tagvalues)
                            {
                                if (s != null && s != string.Empty)
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
                        var propInstance = Activator.CreateInstance(Type.GetType(bindingContext.ValueProvider.GetValue("[" + i + "].PropertyType").AttemptedValue));

                        var propInstanceProperties = propInstance.GetType().GetProperties();

                        foreach (PropertyInfo propInfo in propInstanceProperties)
                        {
                            try
                            {
                                var value = bindingContext.ValueProvider.GetValue("[" + i + "]." + propInfo.Name).AttemptedValue;
                                if (propInfo.PropertyType.FullName == "System.Int32")
                                    propInfo.SetValue(propInstance, Int32.Parse(value));
                                else if (propInfo.PropertyType.FullName == "FluentValidation.IValidator")
                                    propInfo.SetValue(propInstance, Activator.CreateInstance(Type.GetType(value)));
                                else if (propInfo.PropertyType.FullName == "ZCMS.Core.Business.Tab")
                                {
                                    propInfo.SetValue(propInstance, (Tab)Enum.Parse(typeof(Tab), value, true));
                                }
                                else
                                    propInfo.SetValue(propInstance, value);
                            }
                            catch
                            {
                            }
                        }

                        model.Properties.Add((IZCMSProperty)propInstance);
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