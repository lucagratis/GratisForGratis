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
    
    public partial class CONTO_CORRENTE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CONTO_CORRENTE()
        {
            this.ATTIVITA = new HashSet<ATTIVITA>();
            this.TRANSAZIONE = new HashSet<TRANSAZIONE>();
            this.TRANSAZIONE1 = new HashSet<TRANSAZIONE>();
            this.CONTO_CORRENTE_MONETA = new HashSet<CONTO_CORRENTE_MONETA>();
            this.PERSONA = new HashSet<PERSONA>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid TOKEN { get; set; }
        public double PUNTI { get; set; }
        public double PUNTI_SOSPESI { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ATTIVITA> ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE> TRANSAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSAZIONE> TRANSAZIONE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PERSONA> PERSONA { get; set; }
    }
}
