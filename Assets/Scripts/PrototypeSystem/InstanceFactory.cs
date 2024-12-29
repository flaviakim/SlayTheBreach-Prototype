using System.Collections.Generic;
using JetBrains.Annotations;

namespace PrototypeSystem {
    public abstract class InstanceFactory<TInstance, TPrototypeData, TFactory>
        where TInstance : IInstance<TPrototypeData>
        where TPrototypeData : PrototypeData
        where TFactory : InstanceFactory<TInstance, TPrototypeData, TFactory>, new() {


        // Don't set this, as there can be different arguments for different types of prototypes
        // public abstract TInstance CreateInstance(TPrototypeData prototype);

        protected abstract IPrototypeCollection<TPrototypeData> PrototypeCollection { get; }

        private static TFactory Singleton { get; } = new();

        [CanBeNull]
        protected static TPrototypeData TryGetPrototypeForName(string idName) {
            return Singleton.PrototypeCollection.TryGetPrototypeForName(idName);
        }

        public static List<TPrototypeData> GetPrototypes() {
            return Singleton.PrototypeCollection.GetPrototypes();
        }

        public static List<string> GetPrototypeNames() {
            return Singleton.PrototypeCollection.GetPrototypeNames();
        }

        public static void PreloadPrototypes() {
            Singleton.PrototypeCollection.PreloadPrototypes();
        }

    }
}