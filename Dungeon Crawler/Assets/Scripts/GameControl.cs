using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//These next three are for saving data, but it is not encrypted
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {

    public static GameControl control;

    public int highestLevelCompleted;
    public bool[] achievements; // Make separate method to extract achievements, achievements are broken into 3 segment blocks.

    //This is the name of the save data
    //.dat file extension is totally arbitrary, could be .banana
    private string fileName = "DungeonGameSave.dat";

    void Awake() {
        if (control == null) {
            DontDestroyOnLoad(gameObject);
            control = this;
        } else if (control != this) {
            Destroy(gameObject);

        }
    }

    //Put extra methods here for displaying/using data

    //Need to do something else for web?
    public void Save() {
        string filePath = Application.persistentDataPath + "/" + fileName;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);
        PlayerData data = new PlayerData();
        data.highestLevelCompleted = highestLevelCompleted;
        data.achievements = achievements;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load() {
        string filePath = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(filePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

        }
    }
}

[Serializable]
class PlayerData {
    public int highestLevelCompleted;
    public bool[] achievements;
}