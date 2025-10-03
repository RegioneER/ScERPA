namespace ScERPA.Models.Reports
{
    public class SchedaReport
    {
        public Dictionary<string, string> Dati { get; set; } = new();

        public List<Dictionary<string, string>>? SottoSchede { get; set; } = new();

    }
}
