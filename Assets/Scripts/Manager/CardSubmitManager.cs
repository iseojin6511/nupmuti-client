using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // DOTween 필요

public class CardSubmitManager : MonoBehaviour
{
    public Transform handArea;
    public Transform centerPile;
    public float cardSpacing = 80f;

    public HandManager handManager;

    private List<GameObject> submittedCards = new List<GameObject>();

    public void OnSubmit()
    {
        List<GameObject> selectedCards = new List<GameObject>();

        foreach (Transform cardTransform in handArea)
        {
            CardUI cardUI = cardTransform.GetComponent<CardUI>();
            if (cardUI != null && cardUI.IsSelected())
            {
                selectedCards.Add(cardTransform.gameObject);
            }
        }

        float baseOffset = -(selectedCards.Count - 1) * cardSpacing * 0.5f;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            GameObject card = selectedCards[i];
            CardUI cardUI = card.GetComponent<CardUI>();
            cardUI.Deselect();
            cardUI.DisableInteraction(); // 선택 불가

            card.transform.SetParent(centerPile);

            RectTransform rt = card.GetComponent<RectTransform>();

            // DOTween으로 애니메이션 이동 & 회전 & 스케일
            rt.DOAnchorPos(new Vector2(baseOffset + i * cardSpacing, 0), 0.4f).SetEase(Ease.OutCubic);
            rt.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
            rt.DOScale(Vector3.one * 0.9f, 0.4f).SetEase(Ease.OutBack); // 살짝 줄이기

            submittedCards.Add(card);
        }

        handManager.RearrangeHand();
    }


    // 호출 시: 제출된 카드 페이드아웃 후 삭제
    public void ClearCenterPile()
    {
        foreach (GameObject card in submittedCards)
        {
            CanvasGroup cg = card.GetComponent<CanvasGroup>();
            if (cg == null) cg = card.AddComponent<CanvasGroup>();

            cg.DOFade(0, 0.5f).OnComplete(() => Destroy(card));
        }

        submittedCards.Clear();
    }
}
