using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    // aggiungere rewrite con parole trova / compra / acquista
    [HandleError]
    public class CercaController : Controller
    {
        #region ACTION

        // ricerca generica
        [HttpGet]
        public ActionResult Index(RicercaViewModel cerca)
        {
            // setta i cookie principali
            HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
            List<FINDSOTTOCATEGORIE_Result> categorie = HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>;
            var categoria = categorie.Where(c => c.ID == cerca.Cerca_IDCategoria || c.DESCRIZIONE == cerca.Cerca_Categoria)
                .OrderBy(c => c.LIVELLO).OrderBy(c => c.ID_PADRE)
                .FirstOrDefault();
            try
            {
                // recupero la categoria
                cookie["Nome"] = cerca.Cerca_Nome;
                if (categoria == null)
                    RedirectToAction("Index", "Home");
                cookie["Categoria"] = categoria.DESCRIZIONE;
                cookie["IDCategoria"] = categoria.ID.ToString();
                cookie["TipoAcquisto"] = categoria.TIPO_VENDITA.ToString();
                cookie["Livello"] = categoria.LIVELLO.ToString();
                HttpContext.Response.SetCookie(cookie);

                // cerca tra tutte le vendite
                if (categoria.ID == 1)
                {
                    ViewBag.Title = App_GlobalResources.Language.Search + " " + categoria.NOME;
                    int pagineTotali = 1;
                    int numeroRecord = 0;
                    ListaVendite lista = new ListaVendite(categoria.ID,categoria.DESCRIZIONE)
                    {
                        List = FindVendite(cookie, HttpContext.Request.Cookies.Get("filtro"), cerca.Pagina, ref pagineTotali, ref numeroRecord),
                        PageNumber = cerca.Pagina,
                        PageCount = pagineTotali,
                        TotalNumber = numeroRecord
                    };
                    return View(lista);
                }

                // verifica se salvare la ricerca
                if (cerca.Cerca_Submit == "save" && (!string.IsNullOrWhiteSpace(cerca.Cerca_Nome) || cerca.Cerca_IDCategoria > 1))
                    return RedirectToAction("SaveRicerca");
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                RedirectToAction("Index", "Home");
            }

            // reindirizzo alla action corretta in base al tipo di vendita
            if (categoria != null && categoria.TIPO_VENDITA == (int)TipoAcquisto.Servizio)
            {
                return RedirectToAction(cookie["Categoria"], "Servizi", new { categoria = categoria.ID });
            }

            return RedirectToAction(cookie["Categoria"], "Oggetti", new { categoria = categoria.ID });
        }

        // ricerca da link diretto - manca settaggio cookie funzionante e passaggio id categoria
        // controllare che nella sessione ci siano le categorie
        [HttpGet]
        public ActionResult Oggetti(string nomeCategoria = "Tutti", string sottocategoria = "", int categoria = 1, int pagina = 1)
        {
            // imposta i cookie di ricerca
            HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
            ListaOggetti lista = new ListaOggetti();
            try
            {
                List<FINDSOTTOCATEGORIE_Result> listaCategorie = HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>;
                // se esiste il parametro categoria allora cercare per quello, altrimenti per nome
                FINDSOTTOCATEGORIE_Result risultato = listaCategorie.Where(c => c.TIPO_VENDITA == 0 && c.ID == categoria
                        && c.STATO == (int)Stato.ATTIVO).OrderBy(c => c.LIVELLO).OrderBy(c => c.ID_PADRE).FirstOrDefault();
                if (risultato == null)
                    return RedirectToAction("","Cerca");
                cookie["Categoria"] = risultato.DESCRIZIONE;
                cookie["IDCategoria"] = risultato.ID.ToString();
                cookie["TipoAcquisto"] = risultato.TIPO_VENDITA.ToString();
                FINDSOTTOCATEGORIE_Result categoriaPadre = listaCategorie.Where(c => c.ID == risultato.ID_PADRE && c.TIPO_VENDITA > -1).SingleOrDefault();
                if (categoriaPadre != null)
                {
                    cookie["CategoriaPadre"] = categoriaPadre.NOME;
                    cookie["IDCategoriaPadre"] = categoriaPadre.ID.ToString();
                    sottocategoria = risultato.DESCRIZIONE;
                }

                ViewBag.Title = string.Format(App_GlobalResources.MetaTag.TitleSearch, risultato.NOME);

                HttpContext.Response.SetCookie(cookie);
                lista = GetListaOggetti(pagina, cookie);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                RedirectToAction("Index", "Home");
            }
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionSearch, cookie["Categoria"],
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = string.Format(App_GlobalResources.MetaTag.KeywordsSearch, cookie["Categoria"]);
            // se la sottocategoria è vuota, torna la view di default
            return View(sottocategoria, lista);
        }

        [HttpGet]
        public ActionResult Servizi(string nomeCategoria = "Tutti", string sottocategoria = "", int categoria = 1, int pagina = 1)
        {
            // imposta i cookie di ricerca
            HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
            ListaServizi lista = new ListaServizi();

            try
            {
                List<FINDSOTTOCATEGORIE_Result> listaCategorie = HttpContext.Application["categorie"] as List<FINDSOTTOCATEGORIE_Result>;
                // carico la ricerca per la macrocategoria o se c'è la sottocategoria
                /*var risultato = listaCategorie.Where(c =>
                    ((c.TIPO_VENDITA == 1 && c.ID == categoria) || (categoria == 1 && (c.TIPO_VENDITA == -1 || c.TIPO_VENDITA == 1) 
                    && c.LIVELLO <=0 && c.DESCRIZIONE == nomeCategoria))
                    && c.STATO == (int)Stato.ATTIVO).OrderBy(c => c.LIVELLO).OrderBy(c => c.ID_PADRE).FirstOrDefault();*/
                FINDSOTTOCATEGORIE_Result risultato = listaCategorie.Where(c => c.TIPO_VENDITA == 1 && c.ID == categoria
                        && c.STATO == (int)Stato.ATTIVO).OrderBy(c => c.LIVELLO).OrderBy(c => c.ID_PADRE).FirstOrDefault();
                if (risultato == null)
                    return RedirectToAction("", "Cerca");

                cookie["Categoria"] = risultato.DESCRIZIONE;
                cookie["IDCategoria"] = risultato.ID.ToString();
                cookie["TipoAcquisto"] = risultato.TIPO_VENDITA.ToString();
                FINDSOTTOCATEGORIE_Result categoriaPadre = listaCategorie.Where(c => c.ID == risultato.ID_PADRE && c.TIPO_VENDITA > -1).SingleOrDefault();
                if (categoriaPadre != null)
                {
                    cookie["CategoriaPadre"] = categoriaPadre.NOME;
                    cookie["IDCategoriaPadre"] = categoriaPadre.ID.ToString();
                    sottocategoria = risultato.DESCRIZIONE;
                }
                ViewBag.Title = string.Format(App_GlobalResources.MetaTag.TitleSearch, risultato.NOME);
                
                HttpContext.Response.Cookies.Set(cookie);
                lista = GetListaServizi(pagina, cookie);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                RedirectToAction("Index", "Home");
            }
            ViewBag.Description = string.Format(App_GlobalResources.MetaTag.DescriptionSearch, cookie["Categoria"],
                WebConfigurationManager.AppSettings["nomeSito"]);
            ViewBag.Keywords = string.Format(App_GlobalResources.MetaTag.KeywordsSearch, cookie["Categoria"]);
            // se la sottocategoria è vuota, torna la view di default
            return View(sottocategoria, lista);
        }

        [HttpGet]
        public ActionResult SaveRicerca()
        {
            UtenteRicercaViewModel viewModel = null;
            try { 
                if (!Request.IsAuthenticated && Session["utenteRicerca"] == null)
                    return View("Registrazione");

                HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
                using (DatabaseContext db = new DatabaseContext())
                {
                    PersonaModel utente = (Request.IsAuthenticated) ? (Session["utente"] as PersonaModel) : (Session["utenteRicerca"] as PersonaModel);
                    PERSONA_RICERCA model = new PERSONA_RICERCA();
                    model.ID_CATEGORIA = Convert.ToInt32(cookie["IDCategoria"]);
                    model.NOME = cookie["Nome"];
                    //model.UTENTE1 = utente;
                    model.ID_PERSONA = utente.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.DATA_MODIFICA = model.DATA_INSERIMENTO;
                    model.STATO = (int)Stato.ATTIVO;
                    db.PERSONA_RICERCA.Add(model);
                    if (db.SaveChanges() > 0)
                    {
                        ResetFiltriRicerca();
                        viewModel = new UtenteRicercaViewModel(model);
                        SendMailRicercaSalvata(utente, model);
                    }
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            // ritorna vista per l'inserimento della mail
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveRicerca(RegistrazioneRicercaViewModel viewModel)
        {
            try
            {
                TempData["SaveRicerca"] = App_GlobalResources.Language.ErrorSearchRegister;
                HttpCookie cookie = HttpContext.Request.Cookies.Get("ricerca");
                if (!string.IsNullOrWhiteSpace(cookie["Nome"]) || Convert.ToInt32(cookie["IDCategoria"]) > 1)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        PERSONA_RICERCA model = new PERSONA_RICERCA();
                        // se non ha già salvato una ricerca nella stessa sessione
                        if (Session["utenteRicerca"] != null || Request.IsAuthenticated)
                            return RedirectToAction("SaveRicerca");

                        PERSONA utente = db.PERSONA.Where(u => u.PERSONA_EMAIL.Count(item => item.EMAIL == viewModel.Email) > 0).SingleOrDefault();
                        PersonaModel persona = new PersonaModel();
                        if (utente == null)
                        {
                            utente = new PERSONA();
                            utente.NOME = "Non registrato";
                            utente.DATA_INSERIMENTO = DateTime.Now;
                            utente.DATA_MODIFICA = utente.DATA_INSERIMENTO;
                            utente.STATO = (int)Stato.INATTIVO;
                            db.PERSONA.Add(utente);
                            db.SaveChanges();
                            PERSONA_EMAIL personaEmail = new PERSONA_EMAIL();
                            personaEmail.ID_PERSONA = utente.ID;
                            personaEmail.EMAIL = viewModel.Email;
                            personaEmail.TIPO = (int)TipoEmail.Registrazione;
                            personaEmail.DATA_INSERIMENTO = DateTime.Now;
                            personaEmail.STATO = (int)Stato.ATTIVO;
                            db.PERSONA_EMAIL.Add(personaEmail);
                            db.SaveChanges();

                            persona = new PersonaModel(utente);
                            persona.Email.Add(personaEmail);
                        }
                        Session["utenteRicerca"] = persona;
                        //model.UTENTE1 = utente;
                        model.ID_PERSONA = utente.ID;
                        model.ID_CATEGORIA = Convert.ToInt32(cookie["IDCategoria"]);
                        model.NOME = cookie["Nome"];
                        model.DATA_INSERIMENTO = DateTime.Now;
                        model.DATA_MODIFICA = model.DATA_INSERIMENTO;
                        model.STATO = (int)Stato.ATTIVO;
                        db.PERSONA_RICERCA.Add(model);
                        if (db.SaveChanges() > 0)
                        {
                            ResetFiltriRicerca();
                            SendMailRicercaSalvata(persona,model);
                            UtenteRicercaViewModel ricercaViewModel = new UtenteRicercaViewModel(model);
                            return View(ricercaViewModel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            // ritorna vista per l'inserimento della mail
            return View("Registrazione");
        }

        [HttpGet]
        public ActionResult DeleteRicerca(string token, string emailUtente)
        {
            TempData["deleteRicerca"] = App_GlobalResources.Language.ErrorSearchDelete;
            try
            {
                int idRicerca = Utils.DecodeToInt(token.Substring(3).Substring(0, token.Length - 6));
                using (DatabaseContext db = new DatabaseContext())
                {
                    db.PERSONA_RICERCA.Remove(db.PERSONA_RICERCA.Where(r => r.ID == idRicerca && r.PERSONA.PERSONA_EMAIL.Count(item =>
                        item.EMAIL == emailUtente && item.TIPO == (int)TipoEmail.Registrazione) > 0).SingleOrDefault());
                    if (db.SaveChanges() > 0)
                        TempData["deleteRicerca"] = App_GlobalResources.Language.DescriptionSearchDelete;
                }
            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View();
        }

        #endregion

        #region SERVIZI INTERNI

        #region FILTRI

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroVendite(RicercaViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            HttpCookie cookieRicercaBase = HttpContext.Request.Cookies.Get("ricerca");

            //recupero impaginazione e dati
            int pagineTotali = 1;
            int numeroRecord = 0;
            ListaVendite lista = new ListaVendite(Convert.ToInt32(cookieRicercaBase["IdCategoria"]), cookieRicercaBase["Categoria"])
            {
                List = FindVendite(cookieRicercaBase, filtro, 1, ref pagineTotali, ref numeroRecord),
                PageNumber = 1,
                PageCount = pagineTotali,
                TotalNumber = numeroRecord
            };

            return PartialView("Index", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroOggetti(RicercaModelloViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Oggetti", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroTelefoni(RicercaTelefonoViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Telefoni-Smartphone", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroAudioHiFi(RicercaAudioHiFiViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Audio-Hi-Fi", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroPc(RicercaPcViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Pc", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroElettrodomestici(RicercaElettrodomesticoViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Elettrodomestici", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroStrumenti(RicercaStrumentiViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Strumenti-e-accessori", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroMusica(RicercaMusicaViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Musica-elettronica", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroGioco(RicercaGiocoViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Magia", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroVideogames(RicercaVideogamesViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Videogames", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroSport(RicercaSportViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Calcio", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroTecnologia(RicercaTecnologiaViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Tv", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroConsole(RicercaConsoleViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Console", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroVideo(RicercaVideoViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Avventura-azione", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroLibro(RicercaLibroViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Libri-scolastici", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroVeicolo(RicercaVeicoloViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Auto-furgoni", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroVestito(RicercaVestitoViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            // setta i filtri avanzati
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaOggetti lista = GetListaOggetti(1, null, filtro);

            return PartialView("Gonne", lista);
        }

        [HttpGet]
        [Filters.ValidateAjax]
        public ActionResult FiltroServizi(RicercaServizioViewModel ricerca)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage)));
            }
            HttpCookie filtro = ricerca.GetCookieRicerca(HttpContext.Request.Cookies.Get("filtro"));
            HttpContext.Response.Cookies.Set(filtro);

            //recupero impaginazione e dati
            ListaServizi lista = GetListaServizi(1, null, filtro);

            return PartialView("Servizi", lista);
        }

        #endregion

        /**
        ** term: inizio nome oggetto o categoria o marca o modello
        **/
        [HttpGet]
        [Authorize]
        [Filters.ValidateAjax]
        public ActionResult OggettiBarattabili(string term)
        {
            List<AutocompleteGuid> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                string termineSenzaSpazi = term.Trim();
                // aggiungere il contains e l'order by per rilevanza
                lista = db.ANNUNCIO.Where(item => item.ID_PERSONA == utente && item.STATO == (int)StatoVendita.ATTIVO && item.OGGETTO != null && (item.NOME.Contains(termineSenzaSpazi) || item.OGGETTO.MARCA.NOME.Contains(termineSenzaSpazi)))
                    .OrderByDescending(item => item.NOME.Contains(termineSenzaSpazi))
                    .Select(m => new AutocompleteGuid { Label = m.NOME, Value = (Guid)m.TOKEN }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        /**
        ** term: inizio nome servizio o categoria o marca o modello
        **/
        [HttpGet]
        [Authorize]
        [Filters.ValidateAjax]
        public ActionResult ServiziBarattabili(string term)
        {
            List<AutocompleteGuid> lista;

            using (DatabaseContext db = new DatabaseContext())
            {
                int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                lista = db.ANNUNCIO.Where(item => item.ID_PERSONA == utente && item.STATO == (int)StatoVendita.ATTIVO && item.SERVIZIO != null && (item.NOME.StartsWith(term.Trim()))).
                    Select(m => new AutocompleteGuid { Label = m.NOME, Value = (Guid)m.TOKEN }).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region METODI PRIVATI

        // FUNZIONE IN PREPARAZIONE
        private List<VenditaViewModel> FindVendite(HttpCookie ricerca, HttpCookie filtro, int pagina, ref int pagineTotali, ref int numeroRecord)
        {
            List<VenditaViewModel> lista = new List<VenditaViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = 0;
                if (Request.IsAuthenticated)
                    idUtente = ((PersonaModel)Session["utente"]).Persona.ID;

                using (var connessione = db.Database.Connection)
                {
                    connessione.Open();
                    DbCommand cmd = connessione.CreateCommand();
                    string selectText = @"SELECT V.ID, U.ID AS PERSONA, U.NOME + ' ' + U.COGNOME AS UTENTE_NOMINATIVO, 
                                U.TOKEN AS UTENTE_TOKEN, V.TOKEN, V.NOME, V.TIPO_PAGAMENTO,  
                                V.PUNTI,V.SOLDI, V.DATA_INSERIMENTO, V.ID_CATEGORIA AS CATEGORIA_ID, 
                                CATEGORIA.NOME AS CATEGORIA_NOME, P.ID AS ID_ATTIVITA,
                                CATEGORIA.TIPO_VENDITA AS TIPO_ACQUISTO,
                                P.NOME AS PARTNER_NOME, P.TOKEN AS PARTNER_TOKEN 
                                --COMUNE.NOME AS CITTA_NOME, COMUNE.ID AS CITTA_ID,
                                ";

                    string schema = @"FROM ANNUNCIO AS V
                                INNER JOIN CATEGORIA ON V.ID_CATEGORIA=CATEGORIA.ID 
                                --INNER JOIN COMUNE ON O.CITTA=COMUNE.ID
                                INNER JOIN PERSONA AS U ON V.ID_PERSONA=U.ID
                                LEFT JOIN ATTIVITA AS P ON V.ID_ATTIVITA=P.ID";

                    string condizione = @"
                                WHERE V.STATO = 1 AND CATEGORIA.NODO.IsDescendantOf(
                                        (SELECT C2.NODO FROM CATEGORIA AS C2 WHERE C2.ID=@CATEGORIA)
                                    ) = 1 AND ((SELECT COUNT(*) FROM OFFERTA WHERE OFFERTA.ID_ANNUNCIO=V.ID AND ((TIPO_OFFERTA = 0 AND OFFERTA.STATO = 1) OR OFFERTA.STATO = 4)) <= 0)
                                ";

                    if ((ricerca["Nome"] != null) && !String.IsNullOrEmpty(ricerca["Nome"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("NOME", ricerca["Nome"]));
                        condizione += " AND V.NOME LIKE @NOME + '%'";
                    }
                    if (filtro != null)
                        AddFiltriRicerca(Convert.ToInt32(ricerca["IDCategoria"]), filtro, ref cmd, ref schema, ref condizione);

                    cmd.Parameters.Add(new SqlParameter("CATEGORIA", Convert.ToInt32(ricerca["IDCategoria"])));

                    cmd.CommandText = "SELECT COUNT(*) " + schema + condizione;
                    numeroRecord = (int)cmd.ExecuteScalar();

                    pagineTotali = (int)Math.Ceiling((decimal)numeroRecord / Convert.ToDecimal(WebConfigurationManager.AppSettings["numeroAcquisti"]));

                    // impaginazione
                    cmd.CommandText = selectText + schema + condizione + " ORDER BY DATA_INSERIMENTO DESC OFFSET @INIZIO ROWS FETCH NEXT @NUMERORECORD ROWS ONLY";
                    int inizio = (pagina <= 1) ? 0 : ((pagina - 1) * Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"]));
                    cmd.Parameters.Add(new SqlParameter("INIZIO", inizio));
                    cmd.Parameters.Add(new SqlParameter("NUMERORECORD", Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"])));
                    using (DbDataReader res = cmd.ExecuteReader())
                    {
                        while (res.Read())
                        {
                            try
                            {
                                VenditaViewModel viewModel = new VenditaViewModel();
                                viewModel.Id = res["ID"].ToString();
                                viewModel.Venditore = new UtenteVenditaViewModel();
                                if (res["ID_ATTIVITA"].ToString().Trim().Length > 0)
                                {
                                    viewModel.Venditore.Id = (int)res["ID_ATTIVITA"];
                                    viewModel.Venditore.Nominativo = res["PARTNER_NOME"].ToString();
                                    SetFeedbackVenditore(db, viewModel, TipoVenditore.Attivita);
                                }
                                else
                                {
                                    viewModel.Venditore.Id = (int)res["PERSONA"];
                                    viewModel.Venditore.Nominativo = res["UTENTE_NOMINATIVO"].ToString();
                                    SetFeedbackVenditore(db, viewModel, TipoVenditore.Persona);
                                }
                                viewModel.Venditore.VenditoreToken = (Guid)res["UTENTE_TOKEN"];
                                viewModel.Token = Utils.RandomString(3) + res["TOKEN"].ToString() + Utils.RandomString(3);
                                viewModel.Nome = res["NOME"].ToString();
                                viewModel.TipoPagamento = (TipoPagamento)res["TIPO_PAGAMENTO"];
                                //viewModel.Citta = res["CITTA_NOME"].ToString();
                                viewModel.Punti = (int)res["PUNTI"];
                                viewModel.Soldi = (int)res["SOLDI"];
                                viewModel.DataInserimento = (DateTime)res["DATA_INSERIMENTO"];
                                viewModel.CategoriaID = (int)res["CATEGORIA_ID"];
                                viewModel.Categoria = res["CATEGORIA_NOME"].ToString();
                                viewModel.TipoAcquisto = (TipoAcquisto)res["TIPO_ACQUISTO"];
                                int venditaId = (int)res["ID"];
                                viewModel.Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == venditaId).Select(f =>
                                    f.FOTO.FOTO1
                                ).ToList();
                                
                                lista.Add(viewModel);
                            }
                            catch (Exception ex)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            }
                        }
                    }
                }
            }

            return lista;
        }

        private void AddFiltriRicerca(int categoriaID, HttpCookie filtro, ref DbCommand cmd, ref string schema, ref string condizione)
        {
            // filtri standard
            /*if ((filtro["Citta"] != null) && !String.IsNullOrEmpty(filtro["Citta"]))
            {
                cmd.Parameters.Add(new SqlParameter("CITTA", filtro["Citta"]));
                condizione += " AND COMUNI.Nome = @CITTA ";
            }*/
            if (!String.IsNullOrWhiteSpace((filtro["TipoPagamento"])) && (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"]) > 0)
            {
                // filtro per quelli da pagare in qualunque modo o con una modalità specifica
                cmd.Parameters.Add(new SqlParameter("TIPOPAGAMENTO", (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"])));
                condizione += " AND (V.TIPO_PAGAMENTO = @TIPOPAGAMENTO OR V.TIPO_PAGAMENTO = 0)";
            }
            if ((filtro["PuntiMin"] != null) && Convert.ToInt32(filtro["PuntiMin"]) > 0)
            {
                cmd.Parameters.Add(new SqlParameter("PUNTIMIN", Convert.ToInt32(filtro["PuntiMin"])));
                condizione += " AND V.PUNTI >= @PUNTIMIN";
            }
            if ((filtro["PuntiMax"] != null) && Convert.ToInt32(filtro["PuntiMax"]) > 0)
            {
                cmd.Parameters.Add(new SqlParameter("PUNTIMAX", Convert.ToInt32(filtro["PuntiMax"])));
                condizione += " AND V.PUNTI <= @PUNTIMAX";
            }
            /*
            if ((filtro["AnnoMin"] != null) && Convert.ToInt32(filtro["AnnoMin"]) > 0)
            {
                cmd.Parameters.Add(new SqlParameter("ANNOMIN", Convert.ToInt32(filtro["AnnoMin"])));
                condizione += " AND O.ANNO >= @ANNOMIN";
            }
            if ((filtro["AnnoMax"] != null) && Convert.ToInt32(filtro["AnnoMax"]) > 0)
            {
                cmd.Parameters.Add(new SqlParameter("ANNOMAX", Convert.ToInt32(filtro["AnnoMax"])));
                condizione += " AND O.ANNO <= @ANNOMAX";
            }*/
        }
        /**
        Recupero gli oggetti ancora in vendita, della categoria selezionata o di una sua sottocategoria,
        che non abbia ancora ricevuto un'offerta attiva di acquisto in punti o comunque accettata
        **/
        private ListaOggetti GetListaOggetti(int paginaAttuale, HttpCookie ricerca = null, HttpCookie filtro = null)
        {
            //recupero impaginazione e dati
            ListaOggetti lista = new ListaOggetti();
            int pagineTotali = 1;
            int numeroRecord = 0;
            if (ricerca == null)
                ricerca = HttpContext.Request.Cookies.Get("ricerca");
            if (filtro == null)
                filtro = HttpContext.Request.Cookies.Get("filtro");
            lista.List = FindOggetti(ricerca, filtro, paginaAttuale, ref pagineTotali, ref numeroRecord);

            lista.PageNumber = paginaAttuale;
            lista.PageCount = pagineTotali;
            lista.TotalNumber = numeroRecord;
            return lista;
        }

        private List<OggettoViewModel> FindOggetti(HttpCookie ricerca, HttpCookie filtro, int pagina, ref int pagineTotali, ref int numeroRecord)
        {
            List<OggettoViewModel> lista = new List<OggettoViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = 0;
                if (Request.IsAuthenticated)
                    idUtente = ((PersonaModel)Session["utente"]).Persona.ID;

                using (var connessione = db.Database.Connection)
                {
                    connessione.Open();
                    DbCommand cmd = connessione.CreateCommand();
                    string selectText = @"SELECT V.ID, O.ID AS OGGETTO_ID, U.ID AS PERSONA, U.NOME + ' ' + U.COGNOME AS UTENTE_NOMINATIVO, 
                                U.TOKEN AS UTENTE_TOKEN, V.TOKEN, V.NOME, V.TIPO_PAGAMENTO, COMUNE.NOME AS CITTA_NOME, V.PUNTI,V.SOLDI,
                                O.ANNO, M.ID AS MARCA_ID, M.NOME AS MARCA_NOME, O.CONDIZIONE, V.DATA_INSERIMENTO, O.NUMERO_PEZZI,
                                V.ID_CATEGORIA AS CATEGORIA_ID, CATEGORIA.NOME AS CATEGORIA_NOME, COMUNE.ID AS CITTA_ID,
                                P.ID AS ID_ATTIVITA, P.NOME AS PARTNER_NOME, P.TOKEN AS PARTNER_TOKEN, V.STATO AS STATO_VENDITA ";

                    string schema = @"FROM ANNUNCIO AS V
                                INNER JOIN CATEGORIA ON V.ID_CATEGORIA=CATEGORIA.ID 
                                INNER JOIN OGGETTO AS O ON V.ID_OGGETTO=O.ID
                                INNER JOIN COMUNE ON O.ID_COMUNE=COMUNE.ID
                                INNER JOIN MARCA AS M ON O.ID_MARCA=M.ID
                                INNER JOIN PERSONA AS U ON V.ID_PERSONA=U.ID
                                LEFT JOIN ATTIVITA AS P ON V.ID_ATTIVITA=P.ID";

                    string condizione = @"
                                WHERE V.STATO = 1 AND CATEGORIA.NODO.IsDescendantOf(
                                        (SELECT C2.NODO FROM CATEGORIA AS C2 WHERE C2.ID=@CATEGORIA)
                                    ) = 1 AND ((SELECT COUNT(*) FROM OFFERTA WHERE OFFERTA.ID_ANNUNCIO=V.ID AND ((TIPO_OFFERTA = 0 AND OFFERTA.STATO = 1) OR OFFERTA.STATO = 4)) <= 0)
                                ";

                    if ((ricerca["Nome"] != null) && !String.IsNullOrEmpty(ricerca["Nome"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("NOME", ricerca["Nome"]));
                        condizione += " AND V.NOME LIKE @NOME + '%'";
                    }

                    AddFiltriRicercaOggetti(Convert.ToInt32(ricerca["IDCategoria"]), filtro, ref cmd, ref schema,ref condizione);

                    cmd.Parameters.Add(new SqlParameter("CATEGORIA", Convert.ToInt32(ricerca["IDCategoria"])));

                    cmd.CommandText = "SELECT COUNT(*) " + schema + condizione;
                    numeroRecord = (int)cmd.ExecuteScalar();

                    pagineTotali = (int)Math.Ceiling((decimal)numeroRecord / Convert.ToDecimal(WebConfigurationManager.AppSettings["numeroAcquisti"]));

                    // impaginazione
                    cmd.CommandText = selectText + schema + condizione + " ORDER BY DATA_INSERIMENTO DESC OFFSET @INIZIO ROWS FETCH NEXT @NUMERORECORD ROWS ONLY";
                    int inizio = (pagina <= 1) ? 0 : ((pagina - 1) * Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"]));
                    cmd.Parameters.Add(new SqlParameter("INIZIO", inizio));
                    cmd.Parameters.Add(new SqlParameter("NUMERORECORD", Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"])));
                    using (DbDataReader res = cmd.ExecuteReader())
                    {
                        while (res.Read())
                        {
                            try
                            {
                                OggettoViewModel oggetto = new OggettoViewModel();
                                oggetto.VenditaID = (int)res["ID"];
                                oggetto.Id = (int)res["OGGETTO_ID"];
                                oggetto.VenditoreID = (int)res["PERSONA"];
                                oggetto.VenditoreNominativo = res["UTENTE_NOMINATIVO"].ToString();
                                if (res["ID_ATTIVITA"].ToString().Trim().Length > 0)
                                {
                                    oggetto.PartnerId = (int)res["ID_ATTIVITA"];
                                    oggetto.PartnerNominativo = res["PARTNER_NOME"].ToString();
                                }
                                SetFeedbackVenditoreOggetto(db, oggetto);
                                oggetto.Token = Utils.RandomString(3) + res["TOKEN"].ToString() + Utils.RandomString(3);
                                oggetto.Nome = res["NOME"].ToString();
                                oggetto.TipoPagamento = (TipoPagamento)res["TIPO_PAGAMENTO"];
                                oggetto.Citta = res["CITTA_NOME"].ToString();
                                oggetto.Punti = (int)res["PUNTI"];
                                oggetto.Soldi = (int)res["SOLDI"];
                                oggetto.Anno = (int)res["ANNO"];
                                oggetto.Marca = res["MARCA_NOME"].ToString();
                                oggetto.StatoOggetto = (CondizioneOggetto)res["CONDIZIONE"];
                                oggetto.DataInserimento = (DateTime)res["DATA_INSERIMENTO"];
                                oggetto.VenditoreToken = (Guid)res["UTENTE_TOKEN"];
                                oggetto.Quantità = (int)res["NUMERO_PEZZI"];
                                oggetto.CategoriaID = (int)res["CATEGORIA_ID"];
                                oggetto.Categoria = res["CATEGORIA_NOME"].ToString();
                                oggetto.StatoVendita = (StatoVendita)res["STATO_VENDITA"];
                                oggetto.Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == oggetto.VenditaID).Select(f =>
                                    f.FOTO.FOTO1
                                ).ToList();
                                lista.Add(SetOggettoViewModel(db, oggetto));
                            }
                            catch (Exception ex)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            }
                        }
                    }
                }
            }

            return lista;
        }

        private OggettoViewModel SetOggettoViewModel(DatabaseContext db, OggettoViewModel oggetto)
        {
            // gestito cosi, perchè nel caso faccio macroricerche, recupero lo stesso i dati personalizzati
            // sulla specifica sottocategoria.
            // new List<int> { 14 }.IndexOf(oggetto.CategoriaID) != 1
            if (oggetto.CategoriaID == 12)
            {
                TelefonoViewModel viewModel = new TelefonoViewModel(oggetto);
                OGGETTO_TELEFONO model = db.OGGETTO_TELEFONO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 64)
            {
                ConsoleViewModel viewModel = new ConsoleViewModel(oggetto);
                OGGETTO_CONSOLE model = db.OGGETTO_CONSOLE.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 13 || (oggetto.CategoriaID >= 62 && oggetto.CategoriaID <= 63) || oggetto.CategoriaID == 65)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggetto);
                OGGETTO_TECNOLOGIA model = db.OGGETTO_TECNOLOGIA.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 14)
            {
                ComputerViewModel viewModel = new ComputerViewModel(oggetto);
                OGGETTO_COMPUTER model = db.OGGETTO_COMPUTER.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.sistemaOperativoID = model.ID_SISTEMA_OPERATIVO;
                viewModel.sistemaOperativoNome = model.SISTEMA_OPERATIVO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 26)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggetto);
                OGGETTO_ELETTRODOMESTICO model = db.OGGETTO_ELETTRODOMESTICO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if ((oggetto.CategoriaID >= 28 && oggetto.CategoriaID <= 39) || oggetto.CategoriaID == 41)
            {
                MusicaViewModel viewModel = new MusicaViewModel(oggetto);
                OGGETTO_MUSICA model = db.OGGETTO_MUSICA.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.formatoID = model.ID_FORMATO;
                viewModel.formatoNome = model.FORMATO.NOME;
                viewModel.artistaID = model.ID_ARTISTA;
                viewModel.artistaNome = model.ARTISTA.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 40)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggetto);
                OGGETTO_STRUMENTO model = db.OGGETTO_STRUMENTO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID == 45)
            {
                VideogamesViewModel viewModel = new VideogamesViewModel(oggetto);
                OGGETTO_VIDEOGAMES model = db.OGGETTO_VIDEOGAMES.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.piattaformaID = model.ID_PIATTAFORMA;
                viewModel.piattaformaNome = model.PIATTAFORMA.NOME;
                viewModel.genereID = model.ID_GENERE;
                viewModel.genereNome = model.GENERE.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 42 && oggetto.CategoriaID <= 47)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggetto);
                OGGETTO_GIOCO model = db.OGGETTO_GIOCO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 50 && oggetto.CategoriaID <= 61)
            {
                ModelloViewModel viewModel = new ModelloViewModel(oggetto);
                OGGETTO_SPORT model = db.OGGETTO_SPORT.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 67 && oggetto.CategoriaID <= 80)
            {
                VideoViewModel viewModel = new VideoViewModel(oggetto);
                OGGETTO_VIDEO model = db.OGGETTO_VIDEO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.formatoID = model.ID_FORMATO;
                viewModel.formatoNome = model.FORMATO.NOME;
                viewModel.registaID = model.ID_REGISTA;
                viewModel.registaNome = model.REGISTA.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 81 && oggetto.CategoriaID <= 85)
            {
                LibroViewModel viewModel = new LibroViewModel(oggetto);
                OGGETTO_LIBRO model = db.OGGETTO_LIBRO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.autoreID = model.ID_AUTORE;
                viewModel.autoreNome = model.AUTORE.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 89 && oggetto.CategoriaID <= 93)
            {
                VeicoloViewModel viewModel = new VeicoloViewModel(oggetto);
                OGGETTO_VEICOLO model = db.OGGETTO_VEICOLO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.modelloID = model.ID_MODELLO;
                viewModel.modelloNome = model.MODELLO.NOME;
                viewModel.alimentazioneID = model.ID_ALIMENTAZIONE;
                viewModel.alimentazioneNome = model.ALIMENTAZIONE.NOME;
                return viewModel;
            }
            else if (oggetto.CategoriaID >= 127 && oggetto.CategoriaID <= 170 && oggetto.CategoriaID != 161 && oggetto.CategoriaID != 152 && oggetto.CategoriaID != 141 && oggetto.CategoriaID != 127)
            {
                VestitoViewModel viewModel = new VestitoViewModel(oggetto);
                OGGETTO_VESTITO model = db.OGGETTO_VESTITO.Where(m => m.ID_OGGETTO == oggetto.Id).FirstOrDefault();
                viewModel.taglia = model.TAGLIA;
                return viewModel;
            }
            return oggetto;
        }

        private void AddFiltriRicercaOggetti(int categoriaID, HttpCookie filtro, ref DbCommand cmd, ref string schema, ref string condizione)
        {
            if (filtro != null)
            {
                // filtri standard
                if ((filtro["Citta"] != null) && !String.IsNullOrEmpty(filtro["Citta"]))
                {
                    cmd.Parameters.Add(new SqlParameter("CITTA", filtro["Citta"]));
                    condizione += " AND COMUNE.NOME = @CITTA ";
                }
                if (!String.IsNullOrWhiteSpace((filtro["TipoPagamento"])) && (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"]) > 0)
                {
                    // filtro per quelli da pagare in qualunque modo o con una modalità specifica
                    cmd.Parameters.Add(new SqlParameter("TIPOPAGAMENTO", (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"])));
                    condizione += " AND (V.TIPO_PAGAMENTO = @TIPOPAGAMENTO OR V.TIPO_PAGAMENTO = 0)";
                }
                if ((filtro["Marca"] != null) && !string.IsNullOrEmpty(filtro["Marca"]))
                {
                    cmd.Parameters.Add(new SqlParameter("MARCA", filtro["Marca"]));
                    condizione += " AND M.NOME LIKE @MARCA + '%'";
                }
                if (!String.IsNullOrWhiteSpace((filtro["StatoOggetto"])) && !string.IsNullOrEmpty(filtro["StatoOggetto"]) && filtro["StatoOggetto"] != "0")
                {
                    cmd.Parameters.Add(new SqlParameter("CONDIZIONE", (int)(CondizioneOggetto)Enum.Parse(typeof(CondizioneOggetto), filtro["StatoOggetto"])));
                    condizione += " AND O.CONDIZIONE = @CONDIZIONE";
                }
                if ((filtro["PuntiMin"] != null) && Convert.ToInt32(filtro["PuntiMin"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("PUNTIMIN", Convert.ToInt32(filtro["PuntiMin"])));
                    condizione += " AND V.PUNTI >= @PUNTIMIN";
                }
                if ((filtro["PuntiMax"] != null) && Convert.ToInt32(filtro["PuntiMax"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("PUNTIMAX", Convert.ToInt32(filtro["PuntiMax"])));
                    condizione += " AND V.PUNTI <= @PUNTIMAX";
                }
                if ((filtro["AnnoMin"] != null) && Convert.ToInt32(filtro["AnnoMin"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("ANNOMIN", Convert.ToInt32(filtro["AnnoMin"])));
                    condizione += " AND O.ANNO >= @ANNOMIN";
                }
                if ((filtro["AnnoMax"] != null) && Convert.ToInt32(filtro["AnnoMax"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("ANNOMAX", Convert.ToInt32(filtro["AnnoMax"])));
                    condizione += " AND O.ANNO <= @ANNOMAX";
                }

                // filtro personalizzato per categoria
                if (categoriaID == 12)
                {
                    schema += " INNER JOIN OGGETTO_TELEFONO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                    if ((filtro["SistemaOperativo"] != null) && !string.IsNullOrEmpty(filtro["SistemaOperativo"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("SISTEMAOPERATIVO", filtro["SistemaOperativo"]));
                        schema += " INNER JOIN SISTEMA_OPERATIVO ON TIPO.ID_SISTEMA_OPERATIVO=SISTEMA_OPERATIVO.ID";
                        condizione += " AND SISTEMA_OPERATIVO.NOME LIKE @SISTEMAOPERATIVO + '%'";
                    }
                }
                else if (categoriaID == 14)
                {
                    schema += " INNER JOIN OGGETTO_COMPUTER AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                    if ((filtro["SistemaOperativo"] != null) && !string.IsNullOrEmpty(filtro["SistemaOperativo"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("SISTEMAOPERATIVO", filtro["SistemaOperativo"]));
                        schema += " INNER JOIN SISTEMA_OPERATIVO ON TIPO.ID_SISTEMA_OPERATIVO=SISTEMA_OPERATIVO.ID";
                        condizione += " AND SISTEMA_OPERATIVO.NOME LIKE @SISTEMAOPERATIVO + '%'";
                    }
                }
                else if (categoriaID == 13 || (categoriaID >= 62 && categoriaID <= 66))
                {
                    schema += " INNER JOIN OGGETTO_TECNOLOGIA AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                }
                else if (categoriaID == 26)
                {
                    schema += " INNER JOIN OGGETTO_ELETTRODOMESTICO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                }
                else if ((categoriaID >= 28 && categoriaID <= 39) || categoriaID == 41)
                {
                    schema += " INNER JOIN OGGETTO_MUSICA AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Formato"] != null) && !string.IsNullOrEmpty(filtro["Formato"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("FORMATO", filtro["Formato"]));
                        schema += " INNER JOIN FORMATO ON TIPO.ID_FORMATO=FORMATO.ID";
                        condizione += " AND FORMATO.NOME LIKE @FORMATO + '%'";
                    }
                    if ((filtro["Artista"] != null) && !string.IsNullOrEmpty(filtro["Artista"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ARTISTA", filtro["Artista"]));
                        schema += " INNER JOIN ARTISTA ON TIPO.ID_ARTISTA=ARTISTA.ID";
                        condizione += " AND ARTISTA.NOME LIKE @ARTISTA + '%'";
                    }
                }
                else if (categoriaID == 40)
                {
                    schema += " INNER JOIN OGGETTO_STRUMENTO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                }
                else if (categoriaID == 45)
                {
                    schema += " INNER JOIN OGGETTO_VIDEOGAMES AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Piattaforma"] != null) && !string.IsNullOrEmpty(filtro["Piattaforma"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("PIATTAFORMA", filtro["Piattaforma"]));
                        schema += " INNER JOIN PIATTAFORMA ON TIPO.ID_PIATTAFORMA=PIATTAFORMA.ID";
                        condizione += " AND PIATTAFORMA.NOME LIKE @PIATTAFORMA + '%'";
                    }
                    if ((filtro["Genere"] != null) && !string.IsNullOrEmpty(filtro["Genere"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("GENERE", filtro["Genere"]));
                        schema += " INNER JOIN GENERE ON TIPO.ID_GENERE=GENERE.ID";
                        condizione += " AND GENERE.NOME LIKE @GENERE + '%'";
                    }
                }
                else if (categoriaID >= 42 && categoriaID <= 49)
                {
                    schema += " INNER JOIN OGGETTO_GIOCO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                }
                else if (categoriaID >= 50 && categoriaID <= 61)
                {
                    schema += " INNER JOIN OGGETTO_SPORT AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                }
                else if (categoriaID >= 67 && categoriaID <= 80)
                {
                    schema += " INNER JOIN OGGETTO_VIDEO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Formato"] != null) && !string.IsNullOrEmpty(filtro["Formato"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("FORMATO", filtro["Formato"]));
                        schema += " INNER JOIN FORMATO ON TIPO.ID_FORMATO=FORMATO.ID";
                        condizione += " AND FORMATO.NOME LIKE @FORMATO + '%'";
                    }
                    if ((filtro["Regista"] != null) && !string.IsNullOrEmpty(filtro["Regista"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("REGISTA", filtro["Regista"]));
                        schema += " INNER JOIN REGISTA ON TIPO.ID_REGISTA=REGISTA.ID";
                        condizione += " AND REGISTA.NOME LIKE @REGISTA + '%'";
                    }
                }
                else if (categoriaID >= 81 && categoriaID <= 86)
                {

                    schema += " INNER JOIN OGGETTO_LIBRO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Autore"] != null) && !string.IsNullOrEmpty(filtro["Autore"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("AUTORE", filtro["Autore"]));
                        schema += " INNER JOIN AUTORE ON TIPO.ID_AUTORE=AUTORE.ID";
                        condizione += " AND AUTORE.NOME LIKE @AUTORE + '%'";
                    }
                }
                else if (categoriaID >= 89 && categoriaID <= 93)
                {
                    schema += " INNER JOIN OGGETTO_VEICOLO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Modello"] != null) && !string.IsNullOrEmpty(filtro["Modello"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ID_MODELLO", filtro["Modello"]));
                        schema += " INNER JOIN MODELLO ON TIPO.ID_MODELLO=MODELLO.ID";
                        condizione += " AND MODELLO.NOME LIKE @ID_MODELLO + '%'";
                    }
                    if ((filtro["Alimentazione"] != null) && !string.IsNullOrEmpty(filtro["Alimentazione"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("ALIMENTAZIONE", filtro["Alimentazione"]));
                        schema += " INNER JOIN ALIMENTAZIONE ON TIPO.ID_ALIMENTAZIONE=ALIMENTAZIONE.ID";
                        condizione += " AND ALIMENTAZIONE.NOME LIKE @ALIMENTAZIONE + '%'";
                    }
                }
                else if (categoriaID >= 127 && categoriaID <= 170 && categoriaID != 161 && categoriaID != 152 && categoriaID != 141 && categoriaID != 127)
                {
                    schema += " INNER JOIN OGGETTO_VESTITO AS TIPO ON O.ID=TIPO.ID_OGGETTO";

                    if ((filtro["Taglia"] != null) && !string.IsNullOrEmpty(filtro["Taglia"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("VESTITO", filtro["Modello"]));
                        condizione += " AND TIPO.TAGLIA LIKE @VESTITO + '%'";
                    }
                }
            }
        }

    /**
    Recupero i servizi ancora in vendita, della categoria selezionata o di una sua sottocategoria,
    che non abbia ancora ricevuto un'offerta attiva di acquisto in punti o comunque accettata
    **/
    private ListaServizi GetListaServizi(int paginaAttuale, HttpCookie ricerca = null, HttpCookie filtro = null)
    {
        //recupero impaginazione e dati
        ListaServizi lista = new ListaServizi();
        int pagineTotali = 1;
        int numeroRecord = 0;
        if (ricerca == null)
            ricerca = HttpContext.Request.Cookies.Get("ricerca");
        if (filtro == null)
            filtro = HttpContext.Request.Cookies.Get("filtro");
        lista.List = FindServizi(ricerca, filtro, paginaAttuale, ref pagineTotali, ref numeroRecord);

        lista.PageNumber = paginaAttuale;
        lista.PageCount = pagineTotali;
        lista.TotalNumber = numeroRecord;
        return lista;
    }

    private List<ServizioViewModel> FindServizi(HttpCookie ricerca, HttpCookie filtro, int pagina, ref int pagineTotali, ref int numeroRecord)
        {
            List<ServizioViewModel> lista = new List<ServizioViewModel>();
            using (DatabaseContext db = new DatabaseContext())
            {
                int idUtente = 0;
                if (Request.IsAuthenticated)
                    idUtente = ((PersonaModel)Session["utente"]).Persona.ID;

                using (var connessione = db.Database.Connection)
                {
                    connessione.Open();
                    DbCommand cmd = connessione.CreateCommand();
                    string selectText = @"SELECT V.ID, S.ID AS SERVIZIO_ID, S.LUNEDI, S.MARTEDI, S.MERCOLEDI, S.GIOVEDI, S.VENERDI, 
                                S.SABATO AS SABATO, S.DOMENICA, ISNULL(S.TUTTI,0) AS TUTTI, S.ORA_INIZIO_FERIALI, S.ORA_FINE_FERIALI, 
                                S.ORA_INIZIO_FESTIVI, S.ORA_FINE_FESTIVI, S.SERVIZI_OFFERTI, S.RISULTATI_FINALI, S.TARIFFA,
                                U.ID AS PERSONA, U.NOME + ' ' + U.COGNOME AS UTENTE_NOMINATIVO, 
                                U.TOKEN AS UTENTE_TOKEN, V.TOKEN, V.NOME, V.TIPO_PAGAMENTO, COMUNE.NOME AS CITTA_NOME, V.PUNTI,V.SOLDI,
                                V.DATA_INSERIMENTO, V.ID_CATEGORIA AS CATEGORIA_ID, CATEGORIA.NOME AS CATEGORIA_NOME, COMUNE.ID AS CITTA_ID,
                                P.ID AS ID_ATTIVITA, P.NOME AS PARTNER_NOME, P.TOKEN AS PARTNER_TOKEN, V.STATO AS STATO_VENDITA ";


                    string schema = @"FROM ANNUNCIO AS V
                                INNER JOIN CATEGORIA ON V.ID_CATEGORIA=CATEGORIA.ID 
                                INNER JOIN SERVIZIO AS S ON V.ID_SERVIZIO=S.ID
                                INNER JOIN COMUNE ON S.ID_COMUNE=COMUNE.ID
                                INNER JOIN PERSONA AS U ON V.ID_PERSONA=U.ID
                                LEFT JOIN ATTIVITA AS P ON V.ID_ATTIVITA=P.ID";

                    string condizione = @"
                                WHERE V.STATO=1 AND CATEGORIA.NODO.IsDescendantOf(
                                        (SELECT C2.NODO FROM CATEGORIA AS C2 WHERE C2.ID=@CATEGORIA)
                                    ) = 1 AND ((SELECT COUNT(*) FROM OFFERTA WHERE OFFERTA.ID_ANNUNCIO=V.ID AND ((TIPO_OFFERTA = 0 AND OFFERTA.STATO = 1) OR OFFERTA.STATO = 4)) <= 0)
                                ";

                    if (!(ricerca["Nome"] != null) && !String.IsNullOrEmpty(ricerca["Nome"]))
                    {
                        cmd.Parameters.Add(new SqlParameter("NOME", ricerca["Nome"]));
                        condizione += " AND V.NOME LIKE @NOME + '%'";
                    }

                    AddFiltriRicercaServizi(Convert.ToInt32(ricerca["IDCategoria"]), filtro, ref cmd, ref schema, ref condizione);

                    cmd.Parameters.Add(new SqlParameter("CATEGORIA", Convert.ToInt32(ricerca["IDCategoria"])));

                    cmd.CommandText = "SELECT COUNT(*) " + schema + condizione;
                    numeroRecord = (int)cmd.ExecuteScalar();

                    pagineTotali = (int)Math.Ceiling((decimal)numeroRecord / Convert.ToDecimal(WebConfigurationManager.AppSettings["numeroAcquisti"]));

                    // impaginazione
                    cmd.CommandText = selectText + schema + condizione + " ORDER BY DATA_INSERIMENTO DESC OFFSET @INIZIO ROWS FETCH NEXT @NUMERORECORD ROWS ONLY";
                    int inizio = (pagina <= 1) ? 0 : ((pagina - 1) * Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"]));
                    cmd.Parameters.Add(new SqlParameter("INIZIO", inizio));
                    cmd.Parameters.Add(new SqlParameter("NUMERORECORD", Convert.ToInt32(WebConfigurationManager.AppSettings["numeroAcquisti"])));
                    using (IDataReader ires = cmd.ExecuteReader())
                    {
                        SqlDataReader res = (SqlDataReader)ires;
                        while (res.Read())
                        {
                            // mettere try e catch per evitare eventuali problemi su tutte le ricerche
                            try
                            {
                                ServizioViewModel servizio = new ServizioViewModel();
                                servizio.VenditaID = (int)res["ID"];
                                servizio.Id = Convert.ToInt32(res["SERVIZIO_ID"]);
                                servizio.VenditoreID = Convert.ToInt32(res["PERSONA"].ToString());
                                servizio.VenditoreNominativo = res["UTENTE_NOMINATIVO"].ToString();
                                if (res["ID_ATTIVITA"].ToString().Trim().Length > 0)
                                {
                                    servizio.PartnerId = (int)res["ID_ATTIVITA"];
                                    servizio.PartnerNominativo = res["PARTNER_NOME"].ToString();
                                }
                                SetFeedbackVenditoreServizio(db, servizio);
                                servizio.Token = Utils.RandomString(3) + res["TOKEN"].ToString() + Utils.RandomString(3);
                                servizio.Nome = res["NOME"].ToString();
                                servizio.TipoPagamento = (TipoPagamento)res["TIPO_PAGAMENTO"];
                                servizio.Citta = res["CITTA_NOME"].ToString();
                                servizio.Punti = (int)res["PUNTI"];
                                servizio.Soldi = (int)res["SOLDI"];
                                servizio.DataInserimento = (DateTime)res["DATA_INSERIMENTO"];
                                servizio.VenditoreToken = (Guid)res["UTENTE_TOKEN"];
                                servizio.CategoriaID = (int)res["CATEGORIA_ID"];
                                servizio.Categoria = res["CATEGORIA_NOME"].ToString();
                                servizio.Lunedi = (bool)res["LUNEDI"];
                                servizio.Martedi = (bool)res["MARTEDI"];
                                servizio.Mercoledi = (bool)res["MERCOLEDI"];
                                servizio.Giovedi = (bool)res["GIOVEDI"];
                                servizio.Venerdi = (bool)res["VENERDI"];
                                servizio.Sabato = (bool)res["SABATO"];
                                servizio.Domenica = (bool)res["DOMENICA"];
                                servizio.Tutti = (bool)res["TUTTI"];
                                servizio.OraInizio = (TimeSpan)res["ORA_INIZIO_FERIALI"];
                                servizio.OraFine = (TimeSpan)res["ORA_FINE_FERIALI"];
                                servizio.OraInizioFestivita = (res.IsDBNull(res.GetOrdinal("ORA_INIZIO_FESTIVI"))) ? null : (TimeSpan?)res["ORA_INIZIO_FESTIVI"];
                                servizio.OraFineFestivita = (res.IsDBNull(res.GetOrdinal("ORA_FINE_FESTIVI"))) ? null : (TimeSpan?)res["ORA_FINE_FESTIVI"];
                                servizio.RisultatiFinali = res["RISULTATI_FINALI"].ToString();
                                servizio.ServiziOfferti = res["SERVIZI_OFFERTI"].ToString();
                                servizio.StatoVendita = (StatoVendita)res["STATO_VENDITA"];
                                servizio.Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == servizio.VenditaID).Select(f =>
                                    f.FOTO.FOTO1
                                ).ToList();
                                servizio.Tariffa = (Tariffa)res["TARIFFA"];
                                lista.Add(SetServizioViewModel(db, servizio));
                            }
                            catch (Exception ex)
                            {
                                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                            }
                        }
                    }
                }
            }

            return lista;
        }

        // da sistemare
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

        private void AddFiltriRicercaServizi(int categoriaID, HttpCookie filtro, ref DbCommand cmd, ref string schema, ref string condizione)
        {
            if (filtro != null)
            {
                // filtri standard
                if ((filtro["Citta"] != null) && !String.IsNullOrEmpty(filtro["Citta"]))
                {
                    cmd.Parameters.Add(new SqlParameter("CITTA", filtro["Citta"]));
                    condizione += " AND COMUNE.NOME = @CITTA ";
                }
                if (!String.IsNullOrWhiteSpace((filtro["TipoPagamento"])) && (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"]) > 0)
                {
                    // filtro per quelli da pagare in qualunque modo o con una modalità specifica
                    cmd.Parameters.Add(new SqlParameter("TIPOPAGAMENTO", (int)(TipoPagamento)Enum.Parse(typeof(TipoPagamento), filtro["TipoPagamento"])));
                    condizione += " AND (V.TIPO_PAGAMENTO = @TIPOPAGAMENTO OR V.TIPO_PAGAMENTO = 0)";
                }
                if ((filtro["PuntiMin"] != null) && Convert.ToInt32(filtro["PuntiMin"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("PUNTIMIN", Convert.ToInt32(filtro["PuntiMin"])));
                    condizione += " AND V.PUNTI >= @PUNTIMIN";
                }
                if ((filtro["PuntiMax"] != null) && Convert.ToInt32(filtro["PuntiMax"]) > 0)
                {
                    cmd.Parameters.Add(new SqlParameter("PUNTIMAX", Convert.ToInt32(filtro["PuntiMax"])));
                    condizione += " AND V.PUNTI <= @PUNTIMAX";
                }

                if (filtro["Tutti"] != null && Convert.ToBoolean(filtro["Tutti"]))
                {
                    filtro["Lunedi"] = "true";
                    filtro["Martedi"] = "true";
                    filtro["Mercoledi"] = "true";
                    filtro["Giovedi"] = "true";
                    filtro["Venerdi"] = "true";
                    filtro["Sabato"] = "true";
                    filtro["Domenica"] = "true";
                }

                System.Text.StringBuilder filtroPerGiorno = new System.Text.StringBuilder();
                bool primoGiornoCercato = true;

                if (filtro["Lunedi"] != null && Convert.ToBoolean(filtro["Lunedi"]))
                {
                    cmd.Parameters.Add(new SqlParameter("LUNEDI", Convert.ToBoolean(filtro["Lunedi"])));
                    filtroPerGiorno.Append(" S.LUNEDI = @LUNEDI");
                    primoGiornoCercato = false;
                }
                if (filtro["Martedi"] != null && Convert.ToBoolean(filtro["Martedi"]))
                {
                    cmd.Parameters.Add(new SqlParameter("MARTEDI", Convert.ToBoolean(filtro["Martedi"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.MARTEDI = @MARTEDI");
                }
                if (filtro["Mercoledi"] != null && Convert.ToBoolean(filtro["Mercoledi"]))
                {
                    cmd.Parameters.Add(new SqlParameter("MERCOLEDI", Convert.ToBoolean(filtro["Mercoledi"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.MERCOLEDI = @MERCOLEDI");
                }
                if (filtro["Giovedi"] != null && Convert.ToBoolean(filtro["Giovedi"]))
                {
                    cmd.Parameters.Add(new SqlParameter("GIOVEDI", Convert.ToBoolean(filtro["Giovedi"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.GIOVEDI = @GIOVEDI");
                }
                if (filtro["Venerdi"] != null && Convert.ToBoolean(filtro["Venerdi"]))
                {
                    cmd.Parameters.Add(new SqlParameter("VENERDI", Convert.ToBoolean(filtro["Venerdi"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.VENERDI = @VENERDI");
                }
                if (filtro["Sabato"] != null && Convert.ToBoolean(filtro["Sabato"]))
                {
                    cmd.Parameters.Add(new SqlParameter("SABATO", Convert.ToBoolean(filtro["Sabato"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.SABATO = @SABATO");
                }
                if (filtro["Domenica"] != null && Convert.ToBoolean(filtro["Domenica"]))
                {
                    cmd.Parameters.Add(new SqlParameter("DOMENICA", Convert.ToBoolean(filtro["Domenica"])));
                    if (!primoGiornoCercato)
                    {
                        filtroPerGiorno.Append(" OR ");
                    }
                    else
                    {
                        primoGiornoCercato = false;
                    }
                    filtroPerGiorno.Append(" S.DOMENICA = @DOMENICA");
                }

                if (filtroPerGiorno.Length > 0)
                    condizione += " AND (" + filtroPerGiorno.ToString() + " ) ";
            }
        }

        private void ResetFiltriRicerca()
        {
            HttpCookie ricerca = new HttpCookie("ricerca");
            ricerca["IDCategoria"] = "1";
            ricerca["Categoria"] = "Tutti";
            HttpCookie filtro = new HttpCookie("filtro");
            Response.SetCookie(ricerca);
            Response.SetCookie(filtro);
        }

        private void SendMailRicercaSalvata(PersonaModel utente, PERSONA_RICERCA model)
        {
            // invio email salvataggio ricerca
            EmailModel email = new EmailModel(ControllerContext);
            email.To.Add(new System.Net.Mail.MailAddress(utente.Email.FirstOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione).EMAIL, utente.Persona.NOME + ' ' + utente.Persona.COGNOME));
            email.Subject = string.Format(App_GlobalResources.Email.SearchSaveSubject, model.NOME) + " - " + WebConfigurationManager.AppSettings["nomeSito"];
            email.Body = "SalvataggioRicerca";
            email.DatiEmail = model;
            new EmailController().SendEmail(email);
        }

        private void SetFeedbackVenditore(DatabaseContext db, VenditaViewModel viewModel, TipoVenditore tipoVenditore)
        {
            try
            {
                List<int> voti = db.ANNUNCIO_FEEDBACK
                                .Where(item => (tipoVenditore == TipoVenditore.Persona && item.ANNUNCIO.ID_PERSONA == viewModel.Venditore.Id) ||
                                (tipoVenditore == TipoVenditore.Attivita && item.ANNUNCIO.ID_ATTIVITA == viewModel.Venditore.Id)).Select(item => item.VOTO).ToList();

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