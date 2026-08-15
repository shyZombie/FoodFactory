using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private GameObject buildingPrefab;

    private int rotationSteps = 0;

    private void Update()
    {
        HandleRotation();
        HandlePlacement();
    }

    private void HandleRotation()
    {
        if (!Keyboard.current.rKey.wasPressedThisFrame)
            return;

        ConveyorBelt conveyorBelt = GetConveyorUnderMouse();

        if (conveyorBelt != null)
        {
            conveyorBelt.RotateClockwise();
            return;
        }

        rotationSteps++;

        if (rotationSteps >= 4)
        {
            rotationSteps = 0;
        }

        Debug.Log($"Next Building Rotation: {rotationSteps * 90} degrees");
    }

    private ConveyorBelt GetConveyorUnderMouse()
    {
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

        Collider2D hitCollider =
            Physics2D.OverlapPoint(mouseWorldPosition);

        if (hitCollider == null)
            return null;

        return hitCollider.GetComponent<ConveyorBelt>();
    }

    private void HandlePlacement()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PlaceBuilding();
        }
    }

    private void PlaceBuilding()
    {
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

        if (gridManager.IsCellOccupied(gridPosition))
        {
            Debug.Log("Cell is already occupied!");
            return;
        }

        Vector3 worldPosition =
            gridManager.GridToWorldPosition(gridPosition);

        GameObject building = Instantiate(
            buildingPrefab,
            worldPosition,
            Quaternion.identity
        );

        GridObject gridObject =
            building.GetComponent<GridObject>();

        if (gridObject == null)
        {
            Debug.LogError(
                "Building prefab does not contain a GridObject component!"
            );

            Destroy(building);
            return;
        }

        if (!gridManager.TryAddGridObject(
                gridPosition,
                gridObject))
        {
            Destroy(building);
            return;
        }

        ConveyorBelt conveyorBelt =
            building.GetComponent<ConveyorBelt>();

        if (conveyorBelt != null)
        {
            for (int i = 0; i < rotationSteps; i++)
            {
                conveyorBelt.RotateClockwise();
            }
        }
    }
}