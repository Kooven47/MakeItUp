using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New PlayerAbility", menuName = "PlayerAbility")]
public class PlayerAbility : ScriptableObject
{
    public float damage = 10f;
    public AnimationClip[] animations = new AnimationClip[2];
    public EnumLib.DamageType dmgType;

}
