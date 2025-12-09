using UnityEngine;
using Unity.Netcode;

public class TestScript : NetworkBehaviour
{
    [SerializeField] private InputReader inputReader;
    [SerializeField] private int speed;

    public int move = 5;
    public int rotation = 150;
    public Transform turret;

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
        //if (!IsOwner) return;
        //Vector3 dir = new Vector3(movement.x, movement.y, 0f).normalized;
        //transform.position += dir speed * Time.deltaTime;
    }

    private void HandlePrimaryFire(bool isFiring)
    {
        Debug.Log("Disparando: " + isFiring);
    }

    void Update()
    {
        if (!IsOwner) return;

        float movement = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * movement * -rotation * Time.deltaTime);

        if (Input.GetAxis("Vertical") > 0)
        {
            transform.Translate(Vector3.up * move * Time.deltaTime);
        }

        else if (Input.GetAxis("Vertical") < 0)
        {
            transform.Translate(Vector3.down * move * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.O))
        {
            turret.transform.Rotate(Vector3.forward * rotation * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.P))
        {
            turret.transform.Rotate(Vector3.forward * -rotation * Time.deltaTime);
        }
    }
}