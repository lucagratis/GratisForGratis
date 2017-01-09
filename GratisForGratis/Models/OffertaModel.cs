using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    class OffertaModel : OFFERTA
    {
        public OffertaModel(int id, PERSONA venditore)
        {
            this.ID = id;
            this.ANNUNCIO.ID_PERSONA = venditore.ID;
        }
        public bool Accetta(ref string messaggio)
        {
            //using (DatabaseContext db = new DatabaseContext())
            //{
            DatabaseContext db = new DatabaseContext();
            try {
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    DateTime dataModifica = DateTime.Now;
                    OFFERTA offerta = db.OFFERTA.Where(o => o.ID == this.ID && o.ANNUNCIO.ID_PERSONA == this.ANNUNCIO.ID_PERSONA && o.STATO == (int)StatoOfferta.ATTIVA).SingleOrDefault();
                    offerta.STATO = (int)StatoOfferta.ACCETTATA;
                    offerta.DATA_MODIFICA = dataModifica;
                    offerta.ANNUNCIO.DATA_MODIFICA = dataModifica;
                    offerta.ANNUNCIO.STATO = (int)StatoVendita.SOSPESO;
                    // salvataggio offerta
                    if (db.SaveChanges() > 1)
                    {
                        AnnullaOfferteEffettuate(db, offerta.ID_ANNUNCIO);

                        AnnullaOfferteRicevute(db, offerta.ID_ANNUNCIO);

                        transazione.Commit();
                        return true;
                    }
                    transazione.Rollback();
                }
            }catch(Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            //}
            return false;
        }
        public bool Rifiuta()
        {
            //using (DatabaseContext db = new DatabaseContext())
            //{
            DatabaseContext db = new DatabaseContext();
            try {
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    OFFERTA offerta = db.OFFERTA.Where(o => o.ID == this.ID && o.ANNUNCIO.ID_PERSONA == this.ANNUNCIO.ID_PERSONA && o.STATO == (int)StatoOfferta.ATTIVA).SingleOrDefault();
                    offerta.DATA_MODIFICA = DateTime.Now;
                    offerta.STATO = (int)StatoOfferta.ANNULLATA;
                    OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                    offertaMoneta.RemoveCrediti(db, offerta.ID, new PersonaModel(offerta.PERSONA));
                    offerta.PERSONA.DATA_MODIFICA = DateTime.Now;
                    if (db.SaveChanges() > 1)
                    {
                        transazione.Commit();
                        return true;
                    }
                    transazione.Rollback();
                }
            }
            catch (Exception eccezione)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(eccezione);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            //}
            return false;
        }

        public static void AnnullaOfferteEffettuate(DatabaseContext db,int vendita)
        {
            // annullo offerte di baratto effettuate
            foreach(OFFERTA_BARATTO b in db.OFFERTA_BARATTO.Where(b => b.ID_ANNUNCIO == vendita).ToList())
            {
                b.OFFERTA.DATA_MODIFICA = DateTime.Now;
                b.OFFERTA.STATO = (int)StatoOfferta.ANNULLATA;
                OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                offertaMoneta.RemoveCrediti(db, b.ID_OFFERTA, new PersonaModel(b.OFFERTA.PERSONA));
                b.OFFERTA.PERSONA.DATA_MODIFICA = DateTime.Now;
                db.SaveChanges();
            }
        }

        public static void AnnullaOfferteRicevute(DatabaseContext db, int vendita)
        {
            // anullo le altre offerte ricevute
            foreach (OFFERTA o in db.OFFERTA.Where(o => (o.STATO == (int)StatoOfferta.ACCETTATA || o.STATO == (int)StatoOfferta.ATTIVA) && o.ID_ANNUNCIO == vendita).ToList()) { 
                o.DATA_MODIFICA = DateTime.Now;
                o.STATO = (int)StatoOfferta.ANNULLATA;
                o.PERSONA.DATA_MODIFICA = DateTime.Now;
                db.SaveChanges();
                // restituzione crediti sospesi
                OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                offertaMoneta.RemoveCrediti(db, o.ID, new PersonaModel(o.PERSONA));
            }
        }
    }
}
