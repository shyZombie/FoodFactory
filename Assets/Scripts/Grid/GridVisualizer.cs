using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private int gridWidth = 20;
    [SerializeField] private int gridHeight = 20;

    private void OnDrawGizmos()
    {
        if (gridManager == null)
            return;

        Gizmos.color = Color.gray;

        for (int x = -gridWidth / 2; x <= gridWidth / 2; x++)
        {
            Vector3 start = new Vector3(x, -gridHeight / 2, 0);
            Vector3 end = new Vector3(x, gridHeight / 2, 0);

            Gizmos.DrawLine(start, end);
        }

        for (int y = -gridHeight / 2; y <= gridHeight / 2; y++)
        {
            Vector3 start = new Vector3(-gridWidth / 2, y, 0);
            Vector3 end = new Vector3(gridWidth / 2, y, 0);

            Gizmos.DrawLine(start, end);
        }
    }
}
