using System.Collections;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private static readonly WaitForSeconds WaitDiceResult  = new(2f);
    private static readonly WaitForSeconds WaitShort       = new(0.5f);
    private static readonly WaitForSeconds WaitResultShow  = new(2.5f);

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public IEnumerator RunCombat(PlayerBehaviour challenger, PlayerBehaviour defender)
    {
        var ui = UIManager.Instance;

        ui.SetupBattlePhase(challenger, defender);
        ui.ShowBattlePhasePanel(true);
        yield return WaitShort;

        // 1. 선공 결정 — 동률 시 재굴림
        int cRoll, dRoll;
        do
        {
            cRoll = DiceRoller.Roll();
            dRoll = DiceRoller.Roll();
            yield return StartCoroutine(ui.ShowInitiativeRoll(cRoll, dRoll));
        } while (cRoll == dRoll);

        bool challengerFirst = cRoll > dRoll;
        PlayerBehaviour first  = challengerFirst ? challenger : defender;
        PlayerBehaviour second = challengerFirst ? defender   : challenger;
        Debug.Log($"[선공] {first.name} ({(challengerFirst ? cRoll : dRoll)} vs {(challengerFirst ? dRoll : cRoll)})");

        // 2. 라운드 1 (선공 공격, 후공 방어)
        yield return StartCoroutine(RunRound(first, second, first == challenger, challenger, defender));

        // 3. 라운드 2 (역할 교체)
        yield return StartCoroutine(RunRound(second, first, second == challenger, challenger, defender));

        // 4. 결과 표시
        bool challengerWon = challenger.State.HP >= defender.State.HP;
        ui.ShowBattleResult(challengerWon);
        yield return WaitResultShow;

        ui.ShowBattlePhasePanel(false);
    }

    private IEnumerator RunRound(
        PlayerBehaviour attacker, PlayerBehaviour roundDefender,
        bool attackerIsChallenger,
        PlayerBehaviour challenger, PlayerBehaviour defender)
    {
        var ui = UIManager.Instance;
        ui.ClearOverlayTexts();

        // 공격 굴림
        int attackRoll = DiceRoller.Roll();
        yield return StartCoroutine(ui.ShowBattleDiceRoll(attackerIsChallenger, attackRoll));
        yield return WaitDiceResult;

        // 방어 굴림
        int defenseRoll = DiceRoller.Roll();
        yield return StartCoroutine(ui.ShowBattleDiceRoll(!attackerIsChallenger, defenseRoll));
        yield return WaitDiceResult;

        // 피해 계산 — 공격 > 방어일 때만 초과분만큼 피해
        if (attackRoll > defenseRoll)
        {
            int damage = attackRoll - defenseRoll;
            roundDefender.State.HP = Mathf.Max(0, roundDefender.State.HP - damage);
            Debug.Log($"{attacker.name} → {roundDefender.name}: {attackRoll} - {defenseRoll} = {damage} 피해 / 잔여 HP: {roundDefender.State.HP}");
        }
        else
        {
            Debug.Log($"{attacker.name} → {roundDefender.name}: {attackRoll} ≤ {defenseRoll} 무효");
        }

        if (roundDefender.State.HP <= 0)
            roundDefender.State.IsDown = true;

        ui.UpdateBattleHP(challenger.State.HP, defender.State.HP);
    }
}
