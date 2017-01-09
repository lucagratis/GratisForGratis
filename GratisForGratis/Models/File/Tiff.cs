using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Tiff:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Tiff() : base (new String[]{ "49492A00", "4D4D002A" },TipoMedia.FOTO,4)
        {

        }

        public override bool checkFormato(String esadecimaleFile)
        {
            foreach (String id in idEsadecimale)
            {
                if (esadecimaleFile.StartsWith(id))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion METODI
    }
}