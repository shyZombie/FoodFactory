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

    public bool IsValid()
    {
        return string.IsNullOrEmpty(
            GetValidationError()
        );
    }

    public string GetValidationError()
    {
        if (Inputs == null ||
            Inputs.Length == 0)
        {
            return "Recipe has no inputs.";
        }

        for (int i = 0; i < Inputs.Length; i++)
        {
            Ingredient ingredient = Inputs[i];

            if (ingredient == null)
            {
                return $"Input {i} is NULL.";
            }

            if (ingredient.foodItem == null)
            {
                return $"Input {i} has no FoodItemData.";
            }

            if (ingredient.quantity <= 0)
            {
                return $"Input {i} has invalid quantity: " +
                       $"{ingredient.quantity}.";
            }
        }

        if (Outputs == null ||
            Outputs.Length == 0)
        {
            return "Recipe has no outputs.";
        }

        for (int i = 0; i < Outputs.Length; i++)
        {
            Result result = Outputs[i];

            if (result == null)
            {
                return $"Output {i} is NULL.";
            }

            if (result.foodItem == null)
            {
                return $"Output {i} has no FoodItemData.";
            }

            if (result.quantity <= 0)
            {
                return $"Output {i} has invalid quantity: " +
                       $"{result.quantity}.";
            }
        }

        if (ProcessingTime <= 0f)
        {
            return $"Processing time is invalid: " +
                   $"{ProcessingTime}.";
        }

        return string.Empty;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        string validationError = GetValidationError();

        if (!string.IsNullOrEmpty(validationError))
        {
            Debug.LogWarning(
                $"Recipe '{name}' is invalid. " +
                $"Reason: {validationError}",
                this
            );
        }
    }
#endif
}
