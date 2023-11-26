using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnumLib
{
    public enum DamageType {Neutral,Dry,Wet};
    public enum KnockBackPower{None,Sideways,Launch,SideLaunch};

    public enum ProjectileType{None,Dust};

    public static Vector2 KnockbackVector(KnockBackPower power)
    {
        switch (power)
        {
            case KnockBackPower.Sideways:
            {
                return new Vector2(20f,0f);
            }

            case KnockBackPower.Launch:
            {
                return new Vector2(0f,20f);
            }

            case KnockBackPower.SideLaunch:
            {
                return new Vector2(20f,20f);
            }
        }
        return Vector2.zero;
    }
}