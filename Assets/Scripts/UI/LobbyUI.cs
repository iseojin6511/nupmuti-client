using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class LobbyUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField nicknameInput;
    public TMP_Text statusText;
    public Button startButton;

    private SocketManager socketManager;

    private void Start()
    {
        socketManager = FindAnyObjectByType<SocketManager>();
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.CreateRoom>(OnCreateRoom);
        

        nicknameInput.text = GenerateRandomNickname();

        startButton.interactable = false;
        startButton.onClick.AddListener(() => StartCoroutine(OnClickStart()));

        if (socketManager != null)
        {
            statusText.text = "Connecting to server...";
            socketManager.Connect();
            StartCoroutine(WaitForConnection());
        }
        else
        {
            statusText.text = "SocketManager not found!";
        }
    }

    private string GenerateRandomNickname()
    {
        string[] adjectives = { "빠른", "멋진", "똑똑한", "행복한", "어두운", "작은" };
        string[] nouns = { "호랑이", "여우", "판다", "늑대", "곰", "고양이" };
        int number = UnityEngine.Random.Range(0, 999);
        return $"{adjectives[UnityEngine.Random.Range(0, adjectives.Length)]}" +
               $"{nouns[UnityEngine.Random.Range(0, nouns.Length)]}{number:D3}";
    }

    private void OnCreateRoom(ResponsePacketData.CreateRoom data)
    {
        if (data.success)
        {   
            Debug.Log("CreateRoom success");
            SceneManager.LoadScene("Waiting");
        }else{
            Debug.Log("CreateRoom failed");
            string nickname = nicknameInput.text.Trim();
            var req = new RequestPacketData.EnterRoom(nickname);
            NetworkManager.Instance.Send(req);
        }
    }

    private IEnumerator WaitForConnection()
    {
        float timeout = 5f;
        float elapsed = 0f;

        while (elapsed < timeout)
        {
            if (socketManager != null && socketManager.IsConnected)
            {
                statusText.text = "Connected!";
                startButton.interactable = true;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        statusText.text = "Connection failed.";
        startButton.interactable = false;
    }

   private IEnumerator OnClickStart()
    {
        string nickname = nicknameInput.text.Trim();

        if (string.IsNullOrEmpty(nickname))
        {
            statusText.text = "Please enter a nickname!";
            yield break;
        }

        if (!socketManager.IsConnected)
        {
            statusText.text = "Server not connected.";
            yield break;
        }

        // 1. clientId 생성 및 저장 (최초 한 번만)
        if (string.IsNullOrEmpty(PlayerSession.ClientId))
        {
            PlayerSession.ClientId = Guid.NewGuid().ToString();
        }

        // 2. 서버에 입장 요청 (nickname 함께 전송)
        var req = new RequestPacketData.CreateRoom(nickname);
        NetworkManager.Instance.Send(req);

        statusText.text = $"👤 {nickname} entering...";
    }
}
