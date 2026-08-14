using UnityEngine;
using UnityEngine.InputSystem;

public class GridTester : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;

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

        GridPosition gridPosition = gridManager.WorldToGridPosition(mouseWorldPosition);

        Debug.Log($"Mouse Grid Position: {gridPosition}");
    }
}
