namespace Core.Services.UI.Interfaces
{
    public interface IInitializableUIUnit<in TData> 
        where TData : struct
    {
        void Init(TData data);
    }
}