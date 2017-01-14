using Facebook;
using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Controllers
{
    public class UtenteController : AdvancedController
    {
        [HttpGet]
        public ActionResult Index()
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                base.RefreshPunteggioUtente(db);
            }
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.Title = Language.TitleAccess;
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        // recupera login di portaleweb
        public ActionResult Login(UtenteLoginViewModel viewModel)
        {
            ViewBag.Title = Language.TitleAccess;
            if (ModelState.IsValid)
            {
                try
                {
                    if ((Session["pagamento"] == null ? false : (new PagamentoViewModel((PagamentoAbstractModel)Session["pagamento"])).EmailReceivent.Equals(viewModel.Email)))
                    {
                        // non puoi accedere con la stessa mail a cui devi effetturare il pagamento
                        ModelState.AddModelError("Error", string.Concat(Language.ErrorPaymentLogin + viewModel.Email));
                    }
                    else
                    {
                        // ricerca e validazione utente
                        PBKDF2 crypto = new PBKDF2();
                        using (DatabaseContext db = new DatabaseContext())
                        {
                            PERSONA_EMAIL model = db.PERSONA_EMAIL.SingleOrDefault(
                                    item =>
                                    item.EMAIL == viewModel.Email
                                    && item.TIPO == (int)TipoEmail.Registrazione && item.PERSONA.STATO == (int)Stato.ATTIVO);
                            if (model == null)
                            {
                                ModelState.AddModelError("Error", Language.EmailNotExist);
                            }
                            else if (!model.PERSONA.PASSWORD.Equals(crypto.Compute(viewModel.Password, model.PERSONA.TOKEN_PASSWORD)))
                            {
                                ModelState.AddModelError("Error", Language.ErrorPassword);
                            }
                            else
                            {
                                // login effettuata con successo
                                if (model.PERSONA.DATA_ACCESSO == null || DateTime.Now.DayOfYear > model.PERSONA.DATA_ACCESSO.Value.DayOfYear)
                                    AddPuntiLogin(db, model.PERSONA);

                                setSessioneUtente(base.Session, db, model.ID_PERSONA, viewModel.RicordaLogin);
                                // sistemare il return, perchè va in conflitto con il allowonlyanonymous
                                return Redirect((string.IsNullOrWhiteSpace(viewModel.ReturnUrl)) ? FormsAuthentication.DefaultUrl : viewModel.ReturnUrl);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            return View(viewModel);
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        public ActionResult LoginForPay(string returnUrl)
        {
            ViewBag.Title = Language.TitleAccess;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email" // Add other permissions as needed
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;

            // Store the access token in the session
            Session["AccessToken"] = accessToken;

            // update the facebook client with the access token so 
            // we can make requests on behalf of the user
            fb.AccessToken = accessToken;

            // Get the user's information
            dynamic me = fb.Get("me?fields=first_name,last_name,id,email");
            string email = me.email;

            // Set the auth cookie
            FormsAuthentication.SetAuthCookie(email, false);
            FacebookClient app = new FacebookClient(accessToken);

            dynamic result2 = app.Post("https://graph.facebook.com/oauth/access_token", new {
                grant_type = "fb_exchange_token",
                client_id = ConfigurationManager.AppSettings["FacebookApiId"],
                client_secret = ConfigurationManager.AppSettings["FacebookApiSecret"],
                fb_exchange_token = accessToken
            });

            dynamic token = app.Get("https://graph.facebook.com/me/accounts", new
            {
                access_token = result2[0]
            });

            var logJson = Newtonsoft.Json.JsonConvert.SerializeObject(token);

            //dynamic result2 = app.Post("/" + ConfigurationManager.AppSettings["FanPageID"] + "/feed", new Dictionary<string, object> { { "message", "This Post was made from my website" } });
            return Redirect((string.IsNullOrWhiteSpace(RedirectUri.AbsoluteUri)) ? FormsAuthentication.DefaultUrl : RedirectUri.AbsoluteUri);
        }

        [HttpGet]
        public ActionResult CambioPassword()
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            UtenteCambioPasswordViewModel model = new UtenteCambioPasswordViewModel()
            {
                Password = utente.Persona.PASSWORD,
                ConfermaPassword = utente.Persona.PASSWORD
            };
            return base.View(model);
        }

        [HttpPost]
        public ActionResult CambioPassword(UtenteCambioPasswordViewModel model)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = base.Session["utente"] as PersonaModel;
                    PBKDF2 crypto = new PBKDF2();
                    utente.Persona.PASSWORD = crypto.Compute(model.Password, utente.Persona.TOKEN_PASSWORD);
                    db.Entry<PERSONA>(utente.Persona).State = EntityState.Modified;
                    if (db.SaveChanges() > 0)
                    {
                        base.Session["utente"] = utente;
                        base.TempData["salvato"] = true;
                    }
                }
            }
            return base.View(model);
        }

        [HttpGet]
        public ActionResult Impostazioni()
        {
            PersonaModel utente = base.Session["utente"] as PersonaModel;
            UtenteImpostazioniViewModel model = new UtenteImpostazioniViewModel();
            using (DatabaseContext db = new DatabaseContext())
            {
                utente.Persona = db.PERSONA.FirstOrDefault(u => u.ID == utente.Persona.ID);
                model.Email = utente.Email.SingleOrDefault(item => 
                                    item.ID_PERSONA == utente.Persona.ID && item.TIPO == (int)TipoEmail.Registrazione)
                                    .EMAIL;
                model.Nome = utente.Persona.NOME;
                model.Cognome = utente.Persona.COGNOME;
                PERSONA_TELEFONO modelTelefono = utente.Telefono.SingleOrDefault(item =>
                    item.ID_PERSONA == utente.Persona.ID && item.TIPO == (int)TipoTelefono.Privato);
                if (modelTelefono != null)
                    model.Telefono = modelTelefono.TELEFONO;
                PERSONA_INDIRIZZO modelIndirizzo = utente.Indirizzo.SingleOrDefault(item =>
                    item.ID_PERSONA == utente.Persona.ID && item.TIPO == (int)TipoIndirizzo.Residenza);

                if (modelIndirizzo != null && modelIndirizzo.INDIRIZZO != null)
                {
                    model.Citta = modelIndirizzo.INDIRIZZO.COMUNE.NOME;
                    model.IDCitta = modelIndirizzo.INDIRIZZO.ID_COMUNE;
                    model.Indirizzo = modelIndirizzo.INDIRIZZO.INDIRIZZO1;
                    model.Civico = modelIndirizzo.INDIRIZZO.CIVICO;
                }
            }
            return base.View(model);
        }

        [HttpPost]
        public ActionResult Impostazioni(UtenteImpostazioniViewModel model)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = base.Session["utente"] as PersonaModel;
                    utente.SetEmail(db, model.Email);
                    utente.SetTelefono(db, model.Telefono);
                    utente.SetIndirizzo(db, model.IDCitta, model.Indirizzo, model.Civico, (int)TipoIndirizzo.Residenza);
                    utente.SetIndirizzo(db, model.IDCittaSpedizione, model.IndirizzoSpedizione, model.CivicoSpedizione, (int)TipoIndirizzo.Spedizione);
                    if (utente.Persona.NOME != model.Nome || utente.Persona.COGNOME != model.Cognome) {
                        utente.Persona.NOME = model.Nome;
                        utente.Persona.COGNOME = model.Cognome;
                        utente.Persona.DATA_MODIFICA = DateTime.Now;
                        db.Entry(utente.Persona).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    base.Session["utente"] = utente;
                    base.TempData["salvato"] = true;
                }
            }
            return base.View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            base.Session.Clear();
            base.Session.Abandon();
            return base.RedirectToAction("Login");
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult PasswordDimenticata()
        {
            ViewBag.Title = Language.TitleForgotPassword;
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordDimenticata(UtentePasswordDimenticataViewModel model)
        {
            ViewBag.Title = Language.TitleForgotPassword;
            if (base.ModelState.IsValid)
            {
                model.NuovaPassword = Utils.RandomString(10);
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            PERSONA_EMAIL utenteEmail = db.PERSONA_EMAIL
                                .Where(item => item.EMAIL.Equals(model.Email) &&
                                    item.TIPO == (int)TipoEmail.Registrazione &&
                                    item.PERSONA.STATO == (int)Stato.ATTIVO)
                                .SingleOrDefault();
                            if (utenteEmail!= null)
                            {
                                PBKDF2 crypto = new PBKDF2();
                                //utente.TOKEN_PASSWORD = crypto.GenerateSalt(1, 20);
                                utenteEmail.PERSONA.PASSWORD = crypto.Compute(model.NuovaPassword, utenteEmail.PERSONA.TOKEN_PASSWORD);
                                utenteEmail.PERSONA.DATA_MODIFICA = DateTime.Now;

                                if (db.SaveChanges() > 0)
                                {
                                    // invio email nuova password
                                    EmailModel email = new EmailModel(ControllerContext);
                                    email.To.Add(new System.Net.Mail.MailAddress(model.Email));
                                    email.Subject = Email.ForgotPasswordSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                                    email.Body = "PasswordDimenticata";
                                    email.DatiEmail = model;
                                    email.SendAsync = false;
                                    if (new EmailController().SendEmail(email))
                                    {
                                        transaction.Commit();
                                        base.TempData["salvato"] = true;
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        base.ModelState.AddModelError("", Language.ErrorSendForgotPassword);
                                    }
                                }
                                else
                                {
                                    base.ModelState.AddModelError("", Language.ErrorForgotPassword);
                                }
                            }
                            else
                            {
                                base.ModelState.AddModelError("", Language.ErrorForgotPassword);
                            }
                            
                        }
                        catch (Exception exception)
                        {
                            transaction.Rollback();
                            base.ModelState.AddModelError("", exception.Message);
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        }
                    }
                }
            }
            return base.View(model);
        }

        [HttpGet]
        public ActionResult ReclamoOrdine()
        {
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult Registrazione()
        {
            return base.View();
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registrazione(UtenteRegistrazioneViewModel model)
        {
            if (base.ModelState.IsValid)
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    using (DbContextTransaction transazione = db.Database.BeginTransaction())
                    {
                        try
                        {
                            CONTO_CORRENTE conto = db.CONTO_CORRENTE.Create();
                            conto.ID = Guid.NewGuid();
                            conto.TOKEN = Guid.NewGuid();
                            conto.DATA_INSERIMENTO = DateTime.Now;
                            conto.STATO = (int)Stato.ATTIVO;
                            db.CONTO_CORRENTE.Add(conto);
                            db.SaveChanges();
                            PBKDF2 crypto = new PBKDF2();
                            PERSONA persona = db.PERSONA.Create();
                            persona.TOKEN = Guid.NewGuid();
                            persona.TOKEN_PASSWORD = crypto.GenerateSalt(1, 20);
                            persona.PASSWORD = crypto.Compute(model.Password.Trim(), persona.TOKEN_PASSWORD);
                            persona.NOME = model.Nome.Trim();
                            persona.COGNOME = model.Cognome.Trim();
                            persona.ID_CONTO_CORRENTE = conto.ID;
                            persona.ID_ABBONAMENTO = db.ABBONAMENTO.SingleOrDefault(item => item.NOME == "BASE").ID;
                            persona.DATA_INSERIMENTO = DateTime.Now;
                            db.PERSONA.Add(persona);
                            if (db.SaveChanges() > 0)
                            {
                                PERSONA_EMAIL personaEmail = db.PERSONA_EMAIL.Create();
                                personaEmail.ID_PERSONA = persona.ID;
                                personaEmail.EMAIL = model.Email.Trim();
                                personaEmail.TIPO = (int)TipoEmail.Registrazione;
                                personaEmail.DATA_INSERIMENTO = DateTime.Now;
                                personaEmail.STATO = (int)Stato.ATTIVO;
                                db.PERSONA_EMAIL.Add(personaEmail);

                                if (!string.IsNullOrWhiteSpace(model.Telefono))
                                {
                                    PERSONA_TELEFONO personaTelefono = db.PERSONA_TELEFONO.Create();
                                    personaTelefono.ID_PERSONA = persona.ID;
                                    personaTelefono.TELEFONO = model.Telefono;
                                    personaTelefono.TIPO = (int)TipoTelefono.Privato;
                                    personaTelefono.DATA_INSERIMENTO = DateTime.Now;
                                    personaTelefono.STATO = (int)Stato.ATTIVO;
                                    db.PERSONA_TELEFONO.Add(personaTelefono);
                                }

                                PERSONA_PRIVACY personaPrivacy = db.PERSONA_PRIVACY.Create();
                                personaPrivacy.ID_PERSONA = persona.ID;
                                personaPrivacy.ACCETTA_CONDIZIONE = model.AccettaCondizioni;
                                personaPrivacy.DATA_INSERIMENTO = DateTime.Now;
                                personaPrivacy.STATO = (int)Stato.ATTIVO;
                                db.PERSONA_PRIVACY.Add(personaPrivacy);

                                db.SaveChanges();
                                base.TempData["salvato"] = true;
                                // invio email registrazione
                                EmailModel email = new EmailModel(ControllerContext);
                                email.To.Add(new System.Net.Mail.MailAddress(personaEmail.EMAIL, persona.NOME + " " + persona.COGNOME));
                                email.Subject = Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
                                email.Body = "RegistrazioneUtente";
                                email.DatiEmail = new RegistrazioneEmailModel(model)
                                {
                                    PasswordCodificata = persona.PASSWORD
                                };
                                new EmailController().SendEmail(email);
                                transazione.Commit();
                                return View();
                            }

                        }
                        catch (Exception exception)
                        {
                            transazione.Rollback();
                            Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                        }
                    }
                }
            }

            base.ModelState.AddModelError("Errore", Language.ErrorRegister);
            return View(model);
        }

        [AllowAnonymous]
        [OnlyAnonymous]
        [HttpGet]
        public ActionResult ReinvioEmailRegistrazione(UtenteRegistrazioneViewModel model)
        {
            // invio email registrazione
            EmailModel email = new EmailModel(ControllerContext);
            email.To.Add(new System.Net.Mail.MailAddress(model.Email));
            email.Subject = Email.RegistrationSubject + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "RegistrazioneUtente";
            email.DatiEmail = model;
            EmailController emailer = new EmailController();
            return Json(emailer.SendEmail(email));
        }

        [HttpGet]
        public ActionResult SaldoPunti(int pagina = 1)
        {
            if (pagina == 0)
                pagina = 1;
            ViewData["Pagina"] = pagina;
            return base.View("SaldoPunti", GetListaBonus(pagina-1));
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Iscrizione(string token)
        {
            TempData["Messaggio"] = Language.ErrorActiveUser1;
            string tokenDecodificato = Utils.DecodeToString(HttpUtility.UrlDecode(token));
            // verifica tramite email+password dell'esistenza dell'utente e attivazione dello stesso
            using (DatabaseContext db = new DatabaseContext())
            {
                using (DbContextTransaction transazione = db.Database.BeginTransaction())
                {
                    try
                    {
                        PERSONA persona = db.PERSONA_EMAIL.Where(u => (u.EMAIL + u.PERSONA.PASSWORD) == tokenDecodificato && u.PERSONA.STATO == (int)Stato.INATTIVO).Select(u => u.PERSONA).SingleOrDefault();
                        if (persona != null)
                        {
                            int punti = Convert.ToInt32(ConfigurationManager.AppSettings["bonusIscrizione"]);
                            persona.STATO = (int)Stato.ATTIVO;
                            persona.DATA_MODIFICA = DateTime.Now;
                            if (db.SaveChanges() > 0)
                            {
                                // salva log bonus iscrizione se non è già presente
                                if (db.TRANSAZIONE.Count(item => item.ID_CONTO_DESTINATARIO == persona.ID_CONTO_CORRENTE && item.TIPO == (int)TipoTransazione.BonusIscrizione) <= 0)
                                {
                                    Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                                    this.AddBonus(db, persona, tokenPortale, punti, TipoTransazione.BonusIscrizione, Bonus.Registration);
                                }
                                TempData["Messaggio"] = string.Format(Language.ActivatedUser, ConfigurationManager.AppSettings["bonusIscrizione"] + " " + Language.Moneta, ConfigurationManager.AppSettings["bonusPubblicazioniIniziali"] + " " + Language.Moneta);
                                transazione.Commit();
                                return View();
                            }
                            TempData["Messaggio"] = Language.ErrorActiveUser2;
                            transazione.Rollback();
                        }
                    }
                    catch (Exception eccezione)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                        transazione.Rollback();
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Disiscrizione(string token)
        {
            TempData["Messaggio"] = Language.ErrorRemoveUser1;
            string tokenDecodificato = Utils.DecodeToString(HttpUtility.UrlDecode(token));
            // verifica tramite email+password+token+id dell'esistenza dell'utente e attivazione dello stesso
            using (DatabaseContext db = new DatabaseContext())
            {
                PERSONA persona = db.PERSONA_EMAIL.Where(u => (u.EMAIL + u.PERSONA.PASSWORD) == tokenDecodificato && u.PERSONA.STATO == (int)Stato.INATTIVO).Select(u => u.PERSONA).SingleOrDefault();
                if (persona != null)
                {
                    persona.STATO = (int)Stato.ELIMINATO;
                    persona.DATA_MODIFICA = DateTime.Now;
                    if (db.SaveChanges() > 0)
                    {
                        TempData["Messaggio"] = Language.RemoveUser;
                        return View();
                    }
                    TempData["Messaggio"] = Language.ErrorRemoveUser2;
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SavePagamento(string idOfferta)
        {
            bool isPagato = false;
            using (DatabaseContext db = new DatabaseContext())
            {
                if (!string.IsNullOrEmpty(idOfferta))
                {
                }
            }
            return base.View(isPagato);
        }

        [HttpGet]
        public ActionResult RicercheSalvate(int pagina = 1)
        {
            ViewBag.Title = MetaTag.TitleSearchSaved;
            if (pagina == 0)
                pagina = 1;
            ViewData["Pagina"] = pagina;
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);

                 var query = db.PERSONA_RICERCA
                    .Where(item => item.ID_PERSONA == idUtente && item.STATO == (int)Stato.ATTIVO);

                ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);

                List<UtenteRicercaViewModel> viewModel = query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Skip((pagina-1) * numeroElementi)
                    .Take(numeroElementi)
                    .Select(item => new UtenteRicercaViewModel()
                    {
                        Id = item.ID.ToString(),
                        Testo = item.NOME,
                        Categoria = item.CATEGORIA.NOME,
                        DataInserimento = item.DATA_INSERIMENTO,
                        DataModifica = (DateTime)item.DATA_MODIFICA,
                        Stato = (Stato)item.STATO
                    })
                    .ToList();
                return View(viewModel);
            }
        }

        [HttpDelete]
        [ValidateAjax]
        public ActionResult RicercheSalvate(List<string> ricerca)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                ricerca.ForEach(item =>
                {
                    string id = Server.UrlDecode(item);
                    string idPulito = id.Remove(id.Length - 3, 3).Remove(0, 3);
                    int idRicerca = Utils.DecodeToInt(idPulito);
                    PERSONA_RICERCA model = db.PERSONA_RICERCA.SingleOrDefault(m => m.ID == idRicerca);
                    db.PERSONA_RICERCA.Remove(model);
                });
                db.SaveChanges();
            }
            return Json(true);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult AddToCarrello(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpDelete]
        [ValidateAjax]
        public JsonResult DeleteFromCarrello(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult SaveCarrello(int idAnnuncio)
        {
            // da fare solo se l'utente... boh
            // Aggiungo nel carrello dell'utente gli annunci in lista
            return Json(true);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult AddToPossiedo(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpPost]
        [ValidateAjax]
        public JsonResult AddToDesidero(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpDelete]
        [ValidateAjax]
        public JsonResult DeleteFromPossiedo(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        [HttpDelete]
        [ValidateAjax]
        public JsonResult DeleteFromDesidero(int idAnnuncio)
        {
            // verifico esistenza annuncio
            // salvo tra gli acquisti ma nello stato inattivo (poi andare a verificare procedura di acquisto immediato)
            // lato client riporto nei cookie l'annuncio nel carrello (ogni )
            return Json(true);
        }

        #region METODI PRIVATI

        private bool isValid(string email, string password, ref string errore, ref PERSONA utente)
        {
            bool flag;
            PBKDF2 crypto = new PBKDF2();
            using (DatabaseContext db = new DatabaseContext())
            {
                utente = db.PERSONA_EMAIL.Where(u => u.EMAIL == email && u.PERSONA.STATO == (int)Stato.ATTIVO).Select(u => u.PERSONA).SingleOrDefault();
                //utente = db.PERSONA.FirstOrDefault<PERSONA>((PERSONA u) => u.EMAIL == email && u.STATO == 1);
                if (utente == null)
                {
                    errore = Language.EmailNotExist;
                }
                else if (!utente.PASSWORD.Equals(crypto.Compute(password, utente.TOKEN_PASSWORD)))
                {
                    errore = Language.ErrorPassword;
                }
                else
                {
                    flag = true;
                    return flag;
                }
            }
            flag = false;
            return flag;
        }

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }

        private void AddPuntiLogin(DatabaseContext db, PERSONA utente)
        {
            int puntiAccesso = Convert.ToInt32(ConfigurationManager.AppSettings["bonusAccesso"]);
            utente.DATA_ACCESSO = DateTime.Now;
            db.Entry(utente).State = EntityState.Modified;
            if (db.SaveChanges() > 0)
            {
                Guid tokenPortale = Guid.Parse(ConfigurationManager.AppSettings["portaleweb"]);
                this.AddBonus(db, utente, tokenPortale, puntiAccesso, TipoTransazione.BonusLogin, Bonus.Login);
                //db.SaveChanges();
            }
        }

        private List<TRANSAZIONE> GetListaBonus(int pagina)
        {
            List<TRANSAZIONE> model = new List<TRANSAZIONE>();
            using (DatabaseContext db = new DatabaseContext())
            {
                HttpCookie cookie = new HttpCookie("notifiche");
                int num = base.CountOfferteNonConfermate(db);
                cookie["InConferma"] = num.ToString();
                num = base.CountOfferteRicevute(db);
                cookie["Ricevute"] = num.ToString();
                base.RefreshPunteggioUtente(db);

                Guid utente = (Session["utente"] as PersonaModel).Persona.ID_CONTO_CORRENTE;
                var query = db.TRANSAZIONE.Where(item => item.ID_CONTO_DESTINATARIO == utente && 
                    (item.TIPO == (int)TipoTransazione.BonusIscrizione ||
                    item.TIPO == (int)TipoTransazione.BonusIscrizionePartner ||
                    item.TIPO == (int)TipoTransazione.BonusLogin ||
                    item.TIPO == (int)TipoTransazione.BonusPartner ||
                    item.TIPO == (int)TipoTransazione.BonusPubblicazioneIniziale ||
                    item.TIPO == (int)TipoTransazione.BonusFeedback)
                    && 
                    item.STATO == (int)StatoPagamento.ACCETTATO);
                int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);

                return query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Skip(pagina * numeroElementi)
                    .Take(numeroElementi)
                    .ToList();
            }
        }

        #endregion
    }
}