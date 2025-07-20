using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public List<string> playerIds;
    public int currentTurnIndex;
    public string myPlayerId;
    private HashSet<string> passedPlayers = new();
    public CardSpawner cardSpawner; 
    public CardSubmitManager submitManager;
    public Sprite[] profileImages;

    public UnityEngine.UI.Button submitButton;
    public UnityEngine.UI.Button passButton;
    public PlayerRankingUI rankingUI;
    public CardShuffler cardShuffler; 

    private void Start()
    {
        if (rankingUI == null)
        {
            Debug.LogError("rankingUI is not assigned in the Inspector!");
            return;
        }

        if (cardSpawner == null)
        {
            Debug.LogError("cardSpawner is not assigned in the Inspector!");
            return;
        }

        myPlayerId = "PlayerA";

        var playerInfos = new List<PlayerInfo>();
        var nicknames = new[] { "PlayerA", "PlayerB", "PlayerC", "PlayerD", "PlayerE", "PlayerF" };
        var ranks = new[] { 1, 3, 2, 6, 5, 4 };
        var cards = new[] { 5, 3, 2, 5, 3, 2 };

        for (int i = 0; i < nicknames.Length; i++)
        {
            playerInfos.Add(new PlayerInfo
            {
                nickname = nicknames[i],
                rank = ranks[i],
                cardsLeft = cards[i],
                profileImage = profileImages[i]
            });
        }

        playerInfos = playerInfos.OrderBy(p => p.rank).ToList();

        // 🟡 정렬된 순서대로 playerIds 갱신
        playerIds = playerInfos.Select(p => p.nickname).ToList();
        
        cardShuffler.playerIds = playerIds;



        rankingUI.ShowRankings(playerInfos, myPlayerId);

        cardSpawner.SetLocalPlayerId(myPlayerId);

        UpdateTurnUI();
    }

    public void OnClickSubmit()
    {
        if (!IsMyTurn()) return;

        submitManager.OnSubmit();
        NextTurn();
    }

    public void OnClickPass()
    {
        if (!IsMyTurn()) return;

        passedPlayers.Add(myPlayerId);

        if (passedPlayers.Count == playerIds.Count - 1)
        {
            submitManager.ClearCenterPile();
            passedPlayers.Clear();
        }

        NextTurn();
    }

    void NextTurn()
    {
        currentTurnIndex = (currentTurnIndex + 1) % playerIds.Count;
        UpdateTurnUI();
    }

    bool IsMyTurn()
    {
        if (playerIds == null || playerIds.Count == 0 || myPlayerId == null)
        {
            Debug.LogError("[GameManager] 플레이어 정보가 비어 있습니다.");
            return false;
        }

        return playerIds[currentTurnIndex] == myPlayerId;
    }

    void UpdateTurnUI()
    {
        if (submitButton == null || passButton == null) return;

        bool isMyTurn = IsMyTurn();
        submitButton.interactable = isMyTurn;
        passButton.interactable = isMyTurn;

        // 현재 턴 플레이어 하이라이트
        string currentPlayerId = playerIds[currentTurnIndex];
        rankingUI.HighlightCurrentPlayer(currentPlayerId);
    }
}
