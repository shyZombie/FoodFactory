using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [SerializeField] private FoodItemData itemData;

    public FoodItemData ItemData => itemData;

    public void Initialize(FoodItemData data)
    {
        itemData = data;
    }
}
