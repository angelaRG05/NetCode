using Networking.Host;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI joincode;

    public void Start()
    {
        if (HostSingleton.Instance.hostButtonPressed == false) return;
        joincode.text = HostSingleton.Instance.CurrentJoinCode;
    }
}
