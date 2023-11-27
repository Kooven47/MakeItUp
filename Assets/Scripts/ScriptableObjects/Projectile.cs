using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Projectile : ScriptableObject
{
    public EnumLib.DamageType projectileType;
    public int maxBounces = 3;
    public List<AnimationClip> projectileAnims = new List<AnimationClip>(3);
}