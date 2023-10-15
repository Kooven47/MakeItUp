using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Events;
using static KillManager;

public class ObjectiveManager : MonoBehaviour
{
    public Queue<Objective> objList = new Queue<Objective>();
    public static bool activeObjective;
    public static event Action UpdateObjective; // Use 'UpdateObjective?.Invoke();' to invoke this. It will do NextObjective() in this class

    // Start is called before the first frame update
    void Start()
    {
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

}

public class Objective1Kill : Objective
{
    private GameObject signMenu;
    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        ObjectiveManager.activeObjective = true;
        killNum = 0;
        killObj = 1;
        signMenu = GameObject.Find("Signs/AttackTutorial2Sign/Canvas/Sign");
        EnemyStats.OnDeath += KillUpdate;
        // Add the listener for the next obj
    }
    public void KillUpdate()
    {
        killNum++;
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
}

public class Objective2Kill : Objective
{
    private GameObject signMenu;
    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        ObjectiveManager.activeObjective = true;
        killNum = 0;
        killObj = 5;
        signMenu = GameObject.Find("Signs/ContinueSign/Canvas/Sign");
        EnemyStats.OnDeath += KillUpdate;
        // Add the listener for the next obj
    }
    public void KillUpdate()
    {
        killNum++;
        Debug.Log("Kill Number: " + killNum);
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
}
