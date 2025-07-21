using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;         // DOTween 애니메이션용

public class CardSubmitManager : MonoBehaviour
{
    public Transform handArea;
    public Transform centerPile;
    public float cardSpacing = 80f;
    public HandManager handManager;

    private List<GameObject> submittedCards = new();

    public List<int> OnSubmit()
    {
        var selectedCards = new List<GameObject>();
        var submittedValues = new List<int>();

        foreach (Transform cardTransform in handArea)
        {
            var cardUI = cardTransform.GetComponent<CardUI>();
            if (cardUI != null && cardUI.IsSelected())
                selectedCards.Add(cardTransform.gameObject);
            submittedValues.Add(cardUI.cardValue);
        }

        float baseOffset = -(selectedCards.Count - 1) * cardSpacing * 0.5f;

        for (int i = 0; i < selectedCards.Count; i++)
        {
            GameObject card = selectedCards[i];
            var cardUI = card.GetComponent<CardUI>();
            cardUI.Deselect();
            cardUI.DisableInteraction();

            card.transform.SetParent(centerPile);
            RectTransform rt = card.GetComponent<RectTransform>();

            rt.DOAnchorPos(new Vector2(baseOffset + i * cardSpacing, 0), 0.4f).SetEase(Ease.OutCubic);
            rt.DOLocalRotate(Vector3.zero, 0.4f).SetEase(Ease.OutCubic);
            rt.DOScale(Vector3.one * 0.9f, 0.4f).SetEase(Ease.OutBack);

            submittedCards.Add(card);
        }

        handManager.RearrangeHand();
        return submittedValues;
    }

    public void ClearCenterPile()
    {
        // centerPile 아래 모든 자식 오브젝트 제거
        foreach (Transform child in centerPile)
        {
            CanvasGroup cg = child.GetComponent<CanvasGroup>();
            if (cg == null) cg = child.gameObject.AddComponent<CanvasGroup>();

            cg.DOFade(0, 0.5f).OnComplete(() =>
            {
                Destroy(child.gameObject);
            });
        }
    }

}
