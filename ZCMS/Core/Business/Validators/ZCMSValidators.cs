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



    public class DatePropertyNotEmptyValidator : AbstractValidator<IZCMSProperty>
    {
        public DatePropertyNotEmptyValidator(string propertyType)
        {
            RuleFor(x => Convert.ToDateTime(x.PropertyValue)).GreaterThan(DateTime.MinValue).WithMessage(CMS_i18n.BackendResources.ValidationPagePropertyRequired).OverridePropertyName(propertyType);

        }
    }



    public class ZCMSModelValidatorProvider : ModelValidatorProvider
    {
        public override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context)
        {
            if(metadata.Model is List<IZCMSProperty>)
            {
                foreach (var modelItem in (List<IZCMSProperty>)metadata.Model)
                {
                    yield return new ValidationAdapter(metadata, modelItem, context);
                }
            }

            if (!String.IsNullOrEmpty(metadata.DisplayName) && metadata.DisplayName.Equals("Name"))
            {
                yield return new ValidationAdapter(metadata, context);
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
                FluentValidation.Results.ValidationResult result = new TextStringLengthNotNullEmptyValidator("PageName").Validate(Metadata.Model.ToString());

                return result.Errors.Select(fault => new ModelValidationResult
                {
                    MemberName = fault.PropertyName,
                    Message = fault.ErrorMessage
                });
            }

            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}