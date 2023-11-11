using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimFunctions : MonoBehaviour
{
    public BossCore bossCore;
    public virtual void Fire()
    {
        bossCore.Fire();
    }

    public virtual void Recover()
    {
        bossCore.Recovery();
    }
}