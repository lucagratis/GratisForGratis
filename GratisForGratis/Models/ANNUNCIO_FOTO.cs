//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GratisForGratis.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ANNUNCIO_FOTO
    {
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public int ID_FOTO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual FOTO FOTO { get; set; }
        public virtual ANNUNCIO ANNUNCIO { get; set; }
    }
}
