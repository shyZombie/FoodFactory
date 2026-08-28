using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RecipeDiscoveryProgress
{
    public FoodCategory Category;
    public int DiscoveredCount;
    public int RequiredCount;

    public bool IsCompleted =>
        DiscoveredCount >= RequiredCount;
}

public class RecipeDiscoveryManager : MonoBehaviour
{
    private HashSet<Recipe> discoveredRecipes =
        new HashSet<Recipe>();
    public event System.Action<Recipe> RecipeDiscovered;

    private void OnEnable()
    {
        Machine.OnFoodItemProduced += HandleFoodItemProduced;
    }

    private void OnDisable()
    {
        Machine.OnFoodItemProduced -= HandleFoodItemProduced;
    }

    private void HandleFoodItemProduced(
    Recipe recipe,
    FoodItemData foodItemData)
    {
        DiscoverRecipe(recipe);
    }

    public bool IsDiscovered(Recipe recipe)
    {
        if (recipe == null)
            return false;

        return discoveredRecipes.Contains(recipe);
    }

    public bool DiscoverRecipe(Recipe recipe)
    {
        if (recipe == null)
            return false;

        bool wasDiscovered =
            discoveredRecipes.Add(recipe);

        if (wasDiscovered)
        {
            RecipeDiscovered?.Invoke(recipe);
        }

        return wasDiscovered;
    }

    public int GetDiscoveredRecipeCountByCategory(
    FoodCategory category)
    {
        int count = 0;

        foreach (Recipe recipe in discoveredRecipes)
        {
            if (recipe != null &&
                recipe.HasOutputCategory(category))
            {
                count++;
            }
        }

        return count;
    }

    public RecipeDiscoveryProgress GetDiscoveryProgress(
    FoodCategory category,
    int requiredCount)
    {
        RecipeDiscoveryProgress progress =
            new RecipeDiscoveryProgress();

        progress.Category = category;
        progress.DiscoveredCount =
            GetDiscoveredRecipeCountByCategory(category);

        progress.RequiredCount = requiredCount;

        return progress;
    }
}
