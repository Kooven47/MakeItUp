using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Events;
using static KillManager;
using TMPro;
using Object = UnityEngine.Object;

public class ObjectiveManagerLevel3 : MonoBehaviour
{
    public Queue<Objective> objList;
    public static bool activeObjective;
    public Objective currentObjective;
    public static event Action UpdateObjective; // Use 'UpdateObjective?.Invoke();' to invoke this. It will do NextObjective() in this class
    public TMP_Text objectiveText;
    [SerializeField] private GameObject _barriers;
    public static Queue<GameObject> barrierList;
    [SerializeField] private List<Transform> minibossSpawnLocations1;
    public static List<Transform> minibossSpawnLocations2;
    
    // Start is called before the first frame update
    void Start()
    {
        objList = new Queue<Objective>();
        barrierList = new Queue<GameObject>();

        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Head to the east");
        activeObjective = false;

        objList.Enqueue(new Objective1Survive());
        objList.Enqueue(new Objective2KillPianos());
        objList.Enqueue(new Objective3GetToMiniBosses());
        objList.Enqueue(new Objective4DefeatMiniBosses());
        objList.Enqueue(new Objective5DefeatBoss());

        minibossSpawnLocations2 = minibossSpawnLocations1;
        
        if (_barriers != null) 
        { 
            for (int i = 0; i < _barriers.transform.childCount; i++)
            {
                GameObject wallGameObject = _barriers.transform.GetChild(i).gameObject;
                wallGameObject.SetActive(true);
                barrierList.Enqueue(wallGameObject);
            }
        }
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

public class Objective1Survive : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    public int curSec;
    public int secObj;

    public static MonoBehaviour _mb;
    public override void OnStart()
    {
        Debug.Log("on start triggered");
        ObjectiveManagerLevel3.activeObjective = true;
        curSec = 0;
        secObj = 30;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();
        
        _mb = Object.FindObjectOfType<MonoBehaviour>();
        Debug.Log("does mb exist? " + _mb != null);
        _mb.StartCoroutine(TimeUpdate());
        
        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        // Add the listener for the next obj
    }
    
    public IEnumerator TimeUpdate()
    {
        while (curSec <= secObj)
        {
            yield return new WaitForSeconds(1f);
            curSec++;
            Display();
        }
        
        if ((curSec >= secObj) && (secObj != -1)) // Objective completed
        {
            secObj = 0;
            OnComplete();
        }
    }
    
    public override void OnComplete()
    {
        ObjectiveManagerLevel3.activeObjective = false;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Survive: " + (curSec) + "/" + secObj);
    }

    public override void Cleanup()
    {
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective2KillPianos : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        killNum = 0;
        killObj = 4;
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
        ObjectiveManagerLevel3.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Kill the pianos: " + (killNum) + "/" + killObj);
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective3GetToMiniBosses : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/Sign/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        // Add the listener for the next obj
    }
    
    public override void OnComplete()
    {
        ObjectiveManagerLevel3.activeObjective = false;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Get to the boss's bodyguards");
    }

    public override void Cleanup()
    {
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective4DefeatMiniBosses : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;

    public SpawnManager spawnManager;
    private List<Transform> minibossSpawnLocations;
    public override void OnStart()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        minibossSpawnLocations = ObjectiveManagerLevel3.minibossSpawnLocations2;
        ObjectiveManagerLevel3.activeObjective = true;
        killNum = 0;
        killObj = 4;
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
        
        // Spawn 2nd round of minibosses
        if (killNum == 2)
        {
            spawnManager.SpawnEnemy(minibossSpawnLocations[0], 0);
            spawnManager.SpawnEnemy(minibossSpawnLocations[1], 1);
        }
        
        if ((killNum >= killObj) && (killObj != -1)) // Objective completed
        {
            killNum = 0;    
            OnComplete();
        }
    }
    
    public override void OnComplete()
    {
        ObjectiveManagerLevel3.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Kill the boss's bodyguards: " + (killNum) + "/" + killObj);
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective5DefeatBoss : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
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
        ObjectiveManagerLevel3.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Defeat the final boss!");
    }

    public override void Cleanup()
    {
        EnemyStats.OnDeath -= KillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}