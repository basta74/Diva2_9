namespace Diva2.Services
{
    public interface ICacheHelper
    {
        void ClearData(string key);
        void ClearDataSub(string key);
        T GetData<T>(string key);
        T GetDataSub<T>(string key);
        void SetData<T>(string key, T data, int minutes = 10);
        void SetDataSub<T>(string key, T data, int minutes = 10);
    }
}