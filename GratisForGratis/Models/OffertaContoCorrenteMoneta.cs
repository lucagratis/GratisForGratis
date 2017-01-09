using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class OffertaContoCorrenteMoneta : OFFERTA_CONTO_CORRENTE_MONETA
    {
        #region METODI PUBBLICI
        public void AddCrediti(DatabaseContext db, int idOfferta, PersonaModel persona, int punti)
        {
            using (DbContextTransaction transazione = db.Database.BeginTransaction())
            {
                List<CONTO_CORRENTE_MONETA> lista = db.CONTO_CORRENTE_MONETA
                    .Where(item => item.ID_CONTO_CORRENTE == persona.Persona.ID_CONTO_CORRENTE).ToList();
                if (lista == null || lista.Count < punti)
                    throw new Exception(App_GlobalResources.Language.ErrorMoney);

                for (int i=0;i < punti;i++)
                {
                    OFFERTA_CONTO_CORRENTE_MONETA model = new OFFERTA_CONTO_CORRENTE_MONETA();
                    model.ID_OFFERTA = idOfferta;
                    model.ID_CONTO_CORRENTE_MONETA = lista[i].ID;
                    model.DATA_INSERIMENTO = DateTime.Now;
                    model.STATO = (int)StatoOfferta.ATTIVA;
                    db.OFFERTA_CONTO_CORRENTE_MONETA.Add(model);
                    db.CONTO_CORRENTE_MONETA.Attach(lista[i]);
                }
                db.SaveChanges();
                transazione.Commit();
            }
        }

        public void RemoveCrediti(DatabaseContext db, int idOfferta, PersonaModel persona)
        {
            foreach (OFFERTA_CONTO_CORRENTE_MONETA item in db.OFFERTA_CONTO_CORRENTE_MONETA.Where(item => item.ID_OFFERTA == idOfferta).ToList())
            {
                CONTO_CORRENTE_MONETA conto = item.CONTO_CORRENTE_MONETA;
                conto.STATO = (int)StatoMoneta.ASSEGNATA;
                db.CONTO_CORRENTE_MONETA.Attach(conto);
                var entry = db.Entry(conto);
                entry.Property(e => e.STATO).IsModified = true;
                if (db.SaveChanges() > 0)
                {
                    db.OFFERTA_CONTO_CORRENTE_MONETA.Remove(item);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception(App_GlobalResources.Language.ErrorRecoveryPoints);
                }
            }
        }
        #endregion
    }
}
