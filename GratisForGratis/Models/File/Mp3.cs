using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Mp3:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Mp3()
            : base(new String[] { "FF", "49443303" }, TipoMedia.MUSICA, 4)
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