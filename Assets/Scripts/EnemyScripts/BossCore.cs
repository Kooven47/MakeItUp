using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCore : MonoBehaviour
{
    [SerializeField] protected BoxCollider2D _toiletCollider;
    [SerializeField] protected List<EnemyAbility> _moveSet = new List<EnemyAbility>();
    protected Rigidbody2D _rb;
    protected EnemyStats _bossStats;

    protected int _curPhase = 0;

    protected float[] _toNextPhase = new float[1];// HP Thresholds

    protected virtual void Flip()
    {

    }
}