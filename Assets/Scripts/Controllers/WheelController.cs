using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelController : MonoBehaviour
{
    #region Controllers
    public SpinController SpinController;
    #endregion

    #region Constants
    private const int NumStops = 8;
    private const float WheelOffset = 22.5f;
    #endregion

    #region Fields
    private int WheelStop = 0;
    #endregion

    #region Inspector Variables
    /// <summary>
    /// The wheel game object.
    /// </summary>
    public GameObject Wheel;

    /// <summary>
    /// The pointer game object.
    /// </summary>
    public GameObject Pointer;

    /// <summary>
    /// The spin button object.
    /// </summary>
    public GameObject SpinButton;

    /// <summary>
    /// The win message text object.
    /// </summary>
    public Text WinText;

    /// <summary>
    /// The spin music audio source.
    /// </summary>
    public AudioSource SpinMusic;

    /// <summary>
    /// The award audio source.
    /// </summary>
    public AudioSource Award;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Hide();

        // SpinButton.GetComponent<Button>().OnButtonClicked += OnSpinButtonPressed;
    }

    public void Hide()
    {
        // Unregister spin event.
        SpinButton.GetComponent<Button>().OnButtonClicked -= OnSpinButtonPressed;
        SpinButton.GetComponent<UnityEngine.UI.Button>().enabled = false;
        // If the wheel is spinning, halt the spin.
        if(SpinController.CurrentState != SpinController.SpinState.Waiting)
        {
            // Unregister spin event.
            SpinController.OnNaturalSpinStop -= OnWheelStop;
            // Halt the spin.
            float stopAngle = WheelStop * (360.0f / NumStops) + WheelOffset;
            SpinController.HaltSpin(stopAngle);
            // Stop the music if playing.
            SpinMusic.Stop();
        }
        // Stop the award sound if playing.
        if(Award.isPlaying)
        {
            Award.Stop();
        }
        Wheel.SetActive(false);
        Pointer.SetActive(false);
        SpinButton.SetActive(false);
        WinText.gameObject.SetActive(false);
    }

    public void Show()
    {
        // Register spin event if wheel not currently spinning.
        if (SpinController.CurrentState == SpinController.SpinState.Waiting)
        {
            SpinButton.GetComponent<Button>().OnButtonClicked += OnSpinButtonPressed;
            SpinButton.GetComponent<UnityEngine.UI.Button>().enabled = true;
        }
        Wheel.SetActive(true);
        Pointer.SetActive(true);
        SpinButton.SetActive(true);
    }

    private void OnSpinButtonPressed(object sender, EventArgs eventArgs)
    {
        WinText.gameObject.SetActive(false);
        // Unregister spin button event while spinning.
        SpinButton.GetComponent<Button>().OnButtonClicked -= OnSpinButtonPressed;
        SpinButton.GetComponent<UnityEngine.UI.Button>().enabled = false;
        // Register wheel stop event.
        SpinController.OnNaturalSpinStop += OnWheelStop;
        // Get our stop.
        WheelStop = UnityEngine.Random.Range(0, NumStops);

        // Play the spin music.
        SpinMusic.Play();
        // Kill the award sound if it's playing.
        if(Award.isPlaying)
        {
            Award.Stop();
        }

        StartCoroutine(SpinToPosition());
    }

    private void OnWheelStop(object sender, EventArgs eventArgs)
    {
        // Unregister wheel stop event.
        SpinController.OnNaturalSpinStop -= OnWheelStop;

        // Show win message.
        WinText.gameObject.SetActive(true);
        WinText.text = "Wheel Award: " + WheelAwards.Awards[WheelStop];

        // Stop the spin music and play the award sound.
        SpinMusic.Stop();
        Award.Play();

        // Register spin button event.
        SpinButton.GetComponent<Button>().OnButtonClicked += OnSpinButtonPressed;
        SpinButton.GetComponent<UnityEngine.UI.Button>().enabled = true;
    }

    private IEnumerator SpinToPosition()
    {
        SpinController.StartSpin();

        yield return new WaitForSeconds(5.0f);

        float stopAngle = WheelStop * (360.0f / NumStops) + WheelOffset;
        SpinController.StopSpin(stopAngle);
    }
}
