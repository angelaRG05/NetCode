using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{   
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cuando entre en colisíon con otro collider, se destruye
        Destroy(gameObject);
    }
}
