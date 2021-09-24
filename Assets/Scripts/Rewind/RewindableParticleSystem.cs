using UnityEngine;

public class RewindableParticleSystem : MonoBehaviour
{
	//TODO: doesn't rewind properly if there were multiple collisions during rewind time
	public float simulationSpeedScale = 1.0f;

	private ParticleSystem[] _particleSystems;
	private float[] _simulationTimes;
	private float _elapsedSinceStart;
	private bool _isRewinding, _needsToResumeAfterRewind;

	private void OnEnable()
	{
		Rewinder.rew.startRewind += OnStartRewind;
		Rewinder.rew.stopRewind += OnStopRewind;

		//in case, ParticleSys is disabled and enabled 
		if (_particleSystems == null)
			Initialize();
 
		//_particleSystems[0].Simulate(startTime, true, false, true);
	}
	private void OnDisable()
	{
		Rewinder.rew.startRewind -= OnStartRewind;
		Rewinder.rew.stopRewind -= OnStopRewind;
	}

	private void FixedUpdate()
	{
		if(_isRewinding)
			SimulateAnimation();
		else if(_needsToResumeAfterRewind)
			SimulateAnimation(1f);

		_elapsedSinceStart += Time.fixedDeltaTime;
	}
	
	private void Initialize()
	{
		_particleSystems = GetComponentsInChildren<ParticleSystem>(false);
		_simulationTimes = new float[_particleSystems.Length];
		
		_particleSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		//setting this here, because will forget to do on all fx you add to scene
		//causes wonky/weird artifacts if left true
		foreach (var system in _particleSystems)
			system.useAutoRandomSeed = false;

		if (_particleSystems[0].main.playOnAwake)
			_particleSystems[0].Play(true);
	}

	private void SimulateAnimation(float signMultiplier = -1f)
	{
		_particleSystems[0].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		
		for (int i = _particleSystems.Length - 1; i >= 0; i--)
		{
			_particleSystems[i].Play(false);
		
			//decreasing simTime to simulate particle fx in reverse, vice versa
			float deltaTime = _particleSystems[i].main.useUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
			var x = _simulationTimes[i];
			_simulationTimes[i] += (signMultiplier * deltaTime * _particleSystems[i].main.simulationSpeed) * simulationSpeedScale;

			float currentSimulationTime = _simulationTimes[i];
			_particleSystems[i].Simulate(currentSimulationTime, false, !_isRewinding, true);
			
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
			Destroy(gameObject);
			//i--;
		}
	}

	private void OnStartRewind()
	{
		_isRewinding = true;
		
		for (int i = 0; i < _particleSystems.Length; i++)
			_simulationTimes[i] = _elapsedSinceStart;
	}
    
	private void OnStopRewind()
	{
		_isRewinding = false;
		
		for (int i = 0; i < _particleSystems.Length; i++)
		{
			if (!_particleSystems[i].IsAlive())
			{
				_simulationTimes[i] = -2f;
				continue;
			}
			
			//only need to resume after rewind has ended, if the animation is still playing
			_needsToResumeAfterRewind = true;
			_simulationTimes[i] = _particleSystems[i].time;
		}
	}
}
