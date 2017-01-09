using GratisForGratis.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GratisForGratis.Models
{
    public interface IPubblicaOggetto { }
    public abstract class PubblicaViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "Category", ResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorRequiredCategory", ErrorMessageResourceType = typeof(Language))]
        public int CategoriaId { get; set; }

        public string CategoriaNome { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public string Citta { get; set; }

        [DataType(DataType.Text)]
        [Required]
        public List<string> Foto { get; set; }

        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        [Required(ErrorMessageResourceName = "ErrorCity", ErrorMessageResourceType = typeof(Language))]
        public int IDCitta { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "NameObject", ResourceType = typeof(Language))]
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public virtual string Nome { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "OptionalNote", ResourceType = typeof(Language))]
        [StringLength(2000, MinimumLength = 5, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public string NoteAggiuntive { get; set; }

        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorPoints", ErrorMessageResourceType = typeof(Language))]
        [Required]
        public int Punti { get; set; }

        [Display(Name = "WebAddress", ResourceType = typeof(Language))]
        [Url(ErrorMessageResourceName = "ErrorAddress", ErrorMessageResourceType = typeof(Language))]
        public string SchedaProdotto { get; set; }

        [DataType(DataType.Currency, ErrorMessageResourceName = "ErrorMoney", ErrorMessageResourceType = typeof(Language))]
        public int? Soldi { get; set; }

        [Display(Name = "PaymentMethods", ResourceType = typeof(Language))]
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorTypePayment", ErrorMessageResourceType = typeof(Language))]
        [Required]
        public TipoPagamento TipoPagamento { get; set; }

        [Required]
        public TipoAcquisto TipoPubblicazione { get; set; }

        public TipoTrattativa? TipoTrattativa { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Token", ResourceType = typeof(Language))]
        [StringLength(100, MinimumLength = 16, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public string TokenOK { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "ModePublication", ResourceType = typeof(Language))]
        public Guid? Partner { get; set; }

        public PubblicaViewModel()
        {
        }

        public PubblicaViewModel(PubblicaViewModel model)
        {
            this.CopyAttributes(model);
        }

        public void CopyAttributes(PubblicaViewModel model)
        {
            PropertyInfo[] properties = model.GetType().GetProperties();
            for (int i = 0; i < (int)properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                this.GetType().GetProperty(propertyInfo.Name).SetValue(this, propertyInfo.GetValue(model));
            }
        }
    }

    public class PubblicaOggettoViewModel : PubblicaViewModel
    {
        [Range(0, 2147483647, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int? Anno { get; set; }

        public string Colore { get; set; }

        [Display(Name = "StateObject", ResourceType = typeof(Language))]
        [Required]
        public CondizioneOggetto CondizioneOggetto { get; set; }

        [Required]
        public virtual string Marca { get; set; }

        public int? MarcaID { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Language))]
        [Range(1, 2147483647, ErrorMessageResourceName = "ErrorQuantity", ErrorMessageResourceType = typeof(Language))]
        [Required]
        public int Quantità { get; set; }

        public PubblicaOggettoViewModel()
        {
            base.TipoPubblicazione = TipoAcquisto.Oggetto;
            this.Quantità = 1;
            this.CondizioneOggetto = CondizioneOggetto.Usato;
            this.Anno = new int?(DateTime.Now.Year);
        }

        public PubblicaOggettoViewModel(PubblicaViewModel model) : base(model)
        {
            base.TipoPubblicazione = TipoAcquisto.Oggetto;
            this.Quantità = 1;
            this.CondizioneOggetto = CondizioneOggetto.Usato;
            this.Anno = new int?(DateTime.Now.Year);
        }
    }

    public class PubblicaServizioViewModel : PubblicaViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "NameService", ResourceType = typeof(Language))]
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceName = "TextTooLong", ErrorMessageResourceType = typeof(Language))]
        public override string Nome { get; set; }

        [Required]
        [Display(Name = "Sunday", ResourceType = typeof(Language))]
        public bool Domenica { get; set; }

        [Required]
        [Display(Name = "Thursday", ResourceType = typeof(Language))]
        public bool Giovedi { get; set; }

        [Required]
        [Display(Name = "Monday", ResourceType = typeof(Language))]
        public bool Lunedi { get; set; }

        [Required]
        [Display(Name = "Tuesday", ResourceType = typeof(Language))]
        public bool Martedi { get; set; }

        [Required]
        [Display(Name = "Wednesday", ResourceType = typeof(Language))]
        public bool Mercoledi { get; set; }

        [Required]
        [Display(Name = "Bid", ResourceType = typeof(Language))]
        public string Offerta { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        public TimeSpan? OraFineFeriali { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "EndTime", ResourceType = typeof(Language))]
        public TimeSpan? OraFineFestivi { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        public TimeSpan? OraInizioFeriali { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "StartTime", ResourceType = typeof(Language))]
        public TimeSpan? OraInizioFestivi { get; set; }

        [Required]
        [Display(Name = "Results", ResourceType = typeof(Language))]
        public string Risultati { get; set; }

        [Required]
        [Display(Name = "Saturday", ResourceType = typeof(Language))]
        public bool Sabato { get; set; }

        [Required]
        [Display(Name = "AllDays", ResourceType = typeof(Language))]
        public bool Tutti { get; set; }

        [Required]
        [Display(Name = "Friday", ResourceType = typeof(Language))]
        public bool Venerdi { get; set; }

        [Required]
        [Display(Name = "Rate", ResourceType = typeof(Language))]
        public Tariffa Tariffa { get; set; }

        public PubblicaServizioViewModel()
        {
            base.TipoPubblicazione = TipoAcquisto.Servizio;
            Tutti = true;
        }

        public PubblicaServizioViewModel(PubblicaViewModel model) : base(model)
        {
            base.TipoPubblicazione = TipoAcquisto.Servizio;
            Tutti = true;
        }
    }

    public class PubblicaSpedizioneViewModel
    {
        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdCorriereNazione { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string CorriereNazione { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string NomeMittente { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string TelefonoMittente { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string EmailMittente { get; set; }

        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdIndirizzoMittente { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int IdComuneMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string ComuneMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string IndirizzoMittente { get; set; }

        [Required]
        [Range(1, Int32.MaxValue, ErrorMessageResourceName = "ErrorRange", ErrorMessageResourceType = typeof(Language))]
        public int CivicoMittente { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string SezioneMittente { get; set; }
    }

    public class NotificaRicercaViewModel
    {
        public PERSONA_RICERCA RICERCA { get; set; }

        public ANNUNCIO ANNUNCIO { get; set; }

        public string FOTO { get; set; }
    }

    public class SessioneFoto
    {
        public string NomeOriginaleFoto { get; set; }

        public string NomeUnivocoFoto { get; set; }
    }

    #region DETTAGLI
    public class PubblicaConsoleViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Piattaforma
        {
            get;
            set;
        }

        public int? PiattaformaID
        {
            get;
            set;
        }

        public PubblicaConsoleViewModel()
        {
        }
    }

    public class PubblicaLibroViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Autore
        {
            get;
            set;
        }

        public int? AutoreID
        {
            get;
            set;
        }

        public PubblicaLibroViewModel()
        {
        }
    }

    public class PubblicaModelloViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Modello
        {
            get;
            set;
        }

        public int? ModelloID
        {
            get;
            set;
        }

        public PubblicaModelloViewModel()
        {
        }
    }

    public class PubblicaMusicaViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Artista
        {
            get;
            set;
        }

        public int? ArtistaID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        [Required]
        public string Formato
        {
            get;
            set;
        }

        public int? FormatoID
        {
            get;
            set;
        }

        public PubblicaMusicaViewModel()
        {
        }
    }

    public class PubblicaPcViewModel : PubblicaModelloViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "OperatingSystem", ResourceType = typeof(Language))]
        [Required]
        public string SistemaOperativo
        {
            get;
            set;
        }

        public int? SistemaOperativoID
        {
            get;
            set;
        }

        public PubblicaPcViewModel()
        {
        }
    }

    public class PubblicaTelefoniSmartphoneViewModel : PubblicaModelloViewModel
    {
        [DataType(DataType.Text)]
        [Display(Name = "OperatingSystem", ResourceType = typeof(Language))]
        [Required]
        public string SistemaOperativo
        {
            get;
            set;
        }

        public int? SistemaOperativoID
        {
            get;
            set;
        }

        public PubblicaTelefoniSmartphoneViewModel()
        {
        }
    }

    public class PubblicaVeicoloViewModel : PubblicaModelloViewModel
    {
        [DataType(DataType.Text)]
        [Required]
        [Display(Name = "Power", ResourceType = typeof(Language))]
        public string Alimentazione
        {
            get;
            set;
        }

        public int? AlimentazioneID
        {
            get;
            set;
        }

        public PubblicaVeicoloViewModel()
        {
        }
    }

    public class PubblicaVestitoViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Taglia
        {
            get;
            set;
        }

        public PubblicaVestitoViewModel()
        {
        }
    }

    public class PubblicaVideogamesViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Genere
        {
            get;
            set;
        }

        public int? GenereID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        [Required]
        public string Piattaforma
        {
            get;
            set;
        }

        public int? PiattaformaID
        {
            get;
            set;
        }

        public PubblicaVideogamesViewModel()
        {
        }
    }

    public class PubblicaVideoViewModel : IPubblicaOggetto
    {
        [DataType(DataType.Text)]
        [Required]
        public string Formato
        {
            get;
            set;
        }

        public int? FormatoID
        {
            get;
            set;
        }

        [DataType(DataType.Text)]
        [Required]
        public string Regista
        {
            get;
            set;
        }

        public int? RegistaID
        {
            get;
            set;
        }

        public PubblicaVideoViewModel()
        {
        }
    }

    #endregion
}