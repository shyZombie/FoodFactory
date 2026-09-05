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
    [SerializeField]
    private RecipeRegistry recipeRegistry;

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

    private void Awake()
    {
        LoadDiscoveryData();
    }

    private void LoadDiscoveryData()
    {
        RecipeDiscoverySaveData saveData =
            RecipeDiscoverySaveSystem.Load();

        if (saveData == null)
        {
            return;
        }

        LoadFromSaveData(
            saveData,
            recipeRegistry
        );
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

        SaveDiscoveryData();
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
    public RecipeDiscoverySaveData CreateSaveData()
    {
        RecipeDiscoverySaveData saveData =
            new RecipeDiscoverySaveData();

        foreach (Recipe recipe in discoveredRecipes)
        {
            if (recipe == null ||
                string.IsNullOrWhiteSpace(recipe.RecipeId))
            {
                continue;
            }

            saveData.discoveredRecipeIds.Add(
                recipe.RecipeId
            );
        }

        return saveData;
    }

    private void SaveDiscoveryData()
    {
        RecipeDiscoverySaveData saveData =
            CreateSaveData();

        RecipeDiscoverySaveSystem.Save(
            saveData
        );
    }

    public void LoadFromSaveData(
    RecipeDiscoverySaveData saveData,
    RecipeRegistry recipeRegistry)
    {
        discoveredRecipes.Clear();

        if (saveData == null ||
            saveData.discoveredRecipeIds == null ||
            recipeRegistry == null)
        {
            return;
        }

        foreach (string recipeId
                 in saveData.discoveredRecipeIds)
        {
            if (string.IsNullOrWhiteSpace(recipeId))
            {
                continue;
            }

            Recipe recipe =
                recipeRegistry.FindById(recipeId);

            if (recipe != null)
            {
                discoveredRecipes.Add(recipe);
            }
        }
    }
}
