using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using UnityEngine;

public class ConfirmCancelPrompt : MonoBehaviour, IMixedRealityInputActionHandler
{
    /// <summary>
    /// Feedback audio for doubletap / singletap respectively
    /// </summary>
    [SerializeField]
    private AudioClip confirmClip;
    [SerializeField]
    private AudioClip cancelClip;

    /// <summary>
    /// Duration between possible double tap - set in unity inspector
    /// </summary>
    [SerializeField]
    private float clickTime = 1f;


    /// <summary>
    /// Reference for button that initiates the prompt --temporary placeholder
    /// </summary>
    [SerializeField]
    private GameObject button;

    /// <summary>
    /// Reference for canvas displaying both text and feedforward illustration
    /// </summary>
    [SerializeField]
    private GameObject canvas;


    /// <summary>
    /// Determines if prompt is ready to be spawned
    /// </summary>
    private bool isReady = true;

    /// <summary>
    /// reference to the current routine determining possible doubletap: if null there is no potential for double tap
    /// </summary>
    private Coroutine _tapRoutine;

    /// <summary>
    /// used to fix double input by MRTK
    /// </summary>
    private int tapCount = 0;


    /* Public Methods */
    public void SpawnPrompt()
    {
        if (isReady && button && canvas)
        {
            button.SetActive(false);
            canvas.SetActive(true);
        }
    }

    /* Private Methods */

    /// <summary>
    /// Sets up respective fallback handlers for the input-action (click/select)
    /// Such that if no other gameobject responds to the interactable-select action
    /// the respective (OnActionEnded/OnActionStarted) is triggered
    /// </summary>
    private void OnEnable()
    {
        CoreServices.InputSystem?.PushFallbackInputHandler(gameObject);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.PopFallbackInputHandler();
    }

    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (isReady)
        {
            tapCount++;
            if (tapCount == 2)
            {

                if (_tapRoutine == null)
                {
                    _tapRoutine = StartCoroutine(TapTimer());
                }
                else
                {
                    StopCoroutine(_tapRoutine);
                    _tapRoutine = null;

                    PlayAudio(cancelClip);

                }
                tapCount = 0;
            }
        }
    }

    private IEnumerator TapTimer()
    {
        Time.timeScale = 1f;
        yield return new WaitForSeconds(clickTime);

        _tapRoutine = null;

        PlayAudio(confirmClip);
    }

    private void PlayAudio(AudioClip clipToPlay)
    {
        isReady = false;
        var audio = GetComponent<AudioSource>();
        audio.clip = clipToPlay;

        audio.Play();

        StartCoroutine(RemovePrompt());
    }

    private IEnumerator RemovePrompt()
    {
        var audioLength = GetComponent<AudioSource>()?.clip?.length;

        if (audioLength.HasValue)
        {
            canvas?.SetActive(false);

            yield return new WaitForSeconds(audioLength.Value + 0.5f);

            _tapRoutine = null;
            tapCount = 0;
            GetComponent<AudioSource>().clip = null;

            button.SetActive(true);

            isReady = true;
        }
    }

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        //not used
    }
}
