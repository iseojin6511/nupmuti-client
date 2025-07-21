using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField chatInput;
    public Transform chatContent; // ScrollView > Viewport > Content
    public GameObject chatMessagePrefab;
    public ScrollRect scrollRect; // 자동 스크롤을 위해 추가

    void Start()
    {
        chatInput.onEndEdit.AddListener(OnEndEdit);
    }

    void OnEndEdit(string text)
    {
        // 입력 후 Enter 키로 확정했을 때만 실행
        if (Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SendMessage();
        }
    }

    void SendMessage()
    {
        string message = chatInput.text.Trim();
        if (!string.IsNullOrEmpty(message))
        {
            AddMessage("나", message);
            chatInput.text = "";
            chatInput.ActivateInputField(); // 다시 입력 가능하게
        }
    }

    public void AddMessage(string sender, string message)
    {
        GameObject msg = Instantiate(chatMessagePrefab, chatContent);
        
        // Layout이 작동하게 하기 위한 설정 보정 (중요!)
        RectTransform rt = msg.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = Vector2.zero;

        TMP_Text text = msg.GetComponent<TMP_Text>();
        text.text = $"<b>{sender}:</b> {message}";

        // 스크롤을 가장 아래로
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
