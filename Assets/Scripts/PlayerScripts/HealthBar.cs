using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthBar : MonoBehaviour
{
    [SerializeField]private int _maxSegments = 0;
    [SerializeField]private TMP_Text _healthTally;
    private float _curRatio = 1f;

    public static Action<float,float> settingHealth;

    private float _testCurHP = 100f, _testMaxHP = 100f;
    // Start is called before the first frame update
    void Start()
    {
        _maxSegments = transform.childCount;
        settingHealth = SetHealth;
        SetHealth(100,100);
    }

    void SetHealth(float curHP, float maxHP)
    {
        if (curHP < 0f)
        {
            curHP = 0f;
        }
        else if (curHP > maxHP)
        {
            curHP = maxHP;
        }

        // 0.8f new ratio, old ratio = 0.67f
        float newRatio = (curHP/maxHP);

        if (newRatio == _curRatio)
            return;

        float curSegments = _curRatio * _maxSegments;
        float newSegments = newRatio * _maxSegments;

        bool isIncreasing = newRatio > _curRatio;

        int newHPSegment = (int)Mathf.Ceil(newSegments);

        Debug.Log("CurSegment "+curSegments);
        Debug.Log("newSegment "+newSegments);

        float diff = newSegments - Mathf.Floor(newSegments);

        if (!isIncreasing)
        {
            
            for (int i = 0; i < _maxSegments; i++)
            {
                if (i == (newHPSegment - 1))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = diff;
                }
                else if (i > (newHPSegment - 1))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = 0.0f;
                }
            }  
        }
        else
        {
            // Insert HP increasing code for when restoring health.
            
            Debug.Log("Diff is "+diff);
            
            for (int i = 0; i < _maxSegments; i++)
            {
                if (i == (newHPSegment - 1))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = (diff <= 0f ? 1.0f : diff);
                }
                else if (i < (newHPSegment - 1))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = 1.0f;
                }
            } 
        }
        

        _curRatio = newRatio;

        _healthTally.SetText(curHP.ToString()+"/"+maxHP.ToString());

        _testCurHP = curHP;
        
    }

    void Update()
    {
        // For test purposes. Later on the HP management will be done by stat script.
        if (Input.GetKeyUp("c"))
        {
            SetHealth(_testCurHP - 7f,_testMaxHP);
        }
        if (Input.GetKeyUp("v"))
        {
            SetHealth(_testCurHP + 7f,_testMaxHP);
        }
    }
}
