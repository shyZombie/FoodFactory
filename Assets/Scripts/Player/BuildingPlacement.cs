using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject buildingPrefab;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }
    }

    private void PlaceBuilding()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(
                mouseScreenPosition.x,
                mouseScreenPosition.y,
                -Camera.main.transform.position.z
            )
        );

        GridPosition gridPosition =
            gridManager.WorldToGridPosition(mouseWorldPosition);

        if (!gridManager.TryOccupyCell(gridPosition))
        {
            Debug.Log("Cell is already occupied!");
            return;
        }

        Vector3 worldPosition =
            gridManager.GridToWorldPosition(gridPosition);

        Instantiate(
            buildingPrefab,
            worldPosition,
            Quaternion.identity
        );
    }
}