using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace ScERPA.Models.Dictionaries
{

	public enum enumStatoCivile {
		[Display(Name = "Celibe/Nubile")]
		Celibe_Nubile = 1,
		[Display(Name = "Coniugato/a")]
		Coniugato = 2,
		[Display(Name = "Vedovo/a")]
		Vedovo = 3,
		[Display(Name = "Divorziato/a")]
		Divorziato = 4,
		[Display(Name = "Non classificabile/ignoto/n.c")]
		Non_classificabile_ignoto_n_c = 5,
		[Display(Name = "Unito civilmente")]
		Unito_civilmente = 6,
		[Display(Name = "Stato libero a seguito di decesso della parte unita civilmente")]
		Stato_libero_a_seguito_di_decesso_della_parte_unita_civilmente = 7,
		[Display(Name = "Stato libero a seguito di scioglimento dell'unione")]
		Stato_libero_a_seguito_di_scioglimento_della_unione = 8
	}


	public class TipoTestataRichiestaE000
	{
		public string? idOperazioneClient { get; set; } = "1";//Guid.NewGuid().ToString(); NON PUO' essere un guid ma un alfanumerico si più corto di un guid
		public string? codMittente { get; set; } = "600010";
		public string? codDestinatario { get; set; } = "ANPR02";
		public string? operazioneRichiesta { get; set; } = "E002";
		public string? dataOraRichiesta { get; set; } = DateTime.Now.ToString(format: "yyyy-MM-ddThh:mm:ss");
		public string? tipoOperazione { get; set; } = "C";
		public string? protocolloClient { get; set; }
		public string? dataProtocolloClient { get; set; }
		public string? tipoInvio { get; set; } = "TEST";
		public string? dataDecorrenza { get; set; } = DateTime.Now.ToString(format: "yyyy-MM-dd");
		public string? dataDefinizionePratica { get; set; }
		public string? nomeApplicativo { get; set; } = "ScERPA";
		public string? versioneApplicativo { get; set; } = "Alfa";
		public string? fornitoreApplicativo { get; set; } = "Regione Emilia-Romagna";


	}

	public class TipoLuogoNascita3000
	{
		public string? luogoEccezionale { get; set; }
		public TipoComune? comune { get; set; }
		public TipoLocalita? localita { get; set; }
	}

	public class TipoDatiNascitaE000
	{
		public string? dataEvento { get; set; }
		public string? senzaGiorno { get; set; }
		public string? senzaGiornoMese { get; set; }

		public TipoLuogoNascita3000? luogoNascita { get; set; }

	}

	public class TipoCriteriRicercaE002
	{
		public string? codiceFiscale { get; set; }
		public string? cognome { get; set; }
		public string? senzaCognome { get; set; }
		public string? nome { get; set; }
		public string? senzaNome { get; set; }

		public string? sesso { get; set; }

		public TipoDatiNascitaE000? datiNascita { get; set; }

	}

	public class TipoDatiRichiestaE002
	{
		public string? schedaAnagraficaRichiesta { get; set; } = "1";
		public string? dataRiferimentoRichiesta { get; set; } = DateTime.Now.ToString(format: "yyyy-MM-dd");
		public string[]? datiAnagraficiRichiesti { get; set; } = { "1" };
		public string? motivoRichiesta { get; set; } = "1";
		public string casoUso { get; set; } = "";

	}

	public class TipoComuneRichiesta
	{
		public string? nomeComune { get; set; }
		public string? codiceIstat { get; set; }
		public string? siglaProvinciaIstat { get; set; }

	}
	public class TipoCartaIdentita3000
	{
		public TipoComuneRichiesta? comuneRilascio { get; set; }
		public string? codiceConsolatoRilascio { get; set; }
		public string? numero { get; set; }
	}

	public class TipoDatiSoggiorno3000
	{
		public string? questuraRilascioSoggiorno { get; set; }
		public string? numeroSoggiorno { get; set; }

	}


	public class TipoPermessoDiSoggiorno3000
	{
		public string? numeroPassaporto { get; set; }
		public TipoDatiSoggiorno3000? datiSoggiorno30XX { get; set; }
	}

	public class TipoVerificaMatrimonio
	{
		public TipoGeneralita? coniuge { get; set; }
		public TipoDatiEvento? datiMatrimonio { get; set; }
	}

	public class TipoLocalitaEstera
	{
		public TipoIndirizzoEstero? indirizzoEstero { get; set; }
		public TipoConsolato? consolato { get; set; }
	}


	public class TipoVerificaResidenza
	{
		public string? tipoIndirizzo { get; set; }
		public TipoIndirizzo? indirizzo { get; set; }
		public TipoLocalitaEstera? localitaEstera { get; set; }

	}

	public class TipoVerificaLegameSoggetto 
	{
        public string? tipoLegame { get; set; }
        public string? codiceLegame { get; set; }
    }


    public class TipoVerificaDatiSoggettiEnte
	{
        public TipoGeneralita? generalita { get; set; }
		public TipoVerificaLegameSoggetto? legameSoggetto { get; set; }
    }


    public class TipoListaVerificaSoggetti
	{
		public TipoVerificaDatiSoggettiEnte[]? datiSoggetto { get; set; }
    }

    public class TipoVerificaE002 
	{

        public TipoGeneralita? generalita { get; set; }
        public TipoCittadinanza? cittadinanza { get; set; }
        public TipoStatoCivile? statoCivile { get; set; }
        public TipoCartaIdentita3000? cartaIdentita { get; set; }
        public TipoPermessoDiSoggiorno3000? permessoDiSoggiorno { get; set; }
        public TipoDatiEvento? datiDecesso { get; set; }
        public TipoVerificaMatrimonio? matrimonio { get; set; }
        public TipoVerificaResidenza? residenza { get; set; }
        public TipoVedovanza? vedovanza { get; set; }
        public TipoGenitore? paternita { get; set; }
        public TipoGenitore? maternita { get; set; }
        public TipoConvivenzaDiFatto? ConvivenzaDiFatto { get; set; }
        public TipoUnioneCivile? unioneCivile { get; set; }
        public TipoListaVerificaSoggetti? listaSoggetti { get; set; }
	}

	public class RichiestaE002
	{
		//public TipoTestataRichiestaE000? testataRichiesta { get; set; } = new TipoTestataRichiestaE000();
		public string? idOperazioneClient { get; set; } = "";

        public TipoCriteriRicercaE002 criteriRicerca { get; set; } = new TipoCriteriRicercaE002();

		public TipoVerificaE002? verifica { get; set; }

		public TipoDatiRichiestaE002? datiRichiesta { get; set; } = new TipoDatiRichiestaE002();

	}

	public class TipoTestataRispostaE000
	{
		public string? idOperazioneClient { get; set; }
		public string? codMittente { get; set; }
		public string? codDestinatario { get; set; }
		public string? operazioneRichiesta { get; set; }
		public string? dataOraRichiesta { get; set; }
		public string? protocolloClient { get; set; }
		public string? dataProtocolloClient { get; set; }
		public string? idOperazioneANPR { get; set; }
		public string? esitoOperazione { get; set; }

	}

	public class TipoCodiceFiscale
	{
		public string? codFiscale { get; set; }
		public string? validitaCF { get; set; }
		public string? dataAttribuzioneValidita { get; set; }//format: YYYY-MM-DD example: '2021-11-15'

	}


	public class TipoComune
	{
		public string? nomeComune { get; set; }
		public string? codiceIstat { get; set; }
		public string? siglaProvinciaIstat { get; set; }
		public string? descrizioneLocalita { get; set; }
	}

	public class TipoLocalita
	{
		public string? descrizioneLocalita { get; set; }
		public string? descrizioneStato { get; set; }
		public string? codiceStato { get; set; }
		public string? provinciaContea { get; set; }
	}

	public class TipoLuogoEvento
	{
		public string? luogoEccezionale { get; set; }
		public TipoComune? comune { get; set; }
		public TipoLocalita? localita { get; set; }
	}

	public class TipoIdSchedaSoggettoComune
	{
		public string? idSchedaSoggettoComuneIstat { get; set; }
		public string? idSchedaSoggetto { get; set; }

	}

	public class TipoGeneralita
	{
		public TipoCodiceFiscale? codiceFiscale { get; set; }
		public string? cognome { get; set; }
		public string? senzaCognome { get; set; }
		public string? nome { get; set; }
		public string? senzaNome { get; set; }
		public string? sesso { get; set; }
		public string? dataNascita { get; set; } //format: YYYY-MM-DD example: '2021-11-15'
		public string? senzaGiorno { get; set; }
		public string? senzaGiornoMese { get; set; }
		public TipoLuogoEvento? luogoNascita { get; set; }
		public string? soggettoAIRE { get; set; }
		public string? annoEspatrio { get; set; }
		public TipoIdSchedaSoggettoComune? idSchedaSoggettoComune { get; set; }
		public string? idSchedaSoggettoANPR { get; set; }
		public string? note { get; set; }
	}

	public class TipoAttributiSoggetto
	{
		public string? senzaFissaDimora { get; set; }
		public string? soggettoCertificabile { get; set; }
	}

	public class TipoAtto
	{
		public TipoComune? comuneRegistrazione { get; set; }
		public string? ufficioMunicipio { get; set; }
		public string? anno { get; set; }
		public string? parte { get; set; }
		public string? serie { get; set; }
		public string? numeroAtto { get; set; }
		public string? volume { get; set; }
		public string? dataFormazioneAtto { get; set; }
		public string? trascritto { get; set; }
	}

	public class TipoCittadinanza
	{
		public string? descrizioneStato { get; set; }
		public string? codiceStato { get; set; }
		public string? noteStato { get; set; }
		public string? dataValidita { get; set; }
	}

	public class TipoStatoCivile
	{
		public string? statoCivile { get; set; }
		public string? noteStatoCivile { get; set; }
		public string? statoCivileND { get; set; }
	}

	public class TipoGenitore
	{

		public TipoGeneralita? generalita { get; set; }

		public TipoStatoCivile? statoCivile { get; set; }

		public TipoCittadinanza? cittadinanza { get; set; }

		public TipoAltriDati? altriDati { get; set; }

	}

	public class TipoToponimo
	{
		public string? codSpecie { get; set; }
		public string? specie { get; set; }
		public string? specieFonte { get; set; }
		public string? codToponimo { get; set; }
		public string? denominazioneToponimo { get; set; }
		public string? toponimoFonte { get; set; }
	}

	public class TipoCivicoInterno
	{
		public string? corte { get; set; }

		public string? scala { get; set; }


		public string? interno1 { get; set; }


		public string? espInterno1 { get; set; }


		public string? interno2 { get; set; }


		public string? espInterno2 { get; set; }


		public string? scalaEsterna { get; set; }


		public string? secondario { get; set; }


		public string? piano { get; set; }


		public string? nui { get; set; }


		public string? isolato { get; set; }


	}

	public class TipoNumeroCivico
	{
		public string? codiceCivico { get; set; }

		public string? civicoFonte { get; set; }


		public string? numero { get; set; }


		public string? metrico { get; set; }


		public string? progSNC { get; set; }


		public string? lettera { get; set; }


		public string? esponente1 { get; set; }


		public string? colore { get; set; }


		public TipoCivicoInterno? civicoInterno { get; set; }



	}

	public class TipoIndirizzo
	{
		public string? cap { get; set; }

		public TipoComune? comune { get; set; }

		public string? frazione { get; set; }

		public TipoToponimo? toponimo { get; set; }

		public TipoNumeroCivico? numeroCivico { get; set; }

	}

	public class TipoDatoLocalitaEstera
	{
		public string? descrizioneLocalita { get; set; }
		public string? descrizioneStato { get; set; }
		public string? codiceStato { get; set; }
		public string? provinciaContea { get; set; }

	}

	public class TipoToponimoEstero
	{
		public string? denominazione { get; set; }
		public string? numeroCivico { get; set; }
	}

	public class TipoIndirizzoEstero
	{

		public string? cap { get; set; }

		public TipoDatoLocalitaEstera? localita { get; set; }

		public TipoToponimoEstero? toponimo { get; set; }

	}

	public class TipoConsolato
	{
		public string? codiceConsolato { get; set; }
		public string? descrizioneConsolato { get; set; }
	}

	public class TipoLocalitaEstera1
	{
		public TipoIndirizzoEstero? indirizzoEstero { get; set; }
		public TipoConsolato? consolato { get; set; }

	}
	public class TipoResidenza
	{

		public string? tipoIndirizzo { get; set; }
		public string? noteIndirizzo { get; set; }
		public TipoIndirizzo?		 indirizzo { get; set; }
		public TipoLocalitaEstera1? localitaEstera { get; set; }
		public string? presso { get; set; }
		public string? dataDecorrenzaResidenza { get; set; }
	}

	public class TipoAltraLingua
	{
		public string? idLingua { get; set; }
		public string? noteIndirizzo { get; set; }
		public string? descrizioneLocalita { get; set; }
		public string? frazione { get; set; }
		public string? specie { get; set; }
		public string? denominazioneToponimo { get; set; }
		public string? presso { get; set; }
	}

	public class TipoDatiTranslitterati
	{
		public string? cognomeTranslitterato { get; set; }
		public string? nomeTranslitterato { get; set; }
		public string? specieTranslitterata { get; set; }
		public string? denominazioneToponimoTranslitterato { get; set; }
		public string? descrizioneLocalitaTranslitterata { get; set; }
		public string? denominazioneComuneTranslitterato { get; set; }
		public string? descrizioneStatoResidenzaTranslitterato { get; set; }
		public string? denominazioneLocEsteraTranslitterata { get; set; }
		public string? descrizioneConsolatoTranslitterato { get; set; }
		public string? descrizioneStatoNascitaTranslitterato { get; set; }
		public string? descrizioneStatoCittadinanzaTranslitterato { get; set; }
	}

	public class TipoTutoreIntestatario
	{
		public TipoGeneralita? generalita { get; set; }

		public TipoComune? comuneResidenza { get; set; }

		public string? dataIntestarioConvivenza { get; set; }

	}

	public class TipoMatrimonio
	{
		public TipoGeneralita? coniuge { get; set; }
		public TipoCittadinanza? cittadinanza { get; set; }
		public TipoDatiEvento? datiMatrimonio { get; set; }
		public string? ordineMatrimonio { get; set; }
	}

	public class TipoVedovanza
	{
		public TipoDatiEvento? datiMorteconiuge { get; set; }

		public string? ordineMatrimonioPrecedente { get; set; }

	}
	public class TipoSentenza
	{
		public string? data { get; set; }
		public string? numero { get; set; }
		public string? tipoTribunale { get; set; }
		public string? autorita { get; set; }
		public string? dataValidita { get; set; }

	}
	public class TipoAnnullamentoMatrimonio
	{
		public string? tipoCessazione { get; set; }

		public TipoSentenza? sentenza { get; set; }

		public TipoAtto? attoAnnullamentoMatrimonio { get; set; }

	}

	public class TipoDocumentoProtocollato
	{
		public string? data { get; set; }
		public string? dataNotifica { get; set; }
		public string? dataDocumento { get; set; }
		public string? protocollo { get; set; }
		public string? documento { get; set; }
		public string? estensione { get; set; }
		public TipoLuogoEvento? luogoEvento { get; set; }
		public string? professionista { get; set; }
		public TipoComune? comuneRegistrazione { get; set; }
		public string? stato { get; set; }

	}

	public class TipoRisoluzioneConvivenza
	{
		public int motivoRisoluzione { get; set; }
		public TipoDocumentoProtocollato? contratto { get; set; }
	}
	public class TipoConvivenzaDiFatto
	{
		public TipoGeneralita? convivente { get; set; }

		public TipoCittadinanza? cittadinanza { get; set; }

		public TipoDocumentoProtocollato? contrattoStipula { get; set; }

		public TipoRisoluzioneConvivenza? contrattoRisoluzione { get; set; }

		public string? ordineConvivenza { get; set; }

	}

	public class TipoScioglimentoUnione
	{
		public string? motivoScioglimento { get; set; }

		public TipoSentenza? sentenza { get; set; }

		public TipoDatiEvento? datiEvento { get; set; }

	}

	public class TipoUnioneCivile
	{
		public TipoGeneralita? unitoCivilmente { get; set; }

		public TipoDatiEvento? attoUnione { get; set; }

		public TipoScioglimentoUnione? scioglimentoUnione { get; set; }

		public TipoCittadinanza? cittadinanza { get; set; }

		public string? ordineUnione { get; set; }

	}

	public class TipoCartaIdentita
	{
		public string? numero { get; set; }
		public string? dataRilascio { get; set; }
		public string? cartaceaElettronica { get; set; }
		public string? interdizioneEspatrio { get; set; }
		public TipoComune? comuneRilascio { get; set; }
		public TipoConsolato? consolatoRilascio { get; set; }
		public string? dataScadenza { get; set; }
		public string? dataAnnullamento { get; set; }
	}

	public class TipoPermessoSoggiorno
	{
		public string? numeroSoggiorno { get; set; }
		public string? tipoSoggiorno { get; set; }
		public string? noteSoggiorno { get; set; }
		public string? dataRilascio { get; set; }
		public string? dataScadenza { get; set; }
		public string? questuraRilascioSoggiorno { get; set; }
		public TipoComune? comune { get; set; }
		public string? numeroPassaporto { get; set; }

	}

	public class TipoListaElettorale
	{
		public string? elettore { get; set; }

		public TipoComune? comune { get; set; }

	}

	public class TipoListaLeva
	{
		public string? iscritto { get; set; }

		public TipoComune? comune { get; set; }

	}

	public class TipoCensimento
	{
		public string? annoCensimento { get; set; }
		public string? sezioneCensimento { get; set; }
		public string? foglioCensimento { get; set; }
		public string? dataRegolarizzazione { get; set; }
		public string? motivoCompilazione { get; set; }

	}

	public class TipoLegameSoggettoCompleto
	{
		public string? tipoLegame { get; set; }

		public string? dataDecorrenza { get; set; }

		public string? codiceLegame { get; set; }

		public string? progressivoComponente { get; set; }

		public string? dataDecorrenzaLegame { get; set; }

	}
	public class TipoIdFamigliaConvivenzaComune
	{
		public string? idFamigliaConvivenzaComuneIstat { get; set; }
		public string? idFamigliaConvivenza { get; set; }
	}
	public class TipoFamigliaConvivenzaRisposta
	{
		public TipoIdFamigliaConvivenzaComune? idFamigliaConvivenzaComune { get; set; }

		public string? idFamigliaConvivenzaANPR { get; set; }

		public string? famigliaAire { get; set; }

		public string? dataOrigineFamigliaConvivenza { get; set; }

		public string? motivoCostituzione { get; set; }

		public string? denominazioneConvivenza { get; set; }

		public string? specieConvivenza { get; set; }

		public string? tipoMovimentazione { get; set; }

		public string? presenzaFamigliaCoabitante { get; set; }

		public string? tipoScheda { get; set; }

	}

	public class TipoAltriDati
	{
		public string? posizioneProfessionale { get; set; }

		public string? condizioneNonProfessionale { get; set; }

		public string? titoloStudio { get; set; }

	}

	public class TipoAutoveicoli
	{
		public string? possessoAutoveicoli { get; set; }

		public string? possessoPatente { get; set; }
	}

	public class TipoDomicilioDigitale
	{
		public string? indirizzoDigitale { get; set; }
		public string? dataInizioValidita { get; set; }
		public string? enteErogatore { get; set; }
	}

	public class TipoDatiIscrizioneComune
	{
		public string? dataIscrizione { get; set; }

		public string? protocolloComune { get; set; }

		public TipoComune? comune { get; set; }

		public string? idOperazioneComune { get; set; }

		public string? idOperazioneANPR { get; set; }

		public string? dataDefinizionePratica { get; set; }

	}
	public class TipoIscrizioneResidente
	{
		public string? motivoIscrizione { get; set; }
		public string? descrizioneMotivoIscrizione { get; set; }
	}
	public class TipoIscrizioneAIRE
	{
		public string? motivoIscrizione { get; set; }
		public string? noteMotivoIscrizione { get; set; }
		public string? iniziativaIscrizione { get; set; }
		public string? noteIniziativaIscrizione { get; set; }
		public string? individuazioneComuneIscrizione { get; set; }
		public string? noteIndividuazioneComuneIscrizione { get; set; }
		public string? dataArrivoConsolato { get; set; }

	}
	public class TipoDatiIscrizione
	{
		public TipoDatiIscrizioneComune? datiIscrizioneComune { get; set; }

		public TipoIscrizioneResidente? iscrizioneResidente { get; set; }

		public TipoIscrizioneAIRE? tipoIscrizioneAIRE { get; set; }

		public TipoSentenza? sentenza { get; set; }

	}

	public class TipoDatiCancellazione
	{
		public string? motivoCancellazione { get; set; }
		public string? notecancellazione { get; set; }
		public string? dataDecorrenzaCancellazione { get; set; }
		public TipoSentenza? datiSentenza { get; set; }

	}

	public class TipoAttoANSC
	{
		public string? idANSC { get; set; }
		public TipoComune? comuneRegistrazione { get; set; }

		public string? anno { get; set; }

        public string? ufficioMunicipio { get; set; }

        public string? numeroComunale { get; set; }

        public string? dataFormazioneAtto { get; set; }

        public string? trascritto { get; set; }

    }

    public class TipoAttoANPR
	{
		public TipoAtto? atto { get; set;}
		public TipoAttoANSC? attoANSC { get; set; }
    }

	public class TipoDatiEvento
	{
		public string? dataEvento { get; set; }

		public string? senzaGiorno { get; set; }

		public string? senzaGiornoMese { get; set; }

		public TipoLuogoEvento? luogoEvento { get; set; }

		public TipoAtto? attoEvento { get; set; } //versione 1

        public TipoAttoANPR? attoEventoANPR { get; set; } //versione 2

    }

	public class TipoProvenienzaDestinazione
	{
		public TipoComune? comune { get; set; }

		public TipoLocalita? localita { get; set; }

	}
	/*
	public class TipoInfoValore
	{
		public string enumerazione { get; set; }

	}
	*/

	public class TipoInfoSoggettoEnte
	{
		public string? id { get; set; }
		public string? chiave { get; set; }
		public string? valore { get; set; }
		public string? valoreTesto { get; set; }
		public string? valoreData { get; set; }
		public string? dettaglio { get; set; }
	}


	public class TipoInfoDatoCorretto
	{
		public string? percorso { get; set; }

		public string? campo { get; set; }

		public string? valoreCorretto { get; set; }

		public string? valoreDataCorretto { get; set; }

		public string? dettaglio { get; set; }

	}

	public class TipoIdentificativi
	{
		public string? idANPR { get; set; }
	}

	public class TipoInfoStoriaIscrizione
	{
		public string? tipoScheda { get; set; }

		public string? dataIscrizione { get; set; }

		public string? dataCancellazione { get; set; }

		public TipoComune? comuneCompetente { get; set; }

	}

	public class TipoDatiSoggettiEnte
	{
		public TipoGeneralita? generalita { get; set; }

		public TipoStatoCivile? statoCivile { get; set; }

		public TipoAttributiSoggetto? attributiSoggetto { get; set; }

		public TipoAtto? attoNascita { get; set; }

		public TipoCittadinanza[]? cittadinanza { get; set; }

		public TipoGenitore? paternita { get; set; }

		public TipoGenitore? maternita { get; set; }

		public TipoResidenza[]? residenza { get; set; }

		public TipoAltraLingua[]? altraLingua { get; set; }

		public TipoDatiTranslitterati? datiTraslitterati { get; set; }

		public TipoTutoreIntestatario? tutoreIntestatario { get; set; }

		public TipoMatrimonio? matrimonio { get; set; }

		public TipoVedovanza? vedovanza { get; set; }

		public TipoAnnullamentoMatrimonio? annullamentoMatrimonio { get; set; }

		public TipoConvivenzaDiFatto? convivenzaDiFatto { get; set; }

		public TipoUnioneCivile? unioneCivile { get; set; }

		public TipoCartaIdentita? cartaIdentita { get; set; }

		public TipoPermessoSoggiorno? permessoSoggiorno { get; set; }

		public TipoListaElettorale? listaElettorale { get; set; }

		public TipoListaLeva? listaLeva { get; set; }

		public TipoCensimento? censimento { get; set; }

		public TipoLegameSoggettoCompleto? legameSoggetto { get; set; }

		public TipoFamigliaConvivenzaRisposta? famigliaConvivenza { get; set; }

		public TipoAltriDati? altriDati { get; set; }

		public TipoAutoveicoli? autoveicoli { get; set; }

		public TipoDomicilioDigitale? domicilioDigitale { get; set; }

		public TipoDatiIscrizione? datiIscrizione { get; set; }

		public TipoDatiCancellazione? datiCancellazione { get; set; }

		public TipoDatiEvento? datiDecesso { get; set; }

		public TipoProvenienzaDestinazione? provenienza { get; set; }

		public TipoProvenienzaDestinazione? destinazione { get; set; }

		public TipoErroriAnomalia[]? erroriAnomalie { get; set; }

		public TipoInfoSoggettoEnte[]? infoSoggettoEnte { get; set; }

		public TipoInfoDatoCorretto[]? infoDatoCorretto { get; set; }

		public string? idSoggettoNazionale { get; set; }

		public TipoIdentificativi? identificativi { get; set; }

		public TipoInfoStoriaIscrizione[]? infoStoriaIscrizione { get; set; }

	}

	public class TipoListaSoggetti
	{
		public TipoDatiSoggettiEnte[]? datiSoggetto { get; set; }
	}

	public class TipoDatiFamiglia
	{
		public TipoResidenza? residenza { get; set; }

		public TipoFamigliaConvivenzaRisposta? famigliaConvivenza { get; set; }

	}
	public class TipoErroriAnomalia
	{
		public string? codiceErroreAnomalia { get; set; }
		public string? tipoErroreAnomalia { get; set; }
		public string? testoErroreAnomalia { get; set; }
		public string? oggettoErroreAnomalia { get; set; }
		public string? campoErroreAnomalia { get; set; }
		public string? valoreErroreAnomalia { get; set; }

	}

	public class RispostaE002OK
	{
		//public TipoTestataRispostaE000? testataRisposta { get; set; }
		public string? idOperazioneANPR { get; set; }

        public TipoListaSoggetti? listaSoggetti { get; set; }
		public TipoDatiFamiglia? datiFamigliaConvivenza { get; set; }
		public TipoErroriAnomalia[]? listaAnomalie { get; set; }
		public TipoErroriAnomalia[]? listaErrori { get; set; }
	}
	public class RispostaE002KO
	{
        //public TipoTestataRispostaE000? testataRisposta { get; set; }
        public string? idOperazioneANPR { get; set; }
        public TipoErroriAnomalia[]? listaErrori { get; set; }
	}



}
