using Networking.Client; 
using Networking.Host;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_InputField joinCodeField;

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
        HostSingleton.Instance.hostButtonPressed = true;
        // Llamamos a la lógica real de hosting
        await HostSingleton.Instance.GameManager.StartHostAsync();

    }
    // Método llamado por el botón "Client"
    public async void OnClickClientButton()
    {
        if (string.IsNullOrEmpty(joinCodeField.text))
        {
            Debug.LogWarning("Debe ingresar un Join Code válido");
            return;
        }

        string code = joinCodeField.text.Trim().ToUpper();
        Debug.Log($"Botón Client pulsado, Join Code: {code}");

        if (ClienteSingleton.Instance == null || ClienteSingleton.Instance.GameManager == null)
        {
            Debug.LogError("ClienteSingleton o GameManager no está inicializado");
            return;
        }

        await ClienteSingleton.Instance.GameManager.StartClientAsync(code);
    }
}
