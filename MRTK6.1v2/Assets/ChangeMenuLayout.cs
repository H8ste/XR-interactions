using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenuLayout : MonoBehaviour
{
    public GameObject menu1;
    public GameObject menu2;

    bool menu1Active = true;
    bool menu2Active = false;

    void Start()
    {
        menu2.SetActive(false);
    }


    public void SwitchMenu()
    {
        if (menu1Active && !menu2Active)
        {
            menu1.SetActive(false);
            menu1Active = false;
            menu2.SetActive(true);
            menu2Active = true;
        }else if(menu2Active && !menu1Active)
        {
            menu2.SetActive(false);
            menu2Active = false;
            menu1.SetActive(true);
            menu1Active = true;
        }
    }
}
