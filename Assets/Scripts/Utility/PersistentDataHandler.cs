using System;
using System.IO;
using UnityEngine;

namespace Utility
{
    public class PersistentDataHandler : MonoBehaviour
    {
        public static void SaveObject(string fileName, object obj)
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            
            string dataToWrite = JsonUtility.ToJson(obj);
            
            // save that to a file
            using FileStream stream = new FileStream(path, FileMode.Create);
            using StreamWriter writer = new StreamWriter(stream);
            writer.Write(dataToWrite);
        }

        public static T LoadObjectOrNew<T>(string fileName) where T:new()
        {
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(path))
                return new T();

            string data;
            
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    data = reader.ReadToEnd();
                }
            }

            return JsonUtility.FromJson<T>(data) ?? new T();
        }
    }
}