using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LocalizationData", menuName = "Localization/Localization Data", order = 1)]
public class LocalizationData : ScriptableObject
{
    public List<LocalizationEntry> entries = new List<LocalizationEntry>();
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    public string englishText;
    public string frenchText;
}