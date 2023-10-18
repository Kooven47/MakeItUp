using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Events;
using static KillManager;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public Queue<Objective> objList = new Queue<Objective>();
    public static bool activeObjective;
    public static event Action UpdateObjective; // Use 'UpdateObjective?.Invoke();' to invoke this. It will do NextObjective() in this class
    public TMP_Text objectiveText;
    // Start is called before the first frame update
    void Start()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Head to north-east platform");
        activeObjective = false;
        objList.Enqueue(new Objective1Kill());
        objList.Enqueue(new Objective2Kill());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void OnUpdateObjective()
    {
        UpdateObjective?.Invoke();
    }
    public void NextObjective()
    {
        if (!activeObjective) // No active Objective
        {
            if (objList.Count > 0)
            {
                Objective CurrentObjective = objList.Dequeue();
                CurrentObjective.OnStart();
            }
        }
        else
        {
            // There's an active objective, do something here maybe?
        }
    }

    private void OnEnable()
    {
        UpdateObjective += NextObjective;
    }

    private void OnDisable()
    {
        UpdateObjective -= NextObjective;

    }
}

public abstract class Objective
{
    public abstract void OnStart();
    public abstract void OnComplete();
    public abstract void Display();

}

public class Objective1Kill : Objective
{
    private GameObject signMenu;
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    
    public override void OnStart()
    {
        ObjectiveManager.activeObjective = true;
        killNum = 0;
        killObj = 1;
        signMenu = GameObject.Find("Signs/AttackTutorial2Sign/Canvas/Sign");
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();
        EnemyStats.OnDeath += KillUpdate;
        Display();

        // Add the listener for the next obj
    }
    public void KillUpdate()
    {
        killNum++;
        Display();
        if ((killNum >= killObj) && (killObj != -1)) // Objective completed
        {
            killNum = 0;
            OnComplete();
        }
        
    }
    public override void OnComplete()
    {
        ObjectiveManager.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        signMenu.GetComponent<SignMenuEnemy>().ShowSign();
        ObjectiveManager.OnUpdateObjective();
        //Remove listener
    }

    public override void Display()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Kill the monsters: " + (killNum) + "/" + killObj);
    }
}

public class Objective2Kill : Objective
{
    private GameObject signMenu;
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        ObjectiveManager.activeObjective = true;
        killNum = 0;
        killObj = 5;
        signMenu = GameObject.Find("Signs/ContinueSign/Canvas/Sign");
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();
        EnemyStats.OnDeath += KillUpdate;
        Display();

        // Add the listener for the next obj
    }
    public void KillUpdate()
    {
        killNum++;
        Display();
        if ((killNum >= killObj) && (killObj != -1)) // Objective completed
        {
            killNum = 0;
            OnComplete();
        }


    }
    public override void OnComplete()
    {
        ObjectiveManager.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        signMenu.GetComponent<SignMenuEnemy>().ShowSign();
        ObjectiveManager.OnUpdateObjective();
        //Remove listener
    }

    public override void Display()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Kill the monsters: " + (killNum) + "/" + killObj);
    }

}
