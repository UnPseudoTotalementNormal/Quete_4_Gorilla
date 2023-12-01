using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopScript : MonoBehaviour
{
    private GameScript gamemanager;

    private TMP_Dropdown MonkeDropdown;
    private TextMeshProUGUI MonkeDropdownText;


    public void OpenShop()
    {
        MonkeDropdown.ClearOptions();
        Transform all_monkes = GameObject.Find("GameManager").transform.Find("Monkes");
        for (int i  = 0; i < all_monkes.childCount; ++i)
        {
            TMP_Dropdown.OptionData newitem = new TMP_Dropdown.OptionData();
            newitem.text = all_monkes.GetChild(i).name;
            MonkeDropdown.options.Add(newitem);
        }
    }
    void Start()
    {
        gamemanager = GameObject.Find("GameManager").GetComponent<GameScript>();
        MonkeDropdown = transform.Find("ChooseMonke").GetComponent<TMP_Dropdown>();
        MonkeDropdownText = MonkeDropdown.transform.Find("Label").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (!gamemanager.inshop) return;
        UpdateText();
        
    }

    private void UpdateText()
    {
        MonkeDropdownText.text = MonkeDropdown.options[MonkeDropdown.value].text;
    }

    public void QuitShop()
    {
        gamemanager.inshop = false;
        Time.timeScale = 1;
    }
}
