using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Gif:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Gif() : base (new String[]{ "47494638", "474946383761", "474946383961" },TipoMedia.FOTO,6)
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