using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GratisForGratis.Models.File
{
    public abstract class FileMedia
    {
        protected String[] idEsadecimale;
        protected TipoMedia tipoFile;
        protected int numBytesID;

        public TipoMedia TipoFile
        {
            get { return tipoFile; }
        }

        public int NumBytesID
        {
            get { return numBytesID; }
        }

        public FileMedia(String[] idEsadecimale, TipoMedia tipoFile,int numBytesID)
        {
            this.idEsadecimale = idEsadecimale;
            this.tipoFile = tipoFile;
            this.numBytesID = numBytesID;
        }

        public abstract bool checkFormato(String esadecimaleFile);
    }
}