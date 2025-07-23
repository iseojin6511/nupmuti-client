using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int cardValue;
    public TextMeshProUGUI valueText;
    public Sprite[] cardSprites;

    private Outline outline;
    private bool isSelected = false;

    private RectTransform rectTransform;
    private float selectedYOffset = 30f;

    // ✅ 기존 Outline 색상 저장용
    private Color originalOutlineColor;
    public Color selectedOutlineColor = Color.yellow; // Inspector에서 바꿔도 되게 public

    private bool isInteractable = true;

    private Vector3 originalScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        outline = GetComponent<Outline>();
        originalScale = transform.localScale;

        if (outline != null)
            originalOutlineColor = outline.effectColor;
    }

    public void SetCard(int value)
    {
        cardValue = value;
        valueText.text = value.ToString();
        if (cardSprites != null && value > 0 && value < cardSprites.Length+1)
        {   
            // 카드 프리팹의 SourceImage(Sprite) 변경
            var image = GetComponent<Image>();
            if (image != null)
                image.sprite = cardSprites[value - 1];
        }
        else
        {
            // 예외 처리: 값이 범위 밖이면 기본 이미지 유지
            Debug.LogWarning($"CardUI: 카드 값 {value}에 해당하는 이미지가 없습니다.");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;

        // ✅ 테두리 색상만 바꿈
        if (outline != null)
            outline.effectColor = isSelected ? selectedOutlineColor : originalOutlineColor;

        // ✅ 카드 선택 시 scale을 키우고, 해제 시 원래대로
        if (isSelected)
            transform.localScale = originalScale * 1.15f;
        else
            transform.localScale = originalScale;
    }

    public bool IsSelected() => isSelected;

    public void Deselect()
    {
        if (isSelected)
        {
            isSelected = false;

            if (outline != null)
                outline.effectColor = originalOutlineColor;
            
            // 선택 해제 시 카드 크기를 원래대로 복원
            transform.localScale = originalScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스 올렸을 때 살짝 확대
        transform.localScale = originalScale * 1.1f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스 내렸을 때 원래 크기 복원
        transform.localScale = originalScale;
    }


    public void DisableInteraction()
    {
        isInteractable = false;
    }
}
