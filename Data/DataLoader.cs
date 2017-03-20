using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Heroes3.Data
{
    public static class DataLoader
    {
        public static IEnumerable<Faction> GetFactions()
        {
            foreach (var faction in Directory.EnumerateFiles("Content/Factions"))
                yield return JsonConvert.DeserializeObject<Faction>(File.ReadAllText(faction));
        }
    }
}
