using GratisForGratis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GratisForGratis.Controllers
{
    public class OfferteController : AdvancedController
    {
        [HttpGet]
        public ActionResult Effettuate(int pagina = 1)
        {
            List<OffertaEffettuataViewModel> offerte = new List<OffertaEffettuataViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    // verifica stato offerta se attivo o non accettata, identità utente e stato di attivazione, presenza bene o servizio -- vechia
                    // verifica identità utente e stato di attivazione, presenza bene o servizio
                    var query = db.OFFERTA.Where(item => item.PERSONA.ID == utente && item.PERSONA.STATO == (int)Stato.ATTIVO && (item.ANNUNCIO.ID_OGGETTO != null || item.ANNUNCIO.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    string randomString = Utils.RandomString(3);
                    List<OFFERTA> lista = query
                        .OrderByDescending(item => item.DATA_INSERIMENTO)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi).ToList();

                    foreach (OFFERTA item in lista)
                    {
                        OffertaEffettuataViewModel offertaEffettuata = new OffertaEffettuataViewModel();
                        offertaEffettuata.Id = item.ID.ToString();
                        offertaEffettuata.Nome = item.ANNUNCIO.NOME;
                        offertaEffettuata.Venditore = (item.ANNUNCIO.ID_ATTIVITA != null) ? item.ANNUNCIO.ATTIVITA.NOME : item.ANNUNCIO.PERSONA.NOME + ' ' + item.ANNUNCIO.PERSONA.COGNOME;
                        offertaEffettuata.Email = (item.ANNUNCIO.ID_ATTIVITA != null) ? item.ANNUNCIO.ATTIVITA.ATTIVITA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL : item.ANNUNCIO.PERSONA.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL;
                        offertaEffettuata.VenditoreToken = item.ANNUNCIO.PERSONA.TOKEN;
                        if (item.ANNUNCIO.ID_ATTIVITA != null)
                        {
                            ATTIVITA_TELEFONO telefono = item.ANNUNCIO.ATTIVITA.ATTIVITA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato);
                            if (telefono != null)
                                offertaEffettuata.Telefono = telefono.TELEFONO;
                        }else
                        {
                            PERSONA_TELEFONO telefono = item.ANNUNCIO.PERSONA.PERSONA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato);
                            if (telefono != null)
                                offertaEffettuata.Telefono = telefono.TELEFONO;
                        }
                        offertaEffettuata.Punti = (int)item.PUNTI;
                        offertaEffettuata.Soldi = (int)item.SOLDI;
                        offertaEffettuata.Categoria = item.ANNUNCIO.CATEGORIA.NOME;
                        offertaEffettuata.Foto = db.ANNUNCIO_FOTO
                            .Where(f => f.ID_ANNUNCIO == item.ANNUNCIO.ID)
                            .Select(f =>
                                f.FOTO.FOTO1
                            ).ToList();
                        offertaEffettuata.Baratti = db.OFFERTA_BARATTO
                            .Where(b => b.ID_OFFERTA == item.ID && b.ANNUNCIO != null)
                            .Select(b =>
                                new VenditaViewModel()
                                {
                                    Token = randomString + b.ANNUNCIO.TOKEN.ToString() + randomString,
                                    TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                    Nome = b.ANNUNCIO.NOME,
                                    Punti = b.ANNUNCIO.PUNTI,
                                    Soldi = b.ANNUNCIO.SOLDI,
                                }).ToList();
                        offertaEffettuata.Token = item.ANNUNCIO.TOKEN.ToString();
                        offertaEffettuata.TipoOfferta = (TipoOfferta)item.TIPO_OFFERTA;
                        offertaEffettuata.TipoTrattativa = (TipoTrattativa)item.TIPO_TRATTATIVA;
                        offertaEffettuata.TipoPagamento = (TipoPagamento)item.ANNUNCIO.TIPO_PAGAMENTO;
                        offertaEffettuata.StatoOfferta = (StatoOfferta)item.STATO;
                        offertaEffettuata.StatoVendita = (StatoVendita)item.ANNUNCIO.STATO;
                        offertaEffettuata.DataInserimento = (DateTime)item.DATA_INSERIMENTO;
                        offerte.Add(offertaEffettuata);
                    }
                    if (offerte.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(offerte);
        }

        [HttpGet]
        public ActionResult Acquisto(string acquisto = null, string baratto = null)
        {
            if (String.IsNullOrEmpty(acquisto) && String.IsNullOrEmpty(baratto))
                return RedirectToAction("", "Home");

            OffertaEffettuataViewModel viewModel = new OffertaEffettuataViewModel();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    OFFERTA offerta = new OFFERTA();
                    int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                    // fare un if e fare ricerca o per acquisto direttamente o per baratto
                    if (!String.IsNullOrEmpty(acquisto))
                    {
                        Guid tokenAcquisto = Guid.Parse(Utils.DecodeToString(acquisto.Trim().Substring(3, acquisto.Trim().Length - 6)));
                        offerta = db.OFFERTA.SingleOrDefault(c => c.ANNUNCIO.TOKEN == tokenAcquisto && c.ANNUNCIO.ID_PERSONA == idUtente && c.PERSONA.STATO == (int)Stato.ATTIVO);
                    }
                    else
                    {
                        Guid tokenBaratto = Guid.Parse(Utils.DecodeToString(baratto.Trim().Substring(3, baratto.Trim().Length - 6)));
                        offerta = db.OFFERTA.SingleOrDefault(c => c.OFFERTA_BARATTO.Count(b => b.ANNUNCIO.TOKEN == tokenBaratto && b.ANNUNCIO.ID_PERSONA == idUtente) > 0);
                    }
                    
                    viewModel = new OffertaEffettuataViewModel()
                    {
                        Id = offerta.ID.ToString(),
                        Nome = offerta.ANNUNCIO.NOME,
                        Venditore = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.NOME : offerta.ANNUNCIO.PERSONA.NOME + ' ' + offerta.ANNUNCIO.PERSONA.COGNOME,
                        Email = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.ATTIVITA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL : offerta.ANNUNCIO.PERSONA.PERSONA_EMAIL.SingleOrDefault(e => e.TIPO == (int)TipoEmail.Registrazione).EMAIL,
                        VenditoreToken = offerta.ANNUNCIO.PERSONA.TOKEN,
                        Telefono = (offerta.ANNUNCIO.ID_ATTIVITA != null) ? offerta.ANNUNCIO.ATTIVITA.ATTIVITA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato).TELEFONO : offerta.ANNUNCIO.PERSONA.PERSONA_TELEFONO.SingleOrDefault(t => t.TIPO == (int)TipoTelefono.Privato).TELEFONO,
                        Punti = (int)offerta.PUNTI,
                        Soldi = (int)offerta.SOLDI,
                        Categoria = offerta.ANNUNCIO.CATEGORIA.NOME,
                        Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == offerta.ANNUNCIO.ID).Select(f =>
                            f.FOTO.FOTO1
                            ).ToList(),
                        Baratti = db.OFFERTA_BARATTO.Where(b => b.ID_OFFERTA == offerta.ID && b.ANNUNCIO != null).Select(b =>
                                    new VenditaViewModel()
                                    {
                                        Token = b.ANNUNCIO.TOKEN.ToString(),
                                        TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                        Nome = b.ANNUNCIO.NOME,
                                        Punti = b.ANNUNCIO.PUNTI,
                                        Soldi = b.ANNUNCIO.SOLDI,
                                    }).ToList(),
                        Token = offerta.ANNUNCIO.TOKEN.ToString(),
                        TipoOfferta = (TipoOfferta)offerta.TIPO_OFFERTA,
                        TipoTrattativa = (TipoTrattativa)offerta.TIPO_TRATTATIVA,
                        TipoPagamento = (TipoPagamento)offerta.ANNUNCIO.TIPO_PAGAMENTO,
                        StatoOfferta = (StatoOfferta)offerta.STATO,
                        StatoVendita = (StatoVendita)offerta.ANNUNCIO.STATO,
                        DataInserimento = (DateTime)offerta.DATA_INSERIMENTO,
                        //PuntiCompratore = offerta.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA),
                        //PuntiSospesiCompratore = offerta.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.SOSPESA)
                    };
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Ricevute(int pagina = 1)
        {
            List<OffertaRicevutaViewModel> vendite = new List<OffertaRicevutaViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    int utente = ((PersonaModel)Session["utente"]).Persona.ID;
                    var query = db.ANNUNCIO.Where(item => item.ID_PERSONA == utente && (item.ID_OGGETTO != null || item.ID_SERVIZIO != null));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    vendite = query
                        .Select(item => new OffertaRicevutaViewModel()
                        {
                            IdInt = item.ID,
                            Token = item.TOKEN.ToString(),
                            Nome = item.NOME,
                            Punti = item.PUNTI,
                            Soldi = item.SOLDI,
                            TipoPagamento = (TipoPagamento)item.TIPO_PAGAMENTO,
                            TipoTrattativa = (TipoTrattativa)item.TIPO_TRATTATIVA,
                            Categoria = item.CATEGORIA.NOME,
                            Foto = db.ANNUNCIO_FOTO.Where(f => f.ID_ANNUNCIO == item.ID).Select(f =>
                                f.FOTO.FOTO1
                            ).ToList(),
                            DataInserimento = item.DATA_INSERIMENTO,
                            PuntiCompratore = item.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.ASSEGNATA),
                            PuntiSospesiCompratore = item.PERSONA.CONTO_CORRENTE.CONTO_CORRENTE_MONETA.Count(m => m.STATO == (int)StatoMoneta.SOSPESA),
                            StatoVendita = (StatoVendita)item.STATO
                        })
                        .OrderByDescending(item => item.DataInserimento)
                        .Skip(pagina * numeroElementi)
                        .Take(numeroElementi)
                        .ToList();
                    if (vendite.Count > 0)
                        RefreshPunteggioUtente(db);
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(vendite);
        }

        [HttpGet]
        public ActionResult OfferteVendita(string vendita, int pagina = 1)
        {
            if (String.IsNullOrEmpty(vendita))
                return RedirectToAction("","Home");

            OfferteVenditaViewModel offerte = new OfferteVenditaViewModel();
            try
            {
                Guid token = Guid.Parse(Utils.DecodeToString(vendita.Trim().Substring(3, vendita.Trim().Length - 6)));

                using (DatabaseContext db = new DatabaseContext())
                {
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    //var query = db.OFFERTA.Where(c => c.ANNUNCIO == idVendita && (c.STATO != (int)StatoOfferta.ANNULLATA || c.STATO != (int)StatoOfferta.INATTIVA || c.STATO != (int)StatoOfferta.SOSPESA));
                    var query = db.OFFERTA.Where(c => c.ANNUNCIO.TOKEN == token && c.PERSONA.STATO == (int)Stato.ATTIVO);
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)query.Count() / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    string randomString = Utils.RandomString(3);
                    List<OFFERTA> lista = query
                    .OrderByDescending(item => item.DATA_INSERIMENTO)
                    .Skip(pagina * numeroElementi)
                    .Take(numeroElementi)
                    .ToList();
                    // fare update LETTA della lista recuperata
                    offerte.IdInt = lista.Select(o => o.ANNUNCIO.ID).FirstOrDefault();
                    offerte.NomeVendita = lista.Select(o => o.ANNUNCIO.NOME).FirstOrDefault();
                    offerte.DataInserimento = lista.Select(o => o.ANNUNCIO.DATA_INSERIMENTO).FirstOrDefault();
                    lista.ForEach(o =>
                                offerte.Offerte.Add(new OffertaViewModel()
                                {
                                    Id = o.ID.ToString(),
                                    Punti = (int)o.PUNTI,
                                    Soldi = (int)o.SOLDI,
                                    DataInserimento = o.DATA_INSERIMENTO,
                                    TipoOfferta = (TipoOfferta)o.TIPO_OFFERTA,
                                    Compratore = new UtenteVenditaViewModel(o.PERSONA),
                                    StatoOfferta = (StatoOfferta)o.STATO,
                                    Baratti = db.OFFERTA_BARATTO.Where(b => b.ID_OFFERTA == o.ID && b.ANNUNCIO != null).OrderByDescending(item => o.DATA_INSERIMENTO).ToList().Select(b =>
                                        new VenditaViewModel()
                                        {
                                            Token = randomString + b.ANNUNCIO.TOKEN.ToString() + randomString,
                                            TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                            Nome = b.ANNUNCIO.NOME,
                                            Punti = b.ANNUNCIO.PUNTI,
                                            Soldi = b.ANNUNCIO.SOLDI,
                                        }).ToList(),
                                })
                    );

                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            
            return View(offerte);
        }

        [HttpGet]
        public ActionResult StoricoPagamenti(int pagina = 1)
        {
            List<SchedaPagamentoViewModel> risultato = new List<SchedaPagamentoViewModel>();
            try
            {
                using (DatabaseContext db = new DatabaseContext())
                {
                    Guid utente = ((PersonaModel)Session["utente"]).Persona.ID_CONTO_CORRENTE;
                    var query = db.TRANSAZIONE.Where(p => p.TEST == 0 && p.STATO == (int)StatoPagamento.ACCETTATO && (p.ID_CONTO_DESTINATARIO == utente || p.ID_CONTO_MITTENTE == utente));
                    int numeroElementi = Convert.ToInt32(WebConfigurationManager.AppSettings["numeroElementi"]);
                    int elementiTrovati = query.Count();
                    ViewData["TotalePagine"] = (int)Math.Ceiling((decimal)elementiTrovati / (decimal)numeroElementi);
                    ViewData["Pagina"] = pagina;
                    pagina -= 1;
                    string randomString = Utils.RandomString(3);
                    if (elementiTrovati > 0)
                    {
                        query
                            .OrderByDescending(item => item.STATO)
                            //.ThenByDescending(item => item.DATA_INSERIMENTO)
                            .OrderByDescending(item => item.DATA_INSERIMENTO)
                            .Skip(pagina * numeroElementi)
                            .Take(numeroElementi)
                            .ToList()
                            .ForEach(item => {
                                string compratore = string.Empty;
                                int idCompratore = 0;
                                string venditore = string.Empty;
                                int idVenditore = 0;
                                if (item.CONTO_CORRENTE1.PERSONA.Count > 0)
                                {
                                    PERSONA persona = item.CONTO_CORRENTE1.PERSONA.FirstOrDefault();
                                    compratore = persona.NOME + " " + persona.COGNOME;
                                    idCompratore = persona.ID;
                                }
                                else
                                {
                                    ATTIVITA attivita = item.CONTO_CORRENTE1.ATTIVITA.FirstOrDefault();
                                    compratore = attivita.NOME;
                                    idCompratore = attivita.ID;
                                }
                                if (item.CONTO_CORRENTE.PERSONA.Count > 0)
                                {
                                    PERSONA persona = item.CONTO_CORRENTE.PERSONA.FirstOrDefault();
                                    venditore = persona.NOME + " " + persona.COGNOME;
                                    idVenditore = persona.ID;
                                }
                                else
                                {
                                    ATTIVITA attivita = item.CONTO_CORRENTE.ATTIVITA.FirstOrDefault();
                                    venditore = attivita.NOME;
                                    idVenditore = attivita.ID;
                                }
                                risultato.Add(
                                    new SchedaPagamentoViewModel()
                                    {
                                        Token = item.ID.ToString(),
                                        Nome = item.NOME,
                                        Compratore = compratore,
                                        CompratoreId = idCompratore,
                                        Venditore = venditore,
                                        VenditoreId = idVenditore,
                                        Punti = (int)item.PUNTI,
                                        //Soldi = (int)item.SOLDI,
                                        Data = item.DATA_INSERIMENTO,
                                        Baratti = db.OFFERTA_BARATTO.Where(b => b.OFFERTA.ID_TRANSAZIONE == item.ID && b.ANNUNCIO != null).Select(b =>
                                                    new VenditaViewModel()
                                                    {
                                                        Token = randomString + b.ANNUNCIO.TOKEN.ToString() + randomString,
                                                        TipoAcquisto = b.ANNUNCIO.SERVIZIO != null ? TipoAcquisto.Servizio : TipoAcquisto.Oggetto,
                                                        Nome = b.ANNUNCIO.NOME,
                                                        Punti = b.ANNUNCIO.PUNTI,
                                                        //Soldi = b.ANNUNCIO.SOLDI,
                                                    }).ToList(),
                                    });
                            });
                    }
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View(risultato);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult AccettaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token.Substring(3).Substring(0, token.Length - 6));
            string messaggio = "";
            OffertaModel offerta = new OffertaModel(idOfferta, (Session["utente"] as PersonaModel).Persona);
            if (offerta.Accetta(ref messaggio))
            {
                return Json(new { Messaggio = App_GlobalResources.Language.AcceptedBid });
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorAcceptBid);
        }

        [HttpPost]
        [Filters.ValidateAjax]
        public ActionResult RifiutaOfferta(string token)
        {
            int idOfferta = Utils.DecodeToInt(token.Substring(3).Substring(0, token.Length - 6));
            OffertaModel offerta = new OffertaModel(idOfferta, (Session["utente"] as PersonaModel).Persona);
            if (offerta.Rifiuta())
            {
                return Json(App_GlobalResources.Language.RefusedBid);
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorRefuseBid);
        }

        [HttpDelete]
        [Filters.ValidateAjax]
        public ActionResult AnnullaVendita(string token)
        {
            DatabaseContext db = new DatabaseContext();
            try
            {
                token = Server.UrlDecode(token);
                Guid tokenDecriptato = Guid.Parse(Utils.DecodeToString(token.Substring(3).Substring(0, token.Length - 6)));
                int idUtente = (Session["utente"] as PersonaModel).Persona.ID;
                //using (DatabaseContext db = new DatabaseContext())
                //{
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    ANNUNCIO model = db.ANNUNCIO.Where(v => v.TOKEN == tokenDecriptato && v.ID_PERSONA == idUtente && v.STATO != (int)StatoVendita.BARATTATO
                        && v.STATO != (int)StatoVendita.ELIMINATO && v.STATO != (int)StatoVendita.VENDUTO).SingleOrDefault();
                    if (model != null)
                    {
                        model.STATO = (int)StatoVendita.ELIMINATO;
                        model.DATA_MODIFICA = DateTime.Now;
                        if (db.SaveChanges() > 0)
                        {
                            OffertaModel.AnnullaOfferteEffettuate(db, model.ID);
                            OffertaModel.AnnullaOfferteRicevute(db, model.ID);
                            transazione.Commit();
                            return Json(new { Messaggio = App_GlobalResources.Language.StateSellDelete });
                        }
                    }
                    transazione.Rollback();
                    //}
                }
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorRefuseBid);
        }

        [HttpDelete]
        [Filters.ValidateAjax]
        public ActionResult AnnullaBaratto(string token)
        {
            DatabaseContext db = new DatabaseContext();
            try
            {
                token = Server.UrlDecode(token);
                Guid tokenDecriptato = Guid.Parse(Utils.DecodeToString(token.Substring(3).Substring(0, token.Length - 6)));
                PersonaModel utente = (Session["utente"] as PersonaModel);
                //using (DatabaseContext db = new DatabaseContext())
                //{
                db.Database.Connection.Open();
                using (var transazione = db.Database.BeginTransaction())
                {
                    OFFERTA_BARATTO model = db.OFFERTA_BARATTO.Where(b => b.ANNUNCIO.TOKEN == tokenDecriptato && b.OFFERTA.ID_PERSONA == utente.Persona.ID && b.OFFERTA.STATO == (int)StatoOfferta.ATTIVA).SingleOrDefault();
                    if (model != null)
                    {
                        // cambio stato alla vendita
                        model.ANNUNCIO.STATO = (int)StatoVendita.ATTIVO;
                        model.ANNUNCIO.DATA_MODIFICA = DateTime.Now;
                        // cambio stato offerta
                        model.OFFERTA.DATA_MODIFICA = DateTime.Now;
                        model.OFFERTA.STATO = (int)StatoOfferta.ANNULLATA;
                        // restituisco eventuali punti sospesi
                        OffertaContoCorrenteMoneta offertaMoneta = new OffertaContoCorrenteMoneta();
                        offertaMoneta.RemoveCrediti(db, model.ID_OFFERTA, utente);

                        int numeroRecordModificati = db.SaveChanges();
                        if (numeroRecordModificati > 2)
                        {
                            transazione.Commit();
                            return Json(new { Messaggio = App_GlobalResources.Language.StateCanceledBarter });
                        }
                    }
                    transazione.Rollback();
                }
                //}
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            finally
            {
                if (db.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    db.Database.Connection.Close();
                    db.Database.Connection.Dispose();
                }
            }
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            return Json(App_GlobalResources.Language.ErrorCancelBarter);
        }

    }
}