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
    
    public partial class TRANSAZIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TRANSAZIONE()
        {
            this.CONTO_CORRENTE_MONETA = new HashSet<CONTO_CORRENTE_MONETA>();
            this.OFFERTA = new HashSet<OFFERTA>();
        }
    
        public int ID { get; set; }
        public int TEST { get; set; }
        public string NOME { get; set; }
        public int TIPO { get; set; }
        public Nullable<int> ID_ANNUNCIO { get; set; }
        public System.Guid ID_CONTO_MITTENTE { get; set; }
        public System.Guid ID_CONTO_DESTINATARIO { get; set; }
        public Nullable<int> SOLDI { get; set; }
        public Nullable<int> PUNTI { get; set; }
        public Nullable<System.DateTime> DATA_INSERIMENTO { get; set; }
        public Nullable<System.DateTime> DATA_MODIFICA { get; set; }
        public int STATO { get; set; }
    
        public virtual CONTO_CORRENTE CONTO_CORRENTE { get; set; }
        public virtual CONTO_CORRENTE CONTO_CORRENTE1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFERTA> OFFERTA { get; set; }
        public virtual ANNUNCIO ANNUNCIO { get; set; }
    }
}
