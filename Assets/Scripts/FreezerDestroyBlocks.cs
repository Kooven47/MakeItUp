using System.Collections;
using UnityEngine;

public class FreezerDestroyBlocks : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            var block = other.gameObject;
            block.SetActive(false);
            StartCoroutine(SlowFreezerDown(0.2f, 0.5f));
        }
    }

    private IEnumerator SlowFreezerDown(float time, float ratio)
    {
        var freezerStats = this.GetComponent<EnemyAI>();
        freezerStats.speed *= ratio;
        yield return new WaitForSeconds(time);
        freezerStats.speed /= ratio;
    }
}
