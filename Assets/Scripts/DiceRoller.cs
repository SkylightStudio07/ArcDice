using UnityEngine;

public static class DiceRoller
{
    // 아이템으로 보정값 넣는 것도 생각해야 할 듯. 일단은 1~6.
    public static int Roll() => Random.Range(1, 7);
}
