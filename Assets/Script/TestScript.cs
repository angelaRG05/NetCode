using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;
    private void Start()
    {
        inputReader.MoveEvent += HandleMove;
        inputReader.PrimaryFireEvent += HandlePrimaryFire;
    }

    private void OnDestroy()
    {
        inputReader.MoveEvent -= HandleMove;
        inputReader.PrimaryFireEvent -= HandlePrimaryFire;
    }

    private void HandleMove(Vector2 movement)
    {
        Debug.Log("Movimiento: " + movement);
    }
    private void HandlePrimaryFire(bool isFiring)
    {
        Debug.Log("Disparando: " + isFiring);
    }
}
