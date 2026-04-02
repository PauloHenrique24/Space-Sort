using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private static string Pasta => Application.persistentDataPath + "/saves/";

    [Serializable]
    private class SaveWrapper<T>
    {
        public T data;
        public SaveWrapper(T d) => data = d;
    }

    // SALVAR QUALQUER COISA
    public static void Save<T>(string key, T data)
    {
        if (!Directory.Exists(Pasta))
            Directory.CreateDirectory(Pasta);

        string path = Pasta + key + ".json";

        SaveWrapper<T> wrapper = new(data);
        string json = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(path, json);
    }

    // CARREGAR
    public static T Load<T>(string key, T defaultValue = default)
    {
        string path = Pasta + key + ".json";

        if (!File.Exists(path))
        {
            Debug.Log("Save não existe: " + key);
            return defaultValue;
        }

        string json = File.ReadAllText(path);
        SaveWrapper<T> wrapper = JsonUtility.FromJson<SaveWrapper<T>>(json);

        return wrapper.data;
    }

    // DELETAR
    public static void Delete(string key)
    {
        string path = Pasta + key + ".json";
        if (File.Exists(path))
            File.Delete(path);
    }

    public static bool Exists(string key)
    {
        return File.Exists(Pasta + key + ".json");
    }
}
