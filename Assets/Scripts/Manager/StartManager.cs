using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartManager : MonoBehaviour
{
    public TMP_Text gameStartText; // 인스펙터에서 연결
    public CardSpawner cardSpawner;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.FirstRoundRules>(OnFirstRoundRules);
        NetworkManager.Instance.RegisterHandler<ResponsePacketData.YourRank>(OnYourRank);
    }

    private void OnFirstRoundRules(ResponsePacketData.FirstRoundRules data)
    {
        if (gameStartText != null)
        {
            // 1초 동안 알파값을 0으로 페이드아웃
            gameStartText.DOFade(0f, 1f);
        }
    }

    private void OnYourRank(ResponsePacketData.YourRank data)
    {
        // data.number가 카드 값이라고 가정
        if (cardSpawner != null && cardSpawner.cardFrontPrefab != null && cardSpawner.myHandArea != null)
        {
            GameObject card = Instantiate(cardSpawner.cardFrontPrefab, cardSpawner.myHandArea);
            RectTransform cardRT = card.GetComponent<RectTransform>();
            cardRT.localScale = Vector3.one;
            cardRT.anchoredPosition = Vector2.zero;
            cardRT.localRotation = Quaternion.identity;
            CardUI cardUI = card.GetComponent<CardUI>();
            if (cardUI != null)
            {
                cardUI.SetCard(data.rank); // number가 카드 값
            }

        }

    }
}
