using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud
{
    public Transform CloudTransform;
    public float CloudSpeed;

    public Cloud(Transform transform, float cloudSpeed)
    {
        CloudTransform = transform;
        CloudSpeed = cloudSpeed;
    }
}

public class SkyBackground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _skyBG;
    [SerializeField] private Transform[] _cloudsPf;

    private Cloud[] _clouds;

    private float _skyHalfWidth;
    private float _skyHalfHeight;

    private void Awake()
    {
        int numberOfClouds =  Mathf.RoundToInt((_skyBG.size.x + _skyBG.size.y) / 10f);
        _clouds = new Cloud[numberOfClouds];

        _skyHalfWidth = _skyBG.size.x / 2;
        _skyHalfHeight = _skyBG.size.y / 2;

        for(int i = 0; i < numberOfClouds; i++)
        {
            int randomCloud = i % _cloudsPf.Length;

            float randomWidth = Random.Range(-_skyHalfWidth, _skyHalfWidth);
            float randomHeight = Random.Range(-_skyHalfHeight, _skyHalfHeight);
            Vector2 randomPosition = new Vector2(randomWidth, randomHeight);

            Transform instantiatedCloud = Instantiate(_cloudsPf[randomCloud], (Vector2)transform.position + randomPosition, Quaternion.identity, transform);
            _clouds[i] = new Cloud(instantiatedCloud, Random.Range(.1f, 1f));
        }
    }

    private void Update()
    {
        foreach(Cloud cloud in _clouds)
        {
            if(cloud.CloudTransform.localPosition.x >= _skyBG.size.x / 2)
            {
                Vector2 newPos = new Vector2(-_skyHalfWidth, cloud.CloudTransform.localPosition.y);
                cloud.CloudTransform.localPosition = newPos;
            }

            cloud.CloudTransform.localPosition += Vector3.right * cloud.CloudSpeed * Time.deltaTime;
        }
    }
}
