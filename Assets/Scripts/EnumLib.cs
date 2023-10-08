using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnumLib
{
    public enum DamageType {Neutral,Dry,Wet};
    public enum KnockBackPower{Sideways,Launch,SideLaunch};

    public static Vector2 KnockbackVector(KnockBackPower power)
    {
        switch (power)
        {
            case KnockBackPower.Sideways:
            {
                return new Vector2(20f,0f);
            }
        }
        return Vector2.zero;
    }
}