﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ABBONAMENTO> ABBONAMENTO { get; set; }
        public virtual DbSet<ALIMENTAZIONE> ALIMENTAZIONE { get; set; }
        public virtual DbSet<ANNUNCIO_CLICK> ANNUNCIO_CLICK { get; set; }
        public virtual DbSet<ANNUNCIO_FEEDBACK> ANNUNCIO_FEEDBACK { get; set; }
        public virtual DbSet<ANNUNCIO_FOTO> ANNUNCIO_FOTO { get; set; }
        public virtual DbSet<ANNUNCIO_TAG> ANNUNCIO_TAG { get; set; }
        public virtual DbSet<ANNUNCIO_VISUALIZZAZIONE> ANNUNCIO_VISUALIZZAZIONE { get; set; }
        public virtual DbSet<ARTISTA> ARTISTA { get; set; }
        public virtual DbSet<ATTIVITA> ATTIVITA { get; set; }
        public virtual DbSet<ATTIVITA_EMAIL> ATTIVITA_EMAIL { get; set; }
        public virtual DbSet<ATTIVITA_FOTO> ATTIVITA_FOTO { get; set; }
        public virtual DbSet<ATTIVITA_INDIRIZZO> ATTIVITA_INDIRIZZO { get; set; }
        public virtual DbSet<ATTIVITA_TELEFONO> ATTIVITA_TELEFONO { get; set; }
        public virtual DbSet<AUTORE> AUTORE { get; set; }
        public virtual DbSet<BROWSER> BROWSER { get; set; }
        public virtual DbSet<BROWSER_MODELLO> BROWSER_MODELLO { get; set; }
        public virtual DbSet<CATEGORIA> CATEGORIA { get; set; }
        public virtual DbSet<COMUNE> COMUNE { get; set; }
        public virtual DbSet<CONTO_CORRENTE> CONTO_CORRENTE { get; set; }
        public virtual DbSet<CORRIERE> CORRIERE { get; set; }
        public virtual DbSet<CORRIERE_NAZIONE> CORRIERE_NAZIONE { get; set; }
        public virtual DbSet<FORMATO> FORMATO { get; set; }
        public virtual DbSet<FOTO> FOTO { get; set; }
        public virtual DbSet<GENERE> GENERE { get; set; }
        public virtual DbSet<INDIRIZZO> INDIRIZZO { get; set; }
        public virtual DbSet<MONETA> MONETA { get; set; }
        public virtual DbSet<NAZIONE> NAZIONE { get; set; }
        public virtual DbSet<OFFERTA_BARATTO> OFFERTA_BARATTO { get; set; }
        public virtual DbSet<OGGETTO_COMPUTER> OGGETTO_COMPUTER { get; set; }
        public virtual DbSet<OGGETTO_CONSOLE> OGGETTO_CONSOLE { get; set; }
        public virtual DbSet<OGGETTO_ELETTRODOMESTICO> OGGETTO_ELETTRODOMESTICO { get; set; }
        public virtual DbSet<OGGETTO_GIOCO> OGGETTO_GIOCO { get; set; }
        public virtual DbSet<OGGETTO_LIBRO> OGGETTO_LIBRO { get; set; }
        public virtual DbSet<OGGETTO_MUSICA> OGGETTO_MUSICA { get; set; }
        public virtual DbSet<OGGETTO_SPORT> OGGETTO_SPORT { get; set; }
        public virtual DbSet<OGGETTO_STRUMENTO> OGGETTO_STRUMENTO { get; set; }
        public virtual DbSet<OGGETTO_TECNOLOGIA> OGGETTO_TECNOLOGIA { get; set; }
        public virtual DbSet<OGGETTO_TELEFONO> OGGETTO_TELEFONO { get; set; }
        public virtual DbSet<OGGETTO_VEICOLO> OGGETTO_VEICOLO { get; set; }
        public virtual DbSet<OGGETTO_VESTITO> OGGETTO_VESTITO { get; set; }
        public virtual DbSet<OGGETTO_VIDEO> OGGETTO_VIDEO { get; set; }
        public virtual DbSet<OGGETTO_VIDEOGAMES> OGGETTO_VIDEOGAMES { get; set; }
        public virtual DbSet<PERSONA_ATTIVITA> PERSONA_ATTIVITA { get; set; }
        public virtual DbSet<PERSONA_EMAIL> PERSONA_EMAIL { get; set; }
        public virtual DbSet<PERSONA_FOTO> PERSONA_FOTO { get; set; }
        public virtual DbSet<PERSONA_INDIRIZZO> PERSONA_INDIRIZZO { get; set; }
        public virtual DbSet<PERSONA_PRIVACY> PERSONA_PRIVACY { get; set; }
        public virtual DbSet<PERSONA_RICERCA> PERSONA_RICERCA { get; set; }
        public virtual DbSet<PERSONA_SEGNALAZIONE> PERSONA_SEGNALAZIONE { get; set; }
        public virtual DbSet<PERSONA_TELEFONO> PERSONA_TELEFONO { get; set; }
        public virtual DbSet<PIATTAFORMA> PIATTAFORMA { get; set; }
        public virtual DbSet<PROVINCIA> PROVINCIA { get; set; }
        public virtual DbSet<REGIONE> REGIONE { get; set; }
        public virtual DbSet<REGISTA> REGISTA { get; set; }
        public virtual DbSet<SERVIZIO> SERVIZIO { get; set; }
        public virtual DbSet<SISTEMA_OPERATIVO> SISTEMA_OPERATIVO { get; set; }
        public virtual DbSet<TRANSAZIONE> TRANSAZIONE { get; set; }
        public virtual DbSet<MARCA> MARCA { get; set; }
        public virtual DbSet<MODELLO> MODELLO { get; set; }
        public virtual DbSet<OGGETTO> OGGETTO { get; set; }
        public virtual DbSet<OFFERTA_CONTO_CORRENTE_MONETA> OFFERTA_CONTO_CORRENTE_MONETA { get; set; }
        public virtual DbSet<CONTO_CORRENTE_MONETA> CONTO_CORRENTE_MONETA { get; set; }
        public virtual DbSet<OFFERTA> OFFERTA { get; set; }
        public virtual DbSet<ANNUNCIO> ANNUNCIO { get; set; }
        public virtual DbSet<ANNUNCIO_SPEDIZIONE> ANNUNCIO_SPEDIZIONE { get; set; }
        public virtual DbSet<OGGETTO_APPARTENENZA> OGGETTO_APPARTENENZA { get; set; }
        public virtual DbSet<SPEDIZIONE> SPEDIZIONE { get; set; }
        public virtual DbSet<PERSONA> PERSONA { get; set; }
    
        public virtual int AGGIORNAMENTO_CATEGORIE()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("AGGIORNAMENTO_CATEGORIE");
        }
    
        public virtual int BENE_SAVE_BARATTO(Nullable<System.Guid> oFFERTA, Nullable<System.Guid> bARATTO)
        {
            var oFFERTAParameter = oFFERTA.HasValue ?
                new ObjectParameter("OFFERTA", oFFERTA) :
                new ObjectParameter("OFFERTA", typeof(System.Guid));
    
            var bARATTOParameter = bARATTO.HasValue ?
                new ObjectParameter("BARATTO", bARATTO) :
                new ObjectParameter("BARATTO", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BENE_SAVE_BARATTO", oFFERTAParameter, bARATTOParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> CHECK_NODO_FIGLIO(Nullable<int> iD)
        {
            var iDParameter = iD.HasValue ?
                new ObjectParameter("ID", iD) :
                new ObjectParameter("ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("CHECK_NODO_FIGLIO", iDParameter);
        }
    
        public virtual ObjectResult<FINDSOTTOCATEGORIE_Result> FINDSOTTOCATEGORIE(string nOME, Nullable<int> iD_PADRE)
        {
            var nOMEParameter = nOME != null ?
                new ObjectParameter("NOME", nOME) :
                new ObjectParameter("NOME", typeof(string));
    
            var iD_PADREParameter = iD_PADRE.HasValue ?
                new ObjectParameter("ID_PADRE", iD_PADRE) :
                new ObjectParameter("ID_PADRE", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FINDSOTTOCATEGORIE_Result>("FINDSOTTOCATEGORIE", nOMEParameter, iD_PADREParameter);
        }
    
        public virtual int INSERIMENTO_CATEGORIA(string nOME, Nullable<int> iD_PADRE)
        {
            var nOMEParameter = nOME != null ?
                new ObjectParameter("NOME", nOME) :
                new ObjectParameter("NOME", typeof(string));
    
            var iD_PADREParameter = iD_PADRE.HasValue ?
                new ObjectParameter("ID_PADRE", iD_PADRE) :
                new ObjectParameter("ID_PADRE", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("INSERIMENTO_CATEGORIA", nOMEParameter, iD_PADREParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> NUMERO_SOTTOCATEGORIE(string nODO)
        {
            var nODOParameter = nODO != null ?
                new ObjectParameter("NODO", nODO) :
                new ObjectParameter("NODO", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("NUMERO_SOTTOCATEGORIE", nODOParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> BENE_SAVE_OFFERTA(Nullable<System.Guid> tOKEN, Nullable<System.Guid> tOKEN_COMPRATORE, Nullable<int> nUMERO_PEZZI_RICHIESTI, Nullable<int> tIPO_OFFERTA, Nullable<int> pUNTI_OFFERTI, Nullable<int> sOLDI_OFFERTI, string bARATTI, ObjectParameter eRRORE)
        {
            var tOKENParameter = tOKEN.HasValue ?
                new ObjectParameter("TOKEN", tOKEN) :
                new ObjectParameter("TOKEN", typeof(System.Guid));
    
            var tOKEN_COMPRATOREParameter = tOKEN_COMPRATORE.HasValue ?
                new ObjectParameter("TOKEN_COMPRATORE", tOKEN_COMPRATORE) :
                new ObjectParameter("TOKEN_COMPRATORE", typeof(System.Guid));
    
            var nUMERO_PEZZI_RICHIESTIParameter = nUMERO_PEZZI_RICHIESTI.HasValue ?
                new ObjectParameter("NUMERO_PEZZI_RICHIESTI", nUMERO_PEZZI_RICHIESTI) :
                new ObjectParameter("NUMERO_PEZZI_RICHIESTI", typeof(int));
    
            var tIPO_OFFERTAParameter = tIPO_OFFERTA.HasValue ?
                new ObjectParameter("TIPO_OFFERTA", tIPO_OFFERTA) :
                new ObjectParameter("TIPO_OFFERTA", typeof(int));
    
            var pUNTI_OFFERTIParameter = pUNTI_OFFERTI.HasValue ?
                new ObjectParameter("PUNTI_OFFERTI", pUNTI_OFFERTI) :
                new ObjectParameter("PUNTI_OFFERTI", typeof(int));
    
            var sOLDI_OFFERTIParameter = sOLDI_OFFERTI.HasValue ?
                new ObjectParameter("SOLDI_OFFERTI", sOLDI_OFFERTI) :
                new ObjectParameter("SOLDI_OFFERTI", typeof(int));
    
            var bARATTIParameter = bARATTI != null ?
                new ObjectParameter("BARATTI", bARATTI) :
                new ObjectParameter("BARATTI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("BENE_SAVE_OFFERTA", tOKENParameter, tOKEN_COMPRATOREParameter, nUMERO_PEZZI_RICHIESTIParameter, tIPO_OFFERTAParameter, pUNTI_OFFERTIParameter, sOLDI_OFFERTIParameter, bARATTIParameter, eRRORE);
        }
    
        public virtual int BENE_SAVE_OFFERTA_SERVIZIO(Nullable<System.Guid> tOKEN, Nullable<System.Guid> tOKEN_COMPRATORE, Nullable<int> nUMERO_PEZZI_RICHIESTI, Nullable<int> tIPO_OFFERTA, Nullable<int> pUNTI_OFFERTI, Nullable<int> sOLDI_OFFERTI, string bARATTI, ObjectParameter eRRORE)
        {
            var tOKENParameter = tOKEN.HasValue ?
                new ObjectParameter("TOKEN", tOKEN) :
                new ObjectParameter("TOKEN", typeof(System.Guid));
    
            var tOKEN_COMPRATOREParameter = tOKEN_COMPRATORE.HasValue ?
                new ObjectParameter("TOKEN_COMPRATORE", tOKEN_COMPRATORE) :
                new ObjectParameter("TOKEN_COMPRATORE", typeof(System.Guid));
    
            var nUMERO_PEZZI_RICHIESTIParameter = nUMERO_PEZZI_RICHIESTI.HasValue ?
                new ObjectParameter("NUMERO_PEZZI_RICHIESTI", nUMERO_PEZZI_RICHIESTI) :
                new ObjectParameter("NUMERO_PEZZI_RICHIESTI", typeof(int));
    
            var tIPO_OFFERTAParameter = tIPO_OFFERTA.HasValue ?
                new ObjectParameter("TIPO_OFFERTA", tIPO_OFFERTA) :
                new ObjectParameter("TIPO_OFFERTA", typeof(int));
    
            var pUNTI_OFFERTIParameter = pUNTI_OFFERTI.HasValue ?
                new ObjectParameter("PUNTI_OFFERTI", pUNTI_OFFERTI) :
                new ObjectParameter("PUNTI_OFFERTI", typeof(int));
    
            var sOLDI_OFFERTIParameter = sOLDI_OFFERTI.HasValue ?
                new ObjectParameter("SOLDI_OFFERTI", sOLDI_OFFERTI) :
                new ObjectParameter("SOLDI_OFFERTI", typeof(int));
    
            var bARATTIParameter = bARATTI != null ?
                new ObjectParameter("BARATTI", bARATTI) :
                new ObjectParameter("BARATTI", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BENE_SAVE_OFFERTA_SERVIZIO", tOKENParameter, tOKEN_COMPRATOREParameter, nUMERO_PEZZI_RICHIESTIParameter, tIPO_OFFERTAParameter, pUNTI_OFFERTIParameter, sOLDI_OFFERTIParameter, bARATTIParameter, eRRORE);
        }
    
        public virtual ObjectResult<Nullable<int>> BENE_SAVE_PAGAMENTO(Nullable<int> oFFERTA_ID, Nullable<System.Guid> cOMPRATORE_CONTO_ID, ObjectParameter eRRORE)
        {
            var oFFERTA_IDParameter = oFFERTA_ID.HasValue ?
                new ObjectParameter("OFFERTA_ID", oFFERTA_ID) :
                new ObjectParameter("OFFERTA_ID", typeof(int));
    
            var cOMPRATORE_CONTO_IDParameter = cOMPRATORE_CONTO_ID.HasValue ?
                new ObjectParameter("COMPRATORE_CONTO_ID", cOMPRATORE_CONTO_ID) :
                new ObjectParameter("COMPRATORE_CONTO_ID", typeof(System.Guid));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("BENE_SAVE_PAGAMENTO", oFFERTA_IDParameter, cOMPRATORE_CONTO_IDParameter, eRRORE);
        }
    
        public virtual int BENE_UPDATE_OFFERTA(Nullable<int> oFFERTA, Nullable<int> sTATO, Nullable<int> tRANSAZIONE, Nullable<System.Guid> iD_CONTO_COMPRATORE, Nullable<System.Guid> iD_CONTO_VENDITORE, Nullable<int> aNNUNCIO, Nullable<int> pUNTI, ObjectParameter eRRORE)
        {
            var oFFERTAParameter = oFFERTA.HasValue ?
                new ObjectParameter("OFFERTA", oFFERTA) :
                new ObjectParameter("OFFERTA", typeof(int));
    
            var sTATOParameter = sTATO.HasValue ?
                new ObjectParameter("STATO", sTATO) :
                new ObjectParameter("STATO", typeof(int));
    
            var tRANSAZIONEParameter = tRANSAZIONE.HasValue ?
                new ObjectParameter("TRANSAZIONE", tRANSAZIONE) :
                new ObjectParameter("TRANSAZIONE", typeof(int));
    
            var iD_CONTO_COMPRATOREParameter = iD_CONTO_COMPRATORE.HasValue ?
                new ObjectParameter("ID_CONTO_COMPRATORE", iD_CONTO_COMPRATORE) :
                new ObjectParameter("ID_CONTO_COMPRATORE", typeof(System.Guid));
    
            var iD_CONTO_VENDITOREParameter = iD_CONTO_VENDITORE.HasValue ?
                new ObjectParameter("ID_CONTO_VENDITORE", iD_CONTO_VENDITORE) :
                new ObjectParameter("ID_CONTO_VENDITORE", typeof(System.Guid));
    
            var aNNUNCIOParameter = aNNUNCIO.HasValue ?
                new ObjectParameter("ANNUNCIO", aNNUNCIO) :
                new ObjectParameter("ANNUNCIO", typeof(int));
    
            var pUNTIParameter = pUNTI.HasValue ?
                new ObjectParameter("PUNTI", pUNTI) :
                new ObjectParameter("PUNTI", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("BENE_UPDATE_OFFERTA", oFFERTAParameter, sTATOParameter, tRANSAZIONEParameter, iD_CONTO_COMPRATOREParameter, iD_CONTO_VENDITOREParameter, aNNUNCIOParameter, pUNTIParameter, eRRORE);
        }
    }
}
