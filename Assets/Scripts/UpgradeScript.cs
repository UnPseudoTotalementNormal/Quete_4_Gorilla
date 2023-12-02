using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
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
    private int accumulable_max;

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
            if (GetUpgrade(upgrade_id))
            {
                gamemanager.monke_money -= cost;
                already_purchased = true;
            }
        }
        else
        {

        }
    }

    private bool GetUpgrade(string id)
    {
        Transform Monke = GetSelectedMonke();
        HealthComponent Monke_health = Monke.GetComponent<HealthComponent>();
        player Monke_script = Monke.GetComponent<player>();
        switch (id)
        {
            case "heal":
                Monke_health.GiveHp(1);
                break;
            case "speedboost":
                Monke_script.walking_speed *= 1.20f;
                break;
            case "higher_jump":
                Monke_script.jump_force *= 1.15f;
                break;
            case "bigger_explosion_radius":
                Monke_script.explosion_radius += 0.5f;
                break;
            case "shield":
                Monke_health.magic_shield_max += 1;
                break;
            case "triple_shot":
                Monke_script.triple_shot = true;
                break;
            case "multi_shot":
                Monke_script.multi_shot += 1;
                break;
            case "visual_aiming":
                Monke_script.visual_aiming = true;
                break;
            case "higher_force":
                Monke_script._maxforce += 1; 
                break;
            case "charge_speed_boost":
                Monke_script._chargingspeed += 2;
                break;
            case "camera_unzoom":
                gamemanager.camera_normal_zoom += 1;
                break;
        }
        return true;
    }
    private void RandomizeUpgrade()
    {
        int randomupgrade = Random.Range(0, 11);
        switch (randomupgrade)
        {
            case 0:
                upgrade_id = "heal";
                UpgradeName.text = "BANANA :)";
                UpgradeDescription.text = "Refill 1 hp (max 5)\nhmmm banana";
                cost = 2;
                accumulable_max = 99999;
                break;
            case 1:
                upgrade_id = "shield";
                UpgradeName.text = "MAGIC SHIELD";
                UpgradeDescription.text = "Resist +1 hit each wave";
                cost = 6;
                accumulable_max = 99999;
                break;
            case 2:
                upgrade_id = "triple_shot";
                UpgradeName.text = "DART EXPERT";
                UpgradeDescription.text = "Throw 3 dart at the same time";
                cost = 5;
                accumulable_max = 1;
                accumulable_max = 1;
                break;
            case 3:
                upgrade_id = "speedboost";
                UpgradeName.text = "SPEEDRUNING MONKE";
                UpgradeDescription.text = "+20% moving speed boost";
                cost = 2;
                accumulable_max = 99999;
                break;
            case 4:
                upgrade_id = "multi_shot";
                UpgradeName.text = "DART MANIAC";
                UpgradeDescription.text = "Shoot 1 more time per turn";
                cost = 8;
                accumulable_max = 99999;
                break;
            case 5:
                upgrade_id = "higher_jump";
                UpgradeName.text = "JETPACK UPGRADE";
                UpgradeDescription.text = "+15% jump height";
                cost = 2;
                accumulable_max = 99999;
                break;
            case 6:
                upgrade_id = "bigger_explosion_radius";
                UpgradeName.text = "BIGGER WEAPONS";
                UpgradeDescription.text = "Explosion radius is 1 meter bigger";
                cost = 2;
                accumulable_max = 99999;
                break;
            case 7:
                upgrade_id = "visual_aiming";
                UpgradeName.text = "AIMING TRAINING";
                UpgradeDescription.text = "Display the start of your projectile trajectory";
                cost = 3;
                accumulable_max = 1;
                break;
            case 8:
                upgrade_id = "higher_force";
                UpgradeName.text = "MONKE = STRONG";
                UpgradeDescription.text = "+1 m/s max projectile throw speed";
                cost = 1;
                accumulable = true;
                accumulable_max = 99999;
                break;
            case 9:
                upgrade_id = "charge_speed_boost";
                UpgradeName.text = "PREDATOR REFLEX";
                UpgradeDescription.text = "Charge your throw force faster (+2m/s)";
                cost = 1;
                accumulable = true;
                accumulable_max = 99999;
                break;
            case 10:
                upgrade_id = "camera_unzoom";
                UpgradeName.text = "SNIPER MONKE";
                UpgradeDescription.text = "Camera unzoom a little bit";
                cost = 2;
                accumulable = true;
                accumulable_max = 15;
                break;
        }
        BuyButtonText.text = "$" + cost.ToString();
        Sprite newsprite = Resources.Load<Sprite>("Sprites/Upgrades/" + upgrade_id);
        if (newsprite != null)
        {
            UpgradeIcon.sprite = newsprite;
        }
        else
        {
            UpgradeIcon.sprite = Resources.Load<Sprite>("Sprites/Upgrades/upgrade");
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