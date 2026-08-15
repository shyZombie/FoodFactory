using UnityEngine;

[CreateAssetMenu(
    fileName = "Recipe",
    menuName = "Food Factory/Recipe"
)]
public class Recipe : ScriptableObject
{
    [System.Serializable]
    public class Ingredient
    {
        public FoodItemData foodItem;
        public int quantity = 1;
    }

    [System.Serializable]
    public class Result
    {
        public FoodItemData foodItem;
        public int quantity = 1;
    }

    [SerializeField]
    private Ingredient[] inputs;

    [SerializeField]
    private Result[] outputs;

    [SerializeField]
    private float processingTime = 1f;

    public Ingredient[] Inputs => inputs;

    public Result[] Outputs => outputs;

    public float ProcessingTime => processingTime;
}
