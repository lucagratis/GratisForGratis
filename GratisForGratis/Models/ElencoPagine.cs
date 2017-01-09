using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    public class ElencoPagine
    {
        #region COSTRUTTORI

        public ElencoPagine(int numeroPagine, int paginaAttuale, int paginaIniziale, string controller, string action)
        {
            this.NumeroPagine = numeroPagine;
            this.PaginaAttuale = paginaAttuale;
            this.PaginaIniziale = paginaIniziale;
            this.Controller = controller;
            this.Action = action;
        }

        #endregion

        #region PROPRIETA

        public int NumeroPagine { get; private set; }

        public int PaginaAttuale { get; private set; }

        public int PaginaIniziale { get; private set; }

        public string Controller { get; private set; }

        public string Action { get; private set; }

        #endregion
    }

    public class ElencoPagineRicerca : ElencoPagine
    {
        #region COSTRUTTORI

        public ElencoPagineRicerca(int numeroPagine, int paginaAttuale, int paginaIniziale, string controller, string action, string categoria) 
            : base(numeroPagine, paginaAttuale, paginaIniziale, controller, action)
        {
            this.Categoria = categoria;
        }

        #endregion

        #region PROPRIETA

        public string Categoria { get; private set; }

        #endregion
    }
}
