using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteAlways]
[AddComponentMenu("UI/Effects/TextMeshProUGUI Gradient")]
public class TMPTextGradient : MonoBehaviour
{
    public Color m_color1 = Color.white;
    public Color m_color2 = Color.white;
    [Range(-180f, 180f)]
    public float m_angle = 0f;

    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        if (tmpText == null)
        {
            Debug.LogError("No TextMeshProUGUI component found on the GameObject.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        Canvas.willRenderCanvases += ApplyGradient;
        ApplyGradient();
    }

    private void OnDisable()
    {
        Canvas.willRenderCanvases -= ApplyGradient;
    }

    private void OnValidate()
    {
        // This ensures that the gradient is updated in the editor when properties change
        if (tmpText != null)
        {
            tmpText.ForceMeshUpdate();
            ApplyGradient();
        }
    }

    private void ApplyGradient()
    {
        tmpText.ForceMeshUpdate();

        TMP_TextInfo textInfo = tmpText.textInfo;

        if (textInfo == null || 
            textInfo.meshInfo == null || textInfo.meshInfo.Length == 0 || 
            textInfo.meshInfo[0].vertices == null || textInfo.meshInfo[0].vertices.Length == 0)
            return;

        Vector2 direction = new Vector2(Mathf.Sin(m_angle * Mathf.Deg2Rad), Mathf.Cos(m_angle * Mathf.Deg2Rad));

        // Calculate the min and max projections for the entire text mesh
        float minProjection = float.MaxValue;
        float maxProjection = float.MinValue;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            Vector3[] vertices = textInfo.meshInfo[i].vertices;
            for (int j = 0; j < vertices.Length; j++)
            {
                float projection = Vector2.Dot(direction, new Vector2(vertices[j].x, vertices[j].y));
                if (projection < minProjection) minProjection = projection;
                if (projection > maxProjection) maxProjection = projection;
            }
        }

        // Apply the gradient based on the global min and max projections
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            Vector3[] vertices = textInfo.meshInfo[i].vertices;
            Color32[] colors = textInfo.meshInfo[i].colors32;
            for (int j = 0; j < vertices.Length; j++)
            {
                float projection = Vector2.Dot(-direction, new Vector2(vertices[j].x, vertices[j].y));
                float t = (projection - minProjection) / (maxProjection - minProjection);
                colors[j] = Color.Lerp(m_color1, m_color2, t);
            }
        }

        // Update the vertex data with the new color information
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.colors32 = meshInfo.colors32;
            tmpText.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
