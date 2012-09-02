using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation;
using ZCMS.Core.Business.Content;

namespace ZCMS.Core.Business.Validators
{
    public class TextPropertyNotNullEmptyValidator : AbstractValidator<IZCMSProperty>
    {
        public TextPropertyNotNullEmptyValidator(string propertyType)
        {
            RuleFor(x => x.PropertyValue).NotNull().NotEmpty().WithMessage(CMS_i18n.BackendResources.ValidationPagePropertyRequired).OverridePropertyName(propertyType);

        }
    }

    public class TextPropertyStringLengthNotNullEmptyValidator : AbstractValidator<IZCMSProperty>
    {
        public TextPropertyStringLengthNotNullEmptyValidator(string propertyType)
        {
            int from = 3; int to = 50;
            RuleFor(x => x.PropertyValue).NotNull().NotEmpty().WithMessage(String.Format(CMS_i18n.BackendResources.ValidationPagePropertyRequiredAndLength, from, to)).OverridePropertyName(propertyType);

        }
    }

    public class TextStringLengthNotNullEmptyValidator : AbstractValidator<string>
    {
        public TextStringLengthNotNullEmptyValidator(string propertyName)
        {
            int from = 3; int to = 50;
            RuleFor(x => x).NotNull().NotEmpty().WithMessage(String.Format(CMS_i18n.BackendResources.ValidationPagePropertyRequiredAndLength, from, to)).OverridePropertyName(propertyName);

        }
    }

    public class DateTimeValidator : AbstractValidator<string>
    {
        public DateTimeValidator(bool required, string shortDisplayName)
        {
            if (required)
            {
                RuleFor(x => x)
                    .Must(BeAValidDate)
                    .WithMessage(CMS_i18n.BackendResources.ValidationPageDateTime)
                    .Must(RequiredDate)
                    .WithMessage(CMS_i18n.BackendResources.ValidationPagePropertyRequired)
                    .OverridePropertyName(shortDisplayName);
            }
            else
                RuleFor(x => x).NotEmpty().WithMessage(CMS_i18n.BackendResources.ValidationPageDateTime).OverridePropertyName(shortDisplayName);
        }

        private bool BeAValidDate(string value)
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt))
                return true;
            return false;
        }

        private bool RequiredDate(string value)
        {
            DateTime dt;
            if (DateTime.TryParse(value, out dt) && dt >= DateTime.MinValue)
                return true;
            else
                return false;
        }
    }



    public class DatePropertyNotEmptyValidator : AbstractValidator<IZCMSProperty>
    {
        public DatePropertyNotEmptyValidator(string propertyType)
        {
            RuleFor(x => Convert.ToDateTime(x.PropertyValue)).GreaterThan(DateTime.MinValue).WithMessage(CMS_i18n.BackendResources.ValidationPagePropertyRequired).OverridePropertyName(propertyType);

        }
    }



    public class ZCMSModelValidatorProvider : ModelValidatorProvider
    {
        private List<string> validateProperties = new List<string>() { "Name", "Start Publish", "End Publish" };
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if (metadata.Model != null)
            {
                if (metadata.Model is List<IZCMSProperty>)
                {
                    foreach (var modelItem in (List<IZCMSProperty>)metadata.Model)
                    {
                        yield return new ValidationAdapter(metadata, modelItem, context);
                    }
                }

                if (metadata.Model.GetType() == typeof(DateTime) || (!String.IsNullOrEmpty(metadata.DisplayName) && metadata.DisplayName == "Name"))
                {
                    yield return new ValidationAdapter(metadata, context);
                }
            }
        }
    }

    sealed class ValidationAdapter : ModelValidator 
    {
        private IZCMSProperty _currentProperty;

        internal ValidationAdapter(ModelMetadata metadata, IZCMSProperty currentProperty, ControllerContext controllerContext)
            : base(metadata, controllerContext) 
        {
                _currentProperty = currentProperty;
        }

        internal ValidationAdapter(ModelMetadata metadata, ControllerContext controllerContext)
            : base(metadata, controllerContext)
        {
        }

        public override IEnumerable<ModelValidationResult> Validate(object container) 
        {
            if (Metadata.Model != null && _currentProperty!=null && !String.IsNullOrEmpty(_currentProperty.PropertyValidator)) 
            {

                FluentValidation.Results.ValidationResult result = 
                    ((IValidator)Activator.CreateInstance(Type.GetType(_currentProperty.PropertyValidator), _currentProperty.PropertyType)).Validate(_currentProperty);
                
                return result.Errors.Select(fault => new ModelValidationResult
                {
                    MemberName = fault.PropertyName,
                    Message = fault.ErrorMessage
                });
                
            }

            if (Metadata.Model != null)
            {
                FluentValidation.Results.ValidationResult result;
                if (Metadata.Model.GetType() == typeof(string) && Metadata.IsRequired && Metadata.AdditionalValues.Count > 0)
                    result = new TextStringLengthNotNullEmptyValidator(Metadata.AdditionalValues["PropName"].ToString()).Validate(Metadata.Model.ToString());
                else if (Metadata.Model.GetType() == typeof(DateTime))
                {
                    if (Metadata.AdditionalValues.Count > 0 && Metadata.AdditionalValues["PropName"] != null)
                        result = new DateTimeValidator(Metadata.IsRequired, Metadata.AdditionalValues["PropName"].ToString()).Validate(Metadata.Model.ToString());
                    else
                        result = new FluentValidation.Results.ValidationResult();
                }
                else
                    result = new FluentValidation.Results.ValidationResult();
                return result.Errors.Select(fault => new ModelValidationResult
                {
                    MemberName = fault.PropertyName,
                    Message = fault.ErrorMessage
                });
            }
            else
            {

                return Enumerable.Empty<ModelValidationResult>();
            }
        }
    }
}