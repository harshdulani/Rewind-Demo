using System;
using UnityEngine;

public class Rewinder : MonoBehaviour
{
    public static Rewinder rew;
    public Action startRewind, stopRewind;
    
    public float maxRewindTime = 5f;
    public bool isRewinding;
    
    private void Awake()
    {
        if (!rew) rew = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            StartRewinding();
        else if (Input.GetKeyUp(KeyCode.Tab))
            StopRewinding();
    }

    private void StartRewinding()
    {
        isRewinding = true;
        startRewind?.Invoke();
    }

    private void StopRewinding()
    {
        isRewinding = false;
        InvokeStopRewind();
    }

    public void InvokeStopRewind()
    {
        stopRewind?.Invoke();
    }
}
