using System.Collections.Generic;

namespace ScERPA.Services.Interfaces
{
    public interface IElementoSchedaReport
    {
        public Dictionary<string, string> GetData();

        public List<Dictionary<string, string>> GetRelatedData();

    }
}
