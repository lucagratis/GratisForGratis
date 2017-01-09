using System;
using System.Collections.Generic;
using System.Linq;
using GratisForGratis.Models;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    [Authorize]
    [HandleError]
    public class PortaleWebController : Controller
    {
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Profilo(string token)
        {
            PortaleWebProfiloViewModel viewModel = null;
            try
            {
                PortaleWebViewModel portale = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == token).SingleOrDefault();
                viewModel = new PortaleWebProfiloViewModel(portale);
                using (DatabaseContext db = new DatabaseContext())
                {
                    int id = Convert.ToInt32(portale.Id);
                    ATTIVITA model = db.ATTIVITA.SingleOrDefault(p => p.ID == id);
                    viewModel.CopyModel(model, model.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == model.ID).ToList(), model.ATTIVITA_TELEFONO.Where(e => e.ID_ATTIVITA == model.ID).ToList());
                    DateTime unAnnoFa = DateTime.Now.AddYears(-1);
                    // da sistemare e poi verificare che nella pubblicazione venga inviato anche il portale se necessario
                    // e nel caso non ci siano, togliere scelta portale e verificare lato server l'assenza del dato
                    // poi pensare alla visualizzazione del nome del portale sia sulle trattative che sulla visualizzazione
                    // della vendita
                    var bonus = db.TRANSAZIONE.Where(b => b.ID_CONTO_MITTENTE == model.ID_CONTO_CORRENTE && 
                        b.TIPO == (int)TipoTransazione.BonusFeedback
                        && b.DATA_INSERIMENTO > unAnnoFa).ToList();
                    if (bonus!=null)
                        viewModel.BonusSpeso = bonus.Sum(b => b.PUNTI);
                }
            }
            catch(Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Profilo(PortaleWebProfiloViewModel viewModel)
        {
            try
            { 
                if (base.ModelState.IsValid)
                {
                    using (DatabaseContext db = new DatabaseContext())
                    {
                        // da modificare
                        PortaleWebViewModel viewModel2 = (Session["portaleweb"] as List<PortaleWebViewModel>).Where(p => p.Token == viewModel.Token).SingleOrDefault();
                        int idPartner = Convert.ToInt32(viewModel2.Id);
                        ATTIVITA model = db.ATTIVITA.Where(p => p.ID == idPartner).SingleOrDefault();
                        model.NOME = viewModel.Nome;
                        model.DOMINIO = viewModel.Dominio;
                        ATTIVITA_EMAIL modelEmail = model.ATTIVITA_EMAIL.SingleOrDefault(item => item.TIPO == (int)TipoEmail.Registrazione);
                        modelEmail.EMAIL = viewModel.Email;
                        db.ATTIVITA_EMAIL.Attach(modelEmail);
                        ATTIVITA_TELEFONO modelTelefono = model.ATTIVITA_TELEFONO.SingleOrDefault(item => item.TIPO == (int)TipoTelefono.Privato);
                        modelTelefono.TELEFONO = viewModel.Telefono;
                        db.ATTIVITA_TELEFONO.Attach(modelTelefono);
                        model.DATA_MODIFICA = DateTime.Now;
                        db.Entry<ATTIVITA>(model).State = System.Data.Entity.EntityState.Modified;
                        if (db.SaveChanges() > 0)
                        {
                            // trovare portaleweb modificato e sostituirlo
                            //Session["portaleweb"] = model;
                            Session["portaleweb"] = (Session["utente"] as PersonaModel).Persona.PERSONA_ATTIVITA
                                .Select(item => new PortaleWebViewModel(item, 
                                    item.ATTIVITA.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA == item.ID_ATTIVITA).ToList(), 
                                    item.ATTIVITA.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
                                )).ToList();
                            TempData["salvato"] = true;
                        }
                    }
                }
            }
            catch(Exception exception)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }
            return View(viewModel);
        }
        
    }
}