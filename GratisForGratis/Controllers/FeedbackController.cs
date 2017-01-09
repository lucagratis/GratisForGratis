using GratisForGratis.App_GlobalResources;
using GratisForGratis.Filters;
using GratisForGratis.Models;
using GratisForGratis.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class FeedbackController : AdvancedController
    {
        #region ACTION
        [HttpGet]
        public ActionResult Index(string acquisto)
        {
            string nomeView = "";
            // verificare come visualizzare soltanto
            // verificare come differenziare tra compratore e venditore
            FeedbackViewModel viewModel = new FeedbackViewModel();
            try
            {
                if (ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        string acquistoDecodificato = Uri.UnescapeDataString(acquisto);
                        string acquistoPulito = acquistoDecodificato.Trim().Substring(3, acquistoDecodificato.Trim().Length - 6);
                        int idAcquisto = Utils.DecodeToInt(acquistoPulito);
                        int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                        ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Where(f => f.ID_ANNUNCIO == idAcquisto && f.ID_VOTANTE == idUtente).SingleOrDefault();
                        if (model != null)
                        {
                            TempData["feedback"] = model;
                            return RedirectToAction("Inviato", new { id = model.ID });
                        }
                        
                        // se è un nuovo voto, recupero i dati del pagamento
                        ANNUNCIO model2 = db.ANNUNCIO.Where(p => p.ID == idAcquisto && p.ID_PERSONA != idUtente).SingleOrDefault();
                        viewModel.Nome = model2.NOME;
                        viewModel.AcquistoID = acquisto;
                    }
                }
            }
            catch (Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                // se ha un errore generico o semplicemente sta cercando di fare un feedback
                return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
            }
            return View(nomeView, viewModel);
        }

        [HttpPost]
        public ActionResult Index(FeedbackViewModel viewModel)
        {
            using (DatabaseContext db = new DatabaseContext())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.Database.Connection.Open();
                        string acquistoDecodificato = Uri.UnescapeDataString(viewModel.AcquistoID);
                        string acquistoPulito = acquistoDecodificato.Trim().Substring(3, acquistoDecodificato.Trim().Length - 6);
                        int idAcquisto = Utils.DecodeToInt(acquistoPulito);
                        PersonaModel utente = (Session["utente"] as PersonaModel);
                        ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Include("Annuncio.Persona").Where(f => f.ID_VOTANTE == utente.Persona.ID && f.ID_ANNUNCIO == idAcquisto).SingleOrDefault();
                        if (model != null)
                        {
                            TempData["feedback"] = model;
                            TempData["salvato"] = Language.SavedFeedback;
                            return RedirectToAction("Inviato", new { id = model.ID });
                        }

                        model = new ANNUNCIO_FEEDBACK();
                        ANNUNCIO model2 = db.ANNUNCIO.Where(p => p.ID == idAcquisto && p.ID_PERSONA != utente.Persona.ID).SingleOrDefault();
                        model.ID_ANNUNCIO = model2.ID;
                        model.ID_VOTANTE = utente.Persona.ID;
                        model.VOTO = viewModel.Voto;
                        model.COMMENTO = viewModel.Opinione;
                        model.DATA_INSERIMENTO = DateTime.Now;
                        model.DATA_MODIFICA = model.DATA_INSERIMENTO;
                        model.STATO = (int)Stato.ATTIVO;
                        db.ANNUNCIO_FEEDBACK.Add(model);
                        if (db.SaveChanges() > 0)
                        {
                            // feedback salvato
                            viewModel.DataInvio = model.DATA_INSERIMENTO;
                            viewModel.PuntiBonus = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["bonusFeedback"]);
                            TempData["salvato"] = Language.SavedFeedback;
                            AddBonusFeedback(utente.Persona, db, viewModel.PuntiBonus, model.ID_ANNUNCIO);
                            return View("Inviato", viewModel);
                        }
                        else
                        {
                            ModelState.AddModelError("Errore", Language.ErrorFeedback);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                    // se ha un errore generico o semplicemente sta cercando di fare un feedback
                    return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
                }
                finally
                {
                    if(db.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        db.Database.Connection.Close();
                        db.Database.Connection.Dispose();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Inviato(int id)
        {
            FeedbackViewModel viewModel = new FeedbackViewModel();
            using (DatabaseContext db = new DatabaseContext()) {
                try
                {
                    db.Database.Connection.Open();
                    int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                    ANNUNCIO_FEEDBACK model = db.ANNUNCIO_FEEDBACK.Include("Annuncio.Persona").Where(f => f.ID == id && f.ID_VOTANTE == idUtente).SingleOrDefault();
                    viewModel.Ricevente = model.ANNUNCIO.PERSONA.NOME + ' ' + model.ANNUNCIO.PERSONA.COGNOME;
                    // AGGIUNGERE SALVATAGGIO ATTIVITà IN CASO SIA UN'AZIENDA A RILASCIARE IL FEEDBACK
                    viewModel.Voto = model.VOTO;
                    viewModel.Opinione = model.COMMENTO;
                    viewModel.Nome = model.ANNUNCIO.NOME;
                    viewModel.DataInvio = model.DATA_INSERIMENTO;
                    viewModel.PuntiBonus = (int)db.TRANSAZIONE.SingleOrDefault(item => item.ID_ANNUNCIO == model.ID_ANNUNCIO).PUNTI;
                }
                catch (Exception eccezione)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
                    // se ha un errore generico o semplicemente sta cercando di fare un feedback
                    return Redirect(System.Web.Security.FormsAuthentication.DefaultUrl);
                }
                finally
                {
                    if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                    {
                        db.Database.Connection.Close();
                        db.Database.Connection.Dispose();
                    }
                }
            }
            return View(viewModel);
        }
        #endregion

        #region METODI PRIVATI
        private void AddBonusFeedback(PERSONA utente, DatabaseContext db, int punti, int idAnnuncio)
        {
            Guid tokenPortale = Guid.Parse(System.Configuration.ConfigurationManager.AppSettings["portaleweb"]);
            AddBonus(db, utente, tokenPortale, punti, TipoTransazione.BonusFeedback, Bonus.Feedback, idAnnuncio);
        }
        #endregion
    }
}