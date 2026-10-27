using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CustomUIGradient : BaseMeshEffect
{
    public Gradient gradient; // Unity Gradient to handle color interpolation
    [Range(-180f, 180f)]
    public float m_angle = 0f;
    public bool m_ignoreRatio = true;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!isActiveAndEnabled || vh.currentVertCount == 0)
            return;

        List<UIVertex> vertices = new List<UIVertex>();
        vh.GetUIVertexStream(vertices);
        vh.Clear(); // Clear current vertex helper to rebuild it properly

        Rect rect = graphic.rectTransform.rect; // RectTransform bounds
        Vector2 dir = UIGradientUtils.RotationDir(m_angle);

        if (!m_ignoreRatio)
            dir = UIGradientUtils.CompensateAspectRatio(rect, dir);

        UIGradientUtils.Matrix2x3 matrix = UIGradientUtils.LocalPositionMatrix(rect, dir);

        for (int i = 0; i < vertices.Count; i++)
        {
            UIVertex vertex = vertices[i];
            Vector2 localPosition = matrix * vertex.position;
            float lerpFactor = localPosition.y;
            vertex.color *= gradient.Evaluate(lerpFactor);
            vh.AddVert(vertex);
        }

        // Re-add triangles
        for (int i = 0; i < vertices.Count; i += 3)
        {
            vh.AddTriangle(i, i + 1, i + 2);
        }
    }



    private void SubdivideAndAddVertices(List<UIVertex> vertices, VertexHelper vh)
    {
        for (int i = 0; i < vertices.Count; i += 3)
        {
            UIVertex v1 = vertices[i];
            UIVertex v2 = vertices[i + 1];
            UIVertex v3 = vertices[i + 2];

            // Midpoints
            UIVertex m1 = CreateMidpoint(v1, v2);
            UIVertex m2 = CreateMidpoint(v2, v3);
            UIVertex m3 = CreateMidpoint(v3, v1);

            // Add original and midpoints vertices
            int baseIndex = vh.currentVertCount;
            vh.AddVert(v1);
            vh.AddVert(v2);
            vh.AddVert(v3);
            vh.AddVert(m1);
            vh.AddVert(m2);
            vh.AddVert(m3);

            // Add triangles for the 4 new smaller triangles
            vh.AddTriangle(baseIndex, baseIndex + 3, baseIndex + 5); // Triangle 1
            vh.AddTriangle(baseIndex + 3, baseIndex + 1, baseIndex + 4); // Triangle 2
            vh.AddTriangle(baseIndex + 5, baseIndex + 4, baseIndex + 2); // Triangle 3
            vh.AddTriangle(baseIndex + 3, baseIndex + 4, baseIndex + 5); // Triangle 4 (middle triangle)
        }
    }

    private UIVertex CreateMidpoint(UIVertex a, UIVertex b)
    {
        UIVertex midpoint = new UIVertex();
        midpoint.position = Vector3.Lerp(a.position, b.position, 0.5f);
        midpoint.normal = Vector3.Lerp(a.normal, b.normal, 0.5f);
        midpoint.color = Color.Lerp(a.color, b.color, 0.5f);
        midpoint.uv0 = Vector2.Lerp(a.uv0, b.uv0, 0.5f);
        midpoint.uv1 = Vector2.Lerp(a.uv1, b.uv1, 0.5f);
        midpoint.uv2 = Vector2.Lerp(a.uv2, b.uv2, 0.5f);
        midpoint.uv3 = Vector2.Lerp(a.uv3, b.uv3, 0.5f);
        midpoint.tangent = Vector4.Lerp(a.tangent, b.tangent, 0.5f);

        return midpoint;
    }
}
