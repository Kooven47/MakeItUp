using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective
{
    public abstract void OnStart();
    public abstract void OnComplete();
    public abstract void Display();
    public abstract void Cleanup();
}
