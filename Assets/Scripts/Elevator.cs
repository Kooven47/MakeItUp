using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour,ISaveGame
{
    public Animator anim;
    private PlayerControllerJanitor _playerControllerJanitor;

    bool _entered = false;
    
    private void Start()
    {
        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            const int ELEVATOR = 9;
            _entered = true;
            _playerControllerJanitor.PlaySoundEffect(ELEVATOR);
            StartCoroutine(Advance());
        }
    }

    private IEnumerator Advance()
    {
        GlobalSpeedrunTimer.StopTimer();
        _playerControllerJanitor.SetStunned(true);
        anim.SetBool("Fade", true);
        SaveSystem.instance.SaveGame();
        yield return new WaitForSecondsRealtime(2f);
        _playerControllerJanitor.SetStunned(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadSaveData(SaveData data)
    {
        
    }

    public void LoadInitialData(SaveData data)
    {
        
    }

    public void SaveData(ref SaveData data)
    {
        if (!_entered)
            return;
        
        data.currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
        data.numObjectivesCompleted = 0;
    }

    public void SaveInitialData(ref SaveData data)
    {
        if(!_entered)
            return;
        
        data.currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
        data.numObjectivesCompleted = 0;
    }
}
