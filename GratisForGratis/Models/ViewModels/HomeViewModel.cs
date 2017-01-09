using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    public class SegnalazioneViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "AnswerEmail", ResourceType = typeof(App_GlobalResources.Language))]
        public string EmailRisposta { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "MailObject", ResourceType = typeof(App_GlobalResources.Language))]
        public string Oggetto { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(4000, MinimumLength = 10)]
        [Display(Name = "Text", ResourceType = typeof(App_GlobalResources.Language))]
        public string Testo { get; set; }

        [DataType(DataType.Upload)]
        [Display(Name = "Attachment", ResourceType = typeof(App_GlobalResources.Language))]
        public System.Web.HttpPostedFileBase Allegato { get; set; }

        [Required]
        public string Controller { get; set; }

        [Required]
        public string Vista { get; set; }

        public string MacAddress { get; set; }

        [Required]
        public TipoSegnalazione Tipologia { get; set; }
    }
}
