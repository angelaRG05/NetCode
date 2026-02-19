using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace Networking.Client
{
    public class ClienteGameManager
    {
       
        public async Task InitAsync()
        {
            // Inicializamos servicios de unity
            Debug.Log("Inicializando Unity Services...");

            await UnityServices.InitializeAsync();

            Debug.Log("Unity Services inicializados");

            // intento de autenticación
            AuthState authResult = await AuthenticationWrapper.DoAuth(3);

            // Si inicio exitoso
            if (authResult == AuthState.Authenticated)
            {
                GoToMenu(); // vamos al menu 
            }
            else
            {
                Debug.LogError("Fallo en autenticación");
            }
        }

        private void GoToMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        }
    }
}