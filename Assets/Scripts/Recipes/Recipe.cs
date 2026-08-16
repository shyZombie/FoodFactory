using System.Collections.Generic;
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

    public bool HasIngredient(
        FoodItemData foodItemData,
        List<FoodItem> storedIngredients)
    {
        if (foodItemData == null)
            return false;

        if (Inputs == null)
            return false;

        foreach (Ingredient ingredient in Inputs)
        {
            if (ingredient.foodItem != foodItemData)
                continue;

            int storedCount = 0;

            foreach (FoodItem storedItem in storedIngredients)
            {
                if (storedItem != null &&
                    storedItem.ItemData == foodItemData)
                {
                    storedCount++;
                }
            }

            return storedCount < ingredient.quantity;
        }

        return false;
    }

    public bool HasEnoughIngredients(
    List<FoodItem> storedIngredients)
    {
        if (Inputs == null ||
            Inputs.Length == 0)
        {
            return false;
        }

        foreach (Ingredient recipeIngredient in Inputs)
        {
            int count = 0;

            foreach (FoodItem storedItem
                     in storedIngredients)
            {
                if (storedItem.ItemData ==
                    recipeIngredient.foodItem)
                {
                    count++;
                }
            }

            if (count < recipeIngredient.quantity)
            {
                return false;
            }
        }

        return true;
    }
}
