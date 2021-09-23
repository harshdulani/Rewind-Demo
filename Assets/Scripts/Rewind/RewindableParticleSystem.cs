using UnityEngine;

public class RewindableParticleSystem : MonoBehaviour
{
	//TODO: doesn't rewind properly if there were multiple collisions during rewind time
	private ParticleSystem[] _particleSystems;
	
	public float startTime = 2.0f;
	public float simulationSpeedScale = 1.0f;
	
	private float[] _simulationTimes, _currentSimulationTimes;
	private float _lastSimTimestamp = -2f;
	private bool _isRewinding, _needsToResumeAfterRewind;

	private void OnEnable()
	{
		Rewinder.rew.startRewind += OnStartRewind;
		Rewinder.rew.stopRewind += OnStopRewind;

		//in case, ParticleSys is disabled and enabled 
		if (_particleSystems == null)
			Initialize();
 
		for (int i = 0; i < _simulationTimes.Length; i++)
		{
			_simulationTimes[i] = 0.0f;
		}
 
		//_particleSystems[0].Simulate(startTime, true, false, true);
	}
	private void OnDisable()
	{
		Rewinder.rew.startRewind -= OnStartRewind;
		Rewinder.rew.stopRewind -= OnStopRewind;
		
	}

	private void Update()
	{
		if(_isRewinding)
			SimulateAnimation();
		else if(_needsToResumeAfterRewind)
			SimulateAnimation(1f);
	}
	
	private void Initialize()
	{
		_particleSystems = GetComponentsInChildren<ParticleSystem>(false);
		_currentSimulationTimes = _simulationTimes = new float[_particleSystems.Length];
		
		_particleSystems[0].Stop(true,
			ParticleSystemStopBehavior.StopEmittingAndClear);
		//setting this here, because will forget to do on all fx you add to scene
		//causes wonky/weird artifacts if left on
		foreach (var system in _particleSystems)
			system.useAutoRandomSeed = false;

		if (_particleSystems[0].main.playOnAwake)
			_particleSystems[0].Play(true);
	}

	private void SimulateAnimation(float signMultiplier = -1f)
	{
		_particleSystems[0].Stop(true,
			ParticleSystemStopBehavior.StopEmittingAndClear);
		
		for (int i = _particleSystems.Length - 1; i >= 0; i--)
		{
			_particleSystems[i].Play(false);
		
			//decreasing simTime to simulate particle fx in reverse, vice versa
			float deltaTime = _particleSystems[i].main.useUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
			_simulationTimes[i] += (signMultiplier * deltaTime * _particleSystems[i].main.simulationSpeed) * simulationSpeedScale;
			
			float currentSimulationTime = _simulationTimes[i];
			currentSimulationTime += _lastSimTimestamp < 1f ? startTime : _lastSimTimestamp;  
			_particleSystems[i].Simulate(currentSimulationTime, false, false, true);
			
			if(signMultiplier < 0)
			{
				//if particle sys is reversing, continue if the time stamp is above zero
				if (currentSimulationTime >= 0.0f) continue;
			}
			else
			{
				//if particle sys is moving forward, continue if the time stamp is smaller than duration
				if(currentSimulationTime <= _particleSystems[i].main.duration) continue;
			}
			
			//else come here and finalise the particle system
			_particleSystems[i].Play(false);
			_particleSystems[i].Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
			//rig this particle fx for destruction/return to pool
			i--;
		}
	}

	private void OnStartRewind()
	{
		_isRewinding = true;

		_lastSimTimestamp = _particleSystems[0].time;
	}
    
	private void OnStopRewind()
	{
		_isRewinding = false;
		
		foreach (var system in _particleSystems)
		{
			if (!system.IsAlive()) continue;
			
			//only need to resume after rewind has ended, if the animation is still playing
			_needsToResumeAfterRewind = true;
			_lastSimTimestamp = _particleSystems[0].time;
		}
	}
}
