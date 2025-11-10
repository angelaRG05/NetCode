using UnityEngine;

public class TankController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 150f;

    // Pivote
    public Transform turret;

    void Update()
    {
        // Giro W S
        float moveInput = Input.GetAxisRaw("Vertical");
        transform.Translate(Vector3.up * moveInput * moveSpeed * Time.deltaTime);

        // Giro A D (efecto giro 1 rueda + rápido)
        float rotInput = Input.GetAxisRaw("Horizontal");
        // -rotInput para que parezca que gira al mismo sentido que la tecla pulsado
        transform.Rotate(Vector3.forward * -rotInput * rotationSpeed * Time.deltaTime);


        // Rotación con teclas O y P
        if (Input.GetKey(KeyCode.O))
        {
            turret.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.P))
        {
            turret.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
        }
    }
 }

