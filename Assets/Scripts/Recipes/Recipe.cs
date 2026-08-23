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

        string duplicateInputError =
            GetDuplicateInputError();

        if (!string.IsNullOrEmpty(
            duplicateInputError))
        {
            return duplicateInputError;
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

        string duplicateOutputError =
            GetDuplicateOutputError();

        if (!string.IsNullOrEmpty(
            duplicateOutputError))
        {
            return duplicateOutputError;
        }

        if (ProcessingTime <= 0f)
        {
            return $"Processing time is invalid: " +
                   $"{ProcessingTime}.";
        }

        return string.Empty;
    }

    private bool ContainsDuplicateInputs()
    {
        if (Inputs == null)
            return false;

        for (int i = 0; i < Inputs.Length; i++)
        {
            if (Inputs[i] == null ||
                Inputs[i].foodItem == null)
            {
                continue;
            }

            for (int j = i + 1; j < Inputs.Length; j++)
            {
                if (Inputs[j] == null ||
                    Inputs[j].foodItem == null)
                {
                    continue;
                }

                if (Inputs[i].foodItem ==
                    Inputs[j].foodItem)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool ContainsDuplicateOutputs()
    {
        if (Outputs == null)
            return false;

        for (int i = 0; i < Outputs.Length; i++)
        {
            if (Outputs[i] == null ||
                Outputs[i].foodItem == null)
            {
                continue;
            }

            for (int j = i + 1; j < Outputs.Length; j++)
            {
                if (Outputs[j] == null ||
                    Outputs[j].foodItem == null)
                {
                    continue;
                }

                if (Outputs[i].foodItem ==
                    Outputs[j].foodItem)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private string GetDuplicateInputError()
    {
        if (Inputs == null)
            return string.Empty;

        for (int i = 0; i < Inputs.Length; i++)
        {
            if (Inputs[i] == null ||
                Inputs[i].foodItem == null)
            {
                continue;
            }

            for (int j = i + 1; j < Inputs.Length; j++)
            {
                if (Inputs[j] == null ||
                    Inputs[j].foodItem == null)
                {
                    continue;
                }

                if (Inputs[i].foodItem ==
                    Inputs[j].foodItem)
                {
                    return
                        $"Input {i} and Input {j} " +
                        $"use the same FoodItemData: " +
                        $"{Inputs[i].foodItem.name}.";
                }
            }
        }

        return string.Empty;
    }

    private string GetDuplicateOutputError()
    {
        if (Outputs == null)
            return string.Empty;

        for (int i = 0; i < Outputs.Length; i++)
        {
            if (Outputs[i] == null ||
                Outputs[i].foodItem == null)
            {
                continue;
            }

            for (int j = i + 1; j < Outputs.Length; j++)
            {
                if (Outputs[j] == null ||
                    Outputs[j].foodItem == null)
                {
                    continue;
                }

                if (Outputs[i].foodItem ==
                    Outputs[j].foodItem)
                {
                    return
                        $"Output {i} and Output {j} " +
                        $"use the same FoodItemData: " +
                        $"{Outputs[i].foodItem.name}.";
                }
            }
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
