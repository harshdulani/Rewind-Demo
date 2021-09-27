using UnityEngine;

public class RewindableHierarchyController : MonoBehaviour
{
    private Animator _animator;
    
    public bool recordChildren = true;

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
        _animator = GetComponent<Animator>();
        
        gameObject.AddComponent<RewindableHierarchy>();
        if (recordChildren)
            AddRecorderToChildren(transform, transform);
    }

    private void AddRecorderToChildren(Transform me, Transform root)
    {
        if(me.childCount == 0)
            return;
        
        for (int i = 0; i < me.childCount; i++)
        {
            me.GetChild(i).gameObject.AddComponent<RewindableHierarchy>().root = root;
            AddRecorderToChildren(me.GetChild(i), root);
        }
    }
    
    private void OnStartRewind()
    {
        _animator.enabled = false;
    }
    
    private void OnStopRewind()
    {
        _animator.enabled = true;
    }
}
