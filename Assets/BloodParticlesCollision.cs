using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodParticlesCollision : MonoBehaviour
{
    [SerializeField] private Transform[] _splatPrefabs;
    [SerializeField] private Transform _bloodSplattersContainer;

    private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> _collisionEvents;

    public ParticleSystem BloodParticleSystem { get { return _particleSystem; } }

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(_particleSystem, other, _collisionEvents);

        int count = _collisionEvents.Count;

        for(int i = 0; i < count; i++)
        {
            Instantiate(_splatPrefabs[Random.Range(0, _splatPrefabs.Length)], _collisionEvents[i].intersection, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), _bloodSplattersContainer);
        }
    }
}
