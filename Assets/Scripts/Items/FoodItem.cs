using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private FoodItemData itemData;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public FoodItemData ItemData => itemData;

    public void Initialize(FoodItemData data)
    {
        itemData = data;

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{name} ERROR: SpriteRenderer is NULL!"
            );

            return;
        }

        if (itemData == null)
        {
            Debug.LogError(
                $"{name} ERROR: FoodItemData is NULL!"
            );

            return;
        }

        spriteRenderer.sprite = itemData.Icon;
    }
}
