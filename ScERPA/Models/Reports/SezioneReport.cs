using Org.BouncyCastle.Bcpg;

namespace ScERPA.Models.Reports
{
    public class SezioneReport
    {

        public SezioneReport(string nomeBreve,string titolo, string sottotitolo)
        {
            NomeBreve = nomeBreve;
            Titolo=titolo;
            Sottotitolo=sottotitolo;
        }
        public string NomeBreve { get; set; } = "";
        public string Titolo { get; set; } = "";
        public string Sottotitolo { get; set; } = "";
        public float[]? Tabella { get; set; } = Array.Empty<float>();
        public List<SchedaReport> Schede { get; set; } = new();


    }
}
