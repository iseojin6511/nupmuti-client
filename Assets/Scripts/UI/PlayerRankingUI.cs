using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerRankingUI : MonoBehaviour
{
    public GameObject playerProfilePrefab;
    public Transform playerListPanel;
    public Sprite[] profileSprites;

    private List<GameObject> playerItems = new();

    public void ShowRankings(List<PlayerInfo> allPlayers)
    {
        Debug.Log($"[ShowRankings] Total players: {allPlayers.Count}");

        foreach (Transform child in playerListPanel)
        {
            Destroy(child.gameObject);
        }
        playerItems.Clear();

        for (int i = 0; i < allPlayers.Count; i++)
        {
            //Debug.Log($"[ShowRankings] Player {i}: {allPlayers[i].nickname}");
            var p = allPlayers[i];

            GameObject item = Instantiate(playerProfilePrefab, playerListPanel, false);
            playerItems.Add(item);

            // 닉네임 설정
            TMP_Text nameText = item.transform.Find("InfoGroup/NicknameText")?.GetComponent<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = p.nickname;
            }

            // 카드 수 텍스트 (ScoreGroup 안이라면 여기에 추가 필요)
            TMP_Text scoreText = item.transform.Find("InfoGroup/ScoreGroup/CardLeft")?.GetComponent<TMP_Text>();
            if (scoreText != null)
            {
                scoreText.text = $"x{p.cardsLeft}";
            }

            var profileImage = item.transform.Find("Image/Image/ProfileImage")?.GetComponent<Image>();
            if (profileImage != null)
            {
                if (p.profileImage != null)
                {
                    profileImage.sprite = p.profileImage;
                }
                else
                if (i < profileSprites.Length && profileSprites[i] != null)
                {
                    profileImage.sprite = profileSprites[i];
                }
                else
                {
                    Debug.LogWarning($"[PlayerRankingUI] Inspector profileSprites[{i}]가 비어있거나 null입니다.");
                }

                // Outline 설정
                if (profileImage.GetComponent<Outline>() == null)
                {
                    var outline = profileImage.gameObject.AddComponent<Outline>();
                    outline.effectColor = Color.clear;
                    outline.effectDistance = new Vector2(3f, -3f);
                }
            }
        }
    }
    public void HighlightCurrentPlayer(string currentPlayerId)
    {
        foreach (var item in playerItems)
        {
            var nicknameText = item.transform.Find("InfoGroup/NicknameText")?.GetComponent<TMP_Text>();
            if (nicknameText == null) continue;

            bool isCurrent = nicknameText.text == currentPlayerId;
            HighlightPlayer(item.transform, isCurrent);
        }
    }

    public void HighlightPlayer(Transform playerItem, bool isActive)
    {
        var profileImage = playerItem.transform.Find("Image/Image/ProfileImage")?.GetComponent<Image>();
        var outline = profileImage?.GetComponent<Outline>();

        if (profileImage != null)
        {
            profileImage.transform.localScale = isActive ? Vector3.one * 1.3f : Vector3.one;
        }
        var nicknameText = playerItem.transform.Find("InfoGroup/NicknameText")?.GetComponent<TMPro.TMP_Text>();
        if (nicknameText != null)
        {
            nicknameText.color = isActive ? Color.yellow : Color.white;
            nicknameText.fontSize = isActive ? 40 : 32; // 폰트 크기 조정 (원하는 값으로 조정 가능)
        }
    }
    public GameObject FindPlayerUIById(string playerId)
    {
        foreach (var item in playerItems)
        {
            var nameText = item.transform.Find("InfoGroup/NicknameText")?.GetComponent<TMP_Text>();
            if (nameText != null && nameText.text == playerId)
                return item;
        }
        return null;
    }

    public void UpdatePlayerCardsLeft(string playerId, int cardsLeft)
    {
        foreach (var item in playerItems)
        {
            var nameText = item.transform.Find("InfoGroup/NicknameText")?.GetComponent<TMP_Text>();
            if (nameText != null && nameText.text == playerId)
            {
                var scoreText = item.transform.Find("InfoGroup/ScoreGroup/CardLeft")?.GetComponent<TMP_Text>();
                if (scoreText != null)
                {
                    scoreText.text = $"x{cardsLeft}";
                }
                break;
            }
        }
    }

    private Sprite GetRandomProfileSprite()
    {
        int randomIdx = Random.Range(1, 7); // Profile1 ~ Profile6
        Sprite randomSprite = Resources.Load<Sprite>($"Profile{randomIdx}");
        if (randomSprite == null)
        {
            Debug.LogWarning($"[PlayerRankingUI] Profile{randomIdx} 이미지를 찾을 수 없습니다.");
        }
        return randomSprite;
    }
}


