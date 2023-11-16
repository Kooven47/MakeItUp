using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData
{
    public float janitorMaxHealth = -1f, janitorCurrentHealth = -1f;
    public int currentLevel;
    public int numObjectivesCompleted = -1;

    public float janitorStartMaxHealth = -1f, janitorStartCurrentHealth = -1f;
}