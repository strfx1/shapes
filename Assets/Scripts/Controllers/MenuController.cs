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

    /// <summary>
    /// Reference to the WheelController object.
    /// </summary>
    public WheelController WheelController;
    #endregion

    #region Fields
    private bool MenuVisible;
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

    /// <summary>
    /// The pentagon menu button.
    /// </summary>
    public GameObject Pentagon;

    /// <summary>
    /// The wheel game menu button.
    /// </summary>
    public GameObject WheelGame;
    #endregion

    private void Start()
    {
        MenuVisible = false;
        UpdateMenu();

        // Register Events
        MainMenu.GetComponent<Button>().OnButtonClicked += OnMainMenuButtonClicked;
        Hexagon.GetComponent<Button>().OnButtonClicked += OnHexagonClicked;
        Triangle.GetComponent<Button>().OnButtonClicked += OnTriangleClicked;
        Square.GetComponent<Button>().OnButtonClicked += OnSquareClicked;
        Pentagon.GetComponent<Button>().OnButtonClicked += OnPentagonClicked;
        WheelGame.GetComponent<Button>().OnButtonClicked += OnWheelClicked;
    }

    private void UpdateMenu()
    {
        // Toggle additional menu items on or off based on flag.
        Hexagon.SetActive(MenuVisible);
        Triangle.SetActive(MenuVisible);
        Square.SetActive(MenuVisible);
        Pentagon.SetActive(MenuVisible);
        WheelGame.SetActive(MenuVisible);
    }

    private void OnMainMenuButtonClicked(object sender, EventArgs eventArgs)
    {
        MenuVisible = !MenuVisible;
        UpdateMenu();
    }

    private void OnHexagonClicked(object sender, EventArgs eventArgs)
    {
        CreateShape(ShapeController.ShapeType.Hexagon);
    }

    private void OnTriangleClicked(object sender, EventArgs eventArgs)
    {
        CreateShape(ShapeController.ShapeType.Triangle);
    }

    private void OnSquareClicked(object sender, EventArgs eventArgs)
    {
        CreateShape(ShapeController.ShapeType.Square);
    }

    private void OnPentagonClicked(object sender, EventArgs eventArgs)
    {
        CreateShape(ShapeController.ShapeType.Pentagon);
    }

    private void CreateShape(ShapeController.ShapeType shapeType)
    {
        ShapeController.CreateShape(shapeType);
        WheelController.Hide();
        MenuVisible = false;
        UpdateMenu();
    }

    private void OnWheelClicked(object sender, EventArgs eventArgs)
    {
        ShapeController.HideShape();
        WheelController.Show();
        MenuVisible = false;
        UpdateMenu();
    }
}
