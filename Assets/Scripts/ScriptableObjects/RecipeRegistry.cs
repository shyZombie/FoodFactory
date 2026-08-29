using System.Collections.Generic;
using UnityEngine;

public class RecipeRegistry : MonoBehaviour
{
    [SerializeField]
    private Recipe[] recipes;

    private Dictionary<string, Recipe> recipesById =
        new Dictionary<string, Recipe>();

    private void Awake()
    {
        BuildRegistry();
    }

    private void BuildRegistry()
    {
        recipesById.Clear();

        if (recipes == null)
        {
            return;
        }

        foreach (Recipe recipe in recipes)
        {
            if (recipe == null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(recipe.RecipeId))
            {
                continue;
            }

            if (recipesById.ContainsKey(recipe.RecipeId))
            {
                continue;
            }

            recipesById.Add(
                recipe.RecipeId,
                recipe
            );
        }
    }

    public Recipe FindById(string recipeId)
    {
        if (string.IsNullOrWhiteSpace(recipeId))
        {
            return null;
        }

        recipesById.TryGetValue(
            recipeId,
            out Recipe recipe
        );

        return recipe;
    }
}
