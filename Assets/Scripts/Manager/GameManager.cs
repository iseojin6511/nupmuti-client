using System.Collections.Generic;
using System.Collections;
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

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();

    public UnityEngine.UI.Button submitButton;
    public UnityEngine.UI.Button passButton;
    public PlayerRankingUI rankingUI;
    public CardShuffler cardShuffler;
    public PlayerActionUI playerActionUI;
    [SerializeField] private GameObject cardPrefab;

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

        
        var nicknames = new[] { "PlayerA", "PlayerB", "PlayerC", "PlayerD", "PlayerE", "PlayerF" };
        var ranks = new[] { 1, 3, 2, 6, 5, 4 };
        System.Random rand = new System.Random();

        for (int i = 0; i < nicknames.Length; i++)
        {
            var cardValues = new List<int>();
            for (int j = 0; j < 10; j++)  // 각 플레이어에게 카드 10장
            {
                cardValues.Add(rand.Next(1, 11)); // 1~10 사이의 랜덤 값
            }

            playerInfos.Add(new PlayerInfo
            {
                nickname = nicknames[i],
                rank = ranks[i],
                profileImage = profileImages[i],
                cardValues = cardValues
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


    void TestOpponentSubmit()
    {
        string opponentId = "PlayerB";
        Debug.Log("SUBMIT");
        int[] values = new int[] { 2, 2 };
        var dummyCards = CreateDummyCards(2, values); //카드 2장 내는 예시
        OnOpponentSubmit(opponentId, dummyCards);
    }
 
    List<GameObject> CreateDummyCards(int count, int[] cardValues)
    {
        List<GameObject> cards = new();

        for (int i = 0; i < count; i++)
        {
            GameObject card = Instantiate(cardPrefab);  // 프리팹 생성
            card.transform.SetParent(transform);        // 임시 부모 (나중에 다시 옮겨도 됨)

            // 카드 숫자 설정
            CardUI cardUI = card.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCard(cardValues[i]);
            }

            cards.Add(card);
        }

        return cards;
    }

    public void OnClickSubmit()
    {
        if (!IsMyTurn()) return;

        passedPlayers.Clear();

        List<int> submittedValues = submitManager.OnSubmit();

        PlayerInfo myInfo = playerInfos.Find(p => p.nickname == myPlayerId);

        NextTurn();
    }

    public void OnClickPass()
    {
        if (!IsMyTurn()) return;

        passedPlayers.Add(myPlayerId);

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

        if (passedPlayers.Count == playerIds.Count - 1)
        {
            StartCoroutine(HandleAllPassedThenContinue());
            return; // 흐름 중단 (Turn 메시지 띄우지 않음)
        }

        ProceedTurnUI();
    }

    private void ProceedTurnUI()
    {
        bool isMyTurn = IsMyTurn();
        submitButton.interactable = isMyTurn;
        passButton.interactable = isMyTurn;

        string currentPlayerId = playerIds[currentTurnIndex];
        playerActionUI.ShowMessage($"'{currentPlayerId}' Turn!");
        rankingUI.HighlightCurrentPlayer(currentPlayerId);

        if (currentPlayerId == "PlayerB")
        {
            Invoke(nameof(TestOpponentSubmit), 3f);
        }
        else if (currentPlayerId == "PlayerC")
        {
            StartCoroutine(DelayedOpponentPass("PlayerC", 3f));
        }
        else if (currentPlayerId == "PlayerD")
        {
            StartCoroutine(DelayedOpponentPass("PlayerD", 3f));
        }
        else if (currentPlayerId == "PlayerE")
        {
            StartCoroutine(DelayedOpponentPass("PlayerE", 3f));
        }
        else if (currentPlayerId == "PlayerF")
        {
            StartCoroutine(DelayedOpponentPass("PlayerF", 3f));
        }
    }


    private IEnumerator HandleAllPassedThenContinue()
{
    Debug.Log("All players passed. Clearing center pile.");
    playerActionUI.ShowMessage("All Player Passed");

    yield return new WaitForSeconds(1.5f); // 메시지를 보여줄 시간

    submitManager.ClearCenterPile();

    yield return new WaitForSeconds(1.0f); // 카드 정리 애니메이션 시간

    passedPlayers.Clear();

    // 이후 정상적인 턴 진행
    ProceedTurnUI();
}

    private IEnumerator DelayedOpponentPass(string playerId, float delay)
    {
        yield return new WaitForSeconds(delay);
        OnOpponentPass(playerId);
    }

    // 🟡 다른 플레이어가 Submit 했을 때 실행
    public void OnOpponentSubmit(string playerId, List<GameObject> cards)
    {
        Debug.Log($"{playerId} has submitted cards.");

        // 상대방 프로필에서 카드가 센터필로 이동하는 연출
        playerActionUI.PlayCardFromPlayer(playerId, cards);
        passedPlayers.Clear();
        NextTurn();
    }

    // 🟡 다른 플레이어가 Pass 했을 때 실행
    public void OnOpponentPass(string playerId)
    {
        Debug.Log($"{playerId} has passed.");
        StartCoroutine(HandleOpponentPass(playerId));
    }

    private IEnumerator HandleOpponentPass(string playerId)
    {
        // 메시지 먼저 표시
        playerActionUI.ShowMessage($"'{playerId}' Passed");

        // 패스 등록
        passedPlayers.Add(playerId);

        // 1초 대기
        yield return new WaitForSeconds(1.5f);

        // 다음 턴으로
        NextTurn();
    }
    
    private IEnumerator DelayedSequence()
    {
        Debug.Log("[DelayedSequence] 시작됨 (단순 대기)");
        yield return new WaitForSeconds(1.5f);  // 원하는 대기 시간 설정
        Debug.Log("[DelayedSequence] 대기 완료");
    }

    // 🟡 모두가 패스했을 경우 (턴 초기화)
    public void OnAllPassed()
    {
        StartCoroutine(HandleAllPassedSequence());
    }

    private IEnumerator HandleAllPassedSequence()
    {
        Debug.Log("All players passed. Clearing center pile.");
        playerActionUI.ShowMessage("All players passed. Clearing center pile.");

        yield return new WaitForSeconds(1.5f);  // 메시지 표시 시간

        submitManager.ClearCenterPile();

        yield return new WaitForSeconds(1.0f);  // 연출 후 약간 대기

        passedPlayers.Clear();

        // 턴 넘기지 않음! → UpdateTurnUI 흐름 유지
    }
}
