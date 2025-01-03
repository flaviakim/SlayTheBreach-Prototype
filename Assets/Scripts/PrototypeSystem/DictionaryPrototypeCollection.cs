using System.Collections.Generic;

namespace PrototypeSystem {
    public abstract class DictionaryPrototypeCollection<TPrototypeData> : IPrototypeCollection<TPrototypeData> where TPrototypeData : PrototypeData {
        private Dictionary<string, TPrototypeData> _prototypes;
        private Dictionary<string, TPrototypeData> Prototypes => _prototypes ??= LoadPrototypeDatas();

        public TPrototypeData TryGetPrototypeForName(string name) {
            Prototypes.TryGetValue(name, out var prototype);
            return prototype;
        }

        public List<string> GetPrototypeNames() {
            return new List<string>(Prototypes.Keys);
        }

        public List<TPrototypeData> GetPrototypes() {
            return new List<TPrototypeData>(Prototypes.Values);
        }

        public void PreloadPrototypes() {
            _prototypes ??= LoadPrototypeDatas();
        }

        protected abstract Dictionary<string, TPrototypeData> LoadPrototypeDatas();
    }
}