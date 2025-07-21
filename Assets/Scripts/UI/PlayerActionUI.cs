using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PlayerActionUI : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform centerPile;
    public PlayerRankingUI rankingUI;
    public GameObject passTextPrefab;

    public TMP_Text messageText; // message 내부의 TMP_Text 연결
    public GameObject messagePanel;  // 메시지를 띄울 Canvas
    public float messageDuration = 2f; 

    public void PlayCardFromPlayer(string playerId, List<GameObject> cardObjects)
    {
        GameObject playerUI = rankingUI.FindPlayerUIById(playerId);
        if (playerUI == null) return;

        Debug.Log($"[PlayCardFromPlayer] 카드 개수: {cardObjects.Count}");

        Transform sourceImage = playerUI.transform.Find("Image/Image");
        if (sourceImage == null)
        {
            Debug.LogWarning($"[{playerId}] 프로필 이미지 위치를 찾을 수 없습니다.");
            return;
        }

        for (int i = 0; i < cardObjects.Count; i++)
        {
            GameObject card = cardObjects[i];
            card.transform.SetParent(centerPile.parent); // 먼저 parent 설정
            card.transform.SetAsLastSibling();

            RectTransform cardRect = card.GetComponent<RectTransform>();
            cardRect.position = sourceImage.position;
            cardRect.rotation = Quaternion.identity;

            float offsetX = (i - (cardObjects.Count - 1) / 2f) * 60f;
            Vector3 targetPos = centerPile.position + new Vector3(offsetX, 0, 0);

            cardRect.DOMove(targetPos, 1.5f).SetEase(Ease.OutCubic);
            cardRect.DOScale(Vector3.one * 0.9f, 0.8f).SetEase(Ease.OutBack);

            card.transform.SetParent(centerPile); // 이동 후에 센터에 넣음
        }
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
