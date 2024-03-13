using System;
using System.IO;
using UnityEngine;

namespace Data
{
    public class FileDataHandler
    {
        private string dataFilePath;
        private string dataFileName;

        public FileDataHandler(string dataFilePath, string dataFileName)
        {
            this.dataFilePath = dataFilePath;
            this.dataFileName = dataFileName;
        }

        public void Save(GameData gameData)
        {
            string fullPath = Path.Combine(dataFilePath, dataFileName);
            //Debug.Log(fullPath);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string storingData = JsonUtility.ToJson(gameData, true);

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(storingData);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Something wrong when save data on " + fullPath + "\n" + e);
            }
        }

        public T Load<T>() where T : GameData
        {
            string fullPath = Path.Combine(dataFilePath, dataFileName);
            T loaded = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string loadingData = "";

                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            loadingData = reader.ReadToEnd();
                        }
                    }

                    loaded = JsonUtility.FromJson<T>(loadingData);
                }
                catch (Exception e)
                {
                    Debug.LogError("Something wrong when load data on " + fullPath + "\n" + e);
                }
            }

            return loaded;
        }
    }
}