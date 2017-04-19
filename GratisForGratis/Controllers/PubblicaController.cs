using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GratisForGratis.Models;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using Elmah;
using Facebook;
using System.Configuration;
using System.Data.Entity;
using System.Web.Configuration;
using GratisForGratis.App_GlobalResources;

namespace GratisForGratis.Controllers
{
    public class PubblicaController : AdvancedController
    {
        #region ACTION

        // STEP1 - informazioni base oggetto
        [HttpGet]
        public ActionResult Oggetto()
        {
            Session.Remove("pubblicazioneoggetto");
            Session.Remove("foto");
            string directory = Path.Combine(Server.MapPath("~/Temp/Images/"), Session.SessionID);
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            return View(new PubblicaOggettoViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Oggetto(PubblicaOggettoViewModel model)
        {
            try
            {
                if (Session["foto"] == null || ((List<string>)Session["foto"]).Count <= 0)
                    ModelState.AddModelError("Errore", Language.MandatoryPhoto);

                if (ModelState.IsValid)
                {
                    Session["pubblicazioneoggetto"] = model;
                    // verifico se l'utente deve inserire ulteriori informazioni
                    TipoAcquisto tipoAcquisto = TipoAcquisto.Oggetto;
                    List<FINDSOTTOCATEGORIE_Result> listaCategorie = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
                    FINDSOTTOCATEGORIE_Result categoria = listaCategorie.SingleOrDefault(item => item.TIPO_VENDITA == (int)TipoAcquisto.Oggetto && item.ID == model.CategoriaId);
                    string paginaAltreInfo = string.Concat(tipoAcquisto.ToString(), "/", categoria.DESCRIZIONE);
                    if (ViewEngines.Engines.FindView(ControllerContext, paginaAltreInfo, null).View != null)
                        return View(paginaAltreInfo);
                    else if (this.SalvaVenditaSemplice(TipoAcquisto.Oggetto))
                        return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });
                }
                ModelState.AddModelError("Errore", string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("Errore", ex.Message);
            }
            return View(model);
        }

        // STEP1 - informazioni base servizio
        [HttpGet]
        public ActionResult Servizio()
        {
            Session.Remove("pubblicazioneservizio");
            Session.Remove("foto");
            string directory = Path.Combine(Server.MapPath("~/Temp/Images/"), Session.SessionID);
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
            return View(new PubblicaServizioViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Servizio(PubblicaServizioViewModel model)
        {
            try
            {
                if (Session["foto"] == null || ((List<string>)Session["foto"]).Count <= 0)
                    ModelState.AddModelError("Errore", Language.MandatoryPhoto);

                if (ModelState.IsValid)
                {
                    Session["pubblicazioneservizio"] = model;
                    // verifico se l'utente deve inserire ulteriori informazioni
                    TipoAcquisto tipoAcquisto = TipoAcquisto.Servizio;
                    List<FINDSOTTOCATEGORIE_Result> listaCategorie = (HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>);
                    FINDSOTTOCATEGORIE_Result categoria = listaCategorie.SingleOrDefault(item => item.TIPO_VENDITA == (int)TipoAcquisto.Servizio && item.ID == model.CategoriaId);
                    string paginaAltreInfo = string.Concat(tipoAcquisto.ToString(), "/", categoria.DESCRIZIONE);
                    if (ViewEngines.Engines.FindView(ControllerContext, paginaAltreInfo, null).View != null)
                        return View(paginaAltreInfo);
                    else if (this.SalvaVenditaSemplice(TipoAcquisto.Servizio))
                        return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Servizio });
                }
                ModelState.AddModelError("Errore", string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                ModelState.AddModelError("Errore", ex.Message);
            }

            return View(model);
        }

        // STEP4 - riepilogo pubblicazione effettuata
        [HttpGet]
        public ActionResult Conferma(TipoAcquisto tipo)
        {
            // se ancora la registrazione è incompleta, lo obbligo a concluderla
            if (CheckUtenteAttivo(0))
                return RedirectToAction("Impostazioni", "Utente");

            PubblicaViewModel pubblicazione = null;
            try
            {
                if (tipo == TipoAcquisto.Servizio)
                {
                    pubblicazione = Session["pubblicazioneservizio"] as PubblicaServizioViewModel;
                    pubblicazione.Foto = (List<string>)Session["foto"];
                }
                else
                {
                    pubblicazione = Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel;
                    pubblicazione.Foto = (List<string>)Session["foto"];
                }
            }
            catch(Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
                return RedirectToAction("Index","Utente");
            }
            if (!HttpContext.IsDebuggingEnabled)
                SendPostFacebook(pubblicazione.Nome + " GRATIS con " + pubblicazione.Punti + Language.Moneta, GetCurrentDomain() + "/Uploads/Images/" + (Session["utente"] as PersonaModel).Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione) + "/" + DateTime.Now.Year  + "/Normal/" + pubblicazione.Foto[0], GetCurrentDomain());
            return View(pubblicazione);
        }

        #region STEP3 - PAGINE DETTAGLIO ANNUNCIO DA PUBBLICARE

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TelefoniSmartphone(PubblicaTelefoniSmartphoneViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveTelefoniSmartphone,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pc(PubblicaPcViewModel model)
        {
            if (SaveVenditaOggetto(this.SavePc, model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AudioHiFi(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveTecnologia, model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Console(PubblicaConsoleViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveConsole,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Elettrodomestico(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveElettrodomestico,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Gioco(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveGioco,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Libro(PubblicaLibroViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveLibro,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Musica(PubblicaMusicaViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveMusica, model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Sport(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveSport,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Strumento(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveStrumento,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Tecnologia(PubblicaModelloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveTecnologia,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Veicolo(PubblicaVeicoloViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveVeicolo,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Vestito(PubblicaVestitoViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveVestito,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Video(PubblicaVideoViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveVideo,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Videogames(PubblicaVideogamesViewModel model)
        {
            if (SaveVenditaOggetto(this.SaveVideogames,model))
                return RedirectToAction("Conferma", new { tipo = TipoAcquisto.Oggetto });

            return View("Oggetto/" + (Session["pubblicazioneoggetto"] as PubblicaViewModel).CategoriaNome, model);
        }

        #endregion

        #endregion

        #region SERVIZI

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UploadFotoOggetto(HttpPostedFileBase file)
        {
            if (file != null && Utils.CheckFormatoFile(file))
            {
                string nomeFoto = string.Empty;
                if (Session["foto"] != null && ((List<string>)Session["foto"]).Count < 4)
                {
                    nomeFoto = UploadImmagine(file);
                    ((List<string>)Session["foto"]).Add(nomeFoto);
                }
                else if (Session["foto"] == null)
                {
                    nomeFoto = UploadImmagine(file);
                    List<string> lista = new List<string>();
                    lista.Add(nomeFoto);
                    Session["foto"] = lista;
                }
                else
                {
                    //messaggio di errore
                    return Json(new { Success = false, responseText = Language.ErrorMaxUpload });
                }
                return Json(new { Success = true, responseText = nomeFoto });
            }
            //messaggio di errore
            return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        [HttpPost]
        public JsonResult AnnullaUploadFoto(string nome)
        {
            if (Session["foto"] != null)
            {
                try {
                    string pathBase = Path.Combine(Server.MapPath("~/Temp/Images/"), Session.SessionID);
                    List<string> fotoCaricate = ((List<string>)Session["foto"]);
                    string pathImgOriginale = Path.Combine(pathBase, "Original", nome);
                    string pathImgMedia = Path.Combine(pathBase, "Normal", nome);
                    string pathImgPiccola = Path.Combine(pathBase, "Little", nome);

                    System.IO.File.Delete(pathImgOriginale);
                    System.IO.File.Delete(pathImgMedia);
                    System.IO.File.Delete(pathImgPiccola);
                    // elimina da sessione
                    fotoCaricate.Remove(nome);
                    return Json(new { Success = true });
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }
            //messaggio di errore
            return Json(new { Success = false, responseText = Language.ErrorFormatFile });
        }

        // devi essere loggato
        [HttpPost]
        public string AddBaratto(BarattoOggettoViewModel model)
        {
            if (ModelState.IsValid && model.File != null)
            {
                List<string> fotoBaratto = new List<string>();
                foreach (HttpPostedFileBase file in model.File)
                {
                    fotoBaratto.Add(UploadImmagine(file));
                }

                if (fotoBaratto.Count <= 0)
                    ModelState.AddModelError("Errore", "Inserire almeno una foto!");

                PubblicaOggettoViewModel oggetto = model;
                /*if (ModelState.IsValid && SaveOggetto(db, oggetto, fotoBaratto))
                {
                    
                    return "Baratto aggiunto con successo!"; // ritornare id oggetto
                }*/
            }
            throw new Exception("Verificare dati oggetto!!");
        }

        #endregion

        #region METODI PRIVATI

        #region SALAVATAGGI

        private delegate bool SaveDettaglio(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita);

        // funzione dinamica in base alla categoria, per salvare le informazioni aggiuntive
        private bool SaveVenditaOggetto(SaveDettaglio funzione, IPubblicaOggetto iModel)
        {
            if (Session["foto"] == null || ((List<string>)Session["foto"]).Count <= 0 || Session["pubblicazioneoggetto"] == null)
                return false;

            DbContextTransaction transazione = null;
            try
            {
                if (ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        using (transazione = db.Database.BeginTransaction())
                        {
                            ANNUNCIO vendita = new ANNUNCIO();
                            PubblicaOggettoViewModel viewModel = new PubblicaOggettoViewModel();
                            viewModel.CopyAttributes(Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel);
                            if (SaveInformazioniBase(db, viewModel, (List<string>)Session["foto"], vendita) && funzione(db, iModel, vendita))
                            {
                                transazione.Commit();

                                return true;
                            }
                        }
                    }
                }
            }
            catch (System.Data.Entity.Core.EntityException ex)
            {
                if (transazione != null)
                    transazione.Rollback();
                // log errore
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                if (transazione != null)
                    transazione.Rollback();
                // log errore
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            ModelState.AddModelError("Error", Language.ErrorObjectPublish);
            return false;
        }

        private bool SaveInformazioniBase(DatabaseContext db, PubblicaOggettoViewModel model, List<string> fotoCaricate,ANNUNCIO vendita = null)
        {
            PersonaModel utente = ((PersonaModel)Session["utente"]);
            int idUtente = utente.Persona.ID;
            if (vendita == null)
                vendita = new ANNUNCIO();
            OGGETTO oggetto = db.OGGETTO.Create();
            oggetto.ID_COMUNE = model.IDCitta;
            oggetto.NUMERO_PEZZI = (model.Quantità <= 0) ? 1 : model.Quantità;

            // verifica se esiste id o marca nel DB
            if (string.IsNullOrEmpty(model.MarcaID.ToString()))
            {
                MARCA marca;
                string nomeMarca = model.Marca.Trim();
                marca = db.MARCA.Where(item => item.NOME.StartsWith(nomeMarca)).FirstOrDefault();
                if (marca == null)
                {
                    marca = new MARCA();
                    marca.NOME = nomeMarca;
                    marca.DESCRIZIONE = marca.NOME;
                    marca.ID_CATEGORIA = model.CategoriaId;
                    marca.DATA_INSERIMENTO = DateTime.Now;
                    marca.DATA_MODIFICA = marca.DATA_INSERIMENTO;
                    marca.STATO = (int)Stato.ATTIVO;
                    db.MARCA.Add(marca);
                    db.SaveChanges();
                }

                model.MarcaID = marca.ID;
            }
            oggetto.ID_MARCA = (int)model.MarcaID;

            oggetto.COLORE = model.Colore;
            oggetto.CONDIZIONE = (int)model.CondizioneOggetto;
            oggetto.ANNO = model.Anno;
                
            // conversione punti a soldi
            oggetto.DATA_INSERIMENTO = DateTime.Now;
            oggetto.DATA_MODIFICA = oggetto.DATA_INSERIMENTO;

            oggetto.STATO = (int)Stato.ATTIVO; // attiva oggetto

            db.OGGETTO.Add(oggetto);
            if (db.SaveChanges() > 0)
            {
                if (Session["portaleweb"] != null)
                {
                    PortaleWebViewModel portale = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == model.Partner.ToString()).SingleOrDefault();
                    if (portale != null)
                        vendita.ID_ATTIVITA = Convert.ToInt32(portale.Id);
                }
                vendita.ID_PERSONA = idUtente;
                vendita.ID_CATEGORIA = model.CategoriaId;
                vendita.NOME = model.Nome;
                vendita.NOTE_AGGIUNTIVE = model.NoteAggiuntive;
                vendita.PUNTI = model.Punti;
                vendita.TIPO_PAGAMENTO = (int)model.TipoPagamento;
                vendita.SOLDI = Utils.cambioValuta(vendita.PUNTI);
                vendita.TOKEN = Guid.NewGuid();
                vendita.DATA_INSERIMENTO = DateTime.Now;
                vendita.ID_OGGETTO = oggetto.ID;
                // se l'utente non ha completato la registrazione lo stato non è attivo
                vendita.STATO = (int)StatoVendita.INATTIVO;
                if (utente.Persona.STATO == (int)Stato.ATTIVO)
                    vendita.STATO = (int)StatoVendita.ATTIVO;

                db.ANNUNCIO.Add(vendita);
                if (db.SaveChanges() > 0)
                {
                    foreach (string nomeFoto in fotoCaricate)
                    {
                        AnnuncioFoto foto = new AnnuncioFoto();
                        foto.Add(db, Server, Session.SessionID, utente.Persona.TOKEN, vendita.ID, nomeFoto);
                    }
                    /*
                    if (db.SaveChanges() > 0)
                    {*/
                    try { 
                        string fotoNotifica = "/Uploads/Images/" + utente.Persona.TOKEN + "/" + DateTime.Now.Year.ToString() + "/Normal/" + fotoCaricate[0];
                        EmailModel email = new EmailModel(ControllerContext);
                        EmailController emailController = new EmailController();
                        email.Body = emailController.RenderRazorViewToString(ControllerContext, "Email/NotificaRicerca", email.Layout, null);

                        string nomeVendita = vendita.NOME;
                        string tokenVendita = Utils.RandomString(3) + Utils.Encode(vendita.TOKEN.ToString()) + Utils.RandomString(3);

                        System.Threading.Tasks.Task.Run(() => this.InviaNotifichePubblicazione(idUtente, nomeVendita,tokenVendita, model.CategoriaId, "Oggetto", email));
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    this.SaveBonusPubblicazione(db);
                    return true;
                    //}
                }
            }
            return false;
        }

        private async void InviaNotifichePubblicazione(int idUtente,string nomeVendita, string tokenVendita, int categoria, string tipologia, EmailModel email)
        {
            await System.Threading.Tasks.Task.Delay(10);
            using (DatabaseContext db2 = new DatabaseContext())
            {
                double maxGiorniRicerca = Convert.ToInt32(
                        System.Web.Configuration.WebConfigurationManager.AppSettings["maxGiorniRicerca"]
                        );
                string tokenVenditaCodificato = HttpUtility.UrlEncode(tokenVendita);

                db2.PERSONA_RICERCA.Where(r => (r.ID_CATEGORIA == categoria || r.CATEGORIA.ID_PADRE == categoria || r.ID_CATEGORIA == 1 ) && (r.NOME.Contains(nomeVendita) || nomeVendita.Contains(r.NOME)) &&
                    r.ID_PERSONA != idUtente && DbFunctions.DiffDays(DateTime.Now, r.DATA_INSERIMENTO) < maxGiorniRicerca)
                    .ToList().ForEach(r =>
                    {
                        string tokenRicerca = Utils.RandomString(3) + Utils.Encode(r.ID.ToString()) + Utils.RandomString(3);
                        string tokenRicercaCodificato = HttpUtility.UrlEncode(tokenRicerca);
                        string indirizzoEmail = r.PERSONA.PERSONA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                        string emailCodificata = HttpUtility.UrlEncode(indirizzoEmail);
                        email.To.Add(new System.Net.Mail.MailAddress(indirizzoEmail, r.PERSONA.NOME + ' ' + r.PERSONA.COGNOME));
                        email.Subject = string.Format(Email.SearchNotifySubject, r.NOME) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                        email.Body = string.Format(Server.UrlDecode(email.Body), r.NOME, tipologia, tokenVenditaCodificato, tokenRicercaCodificato, nomeVendita, emailCodificata);
                        new EmailController().SendEmailByThread(email);
                    });
            }
        }

        private bool SaveTelefoniSmartphone(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            PubblicaTelefoniSmartphoneViewModel model = (PubblicaTelefoniSmartphoneViewModel)iModel;
            OGGETTO_TELEFONO oggetto = new OGGETTO_TELEFONO();
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_SISTEMA_OPERATIVO = checkSistemaOperativo(db, model.SistemaOperativoID, model.SistemaOperativo, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_TELEFONO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SavePc(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_COMPUTER oggetto = new OGGETTO_COMPUTER();
            PubblicaPcViewModel model = (PubblicaPcViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_SISTEMA_OPERATIVO = checkSistemaOperativo(db, model.SistemaOperativoID, model.SistemaOperativo, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_COMPUTER.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveTecnologia(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_TECNOLOGIA oggetto = new OGGETTO_TECNOLOGIA();
            PubblicaModelloViewModel model = (PubblicaModelloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_TECNOLOGIA.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveConsole(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_CONSOLE oggetto = new OGGETTO_CONSOLE();
            PubblicaConsoleViewModel model = (PubblicaConsoleViewModel)iModel;
            oggetto.ID_PIATTAFORMA = checkPiattaforma(db, model.PiattaformaID, model.Piattaforma, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_CONSOLE.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveElettrodomestico(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_ELETTRODOMESTICO oggetto = new OGGETTO_ELETTRODOMESTICO();
            PubblicaModelloViewModel model = (PubblicaModelloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_ELETTRODOMESTICO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveGioco(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_GIOCO oggetto = new OGGETTO_GIOCO();
            PubblicaModelloViewModel model = (PubblicaModelloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_GIOCO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveLibro(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_LIBRO oggetto = new OGGETTO_LIBRO();
            PubblicaLibroViewModel model = (PubblicaLibroViewModel)iModel;
            oggetto.ID_AUTORE = checkAutore(db, model.AutoreID, model.Autore, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_LIBRO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveMusica(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_MUSICA oggetto = new OGGETTO_MUSICA();
            PubblicaMusicaViewModel model = (PubblicaMusicaViewModel)iModel;
            oggetto.ID_FORMATO = checkFormato(db, model.FormatoID, model.Formato, vendita.ID_CATEGORIA);
            oggetto.ID_ARTISTA = checkArtista(db, model.ArtistaID, model.Artista, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_MUSICA.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveSport(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_SPORT oggetto = new OGGETTO_SPORT();
            PubblicaModelloViewModel model = (PubblicaModelloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_SPORT.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveStrumento(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_STRUMENTO oggetto = new OGGETTO_STRUMENTO();
            PubblicaModelloViewModel model = (PubblicaModelloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_STRUMENTO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveVeicolo(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_VEICOLO oggetto = new OGGETTO_VEICOLO();
            PubblicaVeicoloViewModel model = (PubblicaVeicoloViewModel)iModel;
            oggetto.ID_MODELLO = checkModello(db, model.ModelloID, model.Modello, vendita.OGGETTO.ID_MARCA);
            oggetto.ID_ALIMENTAZIONE = checkAlimentazione(db, model.AlimentazioneID, model.Alimentazione, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_VEICOLO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveVestito(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_VESTITO oggetto = new OGGETTO_VESTITO();
            PubblicaVestitoViewModel model = (PubblicaVestitoViewModel)iModel;
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            oggetto.TAGLIA = model.Taglia;
            db.OGGETTO_VESTITO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveVideo(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_VIDEO oggetto = new OGGETTO_VIDEO();
            PubblicaVideoViewModel model = (PubblicaVideoViewModel)iModel;
            oggetto.ID_FORMATO = checkFormato(db, model.FormatoID, model.Formato, vendita.ID_CATEGORIA);
            oggetto.ID_REGISTA = checkRegista(db, model.RegistaID, model.Regista, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_VIDEO.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveVideogames(DatabaseContext db, IPubblicaOggetto iModel, ANNUNCIO vendita)
        {
            OGGETTO_VIDEOGAMES oggetto = new OGGETTO_VIDEOGAMES();
            PubblicaVideogamesViewModel model = (PubblicaVideogamesViewModel)iModel;
            oggetto.ID_PIATTAFORMA = checkPiattaforma(db, model.PiattaformaID, model.Piattaforma, vendita.ID_CATEGORIA);
            oggetto.ID_GENERE = checkGenere(db, model.GenereID, model.Genere, vendita.ID_CATEGORIA);
            oggetto.ID_OGGETTO = (int)vendita.ID_OGGETTO;
            db.OGGETTO_VIDEOGAMES.Add(oggetto);
            return db.SaveChanges() > 0;
        }

        private bool SaveServizio(DatabaseContext db, PubblicaServizioViewModel model, List<string> fotoCaricate,ANNUNCIO vendita = null)
        {
            PersonaModel utente = ((PersonaModel)Session["utente"]);
            int idUtente = utente.Persona.ID;
            if (vendita == null)
                vendita = new ANNUNCIO();

            SERVIZIO servizio = db.SERVIZIO.Create();
            servizio.ID_COMUNE = model.IDCitta;
            if (model.Tutti)
            {
                servizio.LUNEDI = true;
                servizio.MARTEDI = true;
                servizio.MERCOLEDI = true;
                servizio.GIOVEDI = true;
                servizio.VENERDI = true;
                servizio.SABATO = true;
                servizio.DOMENICA = true;
                servizio.TUTTI = true;
            }
            else
            {
                servizio.LUNEDI = model.Lunedi;
                servizio.MARTEDI = model.Martedi;
                servizio.MERCOLEDI = model.Mercoledi;
                servizio.GIOVEDI = model.Giovedi;
                servizio.VENERDI = model.Venerdi;
                servizio.SABATO = model.Sabato;
                servizio.DOMENICA = model.Domenica;
            }
            servizio.ORA_INIZIO_FERIALI = model.OraInizioFeriali;
            servizio.ORA_INIZIO_FESTIVI = model.OraInizioFestivi;
            servizio.ORA_FINE_FERIALI = model.OraFineFeriali;
            servizio.ORA_FINE_FESTIVI = model.OraFineFestivi;
            servizio.SERVIZI_OFFERTI = model.Offerta;
            servizio.RISULTATI_FINALI = model.Risultati;
            servizio.TARIFFA = (int)model.Tariffa;
            servizio.DATA_INSERIMENTO = DateTime.Now;
            servizio.DATA_MODIFICA = servizio.DATA_INSERIMENTO;
            servizio.STATO = (int)Stato.ATTIVO; // attiva oggetto

            db.SERVIZIO.Add(servizio);
            if (db.SaveChanges() > 0)
            {
                if (Session["portaleweb"] != null && model.Partner != null)
                {
                    PortaleWebViewModel portale = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == model.Partner.ToString()).SingleOrDefault();
                    vendita.ID_ATTIVITA = Convert.ToInt32(portale.Id);
                }
                vendita.ID_PERSONA = utente.Persona.ID;
                vendita.ID_CATEGORIA = model.CategoriaId;
                vendita.NOME = model.Nome;
                vendita.NOTE_AGGIUNTIVE = model.NoteAggiuntive;
                vendita.PUNTI = model.Punti;
                vendita.TIPO_PAGAMENTO = (int)model.TipoPagamento;
                vendita.SOLDI = Utils.cambioValuta(vendita.PUNTI);
                vendita.TOKEN = Guid.NewGuid();
                vendita.DATA_INSERIMENTO = DateTime.Now;
                vendita.ID_SERVIZIO = servizio.ID;
                vendita.STATO = (int)Stato.ATTIVO;
                db.ANNUNCIO.Add(vendita);
                if (db.SaveChanges() > 0)
                {
                    foreach (string nomeFoto in fotoCaricate)
                    {
                        AnnuncioFoto foto = new AnnuncioFoto();
                        foto.Add(db, Server, Session.SessionID, utente.Persona.TOKEN, vendita.ID, nomeFoto);
                    }
                    /*if (db.SaveChanges() > 0)
                    {*/
                    try
                    {
                        string fotoNotifica = "/Uploads/Images/" + utente.Persona.TOKEN + "/" + DateTime.Now.Year.ToString() + "/Normal/" + fotoCaricate[0];
                        EmailModel email = new EmailModel(ControllerContext);
                        EmailController emailController = new EmailController();
                        email.Body = emailController.RenderRazorViewToString(ControllerContext, "Email/NotificaRicerca", email.Layout, null);

                        string nomeVendita = vendita.NOME;
                        string tokenVendita = Utils.RandomString(3) + Utils.Encode(vendita.TOKEN.ToString()) + Utils.RandomString(3);

                        System.Threading.Tasks.Task.Run(() => this.InviaNotifichePubblicazione(idUtente, nomeVendita, tokenVendita, model.CategoriaId, "Servizio", email));
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                    this.SaveBonusPubblicazione(db);
                    return true;
                    //}
                }
            }
            ModelState.AddModelError("Error",Language.ErrorServicePublish);
            return false;
        }

        private bool SaveBonusPubblicazione(DatabaseContext db)
        {
            // verifico se dare un bonus dopo un certo numero di pubblicazioni
            Guid portale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
            PersonaModel utente = (Session["utente"] as PersonaModel);
            Guid idContoCorrente = db.ATTIVITA.SingleOrDefault(p => p.TOKEN == portale).ID_CONTO_CORRENTE;
            int numeroVendite = db.ANNUNCIO.Where(v => v.ID_PERSONA == utente.Persona.ID).GroupBy(v => v.ID_CATEGORIA).Count();
            TRANSAZIONE bonus = db.TRANSAZIONE.Where(b => b.ID_CONTO_MITTENTE == idContoCorrente 
                && b.ID_CONTO_DESTINATARIO == utente.Persona.ID_CONTO_CORRENTE && b.TIPO == (int)TipoTransazione.BonusPubblicazioneIniziale).FirstOrDefault();
            if (numeroVendite == Convert.ToInt32(ConfigurationManager.AppSettings["numeroPubblicazioniBonus"]) 
                && bonus == null)
            {
                db.TRANSAZIONE.Add(new TRANSAZIONE()
                {
                    ID_CONTO_MITTENTE = idContoCorrente,
                    ID_CONTO_DESTINATARIO = utente.Persona.ID_CONTO_CORRENTE,
                    TIPO = (int)TipoBonus.PubblicazioneIniziale,
                    NOME = Bonus.InitialPubblication,
                    PUNTI = Convert.ToInt32(ConfigurationManager.AppSettings["bonusPubblicazioniIniziali"]),
                    DATA_INSERIMENTO = DateTime.Now,
                    STATO = (int)Stato.ATTIVO,
                });
                return (db.SaveChanges() > 0);
            }
            return false;
        }

        #endregion

        #region VERIFICA DATI

        private int checkModello(DatabaseContext db, int? id, string nome, int marca)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                MODELLO modello;
                string nomeModello = nome.Trim();
                modello = db.MODELLO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_MARCA == marca).FirstOrDefault();
                if (modello == null)
                {
                    modello = new MODELLO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_MARCA = marca;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.MODELLO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkSistemaOperativo(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                SISTEMA_OPERATIVO modello;
                string nomeModello = nome.Trim();
                modello = db.SISTEMA_OPERATIVO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new SISTEMA_OPERATIVO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.SISTEMA_OPERATIVO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkAlimentazione(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                ALIMENTAZIONE modello;
                string nomeModello = nome.Trim();
                modello = db.ALIMENTAZIONE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new ALIMENTAZIONE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.ALIMENTAZIONE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkArtista(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                ARTISTA modello;
                string nomeModello = nome.Trim();
                modello = db.ARTISTA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new ARTISTA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.ARTISTA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkAutore(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                AUTORE modello;
                string nomeModello = nome.Trim();
                modello = db.AUTORE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new AUTORE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.AUTORE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkFormato(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                FORMATO modello;
                string nomeModello = nome.Trim();
                modello = db.FORMATO.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new FORMATO();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.FORMATO.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkGenere(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                GENERE modello;
                string nomeModello = nome.Trim();
                modello = db.GENERE.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new GENERE();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.GENERE.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkPiattaforma(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                PIATTAFORMA modello;
                string nomeModello = nome.Trim();
                modello = db.PIATTAFORMA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new PIATTAFORMA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.PIATTAFORMA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        private int checkRegista(DatabaseContext db, int? id, string nome, int categoria)
        {
            // verifica se esiste id o modello nel DB
            if (id == null || id <= 0)
            {
                REGISTA modello;
                string nomeModello = nome.Trim();
                modello = db.REGISTA.Where(item => item.NOME.StartsWith(nomeModello) && item.ID_CATEGORIA == categoria).FirstOrDefault();
                if (modello == null)
                {
                    modello = new REGISTA();
                    modello.NOME = nomeModello;
                    modello.DESCRIZIONE = modello.NOME;
                    modello.ID_CATEGORIA = categoria;
                    modello.DATA_INSERIMENTO = DateTime.Now;
                    modello.DATA_MODIFICA = modello.DATA_INSERIMENTO;
                    modello.STATO = (int)Stato.ATTIVO;
                    db.REGISTA.Add(modello);
                    db.SaveChanges();
                }

                id = modello.ID;
            }
            return (int)id;
        }

        #endregion

        private String UploadImmagine(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                string estensione = new FileInfo(Path.GetFileName(file.FileName)).Extension;
                string nomeFileUnivoco = System.Guid.NewGuid().ToString() + estensione;
                
                string pathImgOriginale = Server.MapPath("~/Temp/Images/" + Session.SessionID + "/Original");
                string pathImgMedia = Server.MapPath("~/Temp/Images/" + Session.SessionID + "/Normal");
                string pathImgPiccola = Server.MapPath("~/Temp/Images/" + Session.SessionID + "/Little");

                Directory.CreateDirectory(pathImgOriginale);
                Directory.CreateDirectory(pathImgMedia);
                Directory.CreateDirectory(pathImgPiccola);

                file.SaveAs(Path.Combine(pathImgOriginale, nomeFileUnivoco));
                using (Image img = Image.FromFile(Path.Combine(pathImgOriginale, nomeFileUnivoco), true))
                {
                    int widthMedia = 300;
                    int heightMedia = 300;
                    int widthPiccola = 100;
                    int heightPiccola = 100;
                    // se orizzontale setto l'altezza altrimenti la larghezza
                    if (img.Width > img.Height)
                    {
                        //setto altezza img media
                        decimal ratioMedia = (decimal)widthMedia / img.Width;
                        decimal tempMedia = img.Height * ratioMedia;
                        heightMedia = (int)tempMedia;
                        //setto altezza img piccola
                        decimal ratioPiccola = (decimal)widthPiccola / img.Width;
                        decimal tempPiccola = img.Height * ratioPiccola;
                        heightPiccola = (int)tempPiccola;
                    }
                    else
                    {
                        //setto larghezza img media
                        decimal ratioMedia = (decimal)heightMedia / img.Height;
                        decimal tempMedia = img.Width * ratioMedia;
                        widthMedia = (int)tempMedia;
                        //setto larghezza img piccola
                        decimal ratioPiccola = (decimal)heightPiccola / img.Height;
                        decimal tempPiccola = img.Width * ratioPiccola;
                        widthPiccola = (int)tempPiccola;
                    }
                    using (Image imgMedia = new Bitmap(img, widthMedia, heightMedia))
                    {
                        imgMedia.Save(Path.Combine(pathImgMedia, nomeFileUnivoco));
                    }

                    using (Image imgPiccola = new Bitmap(img, widthPiccola, heightPiccola))
                    {
                        imgPiccola.Save(Path.Combine(pathImgPiccola, nomeFileUnivoco));
                    }  
                }
                return nomeFileUnivoco;
            }
            return null;
        }

        private string SendPostFacebook(string message, string picture, string link)
        {
            try
            {
                FacebookClient app = new FacebookClient(ConfigurationManager.AppSettings["TokenPermanente"]);
                Dictionary<string, object> feed = new Dictionary<string, object>() {
                    { "message", message },
                    { "picture", picture },
                    { "link", link }
                };
                var isSend = app.Post("/" + ConfigurationManager.AppSettings["FanPageID"] + "/feed", feed);
                return (string)isSend;
            }
            catch (FacebookOAuthException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (FacebookApiException ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return string.Empty;
        }

        private bool SalvaVenditaSemplice(TipoAcquisto tipoAcquisto)
        {
            bool flag;
            if ((Session["pubblicazioneoggetto"] != null ? false : Session["pubblicazioneservizio"] == null))
            {
                throw new Exception(Language.ErrorTimeOutSessionPublish);
            }
            if ((Session["pubblicazioneoggetto"] == null ? false : tipoAcquisto != TipoAcquisto.Oggetto))
            {
                throw new Exception(Language.ErrorPublish);
            }
            if ((Session["pubblicazioneservizio"] == null ? false : tipoAcquisto != TipoAcquisto.Servizio))
            {
                throw new Exception(Language.ErrorPublish);
            }
            using (DatabaseContext databaseContext = new DatabaseContext())
            {
                using (DbContextTransaction dbContextTransaction = databaseContext.Database.BeginTransaction())
                {
                    if (tipoAcquisto != TipoAcquisto.Oggetto)
                    {
                        PubblicaServizioViewModel pubblicaServizioViewModel = new PubblicaServizioViewModel();
                        pubblicaServizioViewModel.CopyAttributes(Session["pubblicazioneservizio"] as PubblicaServizioViewModel);
                        if (this.SaveServizio(databaseContext, pubblicaServizioViewModel, (List<string>)Session["foto"], null))
                        {
                            dbContextTransaction.Commit();
                            flag = true;
                            return flag;
                        }
                    }
                    else
                    {
                        PubblicaOggettoViewModel pubblicaOggettoViewModel = new PubblicaOggettoViewModel();
                        pubblicaOggettoViewModel.CopyAttributes(Session["pubblicazioneoggetto"] as PubblicaOggettoViewModel);
                        if (this.SaveInformazioniBase(databaseContext, pubblicaOggettoViewModel, (List<string>)Session["foto"], null))
                        {
                            dbContextTransaction.Commit();
                            flag = true;
                            return flag;
                        }
                    }
                }
            }
            flag = false;
            return flag;
        }

        #endregion

    }
}