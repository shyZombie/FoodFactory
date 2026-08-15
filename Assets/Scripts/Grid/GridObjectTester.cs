using UnityEngine;
using UnityEngine.InputSystem;

public class GridObjectTester : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

    private void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 mouseScreenPosition =
            Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition =
            Camera.main.ScreenToWorldPoint(
                new Vector3(
                    mouseScreenPosition.x,
                    mouseScreenPosition.y,
                    -Camera.main.transform.position.z
                )
            );

        GridPosition gridPosition =
            gridManager.WorldToGridPosition(mouseWorldPosition);

        GridObject gridObject =
            gridManager.GetGridObject(gridPosition);

        if (gridObject == null)
        {
            Debug.Log(
                $"Cell {gridPosition} is empty."
            );

            return;
        }

        Debug.Log(
            $"Cell {gridPosition} contains: " +
            gridObject.GetType().Name
        );
    }
}
