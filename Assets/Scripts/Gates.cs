using UnityEngine;

public class Gates : MonoBehaviour
{
    int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("ball"))
        {
            Destroy(other.gameObject);
            score++;
            Debug.Log($"Your total score: {score}");
        }
    }
}
