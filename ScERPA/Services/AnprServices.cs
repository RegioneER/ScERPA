using Azure.Core;
using ScERPA.Services.Interfaces;
using ScERPA.ViewModels;
using System.Net;
using ScERPA.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

using Microsoft.Identity.Client;
using ScERPA.Models.Dictionaries;
using System.Configuration;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Text;
using System.Net.Http.Headers;
using NuGet.Packaging;
using Microsoft.Extensions.Options;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Font;
using iText.Layout.Properties;
using iText.Pdfa;
using iText.IO.Font;

using iText.Kernel.Pdf.Navigation;
using iText.Kernel.Geom;
using iText.IO.Font.Constants;
using iText.IO.Image;
using Microsoft.CodeAnalysis.Operations;
using iText.Layout.Borders;
using System;
using Humanizer.Localisation;
using System.Drawing;
using iText.Bouncycastleconnector;
using System;
using System.IO;
using Serilog.Context;
using ScERPA.Models.Reports;
using Microsoft.Extensions.Azure;
using ScERPA.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using iText.StyledXmlParser.Node;
using ScERPA.Models.Exceptions;
using System.Globalization;
using ScERPA.Models.Enums.ANPR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OfficeOpenXml;
using System.IO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml.Table;
using ScERPA.Models.DTOs;

namespace ScERPA.Services
{
    public class AnprServices : IAnprServices
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<AnprServices> _logger;
        private readonly IMassiveRequestsServices _massiveRequestsServices;
        private readonly IAnprApi _anprApi;
        private readonly int _numeroMassimoCodiciFiscali;
        private readonly IConsumerServices _consumerServices;
        private readonly int _delay=1000;


        public AnprServices(IAnprApi anprApi, IConfiguration configuration, ILogger<AnprServices> logger,  IConsumerServices consumerServices, IMassiveRequestsServices massiveRequestsServices) 
        {

            _configuration = configuration;
            _logger = logger;

            _anprApi = anprApi;
            _numeroMassimoCodiciFiscali = _configuration.GetSection("ANPR").GetValue<int>("NumeroMassimoCodiciFiscali");
            _delay = _configuration.GetSection("ANPR").GetValue<int>("delay");
            _consumerServices = consumerServices;
            _massiveRequestsServices = massiveRequestsServices;
        }

        public int NumeroMassimoCodiciFiscali
        {
            get {
                return _numeroMassimoCodiciFiscali;
            }
        }

        public async Task<List<RisultatoVerificaCittadinanzaViewModel>> ServizioVerificaCittadinanzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA,string purpouseID, string? operationGuid)
        {
            List<RisultatoVerificaCittadinanzaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioVerificaSingolaCittadinanzaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }



            return risultati;
        }

        public async Task<RisultatoVerificaCittadinanzaViewModel> ServizioVerificaSingolaCittadinanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID,string? operationGuid)
        {
            RisultatoVerificaCittadinanzaViewModel risultato = new();
            risultato.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;


            risultato.CittadinanzaItaliana = false;
            risultato.CodiceFiscale = codiceFiscale;
            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioVerificaDichiarazioneCittadinanzaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID,operationGuid);

            if (apiResult.Status== ApiResultStatus.Success)
            {
                risposta = apiResult.Data;
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto la VerificaDichiarazioneCittadinanza chiamata Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta?.listaSoggetti?.datiSoggetto?.Count() > 1)
                {

                    risultato.Errore = "Codice fiscale duplicato";
                }
                else
                {
                    bool deceduto = false;

                    if (risposta?.listaSoggetti?.datiSoggetto?.FirstOrDefault()?.datiDecesso is not null)
                    {
                        deceduto = true;
                    }
                    else if (risposta?.listaSoggetti?.datiSoggetto?.FirstOrDefault()?.infoSoggettoEnte is not null)
                    {
                        var Decesso = risposta.listaSoggetti?.datiSoggetto?.FirstOrDefault()?.infoSoggettoEnte?.ToList()?.FirstOrDefault(x => x?.chiave?.ToString() == "Data decesso");
                        if (Decesso is not null)
                        {
                            DateOnly dataDecesso;

                            DateOnly.TryParseExact(Decesso.valoreData, new[] { "dd/MM/yyyy", "yyyy-MM-dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out dataDecesso);

                            deceduto = dataDecesso < DataRiferimentoVerifica;
                        }
                        else
                        {
                            var Record = risposta.listaSoggetti?.datiSoggetto?.FirstOrDefault()?.infoSoggettoEnte?.FirstOrDefault(x => x.chiave?.ToString() == "Verifica cittadinanza");
                            if (Record is not null)
                            {
                                risultato.CittadinanzaItaliana = Record.valore?.ToUpper().Trim() == "S";
                            }

                        }
                    }
                   
                    risultato.Errore = deceduto ? "Deceduto alla data verifica" : "";
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto la VerificaDichiarazioneCittadinanza ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Verifica non riuscita" : apiResult.Message;
                risultato.CodiceFiscale = codiceFiscale;
            }

            return risultato;

        }


        public Report CreateReport(string nomeBreve, string titolo, string sottotitolo, List<IElementoSchedaReport> lista, DateOnly dataVerifica, string userID, string userLocation, string LoA, string? operationGuid)
        {
            Report report = new(nomeBreve, titolo, sottotitolo);
            try { 

                //serzione Principale degli esiti chiamate
                report.Sezioni.Add(CreateReportMainSection(dataVerifica,userID,userLocation,LoA,operationGuid));
            
                if(lista.Count >0 )
                {
                    SezioneReport sezioneEsito = new("Dati esito", "Dati esito", "");
                    foreach (IElementoSchedaReport elemento in lista)
                    {
                        SchedaReport schedaReport = new();
                        schedaReport.Dati = elemento.GetData();
                        sezioneEsito.Schede.Add(schedaReport);
                    }
                    report.Sezioni.Add(sezioneEsito);
                }



                //sezione valori verifica (presente solo se ci sono dettagli per ogni verifica)
                if (lista.Count > 0)
                {
                    SezioneReport sezioneValori = new("Valori esito", "Valori esito", " ");
                
                    foreach (IElementoSchedaReport elemento in lista)
                    {
                        var datiRelazionati = elemento.GetRelatedData();
                    
                        foreach(var datoRelazionato in datiRelazionati)
                        {
                            SchedaReport schedaValori = new();
                            schedaValori.Dati = datoRelazionato;
                            sezioneValori.Schede.Add(schedaValori);
                        }

                    }
                    if(sezioneValori.Schede.Count > 0)
                    {
                        report.Sezioni.Add(sezioneValori);
                    }

                }

            } catch (Exception ex) {
                _logger.LogError(ex,"Si è verificato un errore nella generazione del report");
            }
            return report;

        }

        public SezioneReport CreateReportMainSection(DateOnly dataVerifica, string userID, string userLocation, string LoA, string? operationGuid)
        {
            SezioneReport sezione = new("Dati verifica", "Dati verifica","");

            //aggiungo la scheda e i suoi dati
            SchedaReport schedaReport = new();
            schedaReport.Dati.Add("Eseguito il", DateTime.Now.ToString("dd-MM-yyyy"));
            schedaReport.Dati.Add("Eseguito da", userID);
            schedaReport.Dati.Add("Postazione", userLocation);
            schedaReport.Dati.Add("Livello autenticazione", LoA);
            schedaReport.Dati.Add("Data riferimento verifica", dataVerifica.ToString("dd-MM-yyyy"));
            schedaReport.Dati.Add("Id operazione", operationGuid + "");
            sezione.Schede.Add(schedaReport);
            return sezione;
        }

        public async Task<List<RisultatoAccertamentoResidenzaViewModel>> ServizioAccertamentoResidenzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoResidenzaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaResidenzaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<RisultatoAccertamentoResidenzaViewModel> ServizioAccertamentoSingolaResidenzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoResidenzaViewModel risultato = new();
        
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoResidenzaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto l'AccertamentoResidenza Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;

                    if (datisoggetto.Length > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else
                                {
                                    risultato.Errore = "Deceduto";
                                }

                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else
                                    {
                                        risultato.Errore = "Deceduto";
                                    }

                                }
                            }

                          

                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                            if(soggettoCorrente.generalita is not null)
                            {
                                risultato.Generalita = this.GetAnprGeneralitaViewModel(soggettoCorrente.generalita);
                            }

                            if (soggettoCorrente.residenza is not null)
                            {
                                TipoResidenza? Record = soggettoCorrente.residenza?.Where(x => x.tipoIndirizzo == "1" || x.tipoIndirizzo == "10" || x.tipoIndirizzo == "11" || x.tipoIndirizzo == "4").OrderBy(x => x.tipoIndirizzo).SingleOrDefault();
                                if (Record is not null)
                                {
                                    risultato.DecorrenzaResidenza = Record.dataDecorrenzaResidenza + "";

                                    risultato.ResidenzaAIRE = Record.tipoIndirizzo ?? "";

                                    if (Record.localitaEstera is not null)
                                    {
                                        risultato.Nazione = Record.localitaEstera.indirizzoEstero?.localita?.descrizioneStato + "";
                                        risultato.TipoIndirizzo = Record.tipoIndirizzo + "";
                                        risultato.CodNazione = Record.localitaEstera.indirizzoEstero?.localita?.codiceStato + "";
                                        risultato.Localita = Record.localitaEstera.indirizzoEstero?.localita?.descrizioneLocalita + "";
                                        risultato.CodIstatComune = "";
                                        risultato.Provincia = Record.localitaEstera.indirizzoEstero?.localita?.provinciaContea + "";
                                        risultato.CAP = Record.localitaEstera.indirizzoEstero?.cap + "";
                                        risultato.Frazione = "";
                                        risultato.Indirizzo = Record.localitaEstera.indirizzoEstero?.toponimo?.denominazione + (Record.localitaEstera.consolato is null ? "" : " - " + Record.localitaEstera.consolato.descrizioneConsolato);

                                        risultato.Civico = Record.localitaEstera.indirizzoEstero?.toponimo?.numeroCivico + "";
                                        risultato.Presso = Record.presso ?? "";

                                    }
                                    else
                                    {
                                        risultato.Nazione = "ITALIA";
                                        risultato.TipoIndirizzo = Record.tipoIndirizzo + "";
                                        risultato.CodNazione = "100";
                                        risultato.Localita = Record.indirizzo?.comune?.nomeComune + (Record.indirizzo?.comune?.descrizioneLocalita is null ? "" : " " + Record.indirizzo?.comune?.descrizioneLocalita) + "";
                                        risultato.CodIstatComune = Record.indirizzo?.comune?.codiceIstat ?? "";
                                        risultato.Provincia = Record.indirizzo?.comune?.siglaProvinciaIstat + "";
                                        risultato.CAP = Record.indirizzo?.cap + "";
                                        risultato.Frazione = Record.indirizzo?.frazione + "";
                                        risultato.Indirizzo = Record.indirizzo?.toponimo?.specie + " " + Record.indirizzo?.toponimo?.denominazioneToponimo;
                                        risultato.Civico = Record.indirizzo?.numeroCivico?.numero + Record.indirizzo?.numeroCivico?.lettera + Record.indirizzo?.numeroCivico?.metrico;
                                        risultato.Presso = Record.presso ?? "";
                                    }

                                }


                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati residenza mancanti";
                            }


                        }

                    }

                    risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";
                }

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto la AccertamentoResidenza ma si è verificato un errore", userID, userLocation);
                risultato.Generalita.CodiceFiscale = codiceFiscale;
                risultato.Errore = apiResult.Message is null ? "Verifica non riuscita" : apiResult.Message;
            }

            return risultato;
        }

        public async Task<List<RisultatoAccertamentoStatoDiFamigliaViewModel>> ServizioAccertamentoStatoDiFamigliaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoStatoDiFamigliaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingoloStatoDiFamigliaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);
                    }
                }
            } else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }


            return risultati;
        }

        public async Task<RisultatoAccertamentoStatoDiFamigliaViewModel> ServizioAccertamentoSingoloStatoDiFamigliaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoStatoDiFamigliaViewModel risultato = new();            
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;

            codiceFiscale = codiceFiscale.Trim().ToUpper();
            risultato.CodiceFiscale = codiceFiscale;

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoStatoDiFamigliaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto l'AccertamentoStatoDiFamiglia Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta != null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    foreach (TipoDatiSoggettiEnte soggetto in risposta.listaSoggetti.datiSoggetto)
                    {
                        if (soggetto != null)
                        {
                            TipoDatiSoggettiEnte soggettoCorrente = soggetto;
                            ComponenteStatoDiFamigliaViewModel componente = new();
                            if (soggettoCorrente.generalita != null)
                            {
                                TipoGeneralita generalitaCorrente = soggettoCorrente.generalita;
                                componente.Generalita = GetAnprGeneralitaViewModel(generalitaCorrente);

                                if (soggetto.identificativi is not null)
                                {
                                    componente.idANPR = soggetto.identificativi.idANPR ?? "";
                                }

                            }

                            if (soggettoCorrente.legameSoggetto != null)
                            {
                                TipoLegameSoggettoCompleto tipoLegameSoggetto = soggettoCorrente.legameSoggetto;
                                componente.TipoLegame = tipoLegameSoggetto.tipoLegame + "";

                                componente.TipoRelazioniParentela = tipoLegameSoggetto.codiceLegame?.ToString() + "";
                                componente.TipoLegameConvivenza = tipoLegameSoggetto.codiceLegame?.ToString() + "";

                                componente.DataDecorrenzaRelazioneParentela = tipoLegameSoggetto.dataDecorrenza + "";
                            }


                            risultato.Componenti.Add(componente);
                        }

                    }
          

                    risultato.Componenti.Sort((c1, c2) => c1.TipoRelazioniParentelaCod.CompareTo(c2.TipoRelazioniParentelaCod));
                    risultato.IdInterrogazioneAnpr = risposta.idOperazioneANPR ?? "";

                }

                
            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserID} dalla postazione {UserLocation} ha chiesto la AccertamentoStadoDiFamiglia ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Verifica non riuscita" : apiResult.Message;
                risultato.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<List<RisultatoAccertamentoGeneralitaViewModel>> ServizioAccertamentoGeneralitaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoGeneralitaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaGeneralitaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<List<RisultatoAccertamentoDichDecessoViewModel>> ServizioAccertamentoDichDecessoAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoDichDecessoViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaDichDecessoAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<List<RisultatoAccertamentoCittadinanzaViewModel>> ServizioAccertamentoCittadinanzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoCittadinanzaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaCittadinanzaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<List<RisultatoAccertamentoEsistenzaVitaViewModel>> ServizioAccertamentoEsistenzaVitaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoEsistenzaVitaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaEsistenzaVitaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<List<RisultatoAccertamentoVedovanzaViewModel>> ServizioAccertamentoVedovanzaAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoVedovanzaViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingolaVedovanzaAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }
     

        public async Task<List<RisultatoAccertamentoGenitoreViewModel>> ServizioAccertamentoGenitoreAsync(TipologiaRicercaGenitore tipoRicercaGenitore,string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoGenitoreViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingoloGenitoreAsync(tipoRicercaGenitore,user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<RisultatoAccertamentoDichDecessoViewModel> ServizioAccertamentoSingolaDichDecessoAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoDichDecessoViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoDichDecessoAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoDichDecesso Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {


                            if (soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità mancanti";
                            }

                            if (soggettoCorrente.datiDecesso is not null)
                            {
                                risultato.Decesso = GetAnperDecessoViewModel(soggettoCorrente.datiDecesso);
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var deceduto = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (deceduto.Any())
                                {
                                    risultato.Decesso.Deceduto = "S";
                                    risultato.Decesso.DataDecesso = deceduto.First().valoreData ?? "";
                                }
                            } 

                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                            if(soggettoCorrente.datiCancellazione is not null)
                            {
                                if(risultato.Generalita.SoggettoAIRE.ToLower() == "si")
                                {
                                    risultato.MotivoDiCancellazioneAire = soggettoCorrente.datiCancellazione.motivoCancellazione ?? "";
                                }
                                else
                                {
                                    risultato.MotivoDiCancellazioneNoAire = soggettoCorrente.datiCancellazione.motivoCancellazione ?? "";
                                }
                                risultato.DataCancellazione = soggettoCorrente.datiCancellazione.dataDecorrenzaCancellazione ?? "";
                                risultato.NoteCancellazione = soggettoCorrente.datiCancellazione.notecancellazione ?? "";
                                //mancano i dati sentenza

                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoDichDecesso ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<RisultatoAccertamentoGeneralitaViewModel> ServizioAccertamentoSingolaGeneralitaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoGeneralitaViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoGeneralitaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoGeneralita Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");
               
                if(risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if(datisoggetto.Count()>1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    } else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if(soggettoCorrente is not null )
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if(DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x=>x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore =  "Deceduto il " + dataDecesso.ToString("d") ;
                                    } 
                                    else risultato.Errore =  "Deceduto";

                                }
                            }

                            if(soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            } else {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità mancanti";
                            }

                            if(soggettoCorrente.statoCivile is not null)
                            {
                                TipoStatoCivile statoCivileSoggettoCorrente = soggettoCorrente.statoCivile;
                                risultato.StatoCivile = statoCivileSoggettoCorrente.statoCivile ?? "";
                                risultato.StatoCivileNonDisponibile = statoCivileSoggettoCorrente.statoCivileND ?? "";
                            }

                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                        } 

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoGeneralita ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }


        public async Task<RisultatoAccertamentoCittadinanzaViewModel> ServizioAccertamentoSingolaCittadinanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoCittadinanzaViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoCittadinanzaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoCittadinanza Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else risultato.Errore = "Deceduto";

                                }
                            }

                            if (soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità mancanti";
                            }

                            if (soggettoCorrente.cittadinanza is not null)
                            {
                                TipoCittadinanza[] cittadinanzaSoggettoCorrente = soggettoCorrente.cittadinanza;
                                risultato.Cittadinanza = string.Join(";", cittadinanzaSoggettoCorrente.Select(x=>new string(x.descrizioneStato ?? "")).ToArray());
                                risultato.CodCittadinanza = string.Join(";", cittadinanzaSoggettoCorrente.Select(x => new string(x.codiceStato ?? "")).ToArray());
                                risultato.DataCittadinanza = string.Join(";", cittadinanzaSoggettoCorrente.Select(x => new string(x.dataValidita ?? "")).ToArray());
                            }

                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoGeneralita ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<RisultatoAccertamentoEsistenzaVitaViewModel> ServizioAccertamentoSingolaEsistenzaVitaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoEsistenzaVitaViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoEsistenzaVitaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoEsitenzaVita Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString().ToLower() == "data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else risultato.Errore = "Deceduto";

                                }

                                var inVita = soggettoCorrente.infoSoggettoEnte.Where(x => x.chiave.ToString().ToLower() == "verifica esistenza in vita").SingleOrDefault();

                                risultato.InVita = inVita is not null ? inVita.valore : "N";

                            }

                            if (soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità mancanti";
                            }



                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoEsistenzaVita ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<RisultatoAccertamentoVedovanzaViewModel> ServizioAccertamentoSingolaVedovanzaAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoVedovanzaViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoVedovanzaAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoVedovanzaAnpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else risultato.Errore = "Deceduto";

                                }
                            }

                            if (soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità mancanti";
                            }

                            if (soggettoCorrente.statoCivile is not null)
                            {
                                TipoStatoCivile statoCivileSoggettoCorrente = soggettoCorrente.statoCivile;
                                risultato.StatoCivile = statoCivileSoggettoCorrente.statoCivile ?? "";
                                risultato.StatoCivileNonDisponibile = statoCivileSoggettoCorrente.statoCivileND ?? "";
                            }

                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                            if (soggettoCorrente.matrimonio is not null && soggettoCorrente.matrimonio.coniuge is not null)
                            {
                                risultato.Coniuge.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.matrimonio.coniuge);
                            }

                            if(soggettoCorrente.vedovanza is not null)
                            {
                                var vedovanza = soggettoCorrente.vedovanza;
                                risultato.Vedovanza = "S";
                                if(vedovanza.datiMorteconiuge is not null)
                                {
                                    risultato.Coniuge.Decesso = this.GetAnperDecessoViewModel(vedovanza.datiMorteconiuge);
                                    risultato.Coniuge.Decesso.Deceduto = "S";
                                } else
                                {
                                    risultato.Coniuge.Decesso.Deceduto = "N";
                                }

                            } else
                            {
                                risultato.Vedovanza = "N";
                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoVedovanza ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }




        public AnprGeneralitaViewModel GetAnprGeneralitaViewModel(TipoGeneralita generalitaCorrente)
        {
            AnprGeneralitaViewModel vm = new();
            vm.CodiceFiscale = generalitaCorrente.codiceFiscale?.codFiscale ?? "";
            vm.Cognome = generalitaCorrente.cognome ?? "";
            vm.Nome = generalitaCorrente.nome ?? "";
            vm.DataNascita = generalitaCorrente.dataNascita ?? "";
            vm.Sesso = generalitaCorrente.sesso ?? "";
            vm.CodiceFiscaleVerificato = generalitaCorrente.codiceFiscale?.validitaCF ?? "";
            vm.SoggettoAIRE = generalitaCorrente.soggettoAIRE ?? "";
            vm.AnnoEspatrio = generalitaCorrente.annoEspatrio ?? "";
            vm.SenzaCognome = generalitaCorrente.senzaCognome ?? "";
            vm.SenzaNome = generalitaCorrente.senzaNome ?? "";
            vm.SenzaGiornoNascita = generalitaCorrente.senzaGiorno ?? "";
            vm.SenzaGiornoMeseNascita = generalitaCorrente.senzaGiornoMese ?? "";

            TipoLuogoEvento? luogoNascitaCorrente = generalitaCorrente.luogoNascita;
            if (luogoNascitaCorrente != null)
            {
                if (luogoNascitaCorrente.localita is null)
                {
                    vm.CodNazione = "100";
                    vm.Nazione = "ITALIA";
                    vm.CodIstatComune = luogoNascitaCorrente.comune?.codiceIstat ?? "";
                    vm.Localita = luogoNascitaCorrente.comune?.nomeComune ?? "";
                    vm.Provincia = luogoNascitaCorrente.comune?.siglaProvinciaIstat ?? "";

                }
                else
                {
                    vm.CodNazione = luogoNascitaCorrente.localita?.codiceStato ?? "";
                    vm.Nazione = luogoNascitaCorrente.localita?.descrizioneStato ?? "";
                    vm.Provincia = luogoNascitaCorrente.localita?.provinciaContea ?? "";
                    vm.Localita = luogoNascitaCorrente.localita?.descrizioneLocalita ?? "";
                    vm.CodIstatComune = "";
                }
                vm.LuogoEccezionale = luogoNascitaCorrente.luogoEccezionale ?? "";
            }

            return vm;
        }

        public AnprDecessoViewModel GetAnperDecessoViewModel(TipoDatiEvento decesso)
        {
            AnprDecessoViewModel vm = new();

            vm.Deceduto = "S";
            vm.DataDecesso = decesso.dataEvento ?? "";

            TipoLuogoEvento? luogoDecesso = decesso.luogoEvento;
            if (luogoDecesso is not null)
            {
                if (luogoDecesso.localita is null)
                {
                    vm.CodNazione = "100";
                    vm.Nazione = "ITALIA";
                    vm.CodIstatComune = luogoDecesso.comune?.codiceIstat ?? "";
                    vm.Localita = luogoDecesso.comune?.nomeComune ?? "";
                    vm.Provincia = luogoDecesso.comune?.siglaProvinciaIstat ?? "";
                }
                else
                {
                    vm.CodNazione = luogoDecesso.localita?.codiceStato ?? "";
                    vm.Nazione = luogoDecesso.localita?.descrizioneStato ?? "";
                    vm.Provincia = luogoDecesso.localita?.provinciaContea ?? "";
                    vm.Localita = luogoDecesso.localita?.descrizioneLocalita ?? "";
                    vm.CodIstatComune = "";
                }

                vm.LuogoEccezionale = luogoDecesso.luogoEccezionale ?? "";
            }

            TipoAttoANPR? attoDecessoEvento = decesso.attoEventoANPR;
            if (attoDecessoEvento is not null)
            {
                TipoAtto? atto = attoDecessoEvento.atto;
                TipoAttoANSC? attoANSC = attoDecessoEvento.attoANSC;

                if (atto is not null)
                {
                    vm.DataFormazioneAtto = atto.dataFormazioneAtto ?? "";
                    vm.AnnoAtto = atto.anno ?? "";
                    vm.NumeroAtto = atto.numeroAtto ?? "";
                    vm.ParteAtto = atto.parte ?? "";
                    vm.SerieAtto = atto.serie ?? "";
                    vm.VolumeAtto = atto.volume ?? "";
                    vm.UfficioMunicipioAtto = atto.ufficioMunicipio ?? "";
                    vm.TrascrizioneAtto = atto.trascritto ?? "";

                    TipoComune? comuneAtto = atto.comuneRegistrazione;
                    if(comuneAtto is not null)
                    {
                        vm.NomeComuneAtto = comuneAtto.nomeComune ?? "";
                        vm.CodIstatComuneAtto = comuneAtto.codiceIstat ?? "";
                        vm.ProvinciaComuneAtto = comuneAtto.siglaProvinciaIstat ?? "";
                        vm.Localita = comuneAtto.descrizioneLocalita ?? "";
                    }
                }

                if (attoANSC is not null)
                {
                    vm.DataFormazioneAtto = attoANSC.dataFormazioneAtto ?? "";
                    vm.AnnoAtto = attoANSC.anno ?? "";

                    vm.UfficioMunicipioAtto = attoANSC.ufficioMunicipio ?? "";
                    vm.TrascrizioneAtto = attoANSC.trascritto ?? "";
                    vm.NumeroAtto = attoANSC.numeroComunale ?? "";
                    vm.ParteAtto =  "";
                    vm.SerieAtto =  "";
                    vm.VolumeAtto =  "";
                    TipoComune? comuneAtto = attoANSC.comuneRegistrazione;
                    if (comuneAtto is not null)
                    {
                        vm.NomeComuneAtto = comuneAtto.nomeComune ?? "";
                        vm.CodIstatComuneAtto = comuneAtto.codiceIstat ?? "";
                        vm.ProvinciaComuneAtto = comuneAtto.siglaProvinciaIstat ?? "";
                        vm.Localita = comuneAtto.descrizioneLocalita ?? "";
                    }
                }

            }



            return vm;
        }

        public async Task<RisultatoAccertamentoGenitoreViewModel> ServizioAccertamentoSingoloGenitoreAsync(TipologiaRicercaGenitore tipoRicercaGenitore,string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoGenitoreViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoGenitoreAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoMaternità/Paternità Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else risultato.Errore = "Deceduto";

                                }
                            }

                            if (soggettoCorrente.generalita is not null)
                            {

                                risultato.Generalita = GetAnprGeneralitaViewModel(soggettoCorrente.generalita);

                            }
                            else
                            {
                                //dati di generalità non presenti segnalo l'anomalia
                                risultato.Errore = "Dati generalità soggetto ricercato mancanti";
                            }

                            switch (tipoRicercaGenitore)
                            {
                                case TipologiaRicercaGenitore.Madre:
                                    if(soggettoCorrente.maternita is not null && soggettoCorrente.maternita.generalita is not null)
                                    {
                                        TipoGenitore? madre = soggettoCorrente.maternita;
                                        risultato.Genitore.Generalita = this.GetAnprGeneralitaViewModel(madre.generalita);
                                        if(madre.statoCivile is not null)
                                        {
                                            risultato.Genitore.StatoCivile = madre.statoCivile.statoCivile ?? "";
                                            risultato.Genitore.StatoCivileNonDisponibile = madre.statoCivile.statoCivileND ?? "";
                                        }

                                    } else
                                    {
                                        risultato.Errore += "Dati generalità della madre mancanti";
                                    }
                                    break;
                                case TipologiaRicercaGenitore.Padre:
                                    if (soggettoCorrente.paternita is not null && soggettoCorrente.paternita.generalita is not null)
                                    {
                                        TipoGenitore? padre = soggettoCorrente.paternita;
                                        risultato.Genitore.Generalita = this.GetAnprGeneralitaViewModel(padre.generalita);
                                        if (padre.statoCivile is not null)
                                        {
                                            risultato.Genitore.StatoCivile = padre.statoCivile.statoCivile ?? "";
                                            risultato.Genitore.StatoCivileNonDisponibile = padre.statoCivile.statoCivileND ?? "";
                                        }
                                    }
                                    else
                                    {
                                        risultato.Errore += "Dati generalità del padre mancanti";
                                    }
                                    break;
                            }


                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoMaternità/Paternità ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<List<RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel>> ServizioAccertamentoIdentificativoUnicoNazionaleAsync(string user, string codiciFiscali, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            List<RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel> risultati = new();
            List<string> elencoCodiciFiscali = codiciFiscali.Trim().Split("\r\n").ToList<string>();
            String? CF;
            Soglia soglia;
            int massimo;

            soglia = await _consumerServices.GetFinalityThresholdAsync(purpouseID);

            massimo = Math.Min(_numeroMassimoCodiciFiscali, soglia.SogliaChiamate - soglia.ChiamateEffettuate);

            if (elencoCodiciFiscali.Count <= massimo)
            {
                foreach (string codiceFiscale in elencoCodiciFiscali)
                {
                    CF = codiceFiscale.ToUpper().Trim();
                    if (!string.IsNullOrEmpty(CF))
                    {
                        Thread.Sleep(_delay);
                        var risultato = await this.ServizioAccertamentoSingoloIdentificativoUnicoNazionaleAsync(user, CF, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);
                        risultati.Add(risultato);

                    }
                }
            }
            else
            {
                //segnalo l'errore
                string msg = $"Il numero di chiamate per il servizio supera il limite di {massimo} chiamate richiedibili";
                _logger.LogWarning(msg);
                throw new ThresholdException(msg);
            }

            return risultati;
        }

        public async Task<RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel> ServizioAccertamentoSingoloIdentificativoUnicoNazionaleAsync(string user, string codiceFiscale, DateOnly DataRiferimentoVerifica, string codiceStato, string descrizioneStato, string userID, string userLocation, string LoA, string purpouseID, string? operationGuid)
        {
            RisultatoAccertamentoIdentificativoUnicoNazionaleViewModel risultato = new();
            risultato.Generalita.CodiceFiscale = codiceFiscale;
            string RERAnprID = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            ApiResult<RispostaE002OK> apiResult;
            RispostaE002OK? risposta;
            DateOnly dataDecesso;

            codiceFiscale = codiceFiscale.Trim().ToUpper();

            risultato.IdInterrogazioneRer = RERAnprID;

            apiResult = await _anprApi.APIServizioAccertamentoIdentificativoUnicoNazionaleAsync(user, RERAnprID, codiceFiscale, DataRiferimentoVerifica, codiceStato, descrizioneStato, userID, userLocation, LoA, purpouseID, operationGuid);

            if (apiResult.Status == ApiResultStatus.Success)
            {
                risposta = apiResult.Data;

                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto l'AccertamentoIdentificativoUnicoNazionale Anpr {IdOperazioneANPR}", userID, userLocation, risposta?.idOperazioneANPR ?? "");

                if (risposta is not null && risposta.listaSoggetti is not null && risposta.listaSoggetti.datiSoggetto is not null)
                {
                    var datisoggetto = risposta.listaSoggetti.datiSoggetto;
                    if (datisoggetto.Count() > 1)
                    {
                        //caso di omocodia, segnalo il problema
                        risultato.Errore = "Codice fiscale duplicato";
                    }
                    else
                    {
                        TipoDatiSoggettiEnte? soggettoCorrente = datisoggetto.FirstOrDefault();
                        if (soggettoCorrente is not null)
                        {
                            if (soggettoCorrente.datiDecesso is not null)
                            {

                                if (DateOnly.TryParse(soggettoCorrente.datiDecesso.dataEvento, CultureInfo.InvariantCulture, out dataDecesso))
                                {
                                    risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                }
                                else risultato.Errore = "Deceduto";
                            }
                            else if (soggettoCorrente.infoSoggettoEnte is not null)
                            {
                                var Decesso = soggettoCorrente.infoSoggettoEnte.Where(x => x?.chiave?.ToString() == "Data decesso").Select(x => x);
                                if (Decesso.Any())
                                {


                                    if (DateOnly.TryParse(Decesso.Single().valoreData, CultureInfo.InvariantCulture, out dataDecesso))
                                    {
                                        risultato.Errore = "Deceduto il " + dataDecesso.ToString("d");
                                    }
                                    else risultato.Errore = "Deceduto";

                                }
                            }


                            if (soggettoCorrente.identificativi is not null)
                            {
                                risultato.idANPR = soggettoCorrente.identificativi.idANPR ?? "";
                            }

                        }

                    }
                }

                risultato.IdInterrogazioneAnpr = risposta?.idOperazioneANPR ?? "";

            }
            else
            {
                using (LogContext.PushProperty("OperationGuid", operationGuid)) _logger.LogInformation("l'utente {UserId} dalla postazione {UserLocation} ha chiesto la AccertamentoIdentificativoUnicoNazionale ma si è verificato un errore", userID, userLocation);
                risultato.Errore = apiResult.Message is null ? "Accertamento non riuscito" : apiResult.Message;
                risultato.Generalita.CodiceFiscale = codiceFiscale;
            }


            return risultato;
        }

        public async Task<int> CreaRichistaMassiva(string user, int idFinalita, MemoryStream stream)
        {
            int idRichiestaMassiva;
            List<string> dati = new();


            
            ExcelPackage.License.SetNonCommercialOrganization("Regione Emilia-Romagna");
            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                var worksheet = excelPackage.Workbook.Worksheets[0];


                for (int i = worksheet.Dimension.Start.Row; i <= worksheet.Dimension.End.Row; i++)
                {
                    AnprElementoInputRichiestaMassiva input = new();
                    DateTime dataVerifica;
                    input.CodiceFiscale =  (worksheet.Cells[i, 1].Value.ToString() ?? "").ToUpper().Trim();
                    if (string.IsNullOrEmpty(input.CodiceFiscale)) throw new Exception("Codice fiscale non valido");
                    string data = worksheet.Cells[i, 2].Value.ToString() ?? "";
                    if (string.IsNullOrEmpty(data)) throw new Exception("Data verifica non valida");
                    if(DateTime.TryParse(data, CultureInfo.InvariantCulture, out dataVerifica))
                    {
                        input.DataVerifica = dataVerifica.ToString("yyyy-MM-dd");
                    } else new Exception("Data verifica non valida");

                    dati.Add(JsonSerializer.Serialize(input));

                }                               
            }
            
            idRichiestaMassiva = await _massiveRequestsServices.CreateMassiveRequestWithDataAsync(user, idFinalita, dati);

            return idRichiestaMassiva;
        }
    }
}
