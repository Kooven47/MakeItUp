using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroCutsceneAdvance : MonoBehaviour
{
    private float elapsedTime = 0f;
    [SerializeField] private float cutsceneDuration = 25f;
    public Animator anim;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) StartCoroutine(Advance());

        elapsedTime += Time.deltaTime;
        if (elapsedTime >= cutsceneDuration) StartCoroutine(Advance());
    }
    
    private IEnumerator Advance()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSecondsRealtime(4f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
