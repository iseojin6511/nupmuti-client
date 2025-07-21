 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardShuffler : MonoBehaviour
{
    public GameObject cardBackPrefab;
    public int cardCount = 20; // 덱에 겹칠 카드 수
    public Transform pileCenter; // 중앙 위치 (덱 중심)
    public float pileOffset = 2f; // 카드 간 간격 (시각적 겹침 정도)
    public List<string> playerIds; 

    private List<RectTransform> cardBacks = new List<RectTransform>();
    public CardSpawner cardSpawner;

    void Start()
    {
        if (cardBackPrefab == null || pileCenter == null || cardSpawner == null)
        {
            Debug.LogError("CardShuffler: 필드가 비어 있습니다. 연결을 확인하세요.");
            return;
        }

        CreateCardPile();
        StartCoroutine(ShufflePile());
    }

    public void CreateCardPile()
    {
        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = Instantiate(cardBackPrefab, pileCenter);
            RectTransform rt = card.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = new Vector2(0, -i * pileOffset);
            rt.localRotation = Quaternion.identity;

            cardBacks.Add(rt);
        }
        StartCoroutine(ShufflePile());
    }

    IEnumerator ShufflePile()
    {
        int shuffleLoops = 6;

        for (int loop = 0; loop < shuffleLoops; loop++)
        {
            // 카드 흔들기
            foreach (var card in cardBacks)
            {
                Vector2 offset = new Vector2(Random.Range(-10f, 10f), Random.Range(-5f, 5f));
                float rotationZ = Random.Range(-10f, 10f);

                Sequence shake = DOTween.Sequence();
                shake.Append(card.DOAnchorPos(card.anchoredPosition + offset, 0.1f).SetEase(Ease.OutQuad));
                shake.Join(card.DOLocalRotate(new Vector3(0, 0, rotationZ), 0.1f).SetEase(Ease.OutQuad));
            }

            yield return new WaitForSeconds(0.15f);

            // 카드 원위치 복귀
            foreach (var card in cardBacks)
            {
                Sequence reset = DOTween.Sequence();
                reset.Append(card.DOAnchorPos(new Vector2(0, card.anchoredPosition.y), 0.1f).SetEase(Ease.InQuad));
                reset.Join(card.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad));
            }

            yield return new WaitForSeconds(0.15f);
        }

        // 카드 제거
        foreach (var card in cardBacks)
        {
            if (card != null)
            {
                Destroy(card.gameObject);
            }
        }

        cardBacks.Clear();

        // 카드 분배 시작
        cardSpawner.StartDealing(playerIds);
    }
}
