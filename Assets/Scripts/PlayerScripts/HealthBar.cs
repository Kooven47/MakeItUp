using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]private int _maxSegments = 0;
    private float _curRatio = 1f;

    private float _testCurHP = 100f, _testMaxHP = 100f;
    // Start is called before the first frame update
    void Start()
    {
        _maxSegments = transform.childCount;
    }

    void SetHealth(float curHP, float maxHP)
    {
        // For test purposes. Later on the HP management will be done by stat script.
        if (curHP < 0f)
        {
            curHP = 0f;
        }
        else if (curHP > maxHP)
        {
            curHP = maxHP;
        }

        // Gets new ratio from passed in current HP and max HP
        float newRatio = (curHP/maxHP);

        // If no change, just return
        if (newRatio == _curRatio)
            return;

        // Gathers the number of segments from the current ratio and new ratio
        float curSegments = _curRatio * _maxSegments;
        float newSegments = newRatio * _maxSegments;

        // Check to see if the new ratio is greater than the old ratio
        bool isIncreasing = newRatio > _curRatio;

        // Check which segment will get the fillAmount. Example: 4.75 segments means 5th segment will get fillAmount
        int newHPSegment = (int)Mathf.Ceil(newSegments);

        Debug.Log("CurSegment "+curSegments);
        Debug.Log("newSegment "+newSegments);

        // Gets fill amount by subtracting the segment count by the floor. Example: 4.75 - 4<-(Floor(4.75)) = 0.75 fillamount
        float diff = newSegments - Mathf.Floor(newSegments);

        if (isIncreasing)
        {
            // Insert HP increasing code for when restoring health.
            
            Debug.Log("Diff is "+diff);
            
            for (int i = 0; i < _maxSegments; i++)// Iterate through all segments.
            {
                // Due to child counting starting at 0. Example: 5th segment means Child #4
                if (i == (newHPSegment - 1))
                {
                    // If we get a differnce of 0 (going to whole number) just have fillAmount be 1f
                    // Once we reach that sprite, end loop.
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = (diff <= 0f ? 1.0f : diff);
                    break;
                }
                else if (i < (newHPSegment - 1))
                {
                    // Any segments BEFORE the selected segment will automtically have fillAmount of 1f
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = 1.0f;
                }
            } 
        }
        else
        {
            for (int i = 0; i < _maxSegments; i++)// Iterate through all segments
            {
                if (i == (newHPSegment - 1))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().fillAmount = diff;
                }
                else if (i > (newHPSegment - 1))// If the segments selected are AFTER the selected segment, set it to 0 fillAmount
                {
                    Image toiletSprite = transform.GetChild(i).GetChild(0).GetComponent<Image>();

                    if (toiletSprite.fillAmount == 0.0f)// If the sprite fill already is empty, end loop
                        break;
                    
                    toiletSprite.fillAmount = 0.0f;
                }
            }  
        }

        _curRatio = newRatio;

        _testCurHP = curHP;// For test purposes. Later on the HP management will be done by stat script.
        
    }

    void Update()
    {
        // For test purposes. Later on the HP management will be done by stat script.
        if (Input.GetKeyUp("x"))
        {
            SetHealth(_testCurHP - 7f,_testMaxHP);
        }
        if (Input.GetKeyUp("c"))
        {
            SetHealth(_testCurHP + 7f,_testMaxHP);
        }
    }
}
