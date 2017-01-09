using System.ComponentModel.DataAnnotations;

namespace GratisForGratis.Models
{
    public enum CondizioneOggetto
    {
        [Display(Name = "SecondHand", ResourceType = typeof(App_GlobalResources.Language))]
        Usato = 1,
        [Display(Name = "New", ResourceType = typeof(App_GlobalResources.Language))]
        Nuovo = 2,
        [Display(Name = "Broken", ResourceType = typeof(App_GlobalResources.Language))]
        Rotto = 3,
        [Display(Name = "Fix", ResourceType = typeof(App_GlobalResources.Language))]
        DaSistemare = 4
    }

    public enum Distanza
    {
        Cm = 0,
        M = 1
    }

    public enum Peso
    {
        G = 0,
        Kg = 1
    }

    public enum Tariffa
    {
        [Display(Name = "HourlyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Oraria = 0,
        [Display(Name = "DailyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Giornaliera = 1,
        [Display(Name = "MounthlyRate", ResourceType = typeof(App_GlobalResources.Language))]
        Mensile = 2,
        [Display(Name = "CompleteService", ResourceType = typeof(App_GlobalResources.Language))]
        ServizioCompleto = 3
    }
}
