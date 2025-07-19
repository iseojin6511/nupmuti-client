using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform handArea;
    public List<int> cardValues = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; // 카드 값 목록

    public float radius = 350f;
public float angleRange = 140f; // 기본 각도 범위 (카드 많을 때 기준)
public float maxAngleRange = 100f; // 카드 적을 때 휘는 정도 제한

void Start()
{
    int count = cardValues.Count;

    // 실제 사용할 angleRange 계산
    float baseAngleRange = angleRange;
    if (count <= 5) // 카드 수가 적을 때만 제한 적용
    {
        baseAngleRange = Mathf.Min(angleRange, maxAngleRange);
    }

    float angleStep = baseAngleRange / (count - 1);
    float startAngle = -baseAngleRange / 2f;

    for (int i = 0; i < count; i++)
    {
        float angle = startAngle + i * angleStep;
        float rad = angle * Mathf.Deg2Rad;

        Vector2 pos = new Vector2(
            Mathf.Sin(rad) * radius,
            Mathf.Cos(rad) * radius - radius
        );

        GameObject card = Instantiate(cardPrefab, handArea);
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.localRotation = Quaternion.Euler(0, 0, angle * -0.75f);

        card.GetComponent<CardUI>().SetCard(cardValues[i]);
    }
}
}
