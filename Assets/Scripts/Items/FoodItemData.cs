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
    [SerializeField] private FoodCategory[] categories;

    public string ItemId => itemId;
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public FoodCategory[] Categories => categories;
    public bool HasCategory(FoodCategory category)
    {
        if (categories == null)
            return false;

        foreach (FoodCategory currentCategory in categories)
        {
            if (currentCategory == category)
                return true;
        }
        return false;
    }
    public bool HasAnyCategory(params FoodCategory[] requestedCategories)
    {
        if (categories == null ||
            requestedCategories == null)
        {
            return false;
        }

        foreach (FoodCategory requestedCategory in requestedCategories)
        {
            if (HasCategory(requestedCategory))
                return true;
        }

        return false;
    }
    public bool HasAllCategories(
        params FoodCategory[] requestedCategories)
    {
        if (categories == null ||
            requestedCategories == null ||
            requestedCategories.Length == 0)
        {
            return false;
        }

        foreach (FoodCategory requestedCategory in requestedCategories)
        {
            if (!HasCategory(requestedCategory))
                return false;
        }

        return true;
    }
}