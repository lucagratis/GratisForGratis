using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Txt:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Txt()
            : base(new String[] { "464F524D", "46545854" }, TipoMedia.TESTO, 4)
        {

        }

        public override bool checkFormato(String esadecimaleFile)
        {
            bool isCorretto = false;
            idEsadecimale.ToList().ForEach(item =>
            {
                if (esadecimaleFile.StartsWith(item))
                {
                    isCorretto = true;
                }
            });
            return isCorretto;
        }

        #endregion METODI
    }
}