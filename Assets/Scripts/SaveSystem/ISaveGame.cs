using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISaveGame
{
    void LoadSaveData(SaveData data);
    void SaveData(ref SaveData data);
}
