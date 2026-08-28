using UnityEngine;

[CreateAssetMenu(
    fileName = "Recipe_",
    menuName = "Factory/Recipe Data"
)]
public class RecipeData : ScriptableObject
{
    [Header("Display")]
    [SerializeField]
    private string recipeName;

    [Header("Output")]
    [SerializeField]
    private FoodItemData outputItem;

    public string RecipeName =>
        recipeName;

    public FoodItemData OutputItem =>
        outputItem;
}
