using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    public Animator anim;
    private PlayerControllerJanitor _playerControllerJanitor;
    
    private void Start()
    {
        _playerControllerJanitor = GameObject.FindWithTag("Player").GetComponent<PlayerControllerJanitor>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            const int ELEVATOR = 9;
            _playerControllerJanitor.PlaySoundEffect(ELEVATOR);
            StartCoroutine(Advance());
        }
    }

    private IEnumerator Advance()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
