using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
        string[] adjectives = { "Fast", "Cool", "Smart", "Happy", "Dark", "Tiny" };
        string[] nouns = { "Tiger", "Fox", "Panda", "Wolf", "Bear", "Cat" };
        int number = UnityEngine.Random.Range(0, 999);
        return $"{adjectives[UnityEngine.Random.Range(0, adjectives.Length)]}" +
               $"{nouns[UnityEngine.Random.Range(0, nouns.Length)]}{number:D3}";
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

        string json = $"{{\"event\": \"joinGame\", \"nickname\": \"{nickname}\"}}";
        socketManager.SendMessageToServer(json);
        statusText.text = $"👤 {nickname} entering...";

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Waiting");
    }
}
