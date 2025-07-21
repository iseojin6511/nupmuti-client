using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환용
using TMPro; // ← 이거 꼭 필요함!

public class WaitingManager : MonoBehaviour
{
    public TMP_Text playerCountText;
    private int playerCount = 1;
    private int maxPlayer = 6;

    private void Start()
    {
        // 핸들러 등록
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.PlayerCountChanged>(OnPlayerCountChanged);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.LeaveRoom>(OnLeaveRoom);
        // 최초 입장 시 방 정보 요청 (예시)
        var req = new RequestPacketData.GetRoomInfo(PlayerSession.ClientId);
        NetworkManager.Instance.Send(req);
    }

    private void OnDestroy()
    {
        // 필요하다면 핸들러 해제 로직 추가 (UnregisterHandler 구현 시)
    }

    private void OnLeaveRoom(ResponsePacketData.LeaveRoom data)
    {
        if (data.success)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }

    public void OnClickBackButton()
    {
        // 방 나가기 요청
        var req = new RequestPacketData.LeaveRoom(PlayerSession.ClientId);
        NetworkManager.Instance.Send(req);
        // 이제는 응답(성공) 후에만 씬 이동
    }

    public void UpdatePlayerCount(int count, int max = 6)
    {
        playerCount = count;
        maxPlayer = max;
        playerCountText.text = $"Players: {playerCount}/{maxPlayer}";
    }

    private void OnPlayerCountChanged(ResponsePacketData.PlayerCountChanged data)
    {
        UpdatePlayerCount(data.participantCount, data.maxPlayer);
    }
}

