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
    
    public partial class OGGETTO_APPARTENENZA
    {
        public int ID { get; set; }
        public int ID_OGGETTO { get; set; }
        public int ID_PERSONA { get; set; }
        public Nullable<int> ID_ATTIVITA { get; set; }
        public int TIPO { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public int STATO { get; set; }
    
        public virtual ATTIVITA ATTIVITA { get; set; }
        public virtual OGGETTO OGGETTO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
