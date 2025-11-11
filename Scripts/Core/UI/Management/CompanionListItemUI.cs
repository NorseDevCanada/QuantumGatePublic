using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionListItemUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI cpText;
    public Image iconImage;
    public Button equipButton;
    public Button unequipButton;

    private CompanionInstance companion;
    private CompanionManagementUI managerUI;

    public void Setup(CompanionInstance comp, CompanionManagementUI manager, bool isEquippedList)
    {
        companion = comp;
        managerUI = manager;

        nameText.text = comp.Data.CompanionName;
        levelText.text = $"Lv {comp.Level}";
        rarityText.text = comp.Rarity.ToString();
        cpText.text = $"CP: {comp.GetCombatPower():N0}";

        if (iconImage != null && comp.Data.Icon != null)
            iconImage.sprite = comp.Data.Icon;

        equipButton.gameObject.SetActive(!isEquippedList);
        unequipButton.gameObject.SetActive(isEquippedList);

        equipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.RemoveAllListeners();

        equipButton.onClick.AddListener(() => managerUI.OnEquip(comp));
        unequipButton.onClick.AddListener(() => managerUI.OnUnequip(comp));

        rarityText.color = GetRarityColor(comp.Rarity);
    }

    private Color GetRarityColor(CompanionRarity rarity)
    {
        switch (rarity)
        {
            case CompanionRarity.Common: return Color.gray;
            case CompanionRarity.Rare: return Color.blue;
            case CompanionRarity.Epic: return new Color(0.6f, 0.2f, 1f);
            case CompanionRarity.Legendary: return new Color(1f, 0.7f, 0.2f);
            case CompanionRarity.Mythic: return Color.red;
            default: return Color.white;
        }
    }
}
