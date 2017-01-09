using GratisForGratis.Models;
using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Xml.Linq;
using GratisForGratis.Components;
using System.Collections.Generic;

namespace GratisForGratis.Controllers
{
    [Authorize]
    public class OggettoController : AdvancedController
    {

        #region ACTION
        // verificare che chi chiama questa pagina lo faccia per comprare o per
        // visualizzare un baratto o per modificare l'oggetto... verifica del tipo di visualizzazione
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string nome, string token)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("", "Cerca");
            TempData["azione"] = "View";
            OggettoViewModel oggetto = new OggettoViewModel();
            string nomeView = "Index";
            try
            {
                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);
                string nomeDecodificato = Server.UrlDecode(nome);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    ANNUNCIO vendita = db.ANNUNCIO
                        .Include("Categoria")
                        .Include("Persona")
                        .Include("Oggetto")
                        .Include("Annuncio_Foto")
                        .Where(v => v.TOKEN == tokenGuid && v.NOME == nomeDecodificato && v.OGGETTO != null).FirstOrDefault();
                    // verifico se il prodotto esiste
                    if(Session["utente"] != null && vendita.ID_PERSONA != (Session["utente"] as PersonaModel).Persona.ID)
                    {
                        // verificare se è in vendita
                        if (vendita.STATO == (int)StatoVendita.ATTIVO)
                        {
                            TempData["annuncio"] = vendita;
                            return RedirectToAction("Acquista", "Oggetto", new { nome = nome, token = token });
                        }
                    }
                    else if (Session["utente"] != null && vendita.ID_PERSONA == (Session["utente"] as PersonaModel).Persona.ID)
                    {
                        // se sono il venditore ed è ancora aperto l'annuncio, lo porto alla pagina di modifica
                        if (vendita.STATO == (int)StatoVendita.ATTIVO || vendita.STATO == (int)StatoVendita.INATTIVO || vendita.STATO == (int)StatoVendita.SOSPESO)
                        {
                            TempData["annuncio"] = vendita;
                            return RedirectToAction("Modifica", "Oggetto", new { nome = nome, token = token });
                        }
                    }

                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    SetOggettoViewModel(oggetto,vendita);
                    oggetto = SetInfoCategoriaOggetto(vendita.OGGETTO, oggetto);
                    SetFeedbackVenditoreOggetto(db, oggetto);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("", "Cerca");
            }
            SetMetaTag(oggetto);
            return View(nomeView, oggetto);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Acquista(string nome, string token)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("", "Cerca");

            OggettoViewModel oggetto = new OggettoViewModel();
            string nomeView = "Index";
            try
            {
                TempData["azione"] = "Buy";
                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);

                string nomeDecodificato = Server.UrlDecode(nome);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    ANNUNCIO vendita = new ANNUNCIO();
                    if (TempData["annuncio"] == null)
                    {
                        vendita = db.ANNUNCIO
                            .Include("Categoria")
                            .Include("Persona")
                            .Include("Oggetto")
                            .Include("Annuncio_Foto")
                            .Where(v => v.TOKEN == tokenGuid && v.NOME == nomeDecodificato && v.OGGETTO != null).FirstOrDefault();
                        if (Session["utente"] != null && vendita.ID_PERSONA == (Session["utente"] as PersonaModel).Persona.ID)
                        {
                            if (vendita.STATO == (int)StatoVendita.ATTIVO || vendita.STATO == (int)StatoVendita.INATTIVO || vendita.STATO == (int)StatoVendita.SOSPESO)
                            {
                                TempData["annuncio"] = vendita;
                                return RedirectToAction("Modifica", "Oggetto", new { nome = nome, token = token });
                            }
                        }
                        else if (vendita.STATO != (int)StatoVendita.ATTIVO)
                        {
                            TempData["annuncio"] = vendita;
                            return RedirectToAction("Index", "Oggetto", new { nome = nome, token = token });
                        }
                    }
                    else
                    {
                        vendita = TempData["annuncio"] as ANNUNCIO;
                    }
                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    SetOggettoViewModel(oggetto, vendita);
                    oggetto = SetInfoCategoriaOggetto(vendita.OGGETTO, oggetto);
                    SetFeedbackVenditoreOggetto(db, oggetto);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("", "Cerca");
            }
            SetMetaTag(oggetto);
            return View(nomeView, oggetto);
        }

        // vista per oggetto baratto o con baratto in corso
        [HttpGet]
        public ActionResult Barattato(string nome, string token)
        {
            OggettoViewModel oggetto = new OggettoViewModel();
            string nomeView = "Index";
            string nomeDecodificato = Server.UrlDecode(nome);
            try
            {
                TempData["azione"] = "View";

                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    ANNUNCIO vendita = new ANNUNCIO();
                    if (TempData["annuncio"] == null)
                    {
                        vendita = db.ANNUNCIO.Where(v => v.TOKEN == tokenGuid && v.NOME == nomeDecodificato && v.OGGETTO != null /*&& (v.STATO == (int)StatoVendita.BARATTOINCORSO || v.STATO == (int)StatoVendita.BARATTATO)*/).FirstOrDefault();
                    }
                    else
                    {
                        vendita = TempData["annuncio"] as ANNUNCIO;
                    }
                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    oggetto.Id = (int)vendita.ID_OGGETTO;
                    oggetto.CategoriaID = vendita.ID_CATEGORIA;
                    oggetto.Categoria = vendita.CATEGORIA.NOME;
                    oggetto.Token = Utils.RandomString(3) + vendita.TOKEN.ToString() + Utils.RandomString(3);
                    oggetto.Citta = vendita.OGGETTO.COMUNE.NOME;
                    oggetto.DataInserimento = vendita.DATA_INSERIMENTO;
                    oggetto.VenditoreToken = vendita.PERSONA.TOKEN;
                    //oggetto.Foto = vendita.ANNUNCIO_FOTO.Select(f => f.FOTO).ToList();
                    oggetto.Foto = vendita.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == vendita.ID).Select(f => f.FOTO.FOTO1).ToList();
                    oggetto.Marca = vendita.OGGETTO.MARCA.NOME;
                    oggetto.Nome = vendita.NOME;
                    oggetto.Punti = vendita.PUNTI;
                    oggetto.Soldi = vendita.SOLDI;
                    oggetto.StatoOggetto = (CondizioneOggetto)vendita.OGGETTO.CONDIZIONE;
                    oggetto.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
                    oggetto.VenditoreID = vendita.ID_PERSONA;
                    oggetto.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
                    oggetto.Note = vendita.NOTE_AGGIUNTIVE;
                    oggetto.Quantità = vendita.OGGETTO.NUMERO_PEZZI;
                    //oggetto = SetOggettoViewModel(db, oggetto);
                    oggetto = SetInfoCategoriaOggetto(vendita.OGGETTO, oggetto);
                    SetFeedbackVenditoreOggetto(db, oggetto);

                    ViewBag.Title = App_GlobalResources.Language.Buy + " " + oggetto.Nome + " " + App_GlobalResources.Language.Gratis;
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("", "Cerca");
            }

            return View(nomeView, oggetto);
        }

        [HttpPost]
        public ActionResult Acquista(OggettoViewModel model)
        {
            // VERIFICARE CHE TI MANDI VERAMENTE AL REDIRECT SU TUTTI!
            if (model == null || string.IsNullOrWhiteSpace(model.Token))
                return RedirectToAction("", "Cerca");

            if (model.Offerta.TipoOfferta == TipoOfferta.Baratto && model.Offerta.OggettiBarattati != null && model.Offerta.OggettiBarattati.Length > 4)
                ModelState.AddModelError("ErroreOfferta", ErroreOfferta.PrezzoErrato.ToString());

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
                                    model.Offerta.OggettiBarattati.Select((i, index)
                                        => new XElement("Guid", i)
                                    )
                            );
                            baratti = xml.ToString();
                        }

                        int? idOfferta = db.BENE_SAVE_OFFERTA(tokenGuid, ((PersonaModel)Session["utente"]).Persona.ID_CONTO_CORRENTE, 1, (int)model.Offerta.TipoOfferta, model.Offerta.PuntiOfferti, Utils.cambioValuta(model.Offerta.PuntiOfferti), baratti, errore).FirstOrDefault();

                        if (idOfferta != null && Convert.ToInt32(errore.Value) == 0)
                        {
                            // impostare invio email offerta effettuata
                            /*EmailModel email = new EmailModel(ControllerContext);
                            email.To.Add(new System.Net.Mail.MailAddress(model.Email, App_GlobalResources.Email.BidOf + " " + model.VenditoreNominativo + " - " + WebConfigurationManager.AppSettings["nomeSito"]));
                            email.Subject = App_GlobalResources.Email.BidSubject + " " + model.Nome;
                            email.Body = "Offerta";
                            email.DatiEmail = model;
                            new EmailController().SendEmail(email);*/

                            return RedirectToAction("OffertaInviata", "Oggetto", new { idOfferta = idOfferta });
                        }

                        // ERRORE
                        ModelState.AddModelError("ErroreOfferta", ((ErroreOfferta)errore.Value).GetDisplayName());
                    }
                    else
                    {
                        ModelState.AddModelError("ErroreOfferta", App_GlobalResources.Language.ErrorGenericBid);
                    }

                    int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                    // recupero le informazioni sull'oggetto
                    ANNUNCIO vendita = db.ANNUNCIO.Where(v => v.TOKEN == tokenGuid && v.OGGETTO != null && v.ID_PERSONA != idUtente && (v.STATO == (int)StatoVendita.ATTIVO || v.STATO == (int)StatoVendita.BARATTOINCORSO)).FirstOrDefault();
                    // reindirizza alla lista generale di oggetti
                    if (vendita == null)
                        return RedirectToAction("", "Cerca");
                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    model.Id = (int)vendita.ID_OGGETTO;
                    model.CategoriaID = vendita.ID_CATEGORIA;
                    model.Categoria = nomeView;
                    model.Token = Utils.RandomString(3) + vendita.TOKEN.ToString() + Utils.RandomString(3);
                    model.Citta = vendita.OGGETTO.COMUNE.NOME;
                    model.DataInserimento = vendita.DATA_INSERIMENTO;
                    model.VenditoreToken = vendita.PERSONA.TOKEN;
                    //model.Foto = vendita.ANNUNCIO_FOTO.Select(f => f.FOTO).ToList();
                    model.Foto = vendita.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == vendita.ID).Select(f => f.FOTO.FOTO1).ToList();
                    model.Marca = vendita.OGGETTO.MARCA.NOME;
                    model.Nome = vendita.NOME;
                    model.Punti = vendita.PUNTI;
                    model.Soldi = vendita.SOLDI;
                    model.StatoOggetto = (CondizioneOggetto)vendita.OGGETTO.CONDIZIONE;
                    model.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
                    model.VenditoreID = vendita.ID_PERSONA;
                    model.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
                    model.Note = vendita.NOTE_AGGIUNTIVE;
                    model.Quantità = vendita.OGGETTO.NUMERO_PEZZI;
                    //model = SetOggettoViewModel(db, model);
                    model = SetInfoCategoriaOggetto(vendita.OGGETTO, model);
                    SetFeedbackVenditoreOggetto(db, model);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("", "Cerca");
            }

            return View(nomeView,model);
        }

        // non ancora funzionante
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Modifica(string nome, string token)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("", "Cerca");

            OggettoViewModel oggetto = new OggettoViewModel();
            string nomeView = "Index";
            try
            {
                TempData["azione"] = "Index"; // sarà update
                string tokenDecodificato = Server.UrlDecode(token);
                string tokenPulito = tokenDecodificato.Substring(3).Substring(0, tokenDecodificato.Length - 6);
                Guid tokenGuid = Guid.Parse(tokenPulito);

                string nomeDecodificato = Server.UrlDecode(nome);

                // recuperare oggetto e visualizzare vista corretta
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.Database.Connection.Open();
                    ANNUNCIO vendita = new ANNUNCIO();
                    if (TempData["annuncio"] == null)
                    {
                        vendita = db.ANNUNCIO
                            .Include("Categoria")
                            .Include("Persona")
                            .Include("Oggetto")
                            .Include("Annuncio_Foto")
                            .Where(v => v.TOKEN == tokenGuid && v.NOME == nomeDecodificato && v.OGGETTO != null).FirstOrDefault();
                        if (Session["utente"] != null && vendita.ID_PERSONA == (Session["utente"] as PersonaModel).Persona.ID)
                        {
                            if (vendita.STATO == (int)StatoVendita.ATTIVO)
                            {
                                TempData["annuncio"] = vendita;
                                return RedirectToAction("Acquista", "Oggetto", new { nome = nome, token = token });
                            }
                        }
                        else if (vendita.STATO != (int)StatoVendita.ATTIVO)
                        {
                            TempData["annuncio"] = vendita;
                            return RedirectToAction("Index", "Oggetto", new { nome = nome, token = token });
                        }
                    }
                    else
                    {
                        vendita = TempData["annuncio"] as ANNUNCIO;
                    }
                    nomeView = vendita.CATEGORIA.DESCRIZIONE;
                    SetOggettoViewModel(oggetto, vendita);
                    oggetto = SetInfoCategoriaOggetto(vendita.OGGETTO, oggetto);
                    SetFeedbackVenditoreOggetto(db, oggetto);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("", "Cerca");
            }
            SetMetaTag(oggetto);
            return View(nomeView, oggetto);
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
                    offerta = db.OFFERTA.Where(item => item.ID == idOfferta && item.ID_PERSONA == utente.Persona.ID && (item.STATO == (int)StatoOfferta.ATTIVA || item.STATO == (int)StatoOfferta.ACCETTATA))
                        .Select(item => new OffertaEffettuataViewModel()
                        {
                            Token = item.ANNUNCIO.TOKEN.ToString(),
                            Venditore = item.ANNUNCIO.PERSONA.NOME + " " + item.ANNUNCIO.PERSONA.COGNOME,
                        //Categoria = item.VENDITA1.CATEGORIA.NOME,
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

        private void SetMetaTag(OggettoViewModel oggetto)
        {
            ViewBag.Title = string.Format(App_GlobalResources.MetaTag.TitleGood, oggetto.Nome, oggetto.Categoria);
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionGood, oggetto.Nome, oggetto.Categoria, oggetto.Citta, oggetto.Punti);
            ViewBag.Keywords = string.Format(App_GlobalResources.MetaTag.KeywordsGood, oggetto.Nome, oggetto.Categoria, oggetto.Citta, oggetto.Punti);
        }

        private void SetOggettoViewModel(OggettoViewModel oggetto, ANNUNCIO vendita)
        {
            oggetto.Id = (int)vendita.ID_OGGETTO;
            oggetto.CategoriaID = vendita.ID_CATEGORIA;
            oggetto.Categoria = vendita.CATEGORIA.NOME;
            oggetto.Token = Utils.RandomString(3) + vendita.TOKEN.ToString() + Utils.RandomString(3);
            oggetto.Citta = vendita.OGGETTO.COMUNE.NOME;
            oggetto.DataInserimento = vendita.DATA_INSERIMENTO;
            oggetto.VenditoreToken = vendita.PERSONA.TOKEN;
            oggetto.Foto = vendita.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == vendita.ID).Select(f => f.FOTO.FOTO1).ToList();
            oggetto.Marca = vendita.OGGETTO.MARCA.NOME;
            oggetto.Nome = vendita.NOME;
            oggetto.Punti = vendita.PUNTI;
            oggetto.Soldi = vendita.SOLDI;
            oggetto.StatoOggetto = (CondizioneOggetto)vendita.OGGETTO.CONDIZIONE;
            oggetto.TipoPagamento = (TipoPagamento)vendita.TIPO_PAGAMENTO;
            oggetto.VenditoreID = vendita.ID_PERSONA;
            oggetto.VenditoreNominativo = vendita.PERSONA.NOME + ' ' + vendita.PERSONA.COGNOME;
            oggetto.Note = vendita.NOTE_AGGIUNTIVE;
            oggetto.Quantità = vendita.OGGETTO.NUMERO_PEZZI;
        }

        private OggettoViewModel SetInfoCategoriaOggetto(OGGETTO oggetto, OggettoViewModel oggettoView)
        {
            // gestito cosi, perchè nel caso faccio macroricerche, recupero lo stesso i dati personalizzati
            // sulla specifica sottocategoria.
            // new List<int> { 14 }.IndexOf(oggetto.CategoriaID) != 1
            if (oggettoView.CategoriaID == 12)
            {
                TelefonoViewModel viewModel = new TelefonoViewModel(oggettoView);
                OGGETTO_TELEFONO model = oggetto.OGGETTO_TELEFONO.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 64)
            {
                ConsoleViewModel viewModel = new ConsoleViewModel(oggettoView);
                OGGETTO_CONSOLE model = oggetto.OGGETTO_CONSOLE.FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 13 || (oggettoView.CategoriaID >= 62 && oggettoView.CategoriaID <= 63) || oggettoView.CategoriaID == 65)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_TECNOLOGIA model = oggetto.OGGETTO_TECNOLOGIA.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 14)
            {
                ComputerViewModel viewModel = new ComputerViewModel(oggettoView);
                OGGETTO_COMPUTER model = oggetto.OGGETTO_COMPUTER.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 26)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_ELETTRODOMESTICO model = oggetto.OGGETTO_ELETTRODOMESTICO.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if ((oggettoView.CategoriaID >= 28 && oggettoView.CategoriaID <= 39) || oggettoView.CategoriaID == 41)
            {
                /*MusicaViewModel viewModel = new MusicaViewModel(oggettoView);
                OGGETTO_MUSICA model = oggetto.OGGETTO_MUSICA.FirstOrDefault();
                viewModel.formatoID = model.FORMATO;
                viewModel.formatoNome = model.FORMATO1.NOME;
                viewModel.artistaID = model.ARTISTA;
                viewModel.artistaNome = model.ARTISTA1.NOME;
                return viewModel;*/
            }
            else if (oggettoView.CategoriaID == 40)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_STRUMENTO model = oggetto.OGGETTO_STRUMENTO.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID == 45)
            {
                VideogamesViewModel viewModel = new VideogamesViewModel(oggettoView);
                OGGETTO_VIDEOGAMES model = oggetto.OGGETTO_VIDEOGAMES.FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                viewModel.genereID = model.ID_GENERE;
                viewModel.genereNome = model.GENERE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 42 && oggettoView.CategoriaID <= 47)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_GIOCO model = oggetto.OGGETTO_GIOCO.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 50 && oggettoView.CategoriaID <= 61)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggettoView);
                OGGETTO_SPORT model = oggetto.OGGETTO_SPORT.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 67 && oggettoView.CategoriaID <= 80)
            {
                VideoViewModel viewModel = new VideoViewModel(oggettoView);
                OGGETTO_VIDEO model = oggetto.OGGETTO_VIDEO.FirstOrDefault();
                viewModel.formatoID = model.ID_FORMATO;
                viewModel.formatoNome = model.FORMATO.NOME;
                viewModel.registaID = model.ID_REGISTA;
                viewModel.registaNome = model.REGISTA.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 81 && oggettoView.CategoriaID <= 85)
            {
                LibroViewModel viewModel = new LibroViewModel(oggettoView);
                OGGETTO_LIBRO model = oggetto.OGGETTO_LIBRO.FirstOrDefault();
                viewModel.autoreID = model.ID_AUTORE;
                viewModel.autoreNome = model.AUTORE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 89 && oggettoView.CategoriaID <= 93)
            {
                VeicoloViewModel viewModel = new VeicoloViewModel(oggettoView);
                OGGETTO_VEICOLO model = oggetto.OGGETTO_VEICOLO.FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.alimentazioneID = model.ID_ALIMENTAZIONE;
                viewModel.alimentazioneNome = model.ALIMENTAZIONE.NOME;
                return viewModel;
            }
            else if (oggettoView.CategoriaID >= 127 && oggettoView.CategoriaID <= 170 && oggettoView.CategoriaID != 161 && oggettoView.CategoriaID != 152 && oggettoView.CategoriaID != 141 && oggettoView.CategoriaID != 127)
            {
                VestitoViewModel viewModel = new VestitoViewModel(oggettoView);
                OGGETTO_VESTITO model = oggetto.OGGETTO_VESTITO.FirstOrDefault();
                viewModel.taglia = model.TAGLIA;
                return viewModel;
            }
            return oggettoView;
        }

        private void SetFeedbackVenditoreOggetto(DatabaseContext db, OggettoViewModel viewModel)
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