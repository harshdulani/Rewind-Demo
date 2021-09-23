using TMPro;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private TMP_Text rewindText;
    
    private void OnEnable()
    {
        Rewinder.rew.startRewind += OnStartRewind;
        Rewinder.rew.stopRewind += OnStopRewind;
    }
    
    private void OnDisable()
    {
        Rewinder.rew.startRewind -= OnStartRewind;
        Rewinder.rew.stopRewind -= OnStopRewind;
    }
    
    private void Start()
    {
        rewindText.enabled = false;
    }

    private void OnStartRewind()
    {
        rewindText.enabled = true;
    }
    
    private void OnStopRewind()
    {
        rewindText.enabled = false;
    }
}
