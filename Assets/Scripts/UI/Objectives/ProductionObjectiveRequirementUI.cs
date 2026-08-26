using TMPro;
using UnityEngine;

public class ProductionObjectiveRequirementUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text categoryText;

    [SerializeField]
    private TMP_Text progressText;

    public void SetProgress(
        ProductionObjectiveProgress progress)
    {
        if (categoryText != null)
        {
            categoryText.text =
                progress.Category.ToString();
        }

        if (progressText != null)
        {
            progressText.text =
                $"{progress.Current}/{progress.Required}";
        }
    }

    [ContextMenu("TEST - Set Vegetable 2/3")]
    private void TestSetProgress()
    {
        ProductionObjectiveProgress testProgress =
            new ProductionObjectiveProgress(
                FoodCategory.Vegetable,
                2,
                3
            );

        SetProgress(testProgress);
    }
}