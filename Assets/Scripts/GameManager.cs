using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private PlayerBehaviour[] players;

    private int currentPlayerIndex;
    private bool waitingForRoll;

    private PlayerBehaviour CurrentPlayer => players[currentPlayerIndex];

    private bool IsRollKeyPressed() => Keyboard.current.spaceKey.wasPressedThisFrame;

    private void Start()
    {
        boardManager.InitPlayers();
        InitPlayerHUD();
        StartTurn();
    }

    private void InitPlayerHUD()
    {
        for (int i = 0; i < players.Length; i++)
        {
            UIManager.Instance.SetPortrait(i, players[i].CharacterData);
            UIManager.Instance.UpdateHPText(i, players[i].State.HP);
            UIManager.Instance.SetDiceRollImage(i, players[i].CharacterData);

            Debug.Log($"[GameManager] P{i + 1} HUD 초기화 완료. 이름: {players[i].CharacterData.characterName}, HP: {players[i].State.HP}, 포트레이트 설정됨");
        }
    }

    private void Update()
    {
        if (!waitingForRoll) return;
        if (IsRollKeyPressed()) RollAndMove();
    }

    private void StartTurn()
    {
        UIManager.Instance.UpdateTurnText(currentPlayerIndex + 1);
        Debug.Log($"--- P{currentPlayerIndex + 1} 턴 시작 ---");

        if (CurrentPlayer.State.IsDown)
        {
            StartCoroutine(RevivalCoroutine(CurrentPlayer, currentPlayerIndex));
            return;
        }

        waitingForRoll = true;
        Debug.Log("Space로 주사위 굴림");
    }

    private IEnumerator RevivalCoroutine(PlayerBehaviour player, int playerIndex)
    {
        int required = Random.Range(2, 6);
        UIManager.Instance.ShowRevivalPanel(player.CharacterData, required);

        yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);

        int roll = DiceRoller.Roll();
        bool success = roll >= required;

        yield return StartCoroutine(UIManager.Instance.ShowRevivalResult(roll, success, player.CharacterData));
        UIManager.Instance.HideRevivalPanel();

        if (success)
        {
            player.State.IsDown = false;
            player.State.HP = PlayerState.MaxHP;
            UIManager.Instance.UpdateHPText(playerIndex, player.State.HP);

            int diceResult = DiceRoller.Roll();
            yield return StartCoroutine(UIManager.Instance.ShowDiceRoll(diceResult, playerIndex));
            yield return StartCoroutine(MovePlayerCoroutine(player, diceResult));
        }
        else
        {
            EndTurn();
        }
    }

    private void RollAndMove()
    {
        waitingForRoll = false;

        int result = DiceRoller.Roll();
        Debug.Log($"[P{currentPlayerIndex + 1}] 주사위: {result}");

        StartCoroutine(RollCoroutine(CurrentPlayer, result));
    }

    private IEnumerator RollCoroutine(PlayerBehaviour player, int steps)
    {
        yield return StartCoroutine(UIManager.Instance.ShowDiceRoll(steps, currentPlayerIndex));
        yield return StartCoroutine(MovePlayerCoroutine(player, steps));
    }

    private IEnumerator MovePlayerCoroutine(PlayerBehaviour player, int steps)
    {
        int from = player.CurrentNode.Index; // 노드 인덱스 받아와서

        var path = new List<NodeBehaviour>(); // 이동 경로 계산(그러니까 리스트화해서)
        for (int i = 1; i <= steps; i++)
        {
            int idx = (from + i) % boardManager.NodeCount; // 이동할 노드 인덱스 계산

            // 인덱스가 wrap될 때 = 시작 칸 통과
            if (from + i == boardManager.NodeCount)
            {
                player.State.Flags++;
                Debug.Log($"[{player.name}] 한 바퀴 완주! 깃발 +1 → 총 {player.State.Flags}개");
            }

            path.Add(boardManager.GetNode(idx));
        }

        yield return player.MoveAlongPath(path);

        PlayerBehaviour opponent = FindOpponentOnNode(player);
        if (opponent != null)
        {
            Debug.Log($"[{player.name}] vs [{opponent.name}] 전투 발생!");
            yield return StartCoroutine(UIManager.Instance.ShowBattleIntro(
                opponent.CharacterData, opponent.State.HP));
            yield return StartCoroutine(CombatManager.Instance.RunCombat(player, opponent));

            // 전투 후 HUD HP 갱신
            for (int i = 0; i < players.Length; i++)
                UIManager.Instance.UpdateHPText(i, players[i].State.HP);
        }

        EndTurn();
    }

    private PlayerBehaviour FindOpponentOnNode(PlayerBehaviour self)
    {
        foreach (var p in players)
        {
            if (p == self) continue;
            Debug.Log($"[충돌 체크] {self.name}({(self.CurrentNode != null ? self.CurrentNode.Index : -1)}) vs {p.name}({(p.CurrentNode != null ? p.CurrentNode.Index : -1)})");
            if (p.CurrentNode == self.CurrentNode)
                return p;
        }
        return null;
    }

    private void EndTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        StartTurn();
    }
}
