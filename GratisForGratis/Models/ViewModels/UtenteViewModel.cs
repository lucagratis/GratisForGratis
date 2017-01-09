using GratisForGratis.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GratisForGratis.Models
{
    public class UtenteLoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150,MinimumLength =6)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "StoresUser", ResourceType = typeof(App_GlobalResources.Language))]
        public bool RicordaLogin { get; set; }

        public string ReturnUrl { get; set; }

        public string ErroreLogin;
    }

    public class UtenteRegistrazioneViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(App_GlobalResources.Language))]
        [Compare("Password", ErrorMessageResourceName = "ErrorComparePassword", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string ConfermaPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Surname", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cognome { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessageResourceName = "ErrorTermsAndConditions", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }

    }

    public class UtentePasswordDimenticataViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName = "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string NuovaPassword { get; set; }
    }

    public class UtenteImpostazioniViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorFormatEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [StringLength(200, ErrorMessageResourceName =  "ErrorLengthEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Surname", ResourceType = typeof(App_GlobalResources.Language))]
        public string Cognome { get; set; }

        [RequiredIfNotNull(new string[] { "Civico", "Indirizzo" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ResidenceCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        public int? IDCitta { get; set; }

        [RequiredIfNotNull(new string[] { "Citta", "Civico" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ResidenceAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string Indirizzo { get; set; }

        [RequiredIfNotNull(new string[] { "Citta", "Indirizzo" }, ErrorMessageResourceName = "ErrorRequiredField", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        //[DataType(DataType.Text)]
        //[StringLength(50, MinimumLength = 1)]
        [Display(Name = "Civic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Civico { get; set; }

        [DataType(DataType.PostalCode, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "ShippingCity", ResourceType = typeof(App_GlobalResources.Language))]
        public string CittaSpedizione { get; set; }

        public int? IDCittaSpedizione { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 5)]
        [Display(Name = "ShippingAddress", ResourceType = typeof(App_GlobalResources.Language))]
        public string IndirizzoSpedizione { get; set; }

        //[DataType(DataType.Text)]
        //[StringLength(50, MinimumLength = 1)]
        [Display(Name = "ShippingCivic", ResourceType = typeof(App_GlobalResources.Language))]
        public int? CivicoSpedizione { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(12, MinimumLength = 9, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Display(Name = "AcceptConditions", ResourceType = typeof(App_GlobalResources.Language))]
        public bool AccettaCondizioni { get; set; }
    }

    public class UtenteCambioPasswordViewModel
    {

        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(App_GlobalResources.Language))]
        [Compare("Password", ErrorMessageResourceName = "ErrorComparePassword", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public string ConfermaPassword { get; set; }

    }

    public class RegistrazioneEmailModel : UtenteRegistrazioneViewModel
    {
        public RegistrazioneEmailModel(UtenteRegistrazioneViewModel model)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public string PasswordCodificata { get; set; }
    }

    public class UtenteVenditaViewModel
    {
        public UtenteVenditaViewModel() { }
        public UtenteVenditaViewModel(PERSONA model)
        {
            foreach (System.Reflection.PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                // verifico l'esistenza della proprietà
                if (this.GetType().GetProperty(propertyInfo.Name.First().ToString() + propertyInfo.Name.ToLower().Substring(1)) != null)
                    this.GetType().GetProperty(propertyInfo.Name.First().ToString() + propertyInfo.Name.ToLower().Substring(1)).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        //[DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nominativo { get; set; }

        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Email", ResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        public Guid VenditoreToken { get; set; }
    }

    public class UtenteRicercaViewModel : EncoderViewModel
    {
        #region COSTRUTTORI
        public UtenteRicercaViewModel() { }

        public UtenteRicercaViewModel(PERSONA_RICERCA model)
        {
            List<FINDSOTTOCATEGORIE_Result> categorie = (HttpContext.Current.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
            FINDSOTTOCATEGORIE_Result categoria = categorie.SingleOrDefault(item => item.ID == model.ID_CATEGORIA);
            this.Id = model.ID.ToString();
            this.Categoria = categoria.NOME;
            this.Testo = model.NOME;
            this.DataInserimento = model.DATA_INSERIMENTO;
            this.DataModifica = (DateTime)model.DATA_MODIFICA;
            this.Stato = (Stato)model.STATO;
        }
        #endregion

        #region PROPRIETA
        public string Testo { get; set; }

        public string Categoria { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime DataModifica { get; set; }

        public Stato Stato { get; set; }
        #endregion
    }

}
