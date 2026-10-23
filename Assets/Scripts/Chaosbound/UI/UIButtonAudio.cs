using UnityEngine;
using UnityEngine.EventSystems;
using Chaosbound.Core.Audio;

public sealed class UIButtonAudio :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerClickHandler,
    ISelectHandler,
    ISubmitHandler
{
    [Header("Sound Settings")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playConfirmSound = true;

    private UIAudioPlayer audioPlayer;

    private void Awake()
    {
        audioPlayer = FindAnyObjectByType<UIAudioPlayer>();

        if (audioPlayer == null)
        {
            Debug.LogWarning(
                "[UIButtonAudio] UIAudioPlayer was not found.",
                this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHoverSound)
            PlayHover();
    }

    private bool ignoreInitialSelection = true;

    public void OnSelect(BaseEventData eventData)
    {
        if (ignoreInitialSelection)
        {
            ignoreInitialSelection = false;
            return;
        }

        if (playHoverSound)
            PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playConfirmSound)
            PlayConfirm();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (playConfirmSound)
            PlayConfirm();
    }

    private void PlayHover()
    {
        if (audioPlayer != null)
            audioPlayer.PlayHover();
    }

    private void PlayConfirm()
    {
        if (audioPlayer != null)
            audioPlayer.PlayConfirm();
    }
}