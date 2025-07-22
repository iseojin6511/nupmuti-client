using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartManager : MonoBehaviour
{
    public TMP_Text gameStartText; // 인스펙터에서 연결
    public TMP_Text messageText;
    public GameObject messagePanel;
    public float messageDuration = 2f; 
    public CardSpawner cardSpawner;
    public Transform HandArea;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.StartRound>(OnStartRound);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.FirstRoundRules>(OnFirstRoundRules);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.YourCard>(OnYourCard);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.YourRank>(OnYourRank);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.YourOrder>(OnYourOrder);
    }

    private void OnStartRound(ResponsePacketData.StartRound data)
    {
        Debug.Log("StartRound: " + data.message);
        if (gameStartText != null)
        {
            // 1초 동안 알파값을 0으로 페이드아웃
            gameStartText.DOFade(0f, 1f);
        }
    }

    private void OnFirstRoundRules(ResponsePacketData.FirstRoundRules data)
    {
        Debug.Log("FirstRoundRules: " + data.message);
        ShowMessage(data.message);
    }

    private void OnYourCard(ResponsePacketData.YourCard data)
    {
        Debug.Log("MyNUM", data.cardNum);
        // 1. 뒷면 카드 생성 (cardPrefab은 뒷면 프리팹이어야 함)
        GameObject backCard = Instantiate(cardSpawner.cardBackPrefab, HandArea);
        RectTransform backRT = backCard.GetComponent<RectTransform>();
        backRT.localScale = Vector3.one;
        backRT.anchoredPosition = Vector2.zero;
        backRT.localRotation = Quaternion.identity;

        // 2. 0.5초 후에 뒤집기 연출 시작
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        // 3. 뒷면 Y축 0 → 90도 (절반 뒤집기)
        seq.Append(backRT.DOLocalRotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InCubic));
        // 4. 뒷면 제거 & 앞면 카드 생성
        seq.AppendCallback(() =>
        {
            Destroy(backCard);
            // 앞면 카드 생성 (cardSpawner.cardFrontPrefab이 앞면 프리팹이어야 함)
            GameObject frontCard = Instantiate(cardSpawner.cardFrontPrefab, HandArea);
            RectTransform frontRT = frontCard.GetComponent<RectTransform>();
            frontRT.localScale = Vector3.one;
            frontRT.anchoredPosition = Vector2.zero;
            frontRT.localRotation = Quaternion.Euler(0, -90, 0); // 반대편에서 시작
            CardUI cardUI = frontCard.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCard(data.cardNum);
            }
            // 5. 앞면 Y축 -90 → 0도 (완전히 뒤집힘)
            frontRT.DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
        });
    }

    private void OnYourRank(ResponsePacketData.YourRank data)
    {
        Debug.Log("YourRank: " + data.rank);
        ShowMessage(data.message);
    }

    private void OnYourOrder(ResponsePacketData.YourOrder data)
    {
        Debug.Log("YourOrder: " + data.order);
        ShowMessage(data.message);
        var req = new RequestPacketData.RoundStarted(PlayerSession.ClientId);
        NetworkManager.Instance.Send(req);
        SceneManager.LoadScene("MainGame");
    }
    public void ShowMessage(string message)
    {
        messageText.text = $"{message}";
        messagePanel.SetActive(true); // 패널 표시

        CancelInvoke(nameof(HideMessage)); // 기존 타이머 제거 (겹치는 경우 방지)
        Invoke(nameof(HideMessage), messageDuration); // 일정 시간 뒤 숨기기
    }

    private void HideMessage()
    {
        messagePanel.SetActive(false);
        messageText.text = "";
    }

}
