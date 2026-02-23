using UnityEngine;
using System.Threading.Tasks;

namespace Networking.Host
{
    public class HostSingleton : MonoBehaviour
    {
        private static HostSingleton instance;
        [SerializeField] public bool hostButtonPressed = false;
        public string CurrentJoinCode { get; set; }
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

        //private HostGameManager gameManager;
        public HostGameManager GameManager { get; private set; }

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

            // llamar a la logica de gamemanager
            _ = InitAsync();

            Debug.Log("HostSingleton inicializado");
        }

        // Método público asíncrono para inicializar el HostGameManager
        public async Task InitAsync()
        {
            Debug.Log("Inicializando HostSingleton...");

            GameManager = new HostGameManager();
            await GameManager.InitAsync();

            Debug.Log("HostSingleton inicializado correctamente");
        }
    }
}