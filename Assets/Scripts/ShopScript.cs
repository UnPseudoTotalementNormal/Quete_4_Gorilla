using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private GameScript gamemanager;

    private TMP_Dropdown MonkeDropdown;
    private TextMeshProUGUI MonkeDropdownText;
    private Transform Upgrades;
    private TextMeshProUGUI MoneyText;

    public void OpenShop()
    {
        RefreshMonkeDropdown();
        for (int i = 0; i < Upgrades.childCount; ++i)
        {
            Upgrades.GetChild(i).GetComponent<UpgradeScript>().Regen();
        }
    }
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
        MonkeDropdown = transform.Find("ChooseMonke").GetComponent<TMP_Dropdown>();
        MonkeDropdownText = MonkeDropdown.transform.Find("Label").GetComponent<TextMeshProUGUI>();
        Upgrades = transform.Find("Upgrades").transform;
        MoneyText = transform.Find("TextMoney").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!gamemanager.inshop) return;
        UpdateText();
    }

    private void UpdateText()
    {
        MonkeDropdownText.text = MonkeDropdown.options[MonkeDropdown.value].text;
        MoneyText.text = "$" + gamemanager.monke_money.ToString();
    }

    public void QuitShop()
    {
        gamemanager.inshop = false;
        Time.timeScale = 1;
    }

    private void RefreshMonkeDropdown()
    {
        MonkeDropdown.ClearOptions();
        Transform all_monkes = GameObject.Find("GameManager").transform.Find("Monkes");
        for (int i = 0; i < all_monkes.childCount; ++i)
        {
            TMP_Dropdown.OptionData newitem = new TMP_Dropdown.OptionData();
            newitem.text = all_monkes.GetChild(i).name;
            MonkeDropdown.options.Add(newitem);
        }
    }
    public void RenameMonke(TMP_InputField inputfield)
    {
        if (inputfield.text == "") return;
        Transform monkes = gamemanager.transform.Find("Monkes").transform ;
        for (int i = 0; i < monkes.childCount; ++i)
        {
            if (inputfield.text == monkes.GetChild(i).name)
            {
                
                return;
            }
        }
        GetSelectedMonke().name = inputfield.text;
        inputfield.text = "";
        RefreshMonkeDropdown();
    }

    private Transform GetSelectedMonke() //get monke selected on the ChooseMonke Dropdown
    {
        TMP_Dropdown MonkeDropdown = GameObject.Find("HUD").transform.Find("Shop").transform.Find("ChooseMonke").GetComponent<TMP_Dropdown>();
        string monkename = MonkeDropdown.options[MonkeDropdown.value].text;
        Transform Monke = gamemanager.transform.Find("Monkes").transform.Find(monkename).transform;
        return Monke;

    }
}
