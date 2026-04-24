using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private List<NodeBehaviour> nodes;
    [SerializeField] private List<PlayerBehaviour> player; // 로컬 멀티플레이 전제

    public int NodeCount => nodes.Count;

    public NodeBehaviour GetNode(int index) => nodes[index];

    public int GetNextIndex(int current, int steps) // 현재 위치에서 steps만큼 이동한 다음의 노드 인덱스를 반환하는 메서드.
    {
        return (current + steps) % nodes.Count;
    }

    private void Awake()
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Init(i);
    }

    public void InitPlayers()
    {
        for (int i = 0; i < player.Count; i++)
            player[i].Init(nodes[0]);
    }
}
