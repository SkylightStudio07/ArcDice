using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI turnText;

    [Header("플레이어 관련 HUD 스프라이트 처리")]
    [SerializeField] private TextMeshProUGUI[] playerHPTexts;
    [SerializeField] private TextMeshProUGUI[] playerNameTexts;
    [SerializeField] private Image[] playerPortraits;
    [SerializeField] private Image[] playerRollDiceImages; // 주사위 굴릴 때 패널 위에 띄우는 거.
    [SerializeField] private Image[] playerResultImages; // 승패 결과 띄우는 거. 승리, 패배 각각 다른 이미지로.
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("주사위 관련")]
    [SerializeField] private GameObject dicePanel;
    [SerializeField] private GameObject Player1DicePanel;
    [SerializeField] private GameObject Player2DicePanel;
    [SerializeField] private Image diceImage;
    [SerializeField] private Sprite[] diceSprites; // 1~6 순서대로

    [Header("배틀 인트로")]
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private Image battleDefenderPortrait;
    [SerializeField] private TextMeshProUGUI battleDefenderNameText;
    [SerializeField] private TextMeshProUGUI battleDefenderHPText;

    [Header("배틀 페이즈")]
    [SerializeField] private GameObject battlePhasePanel;
    [SerializeField] private Image challengerPortrait;
    [SerializeField] private Image defenderPortrait;
    [SerializeField] private TextMeshProUGUI challengerNameText;
    [SerializeField] private TextMeshProUGUI defenderNameText;
    [SerializeField] private TextMeshProUGUI challengerHPText;
    [SerializeField] private TextMeshProUGUI defenderHPText;
    [SerializeField] private Image challengerDiceImage;
    [SerializeField] private Image defenderDiceImage;
    [SerializeField] private TextMeshProUGUI challengerOverlayText;
    [SerializeField] private TextMeshProUGUI defenderOverlayText;
    [SerializeField] private TextMeshProUGUI challengerResultText;
    [SerializeField] private TextMeshProUGUI defenderResultText;

    [Header("부활")]
    [SerializeField] private GameObject revivalPanel;
    [SerializeField] private Image revivalPortrait;
    [SerializeField] private Image revivalDiceImage;
    [SerializeField] private TextMeshProUGUI revivalRequiredText;
    [SerializeField] private TextMeshProUGUI revivalResultText;

    private CharacterData _challengerData;
    private CharacterData _defenderData;

    private static readonly WaitForSeconds WaitPanelDelay    = new(0.5f);
    private static readonly WaitForSeconds WaitResultDelay   = new(2f);
    private static readonly WaitForSeconds WaitBattleIntro   = new(2.5f);
    private static readonly WaitForSeconds WaitInitiativeGap = new(1.0f);
    private static readonly WaitForSeconds WaitInitiativeEnd = new(1.5f);
    private static readonly WaitForSeconds WaitRevivalResult  = new(2f);

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        dicePanel.SetActive(false);
        battlePanel.SetActive(false);
        battlePhasePanel.SetActive(false);
        revivalPanel.SetActive(false);
    }

    public void UpdateTurnText(int playerIndex)
    {
        turnText.text = $"플레이어 {playerIndex} 턴";
    }

    public void UpdateHPText(int playerIndex, int hp)
    {
        playerHPTexts[playerIndex].text = $"HP : {hp}";
    }

    public void SetPortrait(int playerIndex, CharacterData data)
    {
        playerPortraits[playerIndex].sprite = data.portrait;
        playerNameTexts[playerIndex].text = data.characterName;
    }

    public void SetDiceRollImage(int playerIndex, CharacterData data)
    {
        playerRollDiceImages[playerIndex].sprite = data.rollDice;
    }

    

    public IEnumerator ShowBattleIntro(CharacterData defender, int defenderHP)
    {
        var parent = battlePanel.transform.parent;
        bool parentActive = parent != null && parent.gameObject.activeInHierarchy;
        Debug.Log($"[ShowBattleIntro] activeSelf:{battlePanel.activeSelf} / activeInHierarchy:{battlePanel.activeInHierarchy} / parentActive:{parentActive}");

        battleDefenderPortrait.sprite = defender.battleStart;
        battleDefenderNameText.text = defender.characterName;
        battleDefenderHPText.text = $"HP : {defenderHP}";

        battlePanel.SetActive(true);
        yield return WaitBattleIntro;
        battlePanel.SetActive(false);
    }

    // 여기서부터는 배틀 페이즈 명시

    public void SetupBattlePhase(PlayerBehaviour challenger, PlayerBehaviour defender)
    {
        _challengerData = challenger.CharacterData;
        _defenderData   = defender.CharacterData;

        challengerPortrait.sprite = _challengerData.battleStart;
        var cs = challengerPortrait.rectTransform.localScale;
        challengerPortrait.rectTransform.localScale = new Vector3(-Mathf.Abs(cs.x), cs.y, cs.z);

        defenderPortrait.sprite = _defenderData.battleStart;
        var ds = defenderPortrait.rectTransform.localScale;
        defenderPortrait.rectTransform.localScale = new Vector3(Mathf.Abs(ds.x), ds.y, ds.z);

        challengerNameText.text = _challengerData.characterName;
        defenderNameText.text   = _defenderData.characterName;

        challengerResultText.text = "";
        defenderResultText.text   = "";

        UpdateBattleHP(challenger.State.HP, defender.State.HP);
    }

    public void ShowBattleResult(bool challengerWon)
    {
        challengerResultText.text = challengerWon ? "Win!" : "Lose!";
        defenderResultText.text   = challengerWon ? "Lose!" : "Win!";

        challengerPortrait.sprite = challengerWon ? _challengerData.win : _challengerData.lose;
        defenderPortrait.sprite   = challengerWon ? _defenderData.lose  : _defenderData.win;
    }

    public void ShowBattlePhasePanel(bool show) => battlePhasePanel.SetActive(show);

    // ── 부활 패널 ──────────────────────────────────────────

    public void ShowRevivalPanel(CharacterData data, int required)
    {
        revivalPortrait.sprite    = data.idle;
        revivalRequiredText.text  = $"{required} 이상을 굴려서";
        revivalResultText.text    = "";
        revivalPanel.SetActive(true);
    }

    public IEnumerator ShowRevivalResult(int roll, bool success, CharacterData data)
    {
        yield return StartCoroutine(AnimateDice(revivalDiceImage, roll));
        revivalPortrait.sprite = success ? data.win : data.lose;
        revivalResultText.text = success ? $"{roll} — 부활!" : $"{roll} — 실패...";
        yield return WaitRevivalResult;
    }

    public void HideRevivalPanel() => revivalPanel.SetActive(false);

    public void UpdateBattleHP(int challengerHP, int defenderHP)
    {
        challengerHPText.text = $"HP : {challengerHP}";
        defenderHPText.text   = $"HP : {defenderHP}";
    }

    public void ClearOverlayTexts()
    {
        challengerOverlayText.text = "";
        defenderOverlayText.text   = "";
    }

    public IEnumerator ShowInitiativeRoll(int challengerRoll, int defenderRoll)
    {
        ClearOverlayTexts();
        yield return StartCoroutine(AnimateDice(challengerDiceImage, challengerRoll));
        challengerOverlayText.text = challengerRoll.ToString();
        yield return WaitInitiativeGap;
        yield return StartCoroutine(AnimateDice(defenderDiceImage, defenderRoll));
        defenderOverlayText.text = defenderRoll.ToString();
        yield return WaitInitiativeEnd;
    }

    public IEnumerator ShowBattleDiceRoll(bool isChallenger, int result)
    {
        Image target = isChallenger ? challengerDiceImage : defenderDiceImage;
        yield return StartCoroutine(AnimateDice(target, result));

        if (isChallenger) challengerOverlayText.text = result.ToString();
        else              defenderOverlayText.text   = result.ToString();
    }

    private IEnumerator AnimateDice(Image diceImg, int result)
    {
        float duration = 1.2f;
        float elapsed = 0f;
        float interval = 0.05f;
        int idx = 0;
        while (elapsed < duration)
        {
            diceImg.sprite = diceSprites[idx % diceSprites.Length];
            idx++;
            elapsed += interval;
            yield return new WaitForSeconds(interval);
            interval = Mathf.Lerp(0.05f, 0.25f, elapsed / duration);
        }
        diceImg.sprite = diceSprites[result - 1];
    }

    public IEnumerator ShowDiceRoll(int result, int playerIndex)
    {
        // 1. 패널 활성화
        dicePanel.SetActive(true);
        if (playerIndex == 0)
            Player1DicePanel.SetActive(true);
        else if (playerIndex == 1)
            Player2DicePanel.SetActive(true);

        yield return WaitPanelDelay;

        // 2. 주사위 굴리기 애니메이션
        float duration = 0.6f;
        float elapsed = 0f;
        float interval = 0.05f;
        int spriteIndex = 0;

        Vector2 originPos = diceImage.rectTransform.anchoredPosition;

        while (elapsed < duration)
        {
            diceImage.sprite = diceSprites[spriteIndex % diceSprites.Length];
            spriteIndex++;

            float arc = Mathf.Sin(elapsed / duration * Mathf.PI) * 80f;
            diceImage.rectTransform.anchoredPosition = originPos + Vector2.up * arc;

            elapsed += interval;
            yield return new WaitForSeconds(interval);
        }

        // 다이스 결과 보여줘야 하는데... 이미지는 나중에 넣자...
        diceImage.sprite = diceSprites[result - 1];
        diceImage.rectTransform.anchoredPosition = originPos;

        yield return WaitResultDelay;

        // 패널 마지막 비활성화 ㅊ ㅓ리
        dicePanel.SetActive(false);
        Player1DicePanel.SetActive(false);
        Player2DicePanel.SetActive(false);
    }
}
