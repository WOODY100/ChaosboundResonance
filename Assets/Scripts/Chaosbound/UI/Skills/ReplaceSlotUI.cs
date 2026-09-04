using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ReplaceSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text skillName;
    [SerializeField] private Button button;

    public void Initialize(string name, Action onClick)
    {
        skillName.text = name;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}