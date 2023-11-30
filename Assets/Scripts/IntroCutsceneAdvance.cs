using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneAdvance : MonoBehaviour
{
    private float elapsedTime = 0f;
    [SerializeField] private float cutsceneDuration = 25f;
    public Animator anim;
    private Coroutine coroutine;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) coroutine = StartCoroutine(Advance());

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= cutsceneDuration && coroutine == null) StartCoroutine(Advance());
    }
    
    private IEnumerator Advance()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
