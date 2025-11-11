using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GearPopupUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupRoot;
    public TextMeshProUGUI newGearNameText;
    public TextMeshProUGUI newGearStatsText;
    public TextMeshProUGUI oldGearNameText;
    public TextMeshProUGUI oldGearStatsText;
    public Button equipButton;
    public Button sellButton;
    public Button cancelButton;
    public Button nextButton;

    private int slotIndex;
    private Queue<Gear> gearQueue = new Queue<Gear>();
    private Gear currentGear;

    void Awake()
    {
        popupRoot.SetActive(false);

        equipButton.onClick.AddListener(OnEquipClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        nextButton.onClick.AddListener(OnNextClicked);
    }

    public void ShowBatch(int slot, List<Gear> newGears)
    {
        slotIndex = slot;
        gearQueue = new Queue<Gear>(newGears);
        ShowNextGear();
    }

    private void ShowNextGear()
    {
        if (gearQueue.Count == 0)
        {
            ClosePopup();
            return;
        }

        currentGear = gearQueue.Dequeue();
        popupRoot.SetActive(true);

        Gear oldGear = PlayerManager.Instance.GetGear(slotIndex);

        newGearNameText.text = $"{currentGear.Name} ({currentGear.Rarity})";
        newGearStatsText.text = $"Power: {currentGear.PowerBonus}\nOther stats...";

        if (oldGear != null)
        {
            oldGearNameText.text = $"{oldGear.Name} ({oldGear.Rarity})";
            oldGearStatsText.text = $"Power: {oldGear.PowerBonus}\nOther stats...";
        }
        else
        {
            oldGearNameText.text = "Empty Slot";
            oldGearStatsText.text = "-";
        }
    }

    private void OnEquipClicked()
    {
        Gear oldGear = PlayerManager.Instance.GetGear(slotIndex);
        PlayerManager.Instance.EquipGear(slotIndex, currentGear);

        if (oldGear != null)
        {
            int creditsGained = Mathf.Max(1, oldGear.PowerBonus * 10);
            int xpGained = Mathf.Max(1, oldGear.PowerBonus * 2);
            CurrencyManager.Instance.AddCredits(creditsGained);
            PlayerManager.Instance.AddXP(xpGained);
        }

        ShowNextGear();
    }

    private void OnSellClicked()
    {
        int creditsGained = Mathf.Max(1, currentGear.PowerBonus * 10);
        int xpGained = Mathf.Max(1, currentGear.PowerBonus * 2);
        CurrencyManager.Instance.AddCredits(creditsGained);
        PlayerManager.Instance.AddXP(xpGained);

        ShowNextGear();
    }

    private void OnCancelClicked()
    {
        ShowNextGear();
    }

    private void OnNextClicked()
    {
        ShowNextGear();
    }

    private void ClosePopup()
    {
        popupRoot.SetActive(false);
        gearQueue.Clear();
        currentGear = null;
    }
}
