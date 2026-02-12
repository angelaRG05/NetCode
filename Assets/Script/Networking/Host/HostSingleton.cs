using UnityEngine;
using System.Threading.Tasks;

namespace Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;
        // Si exista la devuleve; sino la busca en escena; Si no la encuentra muestra error
        public static HostSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    // Busca en escena
                    instance = FindAnyObjectByType<HostSingleton>();

                    if (instance == null)
                    {
                        // Muestra error
                        Debug.LogError("No se encontró ninguna instancia de HostSingleton en la escena.");
                    }
                }
                // Devuelve
                return instance;
            }
        }

        private HostGameManager gameManager;

        // Patron singleton + persistencia entre escenas
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Método público asíncrono para inicializar el HostGameManager
        public async Task InitAsync()
        {
            Debug.Log("Inicializando HostSingleton...");

            gameManager = new HostGameManager();
            await gameManager.InitAsync();

            Debug.Log("HostSingleton inicializado correctamente");
        }
    }
}