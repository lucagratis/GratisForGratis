using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class FotoModel : FOTO
    {
        #region METODI PUBBLICI
        public int Add(DatabaseContext db, string nome)
        {
            FOTO foto = new FOTO();
            foto.FOTO1 = nome;
            foto.DATA_INSERIMENTO = DateTime.Now;
            foto.STATO = (int)Stato.ATTIVO;
            db.FOTO.Add(foto);
            db.SaveChanges();
            return foto.ID;
        }
        #endregion
    }
}
