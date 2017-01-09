using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GratisForGratis.Controllers
{
    [Authorize]
    [HandleError]
    public class AdvancedController : Controller
    {
        #region METODI

        [AllowAnonymous]
        public void setSessioneUtente(HttpSessionStateBase sessione, DatabaseContext db, int utente, bool ricordaLogin)
        {
            PERSONA model = db.PERSONA.SingleOrDefault(m => m.ID == utente);
            PersonaModel persona = new PersonaModel(model);
            persona.Email = db.PERSONA_EMAIL.Where(m => m.ID_PERSONA == utente).ToList();
            persona.Telefono = db.PERSONA_TELEFONO.Where(m => m.ID_PERSONA == utente).ToList();
            persona.Indirizzo = db.PERSONA_INDIRIZZO
                .Include("INDIRIZZO")
                .Include("INDIRIZZO.COMUNE")
                .Where(m => m.ID_PERSONA == utente)
                .ToList();
            db.PERSONA_ATTIVITA.Where(m => m.ID_PERSONA == utente).ToList().ForEach(m => {
                persona.Attivita.Add(new AttivitaModel(m));
            });
            persona.ContoCorrente = db.CONTO_CORRENTE_MONETA.Where(m => m.ID_CONTO_CORRENTE == persona.Persona.ID_CONTO_CORRENTE).ToList();

            sessione["utente"] = persona;
            if (persona.Attivita != null && persona.Attivita.Count > 0)
                sessione["portaleweb"] = persona.Attivita.Select(item => 
                    new PortaleWebViewModel(item, 
                    item.Attivita.ATTIVITA_EMAIL.Where(e => e.ID_ATTIVITA==item.ID_ATTIVITA).ToList(), 
                    item.Attivita.ATTIVITA_TELEFONO.Where(t => t.ID_ATTIVITA == item.ID_ATTIVITA).ToList()
                )).ToList();
            FormsAuthentication.SetAuthCookie(persona.Persona.CONTO_CORRENTE.TOKEN.ToString(), ricordaLogin);
            //FormsAuthentication.RedirectFromLoginPage(utente.EMAIL, ricordaLogin);
        }

        public int CountOfferteNonConfermate(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ID_PERSONA == utente.Persona.ID && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        // su tutte le vendite
        
        public int CountOfferteRicevute(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ANNUNCIO.ID_PERSONA == utente.Persona.ID && item.LETTA == 0 && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        // sulla vendita singola
        
        public int CountOfferteRicevute(DatabaseContext db, int vendita)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            return db.OFFERTA.Where(item => item.ID_ANNUNCIO == vendita && item.ID_PERSONA == utente.Persona.ID && item.STATO == (int)StatoOfferta.ATTIVA).Count();
        }

        public void RefreshUtente(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
             utente.Persona = db.PERSONA.Where(u => u.ID == utente.Persona.ID).SingleOrDefault();
            Session["utente"] = utente;
        }

        public void RefreshPunteggioUtente(DatabaseContext db)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            utente.ContoCorrente = db.CONTO_CORRENTE_MONETA.Where(m => m.ID_CONTO_CORRENTE == utente.Persona.ID_CONTO_CORRENTE).ToList();
            Session["utente"] = utente;
        }
        /*
        public void RefreshPunteggioUtente(int punti, int puntiSospesi)
        {
            PersonaModel utente = Session["utente"] as PersonaModel;
            utente.Punti = punti;
            utente.PuntiSospesi = puntiSospesi;
            Session["utente"] = utente;
        }*/

        [AllowAnonymous]
        public string GetCurrentDomain()
        {
            return Request.Url.Scheme + Uri.SchemeDelimiter + Request.Url.Host +
                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
        }

        // VERIFICARE CHE L'ASSEGNAZIONE DELLA MONETA VADA A BUON FINE E CHE QUINDI LA TRANSAZIONE
        // ABBIA EFFETTO
        protected void AddBonus(DatabaseContext db, PERSONA persona, Guid tokenPortale, int punti, TipoTransazione tipo, string nomeTransazione, int? idAnnuncio = null)
        {
            TRANSAZIONE model = new TRANSAZIONE();
            model.ID_CONTO_MITTENTE = db.ATTIVITA.Where(p => p.TOKEN == tokenPortale).SingleOrDefault().ID_CONTO_CORRENTE;
            model.ID_CONTO_DESTINATARIO = persona.ID_CONTO_CORRENTE;
            model.TIPO = (int)tipo;
            model.NOME = nomeTransazione;
            model.PUNTI = punti;
            model.DATA_INSERIMENTO = DateTime.Now;
            model.STATO = (int)StatoPagamento.ACCETTATO;
            model.ID_ANNUNCIO = idAnnuncio;
            db.TRANSAZIONE.Add(model);
            db.SaveChanges();
            // genero la moneta ogni volta che offro un bonus, in modo da mantenere la concorrenza dei dati
            for (int i = 0; i < punti; i++)
            {
                MONETA moneta = db.MONETA.Create();
                moneta.VALORE = 1;
                moneta.TOKEN = Guid.NewGuid();
                moneta.DATA_INSERIMENTO = DateTime.Now;
                moneta.STATO = (int)Stato.ATTIVO;
                db.MONETA.Add(moneta);
                db.SaveChanges();
                CONTO_CORRENTE_MONETA conto = new CONTO_CORRENTE_MONETA();
                conto.ID_CONTO_CORRENTE = persona.ID_CONTO_CORRENTE;
                conto.ID_MONETA = moneta.ID;
                conto.ID_TRANSAZIONE = model.ID;
                conto.DATA_INSERIMENTO = DateTime.Now;
                conto.STATO = (int)StatoMoneta.ASSEGNATA;
                db.CONTO_CORRENTE_MONETA.Add(conto);
                db.SaveChanges();
            }
        }

        #endregion
    }
}