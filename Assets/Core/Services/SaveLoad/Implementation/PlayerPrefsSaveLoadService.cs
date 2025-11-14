using Core.Services.SaveLoad.Interfaces;
using UnityEngine;

namespace Core.Services.SaveLoad.Implementation
{
    public class PlayerPrefsSaveLoadService : ISaveLoadService
    {
        public void Save<TData>(TData data) where TData : class
        {
            if (data == null)
            {
                Debug.LogError("Object to save is null.");
                return;
            }

            var key = data.GetType().ToString();
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public TData Load<TData>() where TData : class
        {
            var key = typeof(TData).ToString();

            if (!PlayerPrefs.HasKey(key))
            {
                Debug.LogError($"No data found for key: {key}");
                return null;
            }

            var json = PlayerPrefs.GetString(key);
            var data = JsonUtility.FromJson<TData>(json);
            return data;
        }
    }
}