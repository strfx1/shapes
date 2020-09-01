using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region Controllers
    /// <summary>
    /// Reference to the ShapeController object.
    /// </summary>
    public ShapeController ShapeController;
    #endregion

    #region Fields
    private bool MenuVisibile;
    #endregion

    #region Inspector Variables
    /// <summary>
    /// The main menu button.
    /// </summary>
    public GameObject MainMenu;

    /// <summary>
    /// The hexagon menu button.
    /// </summary>
    public GameObject Hexagon;

    /// <summary>
    /// The triangle menu button.
    /// </summary>
    public GameObject Triangle;

    /// <summary>
    /// The square menu button.
    /// </summary>
    public GameObject Square;
    #endregion

    private void Start()
    {
        MenuVisibile = false;
        UpdateMenu();

        // Register Events
        MainMenu.GetComponent<Button>().OnButtonClicked += OnMainMenuButtonClicked;
        Hexagon.GetComponent<Button>().OnButtonClicked += OnHexagonClicked;
        Triangle.GetComponent<Button>().OnButtonClicked += OnTriangleClicked;
        Square.GetComponent<Button>().OnButtonClicked += OnSquareClicked;
    }

    private void UpdateMenu()
    {
        // Toggle additional menu items on or off based on flag.
        Hexagon.SetActive(MenuVisibile);
        Triangle.SetActive(MenuVisibile);
        Square.SetActive(MenuVisibile);
    }

    private void OnMainMenuButtonClicked(object sender, EventArgs eventArgs)
    {
        MenuVisibile = !MenuVisibile;
        UpdateMenu();
    }

    private void OnHexagonClicked(object sender, EventArgs eventArgs)
    {
        ShapeController.CreateHexagon();
        MenuVisibile = false;
        UpdateMenu();
    }

    private void OnTriangleClicked(object sender, EventArgs eventArgs)
    {
        ShapeController.CreateTriangle();
        MenuVisibile = false;
        UpdateMenu();
    }

    private void OnSquareClicked(object sender, EventArgs eventArgs)
    {
        ShapeController.CreateSquare();
        MenuVisibile = false;
        UpdateMenu();
    }
}
