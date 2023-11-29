using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static KillManager;
using TMPro;
using Object = UnityEngine.Object;

public class ObjectiveManagerLevel2 : MonoBehaviour
{
    public Queue<Objective> objList;
    public static bool activeObjective;
    public Objective currentObjective;
    public static event Action UpdateObjective; // Use 'UpdateObjective?.Invoke();' to invoke this. It will do NextObjective() in this class
    public TMP_Text objectiveText;
    // Start is called before the first frame update
    void Start()
    {
        objList = new Queue<Objective>();

        objectiveText.SetText("Level 2: Cafeteria" + System.Environment.NewLine + "Current Objective - Get some well-earned beans");
        activeObjective = false;

        objList.Enqueue(new Objective1Investigate());
        objList.Enqueue(new Objective2EscapeFreezer());
        objList.Enqueue(new Objective3DefeatFreezer());
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
                currentObjective = objList.Dequeue();
                currentObjective.OnStart();
                // GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
                // barrier.SetActive(false);
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

public class Objective1Investigate : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public override void OnStart()
    {
        ObjectiveManagerLevel2.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        // Add the listener for the next obj
    }

    public override void OnComplete()
    {
        ObjectiveManagerLevel2.activeObjective = false;
        ObjectiveManagerLevel2.OnUpdateObjective();
    }

    public override void Display()
    {
        objectiveText.SetText("Level 2: Cafeteria" + System.Environment.NewLine + "Current Objective - Find a way out of the cafeteria");
    }

    public override void Cleanup()
    {
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}
public class Objective2EscapeFreezer : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel2.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        // Add the listener for the next obj
    }
    
    public override void OnComplete()
    {
        ObjectiveManagerLevel2.activeObjective = false;
        ObjectiveManagerLevel2.OnUpdateObjective();
    }

    public override void Display()
    {
        objectiveText.SetText("Level 2: Cafeteria" + System.Environment.NewLine + "Current Objective - Escape the Angry Freezer!");
    }

    public override void Cleanup()
    {
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective3DefeatFreezer : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel2.activeObjective = true;
        killNum = 0;
        killObj = 1;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        EnemyStats.OnDeath += KillUpdate;
        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

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
        ObjectiveManagerLevel2.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        ObjectiveManagerLevel2.OnUpdateObjective();
    }

    public override void Display()
    {
        objectiveText.SetText("Level 2: Cafeteria" + System.Environment.NewLine + "Current Objective - Kill the freezer!");
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}