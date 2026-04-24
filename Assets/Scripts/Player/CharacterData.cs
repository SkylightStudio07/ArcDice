using UnityEngine;


[CreateAssetMenu(fileName = "CharacterData", menuName = "BoardObjects/CharacterData")]
public class CharacterData : ScriptableObject
{

    public string characterName;

    [Header("캐릭터 스프라이트")]
    public Sprite portrait;
    public Sprite idle;
    public Sprite rollDice;
    public Sprite battleStart;
    public Sprite attack;
    public Sprite damaged;
    public Sprite lose;
    public Sprite win;
    public Sprite victory;

}
