using UnityEngine;

public class SpawnGameObjectOnDestroy : MonoBehaviour
{
    [SerializeField]
    public GameObject prefab;
    
    //Instanciar el prefab cuando este objeto sea destruido
    private void OnDestroy()
    {
        if (prefab != null)
        {
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}
