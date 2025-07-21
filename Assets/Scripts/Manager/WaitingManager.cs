using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환용
using TMPro; // ← 이거 꼭 필요함!

public class WaitingManager : MonoBehaviour
{
    public TMP_Text playerCountText;
    private int playerCount = 1;
    private void Start()
    {
        UpdatePlayerCount(1);
    }
    

    public void OnClickBackButton()
    {
        SceneManager.LoadScene("LobbyScene"); // 또는 원하는 씬 이름
    }

    public void UpdatePlayerCount(int count)
    {
        playerCount = count;
        playerCountText.text = $"Players: {playerCount}/6";
    }
}

