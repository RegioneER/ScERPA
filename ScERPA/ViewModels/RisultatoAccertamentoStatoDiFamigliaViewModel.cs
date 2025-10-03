using ScERPA.Models.Dictionaries;
using ScERPA.Models.Enums;
using ScERPA.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Security.Principal;

namespace ScERPA.ViewModels
{
    public class RisultatoAccertamentoStatoDiFamigliaViewModel : IElementoSchedaReport
    {


        public string CodiceFiscale { get; set; } = "";
        public string IdInterrogazioneAnpr { get; set; } = "";
        public string IdInterrogazioneRer { get; set; } = "";
        public string Errore { get; set; } = "";

        public List<ComponenteStatoDiFamigliaViewModel> Componenti { get; set; } = new();
        

        public Dictionary<string, string> GetData()
        {

            Dictionary<string, string> dati = new();
            dati.Add("Codice Fiscale Controllato", this.CodiceFiscale);
            dati.Add("Id Interrogazione Anpr", this.IdInterrogazioneAnpr);
            dati.Add("Id Interrogazione Rer", this.IdInterrogazioneRer);
            dati.Add("Nota", this.Errore);

            return dati;
        }

        public List<Dictionary<string, string>> GetRelatedData()
        {
            List<Dictionary<string, string>> DatiRelazionati = new();

            foreach(var elemento in this.Componenti)
            {
                Dictionary<string, string> dato = new();
                dato.Add("Codice Fiscale Controllato", CodiceFiscale);
                dato.Add("Codice Fiscale componente", elemento.Generalita.CodiceFiscale);
                dato.Add("Codice Fiscale verificato", elemento.Generalita.CodiceFiscaleVerificato);
                dato.Add("Tipo parentela/convivenza", elemento.DescrizioneParentelaConvivenza);
                dato.Add("Data relazione parentela", elemento.DataDecorrenzaRelazioneParentela);
                dato.Add("Cognome", elemento.Generalita.Cognome);
                dato.Add("Nome", elemento.Generalita.Nome);
                dato.Add("Data di nascita", elemento.Generalita.DataNascita);
                dato.Add("Sesso", elemento.Generalita.Sesso);
                dato.Add("Soggetto AIRE", elemento.Generalita.SoggettoAIRE);
                dato.Add("Anno espatrio", elemento.Generalita.AnnoEspatrio);
                dato.Add("Nazione", elemento.Generalita.Nazione);
                dato.Add("Codice Nazione", elemento.Generalita.CodNazione);
                dato.Add("Provincia", elemento.Generalita.Provincia);
                dato.Add("Localita", elemento.Generalita.Localita);
                dato.Add("Codice Istat Comune", elemento.Generalita.CodIstatComune);
                dato.Add("Luogo eccezionale", elemento.Generalita.LuogoEccezionale);
                dato.Add("Senza cognome", elemento.Generalita.SenzaCognome);
                dato.Add("Senza nome", elemento.Generalita.SenzaNome);
                dato.Add("Senza giorno nascita", elemento.Generalita.SenzaGiornoNascita);
                dato.Add("Senza giorno e mese nascita", elemento.Generalita.SenzaGiornoMeseNascita);
                dato.Add("idANPR", elemento.idANPR);

                DatiRelazionati.Add(dato);

            }

            return DatiRelazionati;
        }
    }
}
