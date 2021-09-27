using System.Collections.Generic;
using UnityEngine;

public class RewindableHierarchy : MonoBehaviour
{
    public List<PointInTime> pointsInTime;
    public Transform root;
    
    private static bool _isRewinding;
    private float _maxPointsInTime;
    
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
        _maxPointsInTime = Mathf.Round(Rewinder.rew.maxRewindTime / Time.fixedDeltaTime);
    }
    
    private void FixedUpdate()
    {
        if (_isRewinding)
            Rewind();
        else
            Record();
    }

    private void Rewind()
    {
        if (pointsInTime.Count > 0)
        { 
            var point = pointsInTime[0];

            transform.position = point.Position;
            transform.rotation = point.Rotation;
        
            pointsInTime.RemoveAt(0);
            return;
        }
        
        Rewinder.rew.InvokeStopRewind();
    }

    private void Record()
    {
        if (pointsInTime.Count > _maxPointsInTime)
            pointsInTime.RemoveAt(pointsInTime.Count - 1);
        
        pointsInTime.Insert(0, new PointInTime(transform.position, transform.rotation));
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
