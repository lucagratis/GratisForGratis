using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    public enum ErroreOfferta
    {
        [Display(Name = "Nothing", ResourceType = typeof(App_GlobalResources.Language))]
        Nessuno = 0,
        [Display(Name = "ErrorOwner", ResourceType = typeof(App_GlobalResources.Language))]
        Proprietario = -1,
        [Display(Name = "ErrorSailing", ResourceType = typeof(App_GlobalResources.Language))]
        StatoVendita = -2,
        [Display(Name = "ErrorUserPoints", ResourceType = typeof(App_GlobalResources.Language))]
        PuntiUtente = -3,
        [Display(Name = "ErrorUserState", ResourceType = typeof(App_GlobalResources.Language))]
        StatoUtente = -4,
        [Display(Name = "ErrorQuantityObject", ResourceType = typeof(App_GlobalResources.Language))]
        NumeroPezzi = -5,
        [Display(Name = "ErrorNotSaved", ResourceType = typeof(App_GlobalResources.Language))]
        NonSalvata = -6,
        [Display(Name = "ErrorMaxBarters", ResourceType = typeof(App_GlobalResources.Language))]
        MaxBaratti = -7,
        [Display(Name = "ErrorPrice", ResourceType = typeof(App_GlobalResources.Language))]
        PrezzoErrato = -8,
        [Display(Name = "ErrorNothingBarter", ResourceType = typeof(App_GlobalResources.Language))]
        ErroreNessunBarattoInserito = -9,
        [Display(Name = "ErrorBuyer", ResourceType = typeof(App_GlobalResources.Language))]
        ErroreCompratore = -10,
        [Display(Name = "ErrorBarter", ResourceType = typeof(App_GlobalResources.Language))]
        ErroreBaratto = -11
    }

    public enum ErrorePagamento
    {
        [Display(Name = "Nothing", ResourceType = typeof(App_GlobalResources.Language))]
        Nessuno = 0,
        [Display(Name = "ErrorPayment", ResourceType = typeof(App_GlobalResources.Language))]
        Proprietario = -1,
        [Display(Name = "ErrorPaymentBid", ResourceType = typeof(App_GlobalResources.Language))]
        StatoOggetto = -2,
        [Display(Name = "ErrorPaymentUserPoints", ResourceType = typeof(App_GlobalResources.Language))]
        PuntiUtente = -3,
        [Display(Name = "ErrorPaymentObjectState", ResourceType = typeof(App_GlobalResources.Language))]
        StatoUtente = -4
    }
}
