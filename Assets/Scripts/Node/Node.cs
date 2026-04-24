using UnityEngine;

public enum NodeType
{
    Normal,
    Boost,
    Trap,
    Warp,
    Battle
}

[CreateAssetMenu(fileName = "Node", menuName = "BoardObjects/Node")]
public class Node : ScriptableObject
{
    [Header("노드 시각화")]
    [SerializeField] private Sprite nodeSprite;

    [Header("노드 데이터")]
    [SerializeField] private NodeType nodeType;


    public Sprite NodeSprite => nodeSprite;
    public NodeType NodeType => nodeType;

}
