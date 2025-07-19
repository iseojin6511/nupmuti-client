using UnityEngine;

public class NubmutiController : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.EnterRoom>(OnEnterRoom);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.Login>(OnLogin);
    }

    private void OnEnterRoom(ResponsePacketData.EnterRoom data)
    {
        Debug.Log($"Entered Room: {data.roomName} with ID {data.roomId}");
    }

    private void OnLogin(ResponsePacketData.Login data)
    {
        Debug.Log($"Logged in as {data.nickname}");
    }

    public void OnError(int signal, int code)
    {
        Debug.LogError($"Error from server. Signal: {signal}, Code: {code}");
    }
}

