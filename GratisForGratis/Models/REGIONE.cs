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
    
    public partial class REGIONE
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REGIONE()
        {
            this.PROVINCIA = new HashSet<PROVINCIA>();
        }
    
        public int ID { get; set; }
        public string NOME { get; set; }
        public Nullable<int> ID_NAZIONE { get; set; }
        public int STATO { get; set; }
    
        public virtual NAZIONE NAZIONE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PROVINCIA> PROVINCIA { get; set; }
    }
}
