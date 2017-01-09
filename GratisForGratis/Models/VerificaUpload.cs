using GratisForGratis.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    static class VerificaUpload
    {
        public static FileMedia getIstanziaFile(string estensioneFile)
        {
            if (estensioneFile.Equals(".jpg") || estensioneFile.Equals(".jpeg"))
            {
                return new Jpg();
            }
            else if (estensioneFile.Equals(".bmp"))
            {
                return new Bmp();
            }
            else if (estensioneFile.Equals(".gif"))
            {
                return new Gif();
            }
            else if (estensioneFile.Equals(".png"))
            {
                return new Png();
            }
            else if (estensioneFile.Equals(".tiff"))
            {
                return new Tiff();
            }
            else if (estensioneFile.Equals(".mp4"))
            {
                return new Mp4();
            }
            else if (estensioneFile.Equals(".avi"))
            {
                return new Avi();
            }
            else if (estensioneFile.Equals(".flv"))
            {
                return new Flv();
            }
            else if (estensioneFile.Equals(".wmv"))
            {
                return new Wmv();
            }
            else if (estensioneFile.Equals(".3gp"))
            {
                return new File3gp();
            }
            else if (estensioneFile.Equals(".mpeg"))
            {
                return new Mpeg();
            }
            else if (estensioneFile.Equals(".ogm"))
            {
                return new Ogm();
            }
            else if (estensioneFile.Equals(".mp3"))
            {
                return new Mp3();
            }
            else if (estensioneFile.Equals(".wav"))
            {
                return new Wav();
            }
            else if (estensioneFile.Equals(".mid"))
            {
                return new Mid();
            }
            else if (estensioneFile.Equals(".wma"))
            {
                return new Wma();
            }
            else if (estensioneFile.Equals(".doc"))
            {
                return new Doc();
            }
            else if (estensioneFile.Equals(".txt"))
            {
                return new Txt();
            }
            else if (estensioneFile.Equals(".docx"))
            {
                return new DocX();
            }
            else if (estensioneFile.Equals(".pdf"))
            {
                return new Pdf();
            }
            return null;
        }
    }

    public enum TipoMedia
    {
        NESSUNO = 0,
        FOTO = 1,
        VIDEO = 2,
        EMBED = 3,
        MUSICA = 4,
        TESTO = 5
    }
}
