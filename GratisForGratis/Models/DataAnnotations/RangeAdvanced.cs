using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.DataAnnotations
{
    public class RangeIntAdvanced : RangeAttribute, IClientValidatable
    {
        public RangeIntAdvanced(string min, string max)
            : base(int.Parse(WebConfigurationManager.AppSettings[min]), int.Parse(WebConfigurationManager.AppSettings[max]))
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }

    public class RangeDoubleAdvanced : RangeAttribute, IClientValidatable
    {
        public RangeDoubleAdvanced(string min, string max)
            : base(double.Parse(WebConfigurationManager.AppSettings[min]), double.Parse(WebConfigurationManager.AppSettings[max]))
        {
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }

    public class RangeDate : RangeAttribute
    {
        public RangeDate()
          : base(typeof(DateTime),
                  DateTime.MinValue.ToShortDateString(),
                  DateTime.Now.ToShortDateString())
        { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "range"
            };

            rule.ValidationParameters.Add("min", Minimum);
            rule.ValidationParameters.Add("max", Maximum);

            yield return rule;
        }
    }
}
