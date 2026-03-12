using UnityEngine;
using System.Threading.Tasks;

namespace Networking.Server
{
    public class ServerSingleton : MonoBehaviour
    {
        private static ServerSingleton instance;

        public static ServerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<ServerSingleton>();

                    if (instance == null)
                    {
                        Debug.LogError("No se ha encontrado ninguna instancia en la escena.");
                    }
                }

                return instance;
            }
        }

        public ServerGameManager gameManager { get; private set; }


        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            _ = InitAsync();
        }


        public async Task InitAsync()
        {
            gameManager = new ServerGameManager();
            await gameManager.InitAsync();

            Debug.Log("ClienteSingleton inicializado correctamente");
        }
    }
}
