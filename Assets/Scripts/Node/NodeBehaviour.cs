using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NodeBehaviour : MonoBehaviour
{
    [SerializeField] private Node nodeData;

    public Node Data => nodeData;
    public int Index { get; private set; }
    public bool IsOccupied { get; set; }

    public void Init(int index)
    {
        Index = index;
    }

    [Header("연결된 노드")]
    [SerializeField] private NodeBehaviour nextNode;
    [SerializeField] private NodeBehaviour previousNode;

    public NodeBehaviour NextNode => nextNode;
    public NodeBehaviour PreviousNode => previousNode;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = nodeData.NodeSprite;
        
    }
}
