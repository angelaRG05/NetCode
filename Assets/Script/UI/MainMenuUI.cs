using Networking.Host;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    
    // Método llamado por el botón "Host".
        
    public async void OnClickHostButton()
    {
        Debug.Log("Botón Host pulsado");

        // Comprobamos que exista el HostSingleton
        if (HostSingleton.Instance == null)
        {
            Debug.LogError("HostSingleton no está inicializado");
            return;
        }

        // Llamamos a la lógica real de hosting
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
}
