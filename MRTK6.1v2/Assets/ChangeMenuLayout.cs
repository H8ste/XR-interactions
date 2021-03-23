using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenuLayout : MonoBehaviour
{
    [SerializeField]
    private GameObject menu1;
    [SerializeField]
    private GameObject menu2;
    [SerializeField]
    MenuRaycaster menuRaycasterHandler;

    bool isMenuOne = true;

    /* Public Methods */

    /// <summary>
    /// Triggers a switch of menus (scrollHandler)
    /// </summary>
    public void SwitchMenu()
    {
        if (isMenuOne)
        {
            menu1.SetActive(false);
            menu2.SetActive(true);
        } else {
            menu1.SetActive(true);
            menu2.SetActive(false);
        }

        isMenuOne = !isMenuOne;

        menuRaycasterHandler.SetupScrollHandler(isMenuOne ? menu1 : menu2);
    }

    /* Private Methods */
    void Start()
    {
        if (isMenuOne)
        {
            menu2.SetActive(false);
            menu1.SetActive(true);
        } else {
            menu1.SetActive(false);
            menu2.SetActive(true);
        }
    }
}
