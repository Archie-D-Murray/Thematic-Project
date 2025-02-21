namespace Data {
    public interface ISerialize {
        void OnSave(ref LevelData data);
        void OnLoad(LevelData data);
    }
}