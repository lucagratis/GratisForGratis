using GratisForGratis.App_GlobalResources;
using GratisForGratis.Components;
using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace GratisForGratis.Controllers
{
    [Authorize]
    public class ServizioController : AdvancedController
    {

        #region ACTION
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Acquista(string nome, string token)
        {
            ServizioViewModel servizio = new ServizioViewModel();
            string nomeView = "Index";

            try
            {
                TempData["azione"] = "Buy";

                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    ANNUNCIO vendita = db.ANNUNCIO.Where(v => v.TOKEN == tokenGuid && v.NOME == nome && v.SERVIZIO != null).FirstOrDefault();
                    // reindirizza alla lista generale di oggetti
                    if (vendita == null)
                    {
                        return RedirectToAction("Tutti", "Servizi");
                    }
                    else if (vendita.STATO == (int)StatoVendita.BARATTATO || vendita.STATO == (int)StatoVendita.BARATTOINCORSO)
                    {
                        return RedirectToAction("Barattato", "Servizio", new { nome = nome, token = token });
                    }

                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    servizio.Id = (int)vendita.ID_SERVIZIO;
                    servizio.CategoriaID = vendita.ID_CATEGORIA;
                    servizio.Categoria = vendita.CATEGORIA.NOME;
                    servizio.Token = Utils.RandomString(3) + Utils.Encode(vendita.TOKEN.ToString()) + Utils.RandomString(3);
                    servizio.Citta = vendita.SERVIZIO.COMUNE.NOME;
                    servizio.DataInserimento = vendita.DATA_INSERIMENTO;
                    servizio.VenditoreToken = vendita.PERSONA.TOKEN;
                    servizio.Foto = vendita.ANNUNCIO_FOTO.Select(f => f.FOTO.FOTO1).ToList();
                    servizio.Nome = vendita.NOME;
                    servizio.Punti = vendita.PUNTI;
                    servizio.Soldi = vendita.SOLDI;
                    servizio.Lunedi = (bool)vendita.SERVIZIO.LUNEDI;
                    servizio.Martedi = (bool)vendita.SERVIZIO.MARTEDI;
                    servizio.Mercoledi = (bool)vendita.SERVIZIO.MERCOLEDI;
                    servizio.Giovedi = (bool)vendita.SERVIZIO.GIOVEDI;
                    servizio.Venerdi = (bool)vendita.SERVIZIO.VENERDI;
                    servizio.Sabato = (bool)vendita.SERVIZIO.SABATO;
                    servizio.Domenica = (bool)vendita.SERVIZIO.DOMENICA;
                    if (vendita.SERVIZIO.TUTTI != null)
                        servizio.Tutti = (bool)vendita.SERVIZIO.TUTTI;
                    servizio.OraInizio = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FERIALI;
                    servizio.OraFine = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FERIALI;
                    if (vendita.SERVIZIO.ORA_INIZIO_FESTIVI != null)
                        servizio.OraInizioFestivita = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FESTIVI;
                    if (vendita.SERVIZIO.ORA_FINE_FESTIVI != null)
                        servizio.OraFineFestivita = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FESTIVI;
                    servizio.RisultatiFinali = vendita.SERVIZIO.RISULTATI_FINALI;
                    servizio.ServiziOfferti = vendita.SERVIZIO.SERVIZI_OFFERTI;
                    servizio.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
                    servizio.VenditoreID = vendita.ID_PERSONA;
                    servizio.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
                    servizio.Tariffa = (Tariffa)vendita.SERVIZIO.TARIFFA;
                    servizio.StatoVendita = (StatoVendita)vendita.STATO;
                    servizio = SetServizioViewModel(db, servizio);
                    SetFeedbackVenditoreServizio(db, servizio);
                    // verifico che non stia provando ad accedere il venditore stesso
                    if (servizio.VenditoreID == (Session["utente"] as PersonaModel).Persona.ID)
                        TempData["azione"] = "View";
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            ViewBag.Title = string.Format(App_GlobalResources.MetaTag.TitleService, servizio.Nome, servizio.Categoria);
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionService, servizio.Nome, servizio.Categoria, servizio.Citta, servizio.Punti);
            ViewBag.Keywords = string.Format(App_GlobalResources.MetaTag.KeywordsService, servizio.Nome, servizio.Categoria, servizio.Citta, servizio.Punti);
            return View(nomeView, servizio);
        }

        // vista per servizio barattato o con baratto in corso
        [HttpGet]
        public ActionResult Barattato(string nome, string token)
        {
            ServizioViewModel servizio = new ServizioViewModel();
            string nomeView = "Index";

            try
            {
                TempData["azione"] = "View";

                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    ANNUNCIO vendita = db.ANNUNCIO.Where(v => v.TOKEN == tokenGuid && v.NOME == nome && v.SERVIZIO != null /*&& (v.STATO == (int)StatoVendita.BARATTOINCORSO || v.STATO == (int)StatoVendita.BARATTATO)*/).FirstOrDefault();
                    // reindirizza alla lista generale di oggetti
                    if (vendita == null)
                        return RedirectToAction("Tutti", "Servizi");

                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    servizio.Id = (int)vendita.ID_SERVIZIO;
                    servizio.CategoriaID = vendita.ID_CATEGORIA;
                    servizio.Categoria = vendita.CATEGORIA.NOME;
                    servizio.Token = Utils.RandomString(3) + Utils.Encode(vendita.TOKEN.ToString()) + Utils.RandomString(3);
                    servizio.Citta = vendita.SERVIZIO.COMUNE.NOME;
                    servizio.DataInserimento = vendita.DATA_INSERIMENTO;
                    servizio.VenditoreToken = vendita.PERSONA.TOKEN;
                    servizio.Foto = vendita.ANNUNCIO_FOTO.Select(f => f.FOTO.FOTO1).ToList();
                    servizio.Nome = vendita.NOME;
                    servizio.Punti = vendita.PUNTI;
                    servizio.Soldi = vendita.SOLDI;
                    servizio.Lunedi = (bool)vendita.SERVIZIO.LUNEDI;
                    servizio.Martedi = (bool)vendita.SERVIZIO.MARTEDI;
                    servizio.Mercoledi = (bool)vendita.SERVIZIO.MERCOLEDI;
                    servizio.Giovedi = (bool)vendita.SERVIZIO.GIOVEDI;
                    servizio.Venerdi = (bool)vendita.SERVIZIO.VENERDI;
                    servizio.Sabato = (bool)vendita.SERVIZIO.SABATO;
                    servizio.Domenica = (bool)vendita.SERVIZIO.DOMENICA;
                    if (vendita.SERVIZIO.TUTTI != null)
                        servizio.Tutti = (bool)vendita.SERVIZIO.TUTTI;
                    servizio.OraInizio = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FERIALI;
                    servizio.OraFine = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FERIALI;
                    if (vendita.SERVIZIO.ORA_INIZIO_FESTIVI != null)
                        servizio.OraInizioFestivita = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FESTIVI;
                    if (vendita.SERVIZIO.ORA_FINE_FESTIVI != null)
                        servizio.OraFineFestivita = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FESTIVI;
                    servizio.RisultatiFinali = vendita.SERVIZIO.RISULTATI_FINALI;
                    servizio.ServiziOfferti = vendita.SERVIZIO.SERVIZI_OFFERTI;
                    servizio.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
                    servizio.VenditoreID = vendita.ID_PERSONA;
                    servizio.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
                    servizio.Tariffa = (Tariffa)vendita.SERVIZIO.TARIFFA;
                    servizio.StatoVendita = (StatoVendita)vendita.STATO;
                    servizio = SetServizioViewModel(db, servizio);
                    SetFeedbackVenditoreServizio(db, servizio);
                    ViewBag.Title = App_GlobalResources.Language.Buy + " " + servizio.Nome + " " + App_GlobalResources.Language.Gratis;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return View(nomeView, servizio);
        }

        [HttpPost]
        public ActionResult Acquista(ServizioViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Token))
                return Redirect("/Servizi/Tutti");

            if (model.Offerta.TipoOfferta == TipoOfferta.Baratto && model.Offerta.ServiziBarattati != null && model.Offerta.ServiziBarattati.Length > 4)
                ModelState.AddModelError("ErroreOfferta", ErroreOfferta.PrezzoErrato.ToString());

            if (CheckUtenteAttivo(1))
                return RedirectToAction("Impostazioni", "Utente");

            string nomeView = "Index";
            
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    string tokenDecodificato = Server.UrlDecode(model.Token);
                    string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                    Guid tokenGuid = Guid.Parse(tokenPulito);

                    if (ModelState.IsValid)
                    {
                        ObjectParameter errore = new ObjectParameter("Errore", typeof(int));
                        errore.Value = 0;
                        string baratti = "";
                        if (model.Offerta.TipoOfferta == TipoOfferta.Baratto)
                        {
                            XElement xml = new XElement("Root",
                                    model.Offerta.ServiziBarattati.Select((i, index)
                                        => new XElement("Guid", i)
                                    )
                            );
                            baratti = xml.ToString();
                        }

                        int? idOfferta = db.BENE_SAVE_OFFERTA_SERVIZIO(tokenGuid, ((PersonaModel)Session["utente"]).Persona.ID_CONTO_CORRENTE, 1, (int)model.Offerta.TipoOfferta, model.Offerta.PuntiOfferti, Utils.cambioValuta(model.Offerta.PuntiOfferti), baratti, errore);

                        if (idOfferta != null && Convert.ToInt32(errore.Value) == 0)
                        {
                            // impostare invio email offerta effettuata
                            EmailModel email = new EmailModel(ControllerContext);
                            email.To.Add(new System.Net.Mail.MailAddress(model.VenditoreToken.ToString(), App_GlobalResources.Email.BidOf + " " + model.VenditoreNominativo + " - " + System.Web.Configuration.WebConfigurationManager.AppSettings["nomeSito"]));
                            email.Subject = App_GlobalResources.Email.BidSubject + " " + model.Nome;
                            email.Body = "Offerta";
                            email.DatiEmail = model;
                            new EmailController().SendEmail(email);

                            return RedirectToAction("OffertaInviata", "Servizio", new { idOfferta = idOfferta });
                        }

                        // ERRORE
                        ModelState.AddModelError("ErroreOfferta", ((ErroreOfferta)errore.Value).GetDisplayName());
                    }
                    else
                    {
                        ModelState.AddModelError("ErroreOfferta", App_GlobalResources.Language.ErrorGenericBid);
                    }
                    int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                    // recupero le informazioni sul servizio
                    ANNUNCIO vendita = db.ANNUNCIO.Where(v => v.TOKEN == tokenGuid && v.SERVIZIO != null && v.ID_PERSONA != idUtente && (v.STATO == (int)StatoVendita.ATTIVO || v.STATO == (int)StatoVendita.BARATTOINCORSO)).FirstOrDefault();
                    // reindirizza alla lista generale di servizi
                    if (vendita == null)
                        return RedirectToAction("Tutti", "Servizi");
                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    model.Id = (int)vendita.ID_SERVIZIO;
                    model.CategoriaID = vendita.ID_CATEGORIA;
                    model.Categoria = vendita.CATEGORIA.NOME;
                    model.Token = Utils.RandomString(3) + vendita.TOKEN.ToString() + Utils.RandomString(3);
                    model.Citta = vendita.OGGETTO.COMUNE.NOME;
                    model.DataInserimento = vendita.DATA_INSERIMENTO;
                    model.VenditoreToken = vendita.PERSONA.TOKEN;
                    model.Foto = vendita.ANNUNCIO_FOTO.Select(f => f.FOTO.FOTO1).ToList();
                    model.Nome = vendita.NOME;
                    model.Punti = vendita.PUNTI;
                    model.Soldi = vendita.SOLDI;
                    model.Lunedi = (bool)vendita.SERVIZIO.LUNEDI;
                    model.Martedi = (bool)vendita.SERVIZIO.MARTEDI;
                    model.Mercoledi = (bool)vendita.SERVIZIO.MERCOLEDI;
                    model.Giovedi = (bool)vendita.SERVIZIO.GIOVEDI;
                    model.Venerdi = (bool)vendita.SERVIZIO.VENERDI;
                    model.Sabato = (bool)vendita.SERVIZIO.SABATO;
                    model.Domenica = (bool)vendita.SERVIZIO.DOMENICA;
                    model.Tutti = (bool)vendita.SERVIZIO.TUTTI;
                    model.OraInizio = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FERIALI;
                    model.OraFine = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FERIALI;
                    model.OraInizio = (TimeSpan)vendita.SERVIZIO.ORA_INIZIO_FESTIVI;
                    model.OraFine = (TimeSpan)vendita.SERVIZIO.ORA_FINE_FESTIVI;
                    model.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
                    model.VenditoreID = vendita.ID_PERSONA;
                    model.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
                    model.Tariffa = (Tariffa)vendita.SERVIZIO.TARIFFA;
                    model.StatoVendita = (StatoVendita)vendita.STATO;
                    model = SetServizioViewModel(db, model);
                    SetFeedbackVenditoreServizio(db, model);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            
            return View(nomeView, model);
        }

        [HttpGet]
        public ActionResult OffertaInviata(int idOfferta)
        {
            OffertaEffettuataViewModel offerta = new OffertaEffettuataViewModel();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = Session["utente"] as PersonaModel;
                    offerta = db.OFFERTA.Where(item => item.ID == idOfferta && item.ID_PERSONA == utente.Persona.ID && item.STATO == (int)StatoOfferta.ATTIVA)
                        .Select(item => new OffertaEffettuataViewModel()
                        {
                            Token = item.ANNUNCIO.TOKEN.ToString(),
                            Venditore = item.ANNUNCIO.PERSONA.NOME + " " + item.ANNUNCIO.PERSONA.COGNOME,
                        //Categoria = item.ANNUNCIO.CATEGORIA.NOME,
                        IDCategoria = item.ANNUNCIO.ID_CATEGORIA,
                            Punti = (int)item.PUNTI,
                            Soldi = (int)item.SOLDI,
                            Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == item.ANNUNCIO.ID).Select(f =>
                                f.FOTO.FOTO1
                            ).ToList(),
                            Baratti = db.OFFERTA_BARATTO.Where(b => b.ID_OFFERTA == item.ID).Select(b =>
                                        new VenditaViewModel()
                                        {
                                            Token = b.ANNUNCIO.TOKEN.ToString(),
                                            TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                            Nome = b.ANNUNCIO.NOME,
                                            Punti = b.ANNUNCIO.PUNTI,
                                            Soldi = b.ANNUNCIO.SOLDI,
                                        }).ToList(),
                            Nome = item.ANNUNCIO.NOME,
                            TipoOfferta = (TipoOfferta)item.TIPO_OFFERTA,
                            TipoTrattativa = (TipoTrattativa)item.TIPO_TRATTATIVA,
                            //PuntiCompratore = item.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA),
                            //PuntiSospesiCompratore = item.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.SOSPESA)
                        }).SingleOrDefault();
                    // se non trovo l'offerta lo reindirizzo alla home page
                    if (offerta == null)
                        return RedirectToAction("Index", "Utente");
                    // aggiorno punti attuali utente
                    RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            
            // creare corpo metodo
            return View(offerta);
        }
        #endregion

        #region METODI PRIVATI
        // praticamente non usata
        private ServizioViewModel SetServizioViewModel(DatabaseContext db, ServizioViewModel servizio)
        {
            // gestito cosi, perchè nel caso faccio macroricerche, recupero lo stesso i dati personalizzati
            // sulla specifica sottocategoria.
            switch (servizio.CategoriaID)
            {
                default:
                    break;
            }
            return servizio;
        }
        private void SetFeedbackVenditoreServizio(DatabaseContext db, ServizioViewModel viewModel)
        {
            try
            {
                List<int> voti = db.ANNUNCIO_FEEDBACK
                                .Where(item => (viewModel.PartnerId <= 0 && item.ANNUNCIO.ID_PERSONA == viewModel.VenditoreID) ||
                                (viewModel.PartnerId > 0 && item.ANNUNCIO.ID_ATTIVITA == viewModel.PartnerId)).Select(item => item.VOTO).ToList();

                int votoMassimo = voti.Count * 10;
                if (voti.Count <= 0)
                {
                    viewModel.VenditoreFeedback = -1;
                }
                else
                {
                    int x = voti.Sum() / votoMassimo;
                    viewModel.VenditoreFeedback = x * 100;
                }
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                viewModel.VenditoreFeedback = -1;
            }
        }
        #endregion
    }
}