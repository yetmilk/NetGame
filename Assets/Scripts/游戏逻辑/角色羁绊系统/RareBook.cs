using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "√ÿºÆ", menuName = "≈‰÷√/√ÿºÆ≈‰÷√")]
public class RareBook : ScriptableObject
{
    public RareBookName bookName;

    public SectType type;

    public List<BuffName> buff;

    public Sprite Sprite;

    public string description;
}
