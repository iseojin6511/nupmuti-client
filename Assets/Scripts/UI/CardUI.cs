using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int cardValue;
    public TextMeshProUGUI valueText;
    public Image background;

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
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isSelected = !isSelected;

        // ✅ 테두리 색상만 바꿈
        if (outline != null)
            outline.effectColor = isSelected ? selectedOutlineColor : originalOutlineColor;

        // ✅ 카드 위치 이동
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y += isSelected ? selectedYOffset : -selectedYOffset;
        rectTransform.anchoredPosition = pos;
    }

    public bool IsSelected() => isSelected;

    public void Deselect()
    {
        if (isSelected)
        {
            isSelected = false;

            if (outline != null)
                outline.effectColor = originalOutlineColor;

            Vector2 pos = rectTransform.anchoredPosition;
            pos.y -= selectedYOffset;
            rectTransform.anchoredPosition = pos;
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
