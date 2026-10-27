using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class MaintainAspectRatio : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;

    public bool maintainAspectBasedOnHeight = false; // New public variable to toggle aspect ratio maintenance

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void OnEnable()
    {
        UpdateAspectRatio();
    }

    void OnValidate()
    {
        UpdateAspectRatio();
    }

    void OnRectTransformDimensionsChange()
    {
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        if (rectTransform != null && image != null && image.sprite != null)
        {
            float aspectRatio = image.sprite.bounds.size.x / image.sprite.bounds.size.y;

            if (maintainAspectBasedOnHeight)
            {
                float height = rectTransform.rect.height;
                float width = height * aspectRatio;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
            else
            {
                float width = rectTransform.rect.width;
                float height = width / aspectRatio;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }
    }
}
