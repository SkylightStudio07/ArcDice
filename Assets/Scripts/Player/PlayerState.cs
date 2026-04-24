using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class PlayerState
{
    public const int MaxHP = 6;

    public int HP { get; set; } = MaxHP;
    public int Flags { get; set; } = 0;
    public bool IsDown { get; set; } = false;
    public List<ItemData> Items { get; } = new List<ItemData>();
}
    