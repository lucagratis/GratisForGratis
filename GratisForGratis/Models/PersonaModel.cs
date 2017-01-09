using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace GratisForGratis.Models
{
    public class PersonaModel
    {
        #region PROPRIETA

        public int Punti { get; set; }

        public int PuntiSospesi { get; set; }

        public PERSONA Persona { get; set; }

        public List<PERSONA_EMAIL> Email { get; set; }

        public List<PERSONA_TELEFONO> Telefono { get; set; }

        public List<PERSONA_INDIRIZZO> Indirizzo { get; set; }

        public List<AttivitaModel> Attivita { get; set; }

        public List<CONTO_CORRENTE_MONETA> ContoCorrente { get; set; }

        #endregion

        #region COSTRUTTORI

        public PersonaModel()
        {
            this.SetValoriBase();
        }

        public PersonaModel(PERSONA model)
        {
            this.Persona = model;
            /*foreach (PropertyInfo prop in model.GetType().GetProperties())
                GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(model, null), null);
                */
            this.SetValoriBase();
        }
        #endregion

        #region METODI PRIVATI

        private void SetValoriBase()
        {
            this.Punti = 0;
            this.PuntiSospesi = 0;
            this.Email = new List<PERSONA_EMAIL>();
            this.Telefono = new List<PERSONA_TELEFONO>();
            this.Indirizzo = new List<PERSONA_INDIRIZZO>();
            this.Attivita = new List<AttivitaModel>();
            this.ContoCorrente = new List<CONTO_CORRENTE_MONETA>();
        }

        #endregion

        #region METODI PUBBLICI

        public void SetEmail(DatabaseContext db, string email)
        {
            PERSONA_EMAIL model = this.Email.SingleOrDefault(m => m.TIPO == (int)TipoEmail.Registrazione);
            //this.Email.Remove(model);
            if (model == null)
            {
                if (!string.IsNullOrWhiteSpace(email))
                {
                    model = new PERSONA_EMAIL();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = (int)TipoEmail.Registrazione;
                    model.STATO = (int)Stato.ATTIVO;
                    model.EMAIL = email;
                    db.PERSONA_EMAIL.Add(model);
                    this.Email.Add(model);
                }
            }
            else if (model.EMAIL != email)
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    db.Entry(model).State = EntityState.Deleted;
                    this.Email.Remove(model);
                }
                else
                {
                    model.EMAIL = email;
                    db.Entry(model).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
        }

        public void SetTelefono(DatabaseContext db, string telefono)
        {
            PERSONA_TELEFONO model = this.Telefono.SingleOrDefault(m => m.TIPO == (int)TipoTelefono.Privato);
            //this.Telefono.Remove(model);
            if (model == null)
            {
                if (!string.IsNullOrWhiteSpace(telefono))
                {
                    model = new PERSONA_TELEFONO();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = (int)TipoTelefono.Privato;
                    model.STATO = (int)Stato.ATTIVO;
                    model.TELEFONO = telefono;
                    db.PERSONA_TELEFONO.Add(model);
                    this.Telefono.Add(model);
                }
            }
            else if (model.TELEFONO != telefono)
            {
                if (string.IsNullOrWhiteSpace(telefono))
                {
                    db.Entry(model).State = EntityState.Deleted;
                    this.Telefono.Remove(model);
                }
                else
                {
                    model.TELEFONO = telefono;
                    db.Entry(model).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            /*
            if (!string.IsNullOrWhiteSpace(telefono))
                this.Telefono.Add(model);
            */
        }

        public void SetIndirizzo(DatabaseContext db, int? comune, string indirizzo, int? civico, int tipoIndirizzo)
        {
            PERSONA_INDIRIZZO model = this.Indirizzo.SingleOrDefault(m => m.TIPO == tipoIndirizzo);
            bool modificato = false;
            if (model == null)
            {
                if (!string.IsNullOrWhiteSpace(indirizzo))
                {
                    model = new PERSONA_INDIRIZZO();
                    model.ID_PERSONA = this.Persona.ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.TIPO = tipoIndirizzo;
                    model.STATO = (int)Stato.ATTIVO;
                    model.INDIRIZZO = db.INDIRIZZO.Include("Comune").SingleOrDefault(m => m.INDIRIZZO1 == indirizzo && m.ID_COMUNE == comune && m.CIVICO == civico);
                    if (model.INDIRIZZO == null)
                    {
                        model.INDIRIZZO = new INDIRIZZO();
                        model.INDIRIZZO.DATA_INSERIMENTO = DateTime.Now;
                        model.INDIRIZZO.STATO = (int)Stato.ATTIVO;
                        model.INDIRIZZO.ID_COMUNE = (int)comune;
                        model.INDIRIZZO.INDIRIZZO1 = indirizzo;
                        model.INDIRIZZO.CIVICO = (int)civico;
                        db.INDIRIZZO.Add(model.INDIRIZZO);
                    }
                    model.ID_INDIRIZZO = model.INDIRIZZO.ID;
                    db.PERSONA_INDIRIZZO.Add(model);
                    this.Indirizzo.Add(model);
                }
                modificato = db.SaveChanges() > 0;
            }
            else if (model.INDIRIZZO != null && model.INDIRIZZO.ID_COMUNE != comune || model.INDIRIZZO.INDIRIZZO1 != indirizzo || model.INDIRIZZO.CIVICO != civico)
            {
                if (string.IsNullOrWhiteSpace(indirizzo))
                {
                    db.Entry(model).State = EntityState.Deleted;
                    this.Indirizzo.Remove(model);
                }
                else
                {
                    model.INDIRIZZO = db.INDIRIZZO.Include("Comune").SingleOrDefault(m => m.INDIRIZZO1 == indirizzo && m.ID_COMUNE == comune && m.CIVICO == civico);
                    if (model.INDIRIZZO == null)
                    {
                        model.INDIRIZZO = new INDIRIZZO();
                        model.INDIRIZZO.DATA_INSERIMENTO = DateTime.Now;
                        model.INDIRIZZO.STATO = (int)Stato.ATTIVO;
                        model.INDIRIZZO.ID_COMUNE = (int)comune;
                        model.INDIRIZZO.INDIRIZZO1 = indirizzo;
                        model.INDIRIZZO.CIVICO = (int)civico;
                        db.INDIRIZZO.Add(model.INDIRIZZO);
                    }
                    model.ID_INDIRIZZO = model.INDIRIZZO.ID;
                    db.Entry(model).State = EntityState.Modified;
                }
                modificato = db.SaveChanges() > 0;
            }
        }

        #endregion
    }
}