using DotNetShipping;
using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    #region CLASSI PUBBLICHE

    public class OffertaOggettoViewModel
    {
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Display(Name = "TypeNegotation", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoTrattativa TipoTrattativa { get; set; }

        [Required]
        [Display(Name = "TypeBid", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoOfferta TipoOfferta { get; set; }

        [RequiredIf("TipoOfferta", TipoPagamento.Baratto, ErrorMessageResourceName = "ErrorBid", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Range(0, int.MaxValue)]
        [Display(Name = "OfferPoints", ResourceType = typeof(App_GlobalResources.Language))]
        public int? PuntiOfferti { get; set; }

        [RequiredIf("TipoOfferta", TipoOfferta.Baratto,ErrorMessageResourceName = "ErrorBid", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Barters", ResourceType = typeof(App_GlobalResources.Language))]
        [MinLength(1,ErrorMessageResourceName = "ErrorMinBarters", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public Guid[] OggettiBarattati { get; set; }
    }

    public class OffertaServizioViewModel
    {

        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Display(Name = "TypeNegotation", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoTrattativa TipoTrattativa { get; set; }

        [Required]
        [Display(Name = "TypeBid", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoOfferta TipoOfferta { get; set; }

        [RequiredIf("TipoOfferta", TipoPagamento.Baratto, ErrorMessageResourceName = "ErrorBid", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Range(0, int.MaxValue)]
        [Display(Name = "OfferPoints", ResourceType = typeof(App_GlobalResources.Language))]
        public int? PuntiOfferti { get; set; }

        [RequiredIf("TipoOfferta", TipoOfferta.Baratto, ErrorMessageResourceName = "ErrorBid", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Barters", ResourceType = typeof(App_GlobalResources.Language))]
        [MinLength(1, ErrorMessageResourceName = "ErrorMinBarters", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public Guid[] ServiziBarattati { get; set; }
    }

    public class OffertaEffettuataViewModel : EncoderViewModel
    {

        [Required]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        [Required]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public int IDCategoria { get; set; }

        [Required]
        [Display(Name = "Saller", ResourceType = typeof(App_GlobalResources.Language))]
        public string Venditore { get; set; }

        public Guid VenditoreToken { get; set; }

        [Required]
        [DataType(DataType.EmailAddress, ErrorMessageResourceName = "ErrorEmail", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Email", ResourceType = typeof(App_GlobalResources.Language))]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceName = "ErrorPhoneNumber", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        [Display(Name = "Telephone", ResourceType = typeof(App_GlobalResources.Language))]
        public string Telefono { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int Punti { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int Soldi { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<string> Foto { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int NumeroOggetto { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity", ResourceType = typeof(App_GlobalResources.Language))]
        public int NumeroPezziOfferti { get; set; }

        [Required]
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Required]
        [Display(Name = "OfferKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoOfferta TipoOfferta { get; set; }

        [Required]
        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        [Display(Name = "TypeNegotation", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoTrattativa TipoTrattativa { get; set; }

        [Required]
        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        [Required]
        [Display(Name = "StateBid", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoOfferta StatoOfferta { get; set; }

        [Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime DataInserimento { get; set; }

        public List<VenditaViewModel> Baratti { get; set; }

        //public int PuntiCompratore { get; set; }

        //public int PuntiSospesiCompratore { get; set; }

    }

    public class OffertaRicevutaViewModel : EncoderViewModel
    {
        public int IdInt
        {
            set
            {
                Id = Encode(value);
            }
        }

        [Required]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        public List<OfferteVenditaViewModel> offerte { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int Punti { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int Soldi { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Date", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<string> Foto { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int NumeroOggetto { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity", ResourceType = typeof(App_GlobalResources.Language))]
        public int NumeroPezziOfferti { get; set; }

        [Required]
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Required]
        [Display(Name = "OfferKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        [Display(Name = "ShippingKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoTrattativa TipoInvio { get; set; }

        [Required]
        [Display(Name = "TypeNegotation", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoTrattativa TipoTrattativa { get; set; }

        [Required]
        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        public int PuntiCompratore { get; set; }

        public int PuntiSospesiCompratore { get; set; }
    }

    public class OfferteVenditaViewModel : EncoderViewModel
    {
        public OfferteVenditaViewModel()
        {
            Offerte = new List<OffertaViewModel>();
        }

        public int IdInt
        {
            set
            {
                Id = Encode(value);
            }
        }

        [Required]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string NomeVendita { get; set; }

        [Required]
        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        [Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Required]
        [Display(Name = "Offers", ResourceType = typeof(App_GlobalResources.Language))]
        public List<OffertaViewModel> Offerte { get; set; }
    }

    public class OffertaViewModel : EncoderViewModel
    {
        public OffertaViewModel()
        {
            Baratti = new List<VenditaViewModel>();
        }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int Punti { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int Soldi { get; set; }

        [Required]
        [Display(Name = "Shipment", ResourceType = typeof(App_GlobalResources.Language))]
        public Shipment Spedizione { get; set; }

        [Required]
        [Display(Name = "OfferKind", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoOfferta TipoOfferta { get; set; }

        [Required]
        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        [Display(Name = "InsertDate", ResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Required]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public UtenteVenditaViewModel Compratore { get; set; }

        [Required]
        [Display(Name = "StateBid", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoOfferta StatoOfferta { get; set; }

        public List<VenditaViewModel> Baratti { get; set; }
    }

    #endregion
}
