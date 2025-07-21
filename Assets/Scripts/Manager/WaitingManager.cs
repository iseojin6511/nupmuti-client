using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환용
using TMPro; // ← 이거 꼭 필요함!

public class WaitingManager : MonoBehaviour
{
    public TMP_Text playerCountText;
    public UnityEngine.UI.Button startGameButton;
    private int playerCount = 1;
    private int maxPlayer = 6;
    
    private void Start()
    {
        // 핸들러 등록
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.PlayerCountChanged>(OnPlayerCountChanged);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.LeaveRoom>(OnLeaveRoom);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.YouAreHost>(OnYouAreHost);
        startGameButton.gameObject.SetActive(false); // 기본적으로 숨김
        // 최초 입장 시 방 정보 요청 (예시)
        var req = new RequestPacketData.GetRoomInfo(PlayerSession.ClientId);
        NetworkManager.Instance.Send(req);
    }

    private void OnDestroy()
    {
        // 필요하다면 핸들러 해제 로직 추가 (UnregisterHandler 구현 시)
    }

    /**
    * <방장 여부 확인>
    */

    private void OnYouAreHost(ResponsePacketData.YouAreHost data)
    {
        if (data.isHost) {
            startGameButton.gameObject.SetActive(true); // 방장만 버튼 보임
        }
    }

    /**
    * <서버에서 방 나가기 응답 처리, 성공하면 로비로 이동>
    */
    private void OnLeaveRoom(ResponsePacketData.LeaveRoom data)
    {
        if (data.success)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }

    /**
    * <방 나가기 버튼 클릭 시 서버로 LEAVE_ROOM 요청>
    */
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
    /**
    * <플레이어 수 업데이트>
    */
    private void OnPlayerCountChanged(ResponsePacketData.PlayerCountChanged data)
    {
        UpdatePlayerCount(data.participantCount, data.maxPlayer);
    }

    public void OnClickStartGameButton()
    {
        // 게임 시작 요청 패킷 전송
        NetworkManager.Instance.Send(new RequestPacketData.StartGame());
    }


}

