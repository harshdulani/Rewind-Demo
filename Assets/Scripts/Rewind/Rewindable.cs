using System.Collections.Generic;
using UnityEngine;

public class PointInTime
{
    public static readonly PointInTime Zero = new PointInTime(Vector3.zero, Quaternion.identity);
    
    public Vector3 Position;
    public Quaternion Rotation;
    public PointInTime(Vector3 pos, Quaternion rot)
    {
        Position = pos;
        Rotation = rot;
        //scale = sca;
    }
}

public class Rewindable : MonoBehaviour
{
    //TODO: create an interface for rewindable
    public List<PointInTime> pointsInTime;

    private Rigidbody _rb;
    
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
        _rb = GetComponent<Rigidbody>();
        
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
        _rb.isKinematic = true;
    }
    
    private void OnStopRewind()
    {
        //TODO: create one singular Rewindable script that accepts a list of gameObjects that would interfere with rewinding and/or would be specified by the player
        _rb.isKinematic = false;
        _isRewinding = false;
    }
}
