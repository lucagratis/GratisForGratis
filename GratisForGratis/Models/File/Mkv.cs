using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public class Mkv:FileMedia
    {
        #region FIELDS

        #endregion FIELDS

        #region PROPRIETà

        #endregion PROPRIETà

        #region METODI

        public Mkv()
            : base(new String[] { "1A45DFA3934282886D6174726F736B61428781014285810118538067" }, TipoMedia.VIDEO, 28)
        {

        }

        public override bool checkFormato(String esadecimaleFile)
        {
            if (esadecimaleFile.StartsWith(idEsadecimale[0]))
            {
                return true;
            }
            return false;
        }

        #endregion METODI
    }
}