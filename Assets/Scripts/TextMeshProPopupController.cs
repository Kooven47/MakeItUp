using UnityEngine;
using TMPro;

public class TextMeshProPopupController : MonoBehaviour
{
    public TMP_Text popupText;
    public float displayDistance = 5f; // Adjust this distance as needed.

    private void Start()
    {
        // Initially, hide the popup text.
        popupText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Show the popup text when the player enters the trigger area.
            popupText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide the popup text when the player exits the trigger area.
            popupText.gameObject.SetActive(false);
        }
    }
}