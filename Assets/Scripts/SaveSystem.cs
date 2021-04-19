using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string PlayerFilePath = Application.persistentDataPath + "player." + Application.productName;
    
    public static void SavePlayer()
    {
        var stream = new FileStream(PlayerFilePath, FileMode.Create);
        var binaryFormatter = new BinaryFormatter();
        var saveData = new PlayerInformationSaveData();
        
        binaryFormatter.Serialize(stream, saveData);
        
        stream.Close();
    }

    public static PlayerInformationSaveData LoadPlayer()
    {
        var path = PlayerFilePath;
        
        if (!File.Exists(path)) return null;
        
        var stream = new FileStream(path, FileMode.Open);
        
        if (stream.Length == 0) return null;
        
        var binaryFormatter = new BinaryFormatter();
        var returnVariable = binaryFormatter.Deserialize(stream) as PlayerInformationSaveData;
        
        stream.Close();
        
        return returnVariable;
    }
}
