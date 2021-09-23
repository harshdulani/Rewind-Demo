using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public Vector3 Position;
    public Quaternion Rotation;
    public float DeltaTime;

    public PointInTime(Vector3 pos, Quaternion rot, float delta)
    {
        Position = pos;
        Rotation = rot;
        //scale = sca;
        DeltaTime = delta;

        //maybe store only deltas, so you don't need to update positions on every frame if there is no change
    }
    
    //particle fx rewinder
    //animation.time
}

public class Rewindable : MonoBehaviour
{
    public List<PointInTime> pointsInTime;

    private static bool _isRewinding;
    private static float _elapsedRewind; //holds total of all delta times in all points

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
        pointsInTime = new List<PointInTime>();
    }

    private void Update()
    {
        if (_isRewinding)
            Rewind();
        else
            Record();
    }

    private void Rewind()
    {
        if (pointsInTime.Count == 0)
        {
            Rewinder.rew.InvokeStopRewind();
            return;
        }
        
        var point = pointsInTime[0];
        pointsInTime.RemoveAt(0);

        transform.position = point.Position;
        transform.rotation = point.Rotation;
        _elapsedRewind -= point.DeltaTime;
    }

    private void Record()
    {
        if (_elapsedRewind > Rewinder.rew.maxRewindTime)
        {
            var point = pointsInTime[pointsInTime.Count - 1];
            _elapsedRewind -= point.DeltaTime;
            
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        }
        
        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation, Time.deltaTime));
        _elapsedRewind += Time.deltaTime;
    }

    private void OnStartRewind()
    {
        _isRewinding = true;
    }
    
    private void OnStopRewind()
    {
        _isRewinding = false;
    }
}
