using UnityEngine;

[CreateAssetMenu(
    fileName = "FoodItem_",
    menuName = "Food Factory/Food Item"
)]
public class FoodItemData : ScriptableObject
{
    [SerializeField] private string itemId;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public Sprite Icon => icon;
}