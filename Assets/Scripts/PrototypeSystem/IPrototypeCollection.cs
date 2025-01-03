using System.Collections.Generic;
using JetBrains.Annotations;

namespace PrototypeSystem {
    public interface IPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {
        [CanBeNull] public TPrototypeData TryGetPrototypeForName(string idName);
        public List<string> GetPrototypeNames();
        public List<TPrototypeData> GetPrototypes();
        public void PreloadPrototypes();
    }
}