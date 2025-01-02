using System.Collections.Generic;
using JetBrains.Annotations;

namespace PrototypeSystem {
    public abstract class InstanceFactory<TInstance, TPrototypeData, TFactory>
        where TInstance : IInstance
        where TPrototypeData : PrototypeData
        where TFactory : InstanceFactory<TInstance, TPrototypeData, TFactory>, new() {


        // Don't set this, as there can be different arguments for different types of prototypes
        // public abstract TInstance CreateInstance(TPrototypeData prototype);

        protected abstract IPrototypeCollection<TPrototypeData> PrototypeCollection { get; }

        [CanBeNull]
        protected TPrototypeData TryGetPrototypeForName(string idName) {
            return PrototypeCollection.TryGetPrototypeForName(idName);
        }

        public List<TPrototypeData> GetPrototypes() {
            return PrototypeCollection.GetPrototypes();
        }

        public List<string> GetPrototypeNames() {
            return PrototypeCollection.GetPrototypeNames();
        }

        public void PreloadPrototypes() {
            PrototypeCollection.PreloadPrototypes();
        }

    }
}