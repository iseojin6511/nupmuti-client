using UnityEngine;
using DG.Tweening; 

public class HandManager : MonoBehaviour
{
    public Transform handArea;
    public float radius = 350f;
    public float baseAngleRange = 140f;
    public float minAngleRange = 20f;
    public float curveHeight = 80f;
    public float offsetY = -100f;
    public float rotationFactor = 0.5f;

    public void RearrangeHand()
    {
        int count = handArea.childCount;
        if (count == 0) return;

        if (count == 1)
        {
            // ✅ 카드가 1장일 때는 중앙에 정렬
            RectTransform rt = handArea.GetChild(0).GetComponent<RectTransform>();
            rt.DOAnchorPos(new Vector2(0, curveHeight + offsetY), 0.3f).SetEase(Ease.OutCubic);
            rt.DOLocalRotate(Vector3.zero, 0.3f).SetEase(Ease.OutCubic);
            return;
        }

        // 카드 수 2~10일 때
        float t = Mathf.InverseLerp(3, 10, count);
        float angleRange = Mathf.Lerp(minAngleRange, baseAngleRange, t);

        float angleStep = angleRange / (count - 1);
        float startAngle = -angleRange / 2f;

        for (int i = 0; i < count; i++)
        {
            Transform card = handArea.GetChild(i);
            RectTransform rt = card.GetComponent<RectTransform>();

            float angle = startAngle + i * angleStep;
            float rad = angle * Mathf.Deg2Rad;

            float x = Mathf.Sin(rad) * radius;
            float y = Mathf.Cos(rad) * curveHeight;

            rt.DOAnchorPos(new Vector2(x, y + offsetY), 0.3f).SetEase(Ease.OutCubic);
            rt.DOLocalRotate(new Vector3(0, 0, -angle * rotationFactor), 0.3f).SetEase(Ease.OutCubic);
        }
    }
}
