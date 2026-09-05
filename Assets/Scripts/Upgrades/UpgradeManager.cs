using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private RecipeDiscoveryManager recipeDiscoveryManager;

    [SerializeField]
    private UpgradeDefinition[] upgradeDefinitions;

    private HashSet<UpgradeDefinition> unlockedUpgrades =
        new HashSet<UpgradeDefinition>();

    private void Start()
    {
        RefreshUpgradeStates();
    }

    public void RefreshUpgradeStates()
    {
        if (recipeDiscoveryManager == null)
        {
            return;
        }

        if (upgradeDefinitions == null)
        {
            return;
        }

        foreach (UpgradeDefinition upgrade
                 in upgradeDefinitions)
        {
            if (upgrade == null)
            {
                continue;
            }

            if (unlockedUpgrades.Contains(upgrade))
            {
                continue;
            }

            if (IsRequirementMet(upgrade))
            {
                unlockedUpgrades.Add(upgrade);
            }
        }
    }

    private bool IsRequirementMet(
        UpgradeDefinition upgrade)
    {
        int discoveredCount =
            recipeDiscoveryManager
                .GetDiscoveredRecipeCountByCategory(
                    upgrade.RequiredCategory
                );

        return discoveredCount >=
               upgrade.RequiredRecipeCount;
    }

    public bool IsUnlocked(
        UpgradeDefinition upgrade)
    {
        if (upgrade == null)
        {
            return false;
        }

        return unlockedUpgrades.Contains(upgrade);
    }

    private void OnEnable()
    {
        if (recipeDiscoveryManager != null)
            recipeDiscoveryManager.RecipeDiscovered += OnRecipeDiscovered;
    }

    private void OnDisable()
    {
        if (recipeDiscoveryManager != null)
        {
            recipeDiscoveryManager.RecipeDiscovered -=
                OnRecipeDiscovered;
        }
    }
    private void OnRecipeDiscovered(Recipe recipe)
    {
        RefreshUpgradeStates();
    }

    public float GetSpeedMultiplier(
    UpgradeTarget target)
    {
        float multiplier = 1f;

        if (upgradeDefinitions == null)
        {
            return multiplier;
        }

        foreach (UpgradeDefinition upgrade
                 in upgradeDefinitions)
        {
            if (upgrade == null)
            {
                continue;
            }

            if (upgrade.Target != target)
            {
                continue;
            }

            if (!IsUnlocked(upgrade))
            {
                continue;
            }

            if (upgrade.SpeedMultiplier > multiplier)
            {
                multiplier = upgrade.SpeedMultiplier;
            }
        }

        return multiplier;
    }

    [ContextMenu("TEST - Print Upgrade States")]
    private void TestPrintUpgradeStates()
    {
        if (upgradeDefinitions == null)
        {
            return;
        }

        foreach (UpgradeDefinition upgrade in upgradeDefinitions)
        {
            if (upgrade == null)
            {
                continue;
            }

            Debug.Log(
                $"Upgrade: {upgrade.UpgradeId} | " +
                $"Target: {upgrade.Target} | " +
                $"Speed Multiplier: {upgrade.SpeedMultiplier:F2} | " +
                $"Unlocked: {IsUnlocked(upgrade)}",
                this
            );
        }
    }

    [ContextMenu("TEST - Print Speed Multipliers")]
    private void TestPrintSpeedMultipliers()
    {
        Debug.Log(
            $"Belt: " +
            $"{GetSpeedMultiplier(UpgradeTarget.Belt)}",
            this
        );

        Debug.Log(
            $"Extractor: " +
            $"{GetSpeedMultiplier(UpgradeTarget.Extractor)}",
            this
        );

        Debug.Log(
            $"Cutter: " +
            $"{GetSpeedMultiplier(UpgradeTarget.Cutter)}",
            this
        );

        Debug.Log(
            $"Oven: " +
            $"{GetSpeedMultiplier(UpgradeTarget.Oven)}",
            this
        );
    }

    [ContextMenu("Test All Upgrade Targets")]
    private void TestAllUpgradeTargets()
    {
        Debug.Log($"Belt | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Belt):F2}");
        Debug.Log($"Extractor | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Extractor):F2}");
        Debug.Log($"Cutter | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Cutter):F2}");
        Debug.Log($"Oven | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Oven):F2}");
        Debug.Log($"Fryer | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Fryer):F2}");
        Debug.Log($"Assembler | Speed Multiplier: {GetSpeedMultiplier(UpgradeTarget.Assembler):F2}");
    }
}
