namespace Core.Services.SaveLoad.Interfaces
{
    public interface ISaveLoadService
    {
        public void Save<TData>(TData data) where TData : class;
        public TData Load<TData>() where TData : class;
    }
}