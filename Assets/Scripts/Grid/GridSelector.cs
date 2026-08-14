using UnityEngine;
using UnityEngine.InputSystem;

public class GridSelector : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private GridPosition currentGridPosition;

    private void Update()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                -Camera.main.transform.position.z
            )
        );

        currentGridPosition = gridManager.WorldToGridPosition(mouseWorldPosition);
    }

    private void OnDrawGizmos()
    {
        if (gridManager == null)
            return;

        Vector3 worldPosition = gridManager.GridToWorldPosition(currentGridPosition);

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            worldPosition,
            Vector3.one
        );
    }
}
