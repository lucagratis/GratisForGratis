using GratisForGratis.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web;

namespace GratisForGratis.Models
{
    public abstract class EncoderViewModel
    {
        #region ATTRIBUTI
        private string id;
        private string token;
        #endregion

        #region PROPRIETA

        public string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = Encode(value);
            }
        }

        public string Token
        {
            get
            {
                return token;
            }
            set
            {
                token = Encode(value);
            }
        }

        #endregion

        #region METODI PROTETTI

        protected string Encode(string valore)
        {
            return Utils.RandomString(3, false) + Utils.Encode(valore) + Utils.RandomString(3, false);
        }

        protected string Encode(int valore)
        {
            return Utils.RandomString(3, false) + Utils.Encode(valore) + Utils.RandomString(3, false);
        }

        #endregion
    }

    public class VenditaViewModel
    {
        // aggiungere partner venditore
        public string Id { get; set; }
        
        public string Token { get; set; }

        public TipoAcquisto TipoAcquisto { get; set; }

        public UtenteVenditaViewModel Venditore { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<string> Foto { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Display(Name = "Feedback", ResourceType = typeof(App_GlobalResources.Language))]
        public double VenditoreFeedback { get; set; }

    }

    public interface IOggetto
    {
        int VenditaID { get; set; }

        int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        int VenditoreID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        string VenditoreNominativo { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        int NumeroOggetto { get; set; }

        [DataType(DataType.Text)]
        string Token { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        string Nome { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        int? Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        List<string> Foto { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(App_GlobalResources.Language))]
        CondizioneOggetto StatoOggetto { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Brand", ResourceType = typeof(App_GlobalResources.Language))]
        string Marca { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Year", ResourceType = typeof(App_GlobalResources.Language))]
        int? Anno { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "High", ResourceType = typeof(App_GlobalResources.Language))]
        decimal? Altezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width", ResourceType = typeof(App_GlobalResources.Language))]
        decimal? Larghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length", ResourceType = typeof(App_GlobalResources.Language))]
        decimal? Lunghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Weight", ResourceType = typeof(App_GlobalResources.Language))]
        decimal? Peso { get; set; }

        Guid VenditoreToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        DateTime? DataInserimento { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity", ResourceType = typeof(App_GlobalResources.Language))]
        int? Quantità { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Note", ResourceType = typeof(App_GlobalResources.Language))]
        string Note { get; set; }

        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        StatoVendita StatoVendita { get; set; }

        // tolto campo compra
    }

    public class OggettoViewModel : IOggetto
    {
        public OggettoViewModel()
        {
            Offerta = new OffertaOggettoViewModel();
        }

        public OggettoViewModel(OggettoViewModel model)
        {
            Offerta = new OffertaOggettoViewModel();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public int VenditaID { get; set; }

        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public int VenditoreID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        public string VenditoreNominativo { get; set; }

        [Display(Name = "Feedback", ResourceType = typeof(App_GlobalResources.Language))]
        public double VenditoreFeedback { get; set; }

        public int PartnerId { get; set; }

        [DataType(DataType.Text)]
        public Guid PartnerToken { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Partners", ResourceType = typeof(App_GlobalResources.Language))]
        public string PartnerNominativo { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int NumeroOggetto { get; set; }

        [DataType(DataType.Text)]
        public string Token { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<string> Foto { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(App_GlobalResources.Language))]
        public CondizioneOggetto StatoOggetto { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Brand", ResourceType = typeof(App_GlobalResources.Language))]
        public string Marca { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Year", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Anno { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "High", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Altezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Larghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Lunghezza { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Weight", ResourceType = typeof(App_GlobalResources.Language))]
        public decimal? Peso { get; set; }

        public Guid VenditoreToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Quantity", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Quantità { get; set; }

        public string Note { get; set; }

        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }

        public OffertaOggettoViewModel Offerta { get; set; }
    }

    public class ModelloViewModel : OggettoViewModel, IOggetto
    {

        public ModelloViewModel() : base() { }

        public ModelloViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int modelloID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string modelloNome { get; set; }

    }

    public class TelefonoViewModel : ModelloViewModel
    {

        public TelefonoViewModel() : base() { }

        public TelefonoViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int sistemaOperativoID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string sistemaOperativoNome { get; set; }
    }

    public class ComputerViewModel : ModelloViewModel
    {
        public ComputerViewModel() : base() { }

        public ComputerViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int sistemaOperativoID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string sistemaOperativoNome { get; set; }
    }

    public class MusicaViewModel : OggettoViewModel
    {
        public MusicaViewModel() : base() { }

        public MusicaViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int formatoID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string formatoNome { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int artistaID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string artistaNome { get; set; }
    }

    public class VideogamesViewModel : OggettoViewModel
    {
        public VideogamesViewModel() : base() { }

        public VideogamesViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int piattaformaID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string piattaformaNome { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int genereID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string genereNome { get; set; }
    }

    public class ConsoleViewModel : OggettoViewModel
    {
        public ConsoleViewModel() : base() { }

        public ConsoleViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int piattaformaID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string piattaformaNome { get; set; }
    }

    public class VideoViewModel : OggettoViewModel
    {
        public VideoViewModel() : base() { }

        public VideoViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int formatoID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Format", ResourceType = typeof(App_GlobalResources.Language))]
        public string formatoNome { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int registaID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Director", ResourceType = typeof(App_GlobalResources.Language))]
        public string registaNome { get; set; }
    }

    public class LibroViewModel : OggettoViewModel
    {
        public LibroViewModel() : base() { }

        public LibroViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int autoreID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string autoreNome { get; set; }
    }

    public class VeicoloViewModel : ModelloViewModel
    {
        public VeicoloViewModel() : base() { }

        public VeicoloViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [Range(1, int.MaxValue)]
        public int alimentazioneID { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Power", ResourceType = typeof(App_GlobalResources.Language))]
        public string alimentazioneNome { get; set; }
    }

    public class VestitoViewModel : OggettoViewModel
    {
        public VestitoViewModel() : base() { }

        public VestitoViewModel(OggettoViewModel model) : base(model) { }

        [Required]
        [DataType(DataType.Text)]
        public string taglia { get; set; }
    }

    public class BarattoOggettoViewModel : PubblicaOggettoViewModel
    {
        public HttpPostedFileBase[] File
        {
            get;
            set;
        }

        public BarattoOggettoViewModel()
        {
        }
    }

    public class BarattoServizioViewModel : PubblicaOggettoViewModel
    {
        public HttpPostedFileBase[] File
        {
            get;
            set;
        }

        public BarattoServizioViewModel()
        {
        }
    }

    public interface IServizio
    {
        int VenditaID { get; set; }

        int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        int VenditoreID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        string VenditoreNominativo { get; set; }

        [DataType(DataType.Text)]
        string Token { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        string Nome { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        int? Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        List<string> Foto { get; set; }

        [Display(Name = "Monday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Lunedi { get; set; }

        [Display(Name = "Tuesday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Martedi { get; set; }

        [Display(Name = "Wednesday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Mercoledi { get; set; }

        [Display(Name = "Thursday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Giovedi { get; set; }

        [Display(Name = "Friday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Venerdi { get; set; }

        [Display(Name = "Saturday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Sabato { get; set; }

        [Display(Name = "Sunday", ResourceType = typeof(App_GlobalResources.Language))]
        bool Domenica { get; set; }

        [Display(Name = "AllDays", ResourceType = typeof(App_GlobalResources.Language))]
        bool Tutti { get; set; }

        [Display(Name = "Holidays", ResourceType = typeof(App_GlobalResources.Language))]
        bool Festivita { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        TimeSpan OraInizio { get; set; }

        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        TimeSpan OraFine { get; set; }

        Guid VenditoreToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        DateTime? DataInserimento { get; set; }

        string Compra { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Note", ResourceType = typeof(App_GlobalResources.Language))]
        string Note { get; set; }

        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        StatoVendita StatoVendita { get; set; }
    }

    public class ServizioViewModel : IServizio
    {
        public ServizioViewModel()
        {
            Offerta = new OffertaServizioViewModel();
        }

        public ServizioViewModel(ServizioViewModel model)
        {
            Offerta = new OffertaServizioViewModel();
            foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
            {
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }

        public int VenditaID { get; set; }

        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Seller", ResourceType = typeof(App_GlobalResources.Language))]
        public int VenditoreID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Nominative", ResourceType = typeof(App_GlobalResources.Language))]
        public string VenditoreNominativo { get; set; }

        [Display(Name = "Feedback", ResourceType = typeof(App_GlobalResources.Language))]
        public double VenditoreFeedback { get; set; }

        public int PartnerId { get; set; }

        [DataType(DataType.Text)]
        public Guid PartnerToken { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Partners", ResourceType = typeof(App_GlobalResources.Language))]
        public string PartnerNominativo { get; set; }

        [DataType(DataType.Text)]
        public string Token { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name", ResourceType = typeof(App_GlobalResources.Language))]
        public string Nome { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(App_GlobalResources.Language))]
        public TipoPagamento TipoPagamento { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public int CategoriaID { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(App_GlobalResources.Language))]
        public string Categoria { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "City", ResourceType = typeof(App_GlobalResources.Language))]
        public string Citta { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Punti { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Money", ResourceType = typeof(App_GlobalResources.Language))]
        public int? Soldi { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Phote", ResourceType = typeof(App_GlobalResources.Language))]
        public List<string> Foto { get; set; }

        public Guid VenditoreToken { get; set; }

        [DataType(DataType.DateTime, ErrorMessageResourceName = "ErrorDate", ErrorMessageResourceType = typeof(App_GlobalResources.Language))]
        public DateTime? DataInserimento { get; set; }

        public string Compra { get; set; }

        public string Note { get; set; }

        [Display(Name = "Monday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Lunedi { get; set; }

        [Display(Name = "Tuesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Martedi { get; set; }

        [Display(Name = "Wednesday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Mercoledi { get; set; }

        [Display(Name = "Thursday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Giovedi { get; set; }

        [Display(Name = "Friday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Venerdi { get; set; }

        [Display(Name = "Saturday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Sabato { get; set; }

        [Display(Name = "Sunday", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Domenica { get; set; }

        [Display(Name = "AllDays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Tutti { get; set; }

        [Display(Name = "Holidays", ResourceType = typeof(App_GlobalResources.Language))]
        public bool Festivita { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan OraInizio { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan OraFine { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraInizioFestivita { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(App_GlobalResources.Language))]
        public TimeSpan? OraFineFestivita { get; set; }

        [Display(Name = "Bid", ResourceType = typeof(App_GlobalResources.Language))]
        public string ServiziOfferti { get; set; }

        [Display(Name = "Results", ResourceType = typeof(App_GlobalResources.Language))]
        public string RisultatiFinali { get; set; }

        [Display(Name = "Rate", ResourceType = typeof(App_GlobalResources.Language))]
        public Tariffa Tariffa { get; set; }

        public OffertaServizioViewModel Offerta { get; set; }

        [Display(Name = "StateSelling", ResourceType = typeof(App_GlobalResources.Language))]
        public StatoVendita StatoVendita { get; set; }
    }
}
