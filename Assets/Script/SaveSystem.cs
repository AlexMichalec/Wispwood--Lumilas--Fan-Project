using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;



public static class SaveSystem
{
    public static void SaveHighscore(int score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/hiscore.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, score);
        stream.Close();
    }

    public static int LoadHighscore()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/hiscore.bin";
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            int hiScore = (int)formatter.Deserialize(stream);
            stream.Close();

            return hiScore;
        }
        else
        {
            return 0;
        }
        
    }

    public static void SaveCustomSet(List<int> customSet)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/customSet.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, customSet);
        stream.Close();
    }

    public static List<int> LoadCustomSet()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/customSet.bin";
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            List<int> customSet = formatter.Deserialize(stream) as List<int>;
            stream.Close();

            return customSet;
        }
        else
        {
            return new List<int> {0,0,0,0,0};
        }





    }
}
