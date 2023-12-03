using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static KillManager;
using TMPro;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class ObjectiveManagerLevel3 : MonoBehaviour, ISaveGame
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
    private int _objectivesComplete = -1;

    // Start is called before the first frame update
    void Start()
    {
        if (_objectivesComplete < 0)
        {
            Debug.Log("Objectives completed not initialized");
            SaveSystem.instance.LoadGame();
        }
        
        objList = new Queue<Objective>();
        barrierList = new Queue<GameObject>();

        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Head to the east");
        activeObjective = false;
        Debug.Log($"num objectives completed: {_objectivesComplete}");
        
        if (_objectivesComplete < 1)
            objList.Enqueue(new Objective1Survive());
        if (_objectivesComplete < 2)
            objList.Enqueue(new Objective2GetOut());
        if (_objectivesComplete < 3)
            objList.Enqueue(new Objective3KillPianos());
        if (_objectivesComplete <= 4)
            objList.Enqueue(new Objective4GetToMiniBosses());
        if (_objectivesComplete < 5)
            objList.Enqueue(new Objective5DefeatMiniBosses());
        if (_objectivesComplete < 6)
            objList.Enqueue(new Objective6DefeatBoss());

        if (_barriers != null) 
        { 
            for (int i = 0; i < _barriers.transform.childCount; i++)
            {
                GameObject wallGameObject = _barriers.transform.GetChild(i).gameObject;
                if ((i + 1) < _objectivesComplete)
                {
                    // wallGameObject.SetActive(false); 
                    continue;
                }
                
                Debug.Log("added in barrier "+wallGameObject.name);
                wallGameObject.SetActive(true);
                barrierList.Enqueue(wallGameObject);
            }
        }
        
        if (_objectivesComplete >= 2)
        {
            GameObject.Find("Grid/EnemyEnclosure1").GetComponent<WallEnclosureCollisionLevel3>().spawnEnemies = false;
            GameObject.Find("Grid/EnemyEnclosure1").GetComponent<WallEnclosureCollisionLevel3>().triggerObj = false;
            GameObject.Find("Grid/EnemyEnclosure1/Wall1").gameObject.SetActive(true);
            _barriers.transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("Grid/EnemyEnclosure2").GetComponent<WallEnclosureCollisionLevel3>().triggerObj = false;
        }

        if (_objectivesComplete >= 3)
        {
            GameObject.Find("Grid/EnemyEnclosure2").GetComponent<WallEnclosureCollisionLevel3>().spawnEnemies = false;
            _barriers.transform.GetChild(0).gameObject.SetActive(true);
            _barriers.transform.GetChild(1).gameObject.SetActive(false);
        }

        // if (_objectivesComplete >= 4)
        // {
            // GameObject.Find("Grid/EnemyEnclosure4").GetComponent<WallEnclosureCollisionLevel3>().triggerObj = false;
        // }
        
        if (_objectivesComplete >= 5)
        {
            GameObject.Find("Grid/EnemyEnclosure4").GetComponent<WallEnclosureCollisionLevel3>().triggerObj = false;
            GameObject.Find("Grid/EnemyEnclosure4").GetComponent<WallEnclosureCollisionLevel3>().spawnEnemies = false;
            _barriers.transform.GetChild(0).gameObject.SetActive(true);
            _barriers.transform.GetChild(2).gameObject.SetActive(false);
        }

        if (_objectivesComplete >= 6)
        {
            objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Get to the Elevator");
            GameObject.Find("Grid/EnemyEnclosure5").GetComponent<WallEnclosureCollisionLevel3>().spawnEnemies = false;
            GameObject.Find("Grid/EnemyEnclosure5").GetComponent<WallEnclosureCollisionLevel3>().isBossEntrance = false;
            _barriers.transform.GetChild(2).gameObject.SetActive(true);
            _barriers.transform.GetChild(4).gameObject.SetActive(false);
        }

        minibossSpawnLocations2 = minibossSpawnLocations1;
        
        if (_objectivesComplete > 0)
        {
            UpdateObjective?.Invoke();
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

public class Objective1Survive : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    public int curSec;
    public int secObj;

    public static MonoBehaviour _mb;
    private List<GameObject> _enemies;
    public override void OnStart()
    {
        _enemies = new List<GameObject>();
        Debug.Log("on start triggered");
        ObjectiveManagerLevel3.activeObjective = true;
        curSec = 0;
        secObj = 30;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
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
        while (curSec < secObj)
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
        
        foreach (Transform child in GameObject.Find("Enemies").GetComponent<Transform>())
        {
            _enemies.Add(child.gameObject);
        }
        _mb.StartCoroutine(KillOffEnemies());
    }

    public IEnumerator KillOffEnemies()
    {
        while (_enemies.Count > 0)
        {
            yield return new WaitForSeconds(0.1f);
            var enemyToRemove = Random.Range(0, _enemies.Count);
            Object.Destroy(_enemies[enemyToRemove].gameObject);
            _enemies.RemoveAt(enemyToRemove);
            Debug.Log($"removed enemy, count is now {_enemies.Count}");
        }
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

public class Objective2GetOut : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    private PlayerControllerJanitor _playerControllerJanitor;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();

        // Add the listener for the next obj
    }
    
    public override void OnComplete()
    {
        CheckpointManager.setCheckPoint?.Invoke(2);
        ObjectiveManagerLevel3.activeObjective = false;
        ObjectiveManagerLevel3.OnUpdateObjective();
        const int SUCCESS = 13;
        _playerControllerJanitor.PlaySoundEffect(SUCCESS);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Escape this room!");
    }

    public override void Cleanup()
    {
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}

public class Objective3KillPianos : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;
    
    private PlayerControllerJanitor _playerControllerJanitor;

    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        killNum = 0;
        killObj = 4;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        EnemyStats.OnDeath += KillUpdate;
        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();

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
        const int SUCCESS = 13;
        _playerControllerJanitor.PlaySoundEffect(SUCCESS);
        
        CheckpointManager.setCheckPoint?.Invoke(3);
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

public class Objective4GetToMiniBosses : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        // Add the listener for the next obj
    }
    
    public override void OnComplete()
    {
        CheckpointManager.setCheckPoint?.Invoke(4);
        ObjectiveManagerLevel3.activeObjective = false;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        // GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        // barrier.SetActive(false);
        // Debug.Log("dequeued barrier " + barrier.transform.name);
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

public class Objective5DefeatMiniBosses : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;

    public int killNum;
    public int killObj;

    public SpawnManager spawnManager;
    private List<Transform> minibossSpawnLocations;
    
    private PlayerControllerJanitor _playerControllerJanitor;
    
    public override void OnStart()
    {
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        minibossSpawnLocations = ObjectiveManagerLevel3.minibossSpawnLocations2;
        ObjectiveManagerLevel3.activeObjective = true;
        killNum = 0;
        killObj = 4;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();

        EnemyStats.OnDeath += KillUpdate;
        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;

        Display();

        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();

        // Add the listener for the next obj
    }
    
    public void KillUpdate()
    {
        killNum++;
        Display();
        
        // Spawn 2nd round of minibosses
        if (killNum == 2)
        {
            spawnManager.SpawnEnemy(minibossSpawnLocations[0], 8);
            spawnManager.SpawnEnemy(minibossSpawnLocations[1], 9);
        }
        
        if ((killNum >= killObj) && (killObj != -1)) // Objective completed
        {
            killNum = 0;    
            OnComplete();
        }
    }
    
    public override void OnComplete()
    {
        CheckpointManager.setCheckPoint?.Invoke(5);
        ObjectiveManagerLevel3.activeObjective = false;
        EnemyStats.OnDeath -= KillUpdate;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
        // im so tired
        if (barrier.name.Equals("FILLER"))
            GameObject.Find("Grid/Barriers/Wall5Boss").SetActive(false);
        const int SUCCESS = 13;
        _playerControllerJanitor.PlaySoundEffect(SUCCESS);
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

public class Objective6DefeatBoss : Objective
{
    private GameObject objectiveTextObject;
    private TMP_Text objectiveText;
    
    private PlayerControllerJanitor _playerControllerJanitor;

    public override void OnStart()
    {
        ObjectiveManagerLevel3.activeObjective = true;
        objectiveTextObject = GameObject.Find("ObjectiveManager/Canvas/TextBox/ObjectiveText"); // This is to find the ObjectiveText object for display
        objectiveText = objectiveTextObject.GetComponent<TMP_Text>();
        
        EnemyStats.BossOnDeath += BossKillUpdate;
        PauseMenu.cleanUp += Cleanup;
        GameOverMenu.cleanUp += Cleanup;
        Display();

        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();

        // Add the listener for the next obj
    }
 
    public void BossKillUpdate()
    {
        Debug.Log("Boss Defeated");
        OnComplete();
    }

    public override void OnComplete()
    {
        CheckpointManager.setCheckPoint?.Invoke(6);
        ObjectiveManagerLevel3.activeObjective = false;
        ObjectiveManagerLevel3.OnUpdateObjective();
        
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Get to the Elevator");
        
        GameObject barrier = ObjectiveManagerLevel3.barrierList.Dequeue(); // This and the next line removes the barrier
        Debug.Log("dequeued barrier " + barrier.transform.name);
        if (barrier.name.Equals("FILLER"))
            barrier = ObjectiveManagerLevel1.barrierList.Dequeue();
        barrier.SetActive(false);
        Debug.Log("dequeued barrier " + barrier.transform.name);
        const int SUCCESS = 13;
        _playerControllerJanitor.PlaySoundEffect(SUCCESS);
    }

    public override void Display()
    {
        objectiveText.SetText("Level 3: Executive Suite" + System.Environment.NewLine + "Current Objective - Defeat the final boss!");
    }

    public override void Cleanup()
    {
        EnemyStats.BossOnDeath -= BossKillUpdate;
        PauseMenu.cleanUp -= Cleanup;
        GameOverMenu.cleanUp -= Cleanup;
    }
}