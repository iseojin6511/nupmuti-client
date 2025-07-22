using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement; 

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
        // 1. 뒷면 카드 생성 (시작 위치는 HandArea 위쪽)
        GameObject backCard = Instantiate(cardSpawner.cardBackPrefab, HandArea);
        RectTransform backRT = backCard.GetComponent<RectTransform>();
        backRT.localScale = Vector3.one;
        backRT.anchoredPosition = new Vector2(0, 500); // 위에서 시작 (500은 예시, 필요시 조정)
        backRT.localRotation = Quaternion.identity;

        // 2. HandArea 위치로 이동
        Sequence seq = DOTween.Sequence();
        seq.Append(backRT.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutCubic)); // 0.5초 동안 이동

        // 3. 도착 후 0.5초 대기
        seq.AppendInterval(0.5f);

        // 4. 뒷면 Y축 0 → 90도 (절반 뒤집기)
        seq.Append(backRT.DOLocalRotate(new Vector3(0, 90, 0), 0.3f).SetEase(Ease.InCubic));

        // 5. 뒷면 제거 & 앞면 카드 생성
        seq.AppendCallback(() =>
        {
            Destroy(backCard);
            GameObject frontCard = Instantiate(cardSpawner.cardFrontPrefab, HandArea);
            RectTransform frontRT = frontCard.GetComponent<RectTransform>();
            frontRT.localScale = Vector3.one;
            frontRT.anchoredPosition = Vector2.zero;
            frontRT.localRotation = Quaternion.Euler(0, -90, 0); // 반대편에서 시작
            CardUI cardUI = frontCard.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCard(data.cardNumber);
            }
            // 6. 앞면 Y축 -90 → 0도 (완전히 뒤집힘)
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
