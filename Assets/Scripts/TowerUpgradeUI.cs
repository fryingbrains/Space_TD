using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerUpgradeUI : MonoBehaviour
{
    public GameObject upgradePanel;
    public Button rangeButton;
    public Button damageButton;
    public Button fireRateButton;
    public Button elementalButton;
    public Button closeButton;

    public TextMeshProUGUI rangeText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI elementalText;
    public GameManager gameManager;
    private bool buttonClickedThisFrame = false;
    private Tower selectedTower;

    void Start()
    {
        upgradePanel.SetActive(false);
        gameManager = Camera.main.GetComponent<GameManager>();
        rangeButton.onClick.AddListener(UpgradeRange);
        damageButton.onClick.AddListener(UpgradeDamage);
        fireRateButton.onClick.AddListener(UpgradeFireRate);
    }

    public void ShowUpgradeMenu(Tower tower)
    {
        selectedTower = tower;
        upgradePanel.SetActive(true);

        // Position panel below tower
        Vector3 screenPos = Camera.main.WorldToScreenPoint(tower.transform.position);
        screenPos.y -= 100; // offset below tower
        upgradePanel.transform.position = screenPos;

        // Update button texts
        rangeText.text = $"Range +25%\n{tower.rangeUpgradeCost}G";
        damageText.text = $"Damage +25%\n{tower.damageUpgradeCost}G";
        fireRateText.text = $"Fire rate +25%\n{tower.fireRateUpgradeCost}G";
        elementalText.text = $"Elemental\n{tower.ElementalUpgradeCost}G";

        if (tower.rangeUpgradeCost > gameManager.playerGold) rangeButton.interactable = false;
        else rangeButton.interactable = true;


        if (tower.fireRateUpgradeCost > gameManager.playerGold) fireRateButton.interactable = false;
        else fireRateButton.interactable = true;


        if (tower.damageUpgradeCost > gameManager.playerGold) damageButton.interactable = false;
        else damageButton.interactable = true;
    }

    void UpgradeRange()
    {
        buttonClickedThisFrame = true;
        selectedTower.UpgradeRange();
        gameManager.UpdateUI();
        CloseMenu();

    }
    void UpgradeFireRate()
    {
        buttonClickedThisFrame = true;
        selectedTower.UpgradeFireRate();
        gameManager.UpdateUI();
        CloseMenu();
    }

    void UpgradeDamage()
    {
        buttonClickedThisFrame = true;
        selectedTower.UpgradeDamage();
        gameManager.UpdateUI();
        CloseMenu();
    }

    public void CloseMenu()
    {
        upgradePanel.SetActive(false);
        selectedTower = null;
    }
    void LateUpdate()
    {
        buttonClickedThisFrame = false; // reset at end of frame
    }

    public bool WasButtonClickedThisFrame()
    {
        return buttonClickedThisFrame;
    }
}