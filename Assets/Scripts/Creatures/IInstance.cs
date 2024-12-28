public interface IInstance<out TPrototypeData> where TPrototypeData : PrototypeData {
    public TPrototypeData PrototypeData { get; }
    public string IDName => PrototypeData.IDName;
}