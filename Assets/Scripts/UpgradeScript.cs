using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeScript : MonoBehaviour
{
    private GameScript gamemanager;

    private Button BuyButton;
    private TextMeshProUGUI BuyButtonText;
    private TextMeshProUGUI UpgradeName;
    private TextMeshProUGUI UpgradeDescription;
    private Image UpgradeIcon;
    private Image Cross;

    private string upgrade_id;
    private bool already_purchased = false;
    private int cost;
    private bool accumulable;

    private void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
        UpgradeName = transform.Find("UpgradeName").GetComponentInChildren<TextMeshProUGUI>();
        UpgradeDescription = transform.Find("UpgradeDescription").GetComponent<TextMeshProUGUI>();
        UpgradeIcon = transform.Find("UpgradeIcon").GetComponent<Image>();
        Cross = transform.Find("Cross").GetComponent<Image>();
        BuyButton = transform.Find("BuyButton").GetComponent<Button>();
        BuyButtonText = BuyButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        Cross.enabled = already_purchased;
    }

    public void Regen()
    {
        already_purchased = false;
        RandomizeUpgrade();
    }
    public void buy()
    {
        if (cost <= gamemanager.monke_money && !already_purchased)
        {
            already_purchased = true;
            gamemanager.monke_money -= cost;
            GetUpgrade(upgrade_id);
        }
        else
        {

        }
    }

    private void GetUpgrade(string id)
    {
        Transform Monke = GetSelectedMonke();
        HealthComponent Monke_health = Monke.GetComponent<HealthComponent>();
        player Monke_script = Monke.GetComponent<player>();
        switch (id)
        {
            case "heal":
                Monke_health.GiveHp(1);
                break;
        }
    }
    private void RandomizeUpgrade()
    {
        int randomupgrade = Random.Range(0, 4);
        switch (randomupgrade)
        {
            case 0:
                upgrade_id = "heal";
                UpgradeName.text = "BANANA :)";
                UpgradeDescription.text = "Refill 1 hp\nhmmm banana";
                cost = 2;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = true;
                break;
            case 1:
                upgrade_id = "shield";
                UpgradeName.text = "MAGIC SHIELD";
                UpgradeDescription.text = "Resist 1 hit each wave";
                cost = 6;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = false;
                break;
            case 2:
                upgrade_id = "triple_shot";
                UpgradeName.text = "DART EXPERT";
                UpgradeDescription.text = "Throw 3 dart at the same time";
                cost = 3;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = false;
                break;
            case 3:
                upgrade_id = "speedboost";
                UpgradeName.text = "SPEEDRUNING MONKE";
                UpgradeDescription.text = "10% moving speed boost";
                cost = 1;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = true;
                break;
            case 4:
                upgrade_id = "multi_shot";
                UpgradeName.text = "DART MANIAC";
                UpgradeDescription.text = "Shoot 2 times per turn";
                cost = 6;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = true;
                break;
            case 5:
                upgrade_id = "higher_jump";
                UpgradeName.text = "PARKOUR TRAINING";
                UpgradeDescription.text = "Jump 10% higher";
                cost = 1;
                BuyButtonText.text = "$" + cost.ToString();
                accumulable = true;
                break;
        }
    }

    private Transform GetSelectedMonke() //get monke selected on the ChooseMonke Dropdown
    {
        TMP_Dropdown MonkeDropdown = GameObject.Find("HUD").transform.Find("Shop").transform.Find("ChooseMonke").GetComponent<TMP_Dropdown>();
        string monkename = MonkeDropdown.options[MonkeDropdown.value].text;
        Transform Monke = gamemanager.transform.Find("Monkes").transform.Find(monkename).transform;
        return Monke;

    }
}