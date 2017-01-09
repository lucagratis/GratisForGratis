using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public enum Stato
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3
    }

    public enum StatoVendita
    {
        [Display(Name = "StateSellInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0, // non visibile
        [Display(Name = "StateSellActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1, // in vendita
        [Display(Name = "StateSellDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATO = 2, // annullata
        [Display(Name = "StateSellPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3, // in attesa di pagamento
        [Display(Name = "StateSellSold", ResourceType = typeof(App_GlobalResources.Language))]
        VENDUTO = 4, // pagamento ricevuto
        [Display(Name = "StateSellProgressBarter", ResourceType = typeof(App_GlobalResources.Language))]
        BARATTOINCORSO = 5, // in corso un baratto
        [Display(Name = "StateSellBarter", ResourceType = typeof(App_GlobalResources.Language))]
        BARATTATO = 6 // baratto effettuato
    }

    public enum StatoOfferta
    {
        [Display(Name = "StateBidInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVA = 0,
        [Display(Name = "StateBidActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVA = 1,
        [Display(Name = "StateBidDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATA = 2,
        [Display(Name = "StateBidPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESA = 3,
        [Display(Name = "StateBidAccepted", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATA = 4
}
    // NON PIù IN USO
    public enum StatoBaratto
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateCancel", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3,
        [Display(Name = "StateAccept", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATO = 4
    }

    public enum StatoPagamento
    {
        [Display(Name = "StateInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVO = 0,
        [Display(Name = "StateActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVO = 1,
        [Display(Name = "StateCancel", ResourceType = typeof(App_GlobalResources.Language))]
        ANNULLATO = 2,
        [Display(Name = "StatePause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESO = 3,
        [Display(Name = "StateAccept", ResourceType = typeof(App_GlobalResources.Language))]
        ACCETTATO = 4
    }

    public enum StatoMoneta
    {
        [Display(Name = "StateMoneyInactive", ResourceType = typeof(App_GlobalResources.Language))]
        INATTIVA = 0,
        [Display(Name = "StateMoneyActive", ResourceType = typeof(App_GlobalResources.Language))]
        ATTIVA = 1,
        [Display(Name = "StateMoneyDelete", ResourceType = typeof(App_GlobalResources.Language))]
        ELIMINATA = 2,
        [Display(Name = "StateMoneyPause", ResourceType = typeof(App_GlobalResources.Language))]
        SOSPESA = 3,
        [Display(Name = "StateMoneyAssigned", ResourceType = typeof(App_GlobalResources.Language))]
        ASSEGNATA = 4,
        CEDUTA = 5
    }
}
