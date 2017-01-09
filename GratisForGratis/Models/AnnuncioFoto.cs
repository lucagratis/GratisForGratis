using Elmah;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GratisForGratis.Models
{
    public class AnnuncioFoto : ANNUNCIO_FOTO
    {
        #region METODI PUBBLICI
        public void Add(DatabaseContext db, HttpServerUtilityBase server, string sessione, Guid tokenUtente, int idAnnuncio, string nome)
        {
            ANNUNCIO_FOTO foto = new ANNUNCIO_FOTO();
            foto.ID_ANNUNCIO = idAnnuncio;
            FotoModel model = new FotoModel();
            foto.ID_FOTO = model.Add(db, nome);
            foto.DATA_INSERIMENTO = DateTime.Now;
            foto.DATA_MODIFICA = foto.DATA_INSERIMENTO;
            foto.STATO = (int)Stato.ATTIVO;
            string pathImgOriginale = server.MapPath("~/Uploads/Images/" + tokenUtente + "/" + DateTime.Now.Year.ToString() + "/Original/");
            string pathImgMedia = server.MapPath("~/Uploads/Images/" + tokenUtente + "/" + DateTime.Now.Year.ToString() + "/Normal/");
            string pathImgPiccola = server.MapPath("~/Uploads/Images/" + tokenUtente + "/" + DateTime.Now.Year.ToString() + "/Little/");
            Directory.CreateDirectory(pathImgOriginale);
            Directory.CreateDirectory(pathImgMedia);
            Directory.CreateDirectory(pathImgPiccola);
            try
            {
                System.IO.File.Move(server.MapPath("~/Temp/Images/" + sessione + "/Original/" + nome), pathImgOriginale + nome);
                System.IO.File.Move(server.MapPath("~/Temp/Images/" + sessione + "/Normal/" + nome), pathImgMedia + nome);
                System.IO.File.Move(server.MapPath("~/Temp/Images/" + sessione + "/Little/" + nome), pathImgPiccola + nome);
            }
            catch (IOException fileEx)
            {
                ErrorSignal.FromCurrentContext().Raise(fileEx);
            }
            db.ANNUNCIO_FOTO.Add(foto);
            db.SaveChanges();
        }
        #endregion
    }
}
