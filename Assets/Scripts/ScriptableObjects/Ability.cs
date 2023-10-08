using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Ability : ScriptableObject
{
    public float damage = 10f;
    public AnimationClip[] animations = new AnimationClip[2];

    public EnumLib.KnockBackPower force = EnumLib.KnockBackPower.Sideways;
}
