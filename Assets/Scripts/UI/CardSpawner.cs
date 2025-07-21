using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardBackPrefab;
    public GameObject cardFrontPrefab;
    public Transform deckPosition;

    public Transform myHandArea;
    public PlayerRankingUI playerRankingUI;

    private string localPlayerId;
    public List<int> cardValues;

    public float radius = 350f;
    public float angleRange = 140f;
    public float minAngleRange = 20f;
    public float curveHeight = 80f;
    public float offsetY = -100f;
    public float rotationFactor = 0.5f;

    public void SetLocalPlayerId(string playerId)
    {
        localPlayerId = playerId;
    }

    public void StartDealing(List<string> playerIds)
    {
        if (string.IsNullOrEmpty(localPlayerId))
        {
            Debug.LogError("localPlayerId is not set!");
            return;
        }

        StartCoroutine(DealCardsToPlayers(playerIds));
    }

    IEnumerator DealCardsToPlayers(List<string> playerIds)
    {
        int myCardIndex = 0;
        int myCardCount = cardValues.Count;
        int playerCount = playerIds.Count;


        while (myCardIndex < myCardCount)
        {
            for (int i = 0; i < playerCount; i++)
            {
                string playerId = playerIds[i];
                bool isLocalPlayer = playerId == localPlayerId;

                GameObject card = Instantiate(cardBackPrefab, deckPosition);
                RectTransform cardRT = card.GetComponent<RectTransform>();
                cardRT.localScale = Vector3.one;
                cardRT.position = deckPosition.position;

                if (isLocalPlayer)
                {
                    if (myCardIndex >= myCardCount)
                    {
                        Destroy(card);
                        continue;
                    }

                    // 🎯 아치형 위치 계산
                    float t = Mathf.InverseLerp(3, 10, myCardCount);
                    float angleR = Mathf.Lerp(minAngleRange, angleRange, t);
                    float angleStep = (myCardCount > 1) ? angleR / (myCardCount - 1) : 0;
                    float startAngle = -angleR / 2f;
                    float angle = startAngle + myCardIndex * angleStep;
                    float rad = angle * Mathf.Deg2Rad;

                    Vector2 finalPos = new Vector2(Mathf.Sin(rad) * radius, Mathf.Cos(rad) * curveHeight + offsetY);
                    Quaternion finalRot = Quaternion.Euler(0, 0, -angle * rotationFactor);

                    card.transform.SetParent(myHandArea);
                    RectTransform handRT = card.GetComponent<RectTransform>();
                    handRT.localScale = Vector3.one;
                    handRT.anchoredPosition = Vector2.zero;
                    handRT.localRotation = Quaternion.identity;

                    int cardValue = cardValues[myCardIndex];

                    Sequence seq = DOTween.Sequence();
                    seq.Append(handRT.DOAnchorPos(finalPos * 0.5f, 0.2f));
                    seq.Join(handRT.DOLocalRotate(new Vector3(0, 90, 0), 0.2f));

                    seq.AppendCallback(() =>
                    {
                        Destroy(card);
                        GameObject frontCard = Instantiate(cardFrontPrefab, myHandArea);
                        RectTransform frontRT = frontCard.GetComponent<RectTransform>();
                        frontRT.localScale = Vector3.one;
                        frontRT.anchoredPosition = finalPos * 0.5f;
                        frontRT.localRotation = Quaternion.Euler(0, -90, 0);
                        frontCard.GetComponent<CardUI>().SetCard(cardValue);

                        Sequence flipSeq = DOTween.Sequence();
                        flipSeq.Append(frontRT.DOAnchorPos(finalPos, 0.2f));
                        flipSeq.Join(frontRT.DOLocalRotateQuaternion(finalRot, 0.2f));
                    });

                    yield return seq.WaitForCompletion();
                    myCardIndex++;
                }
                else
                {
                    // 🎭 연출만
                    GameObject targetUI = playerRankingUI.FindPlayerUIById(playerId);
                    Transform targetTransform = targetUI?.transform.Find("Image/Image");

                    if (targetTransform == null)
                    {
                        Destroy(card);
                        continue;
                    }

                    card.transform.SetParent(targetTransform.root);
                    Vector3 targetPos = targetTransform.position;

                    Sequence seq = DOTween.Sequence();
                    seq.Append(cardRT.DOMove(targetPos, 0.8f));
                    seq.Join(cardRT.DOScale(Vector3.zero, 0.8f));
                    seq.AppendCallback(() => Destroy(card));
                }

                yield return new WaitForSeconds(0.1f);

                if (myCardIndex >= myCardCount)
                    break;
            }
        }
    }
}
