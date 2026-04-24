using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private float stepDuration = 0.2f;
    [SerializeField] private float arcHeight = 1.5f;
    [SerializeField] private CharacterData characterData;

    public PlayerState State { get; private set; }
    public CharacterData CharacterData => characterData;
    public NodeBehaviour CurrentNode;

    private static readonly Vector3 YOffset = new(0, 2f, 0);

    public void Init(NodeBehaviour startNode)
    {
        State = new PlayerState(); // 그냥 런타임때 인스턴스 들고 시작하는게 제일 편하겠다~... 어차피 스테이트 지금 깡통이니까.
        MoveToNode(startNode);
        Debug.Log($"[{name}] 초기화 완료. 시작 노드: {startNode.name}, 위치: {transform.position}");
    }

    public void MoveToNode(NodeBehaviour node)
    {
        CurrentNode = node;
        transform.position = node.transform.position + YOffset;
    }

    public IEnumerator MoveAlongPath(List<NodeBehaviour> path)
    {
        foreach (var node in path)
        {
            yield return StartCoroutine(JumpToNode(node));
        }
    }

    private IEnumerator JumpToNode(NodeBehaviour node) // 오렌지 100% 이동 효과
    {
        Vector3 from = transform.position;
        Vector3 to = node.transform.position + YOffset;
        float elapsed = 0f; // 이동 시간 측정

        while (elapsed < stepDuration) // stepDuration 동안 이동 애니메이션 실행
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / stepDuration);
            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = Vector3.Lerp(from, to, t) + Vector3.up * arc;
            yield return null;
        }

        CurrentNode = node;
        transform.position = to;
    }
}
