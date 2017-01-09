using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public enum TipoTrattativa
    {
        Privata = 0,
        Spedizione = 1
    }

    public enum TipoAcquisto
    {
        Oggetto = 0,
        Servizio = 1
    }

    public enum TipoPagamento
    {
        [Display(Name = "Whatever", ResourceType = typeof(App_GlobalResources.Language))]
        Qualunque = 0,
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        Vendita = 1,
        [Display(Name = "Barter", ResourceType = typeof(App_GlobalResources.Language))]
        Baratto = 2,
    }

    public enum TipoOfferta
    {
        [Display(Name = "Points", ResourceType = typeof(App_GlobalResources.Language))]
        Punti = 0,
        [Display(Name = "Barter", ResourceType = typeof(App_GlobalResources.Language))]
        Baratto = 1,
    }

    public enum TipoSegnalazione
    {
        [Display(Name = "Bug", ResourceType = typeof(App_GlobalResources.Language))]
        Bug = 0,
        [Display(Name = "Improvement", ResourceType = typeof(App_GlobalResources.Language))]
        Miglioramento = 1
    }

    public enum TipoBonus
    {
        Iscrizione = 0,
        PubblicazioneIniziale = 1,
        IscrizionePartner = 2,
        BonusPartner = 3,
        Login = 4
    }

    public enum RuoloProfilo
    {
        Amministratore = 0,
        Operatore = 1
    }

    // usato nella tabella TRANSAZIONE
    public enum TipoTransazione
    {
        Annuncio = 0,
        Bonifico = 1,
        Gxg = 2,
        BonusIscrizione = 3,
        BonusPubblicazioneIniziale = 4,
        BonusIscrizionePartner = 5,
        BonusPartner = 6,
        BonusLogin = 7,
        BonusFeedback = 8
    }

    public enum TipoEmail
    {
        Contatto = 0,
        Registrazione = 1
    }

    public enum TipoTelefono
    {
        Privato = 0,
        Ufficio = 1
    }

    public enum TipoIndirizzo
    {
        Residenza = 0,
        Domicilio = 1,
        Spedizione = 2
    }

    public enum TipoVenditore
    {
        Persona = 0,
        Attivita = 1
    }
}
