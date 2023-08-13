using UnityEngine;


public class Selection : MonoBehaviour
{
    public GridType gridType;
    public bool isSelected;

    public Vector2Int Location;
    
    public enum GridType
    {
        WhiteGrid = 0,
        BlackGrid = 1,
        NormalGrid = 2,
    }

    private void Awake()
    {
        gridType = GridType.NormalGrid;
    }

    private void OnTriggerEnter(Collider other)
    {
        gridType = (GridType)other.GetComponent<Chess>().camp;
    }

    private void OnTriggerExit(Collider other)
    {
        gridType = GridType.NormalGrid;
    }
    
    public void Select(Material material)
    {
        MatchManager.Instance.currentSelection = this;
        GetComponent<Renderer>().material = material;
    }

    public void Deselect(Material defaultMaterial)
    {
        MatchManager.Instance.currentSelection = null;
        GetComponent<Renderer>().material = defaultMaterial;
    }
}


public class MatchManager
{
    private static MatchManager instance;

    public static MatchManager Instance => instance ??= new MatchManager();

    public Selection currentSelection;

    public Chess currentChess;
}