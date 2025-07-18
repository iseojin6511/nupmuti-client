using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nicknameInput;
    public TMP_Text statusText;
    public Button startButton;

    private void Start()
    {
        // 버튼 리스너 등록
        startButton.onClick.AddListener(OnClickStart);

        // 서버 연결 시도
        if (SocketManagerInstance() != null)
        {
            SocketManagerInstance().Connect();
            statusText.text = "🔌 서버 연결 중...";
        }
        else
        {
            statusText.text = "❌ SocketManager 없음!";
        }
    }

    private void OnClickStart()
    {
        string nickname = nicknameInput.text.Trim();

        if (string.IsNullOrEmpty(nickname))
        {
            statusText.text = "⚠️ 닉네임을 입력하세요.";
            return;
        }

        // 서버에 닉네임 전송
        var json = $"{{\"event\": \"joinGame\", \"nickname\": \"{nickname}\"}}";
        SocketManagerInstance()?.SendSocketMessage(json);
        statusText.text = $"🚀 {nickname}님 입장 요청 전송 중...";
    }

    // SocketManager 싱글톤 인스턴스 반환
    private SocketManager SocketManagerInstance()
    {
        return FindAnyObjectByType<SocketManager>();
    }
}
