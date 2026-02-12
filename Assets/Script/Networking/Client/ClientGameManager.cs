using System.Threading.Tasks;
using UnityEngine;

namespace Networking.Client
{
    public class ClienteGameManager
    {
        public async Task InitAsync()
        {
            Debug.Log("Inicializando ClienteGameManager (simulando login, UGS, etc...)");

            // Simulación de espera de red
            await Task.Delay(1000);

            Debug.Log("ClienteGameManager inicializado correctamente");
        }
    }
}