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
    
    public partial class OFFERTA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OFFERTA()
        {
            this.OFFERTA_BARATTO = new HashSet<OFFERTA_BARATTO>();
            this.OFFERTA_CONTO_CORRENTE_MONETA = new HashSet<OFFERTA_CONTO_CORRENTE_MONETA>();
        }
    
        public int ID { get; set; }
        public int ID_ANNUNCIO { get; set; }
        public Nullable<int> ID_PERSONA { get; set; }
        public Nullable<int> ID_ATTIVITA { get; set; }
        public int TIPO_OFFERTA { get; set; }
        public int TIPO_TRATTATIVA { get; set; }
        public Nullable<decimal> SOLDI { get; set; }
        public Nullable<int> PUNTI { get; set; }
        public string NOTE { get; set; }
        public Nullable<int> ID_TRANSAZIONE { get; set; }
        public System.DateTime DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int LETTA { get; set; }
        public int STATO { get; set; }
    
        public virtual ATTIVITA ATTIVITA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA_BARATTO> OFFERTA_BARATTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA_CONTO_CORRENTE_MONETA> OFFERTA_CONTO_CORRENTE_MONETA { get; set; }
        public virtual TRANSAZIONE TRANSAZIONE { get; set; }
        public virtual ANNUNCIO ANNUNCIO { get; set; }
        public virtual PERSONA PERSONA { get; set; }
    }
}
