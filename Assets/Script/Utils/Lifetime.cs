using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField]
    private float lifetime = 2f; // duracion bala x segundo

    private void Start()
    {
        // Destruir el objeto cuando pase lifetime
        Destroy(gameObject, lifetime);
    }
}
