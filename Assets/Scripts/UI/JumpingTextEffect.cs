using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class JumpingTextEffect : MonoBehaviour
{
    private TMP_Text tmpText;
    private TMP_TextInfo textInfo;

    public float jumpHeight = 10f;
    public float jumpDuration = 0.4f;
    public float delayBetweenChars = 0.1f;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(AnimateCharacters());
    }

    IEnumerator AnimateCharacters()
    {
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;

        Vector3[][] copyOfVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < copyOfVertices.Length; i++)
        {
            copyOfVertices[i] = new Vector3[textInfo.meshInfo[i].vertices.Length];
        }

        while (true)
        {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] sourceVertices = textInfo.meshInfo[materialIndex].vertices;

                // Copy original vertices
                for (int j = 0; j < 4; j++)
                    copyOfVertices[materialIndex][vertexIndex + j] = sourceVertices[vertexIndex + j];

                // Animate upward
                float progress = 0f;
                while (progress < jumpDuration)
                {
                    float offset = Mathf.Sin(progress / jumpDuration * Mathf.PI) * jumpHeight;
                    for (int j = 0; j < 4; j++)
                    {
                        sourceVertices[vertexIndex + j] = copyOfVertices[materialIndex][vertexIndex + j] + new Vector3(0, offset, 0);
                    }

                    tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
                    progress += Time.deltaTime;
                    yield return null;
                }

                // Reset
                for (int j = 0; j < 4; j++)
                    sourceVertices[vertexIndex + j] = copyOfVertices[materialIndex][vertexIndex + j];

                tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                yield return new WaitForSeconds(delayBetweenChars);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
