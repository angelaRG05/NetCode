using UnityEngine;
using System.Threading.Tasks;

namespace Networking.Client
{
    public class ClienteSingleton : MonoBehaviour
    {
        private static ClienteSingleton instance;

        // Si exista la devuleve; sino la busca en escena; Si no la encuentra muestra error
        public static ClienteSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    // Busca en escena
                    instance = FindAnyObjectByType<ClienteSingleton>();

                    if (instance == null)
                    {
                        // Muestra error
                        Debug.LogError("No se encontró ninguna instancia de ClienteSingleton en la escena.");
                    }
                }
                // Devuelve
                return instance;
            }
        }

        private ClienteGameManager gameManager;

        // Patrón Singleton + mantiene en escenas
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            // Inicia automáticamente la lógica del cliente
            _ = InitAsync();

        }

        // Crea ClientGameManager (logica del ciente)
        // Metodo asincrono que simula (login, conexiones...)
        public async Task InitAsync()
        {
            Debug.Log("Inicializando ClienteSingleton...");

            gameManager = new ClienteGameManager();
            await gameManager.InitAsync();

            Debug.Log("ClienteSingleton inicializado correctamente");
        }
    }
}