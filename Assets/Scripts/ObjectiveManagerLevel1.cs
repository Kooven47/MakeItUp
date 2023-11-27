using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Events;
using static KillManager;
using TMPro;

public class ObjectiveManagerLevel1 : MonoBehaviour,ISaveGame
{
    public Queue<Objective> objList;
    public static bool activeObjective;
    public static event Action UpdateObjective; // Use 'UpdateObjective?.Invoke();' to invoke this. It will do NextObjective() in this class
    public TMP_Text objectiveText;
    [SerializeField] private GameObject _barriers;
    public static Queue<GameObject> barrierList;
    private int _objectivesComplete = -1;
    // Start is called before the first frame update
    void Start()
    {
        objList = new Queue<Objective>();
        barrierList = new Queue<GameObject>();

        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Head to north-east platform");
        activeObjective = false;
        Debug.Log("Objectives completed "+_objectivesComplete);
        if (_objectivesComplete < 2)
            objList.Enqueue(new Objective1KillSpaghettiMonster());
        if (_objectivesComplete < 3)
            objList.Enqueue(new Objective2KillSpaghettiAndDustBunny());
        if (_objectivesComplete < 5)
            objList.Enqueue(new Objective3KillBoss());
        
        if (_barriers != null) 
        { 
            for (int i = 0; i < _barriers.transform.childCount; i ++)
            {                
                if ((i + 1) < _objectivesComplete) continue;
                GameObject wallGameObject = _barriers.transform.GetChild(i).gameObject;
                wallGameObject.SetActive(true);
                barrierList.Enqueue(wallGameObject);
            }
        }

        if (_objectivesComplete > 0)
        {
            UpdateObjective?.Invoke();
        }
    }
    
    public static void OnUpdateObjective()
    {
        UpdateObjective?.Invoke();
        //SaveSystem.instance.SaveGame();
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

    public void SaveData(ref SaveData sd)
    {
        
    }

    public void SaveInitialData(ref SaveData sd)
    {
        
    }

    public void LoadSaveData(SaveData sd)
    {
        _objectivesComplete = sd.numObjectivesCompleted;
    }

    public void LoadInitialData(SaveData sd)
    {
        _objectivesComplete = -1;
    }
}

public class Objective1KillSpaghettiMonster : Objective
{
    private GameObject signMenu;
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    
    public override void OnStart()
    {
        CheckpointManager.setCheckPoint?.Invoke(1);
        ObjectiveManagerLevel1.activeObjective = true;
        killNum = 0;
        killObj = 1;
        signMenu = GameObject.Find("Signs/AttackTutorial2Sign/Canvas/Sign");
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
        ObjectiveManagerLevel1.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        signMenu.GetComponent<SignMenu>().ShowSign();
        ObjectiveManagerLevel1.OnUpdateObjective();
        GameObject barrier = ObjectiveManagerLevel1.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Kill the monsters: " + (killNum) + "/" + killObj);
    }
    
    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective2KillSpaghettiAndDustBunny : Objective
{
    private GameObject signMenu;
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        CheckpointManager.setCheckPoint?.Invoke(2);
        ObjectiveManagerLevel1.activeObjective = true;
        killNum = 0;
        killObj = 5;
        signMenu = GameObject.Find("Signs/ContinueSign/Canvas/Sign");
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
        CheckpointManager.setCheckPoint?.Invoke(3);
        ObjectiveManagerLevel1.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        signMenu.GetComponent<SignMenuEnemy>().ShowSign();
        ObjectiveManagerLevel1.OnUpdateObjective();
        GameObject barrier = ObjectiveManagerLevel1.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Kill the monsters: " + (killNum) + "/" + killObj);
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective3KillBoss : Objective
{
    private GameObject signMenu;
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel1.activeObjective = true;
        killNum = 0;
        killObj = 1;
        signMenu = GameObject.Find("Signs/BossDefeatSign/Canvas/Sign"); // Change this to the sign object location
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
        CheckpointManager.setCheckPoint?.Invoke(5);
        ObjectiveManagerLevel1.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        signMenu.GetComponent<SignMenu>().ShowSign();
        ObjectiveManagerLevel1.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel1.barrierList.Dequeue(); // This and the next line removes the barrier

        if (barrier.name.Equals("Filler"))
            barrier = ObjectiveManagerLevel1.barrierList.Dequeue();
        
        barrier.SetActive(false);
        Debug.Log("Deactivated this barrier "+barrier.name);
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Get to the Elevator");
    }

    public override void Display()
    {
        objectiveText.SetText("Level 1: Janitor's Closet" + System.Environment.NewLine + "Current Objective - Kill the Toilet Monster");
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

// -------- Level 2 -------- //

// TODO
// Get can of beans from freezer <-- spawn manager
// Escape freezer <-- spawn manager
// Defeat freezer