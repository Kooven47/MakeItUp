using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField]private TMP_Text _healthTally;
    public static Action<float,float> settingHealth;
    public static Action activateHealthBar;

    private Image _healthBar;

    // Start is called before the first frame update
    void Awake()
    {
        settingHealth = SetHealth;
        activateHealthBar = ActivateHealthBar;
        _healthBar = transform.GetChild(1).GetComponent<Image>();
        gameObject.SetActive(false);
    }

    void ActivateHealthBar()
    {
        gameObject.SetActive(true);
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
        _healthBar.fillAmount = (curHP/maxHP);

        if (_healthBar.fillAmount == 0f)
            gameObject.SetActive(false);

        //_healthTally.SetText(curHP.ToString()+"/"+maxHP.ToString());

        // _healthTally.SetText(curHP.ToString()+"/"+maxHP.ToString());
        Debug.Log("Called Healthbar on awake!");
        
    }
}
