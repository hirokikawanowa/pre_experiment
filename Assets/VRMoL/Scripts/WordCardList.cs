using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordCardData
{
    public string word;
    public Sprite image;
}

[CreateAssetMenu(menuName = "VRMoL/WordCardList")]
public class WordCardList : ScriptableObject
{
    public List<WordCardData> cards;
} 