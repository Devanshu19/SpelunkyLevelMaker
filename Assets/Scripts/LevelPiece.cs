using UnityEngine;

public class LevelPiece : MonoBehaviour
{
    [SerializeField] GameObject levelPieceTemplate;

    [SerializeField] Vector2 coordinate;

    [SerializeField] LevelGridChannelSO levelGridChannelSO;
    [SerializeField] PiecePoolerChannelSO piecePoolerChannelSO;

    public void PlaceLevelPiece(Vector2Int coordinates, int debug, GateType entryGateType)
    {
        this.coordinate = coordinates;

        // Set the Position and Update the grid
        transform.position = levelGridChannelSO.RaiseGetPositionFromGridCoordinates(coordinates);
        levelGridChannelSO.RaiseUpdateLevelGrid(coordinates, 1);

        // Get data about the next level piece
        SurroudingData surrData = GetSurroundingData(coordinates);
        Vector2Int nextDirection = surrData.GetRandomDirection();
        GateType exitGateType = GetExitGateType(nextDirection);

        // Update this empty level piece to match the entry and exit gates
        var thisLevelPiece = piecePoolerChannelSO.RaiseRequestPoolObjectFromGates(entryGateType, exitGateType);
        thisLevelPiece.transform.SetParent(this.transform);
        thisLevelPiece.transform.localPosition = Vector3.zero;
        thisLevelPiece.SetActive(true);

        // Create the next level piece
        GameObject nextLevelPiece = piecePoolerChannelSO.RaiseRequestPoolObjectFromGates(GateType.None, GateType.None);
        LevelPiece levelPiece = nextLevelPiece.GetComponent<LevelPiece>();
        nextLevelPiece.SetActive(true);
        nextLevelPiece.transform.parent = null;

        if (nextDirection == Vector2Int.zero)
        {
            return;
        }

        // Run the same process on the next level piece
        levelPiece.PlaceLevelPiece(new Vector2Int(coordinates.x + nextDirection.x, coordinates.y + nextDirection.y), debug + 1, GetEntryGateForNextLevelPiece(nextDirection));
    }

    private SurroudingData GetSurroundingData(Vector2Int coordinates)
    {
        SurroudingData surroudingData = new SurroudingData();

        surroudingData.up = levelGridChannelSO.RaiseGetValueAtGridCoordinate(coordinates - Vector2Int.up) == 0 ? false : true;
        surroudingData.down = levelGridChannelSO.RaiseGetValueAtGridCoordinate(coordinates - Vector2Int.down) == 0 ? false : true;
        surroudingData.left = levelGridChannelSO.RaiseGetValueAtGridCoordinate(coordinates + Vector2Int.left) == 0 ? false : true;
        surroudingData.right = levelGridChannelSO.RaiseGetValueAtGridCoordinate(coordinates + Vector2Int.right) == 0 ? false : true;

        return surroudingData;
    }

    // Returns the starting gate of the next level piece based on where it was placed on the grid
    private GateType GetEntryGateForNextLevelPiece(Vector2Int nextDirection)
    {
        if (nextDirection == Vector2Int.up)
        {
            return GateType.UP;
        }
        else if (nextDirection == Vector2Int.left)
        {
            return GateType.RIGHT;
        }
        else if (nextDirection == Vector2Int.right)
        {
            return GateType.LEFT;
        }
        else
        {
            return GateType.None;
        }
    }

    // Returns the exit gate type based on where the next level piece is placed on the grid
    private GateType GetExitGateType(Vector2Int nextDirection)
    {
        if (nextDirection == Vector2Int.up)
        {
            return GateType.DOWN;
        }
        else if (nextDirection == Vector2Int.left)
        {
            return GateType.LEFT;
        }
        else if (nextDirection == Vector2Int.right)
        {
            return GateType.RIGHT;
        }
        else
        {
            return GateType.None;
        }
    }
}
