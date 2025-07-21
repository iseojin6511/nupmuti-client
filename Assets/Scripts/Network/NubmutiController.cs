using UnityEngine;

public class NubmutiController : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.EnterRoom>(OnEnterRoom);
        // Login 관련 핸들러 제거
    }

    private void OnEnterRoom(ResponsePacketData.EnterRoom data)
    {
        //Debug.Log($"Entered Room: {data.roomName} with ID {data.roomId}, Players: {data.participantCount}/{data.maxPlayerCount}");
    }

    public void OnError(int signal, int code)
    {
        Debug.LogError($"Error from server. Signal: {signal}, Code: {code}");
    }
}

