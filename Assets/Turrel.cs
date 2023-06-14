using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turrel : MonoBehaviour
{
    [SerializeField] private float _fireRate = .2f;
    [SerializeField] private Vector2 _fireVector;

    [SerializeField] private Transform _bulletPf;
    [SerializeField] private int _bulletsPoolCount = 10;
    [SerializeField] private float _bulletSpeed = 20f;

    private TurrelBullet[] _bulletsPool;
    private int _currentBullet = 0;

    private Vector2 _fireDir;
    private Vector2 _firePos;

    private Coroutine _turrelShootingCoroutine;

    private void OnValidate()
    {
        Init();
    }

    private void OnEnable()
    {
        TurrelBullet.OnBulletHit += OnBulletHit;
    }

    private void OnDisable()
    {
        TurrelBullet.OnBulletHit -= OnBulletHit;
    }

    private void Awake()
    {
        _bulletsPool = new TurrelBullet[_bulletsPoolCount];
        for(int i = 0; i < _bulletsPoolCount; i++)
        {
            _bulletsPool[i] = Instantiate(_bulletPf, transform.position, Quaternion.identity).GetComponent<TurrelBullet>();
            _bulletsPool[i].transform.parent = transform;
            _bulletsPool[i].gameObject.SetActive(false);
        }

        Init();

        _turrelShootingCoroutine = StartCoroutine(TurrelUpdate());
    }

    private void Init()
    {
        _fireDir = _fireVector.normalized;

        float rotZ = Mathf.Atan2(_fireDir.y, _fireDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);
        _firePos = transform.right * .65f;
    }

    private void OnBulletHit(object sender, System.EventArgs e)
    {
        TurrelBullet bullet = sender as TurrelBullet;

        bullet.gameObject.SetActive(false);
    }

    IEnumerator TurrelUpdate()
    {
        while (true)
        {
            if(_currentBullet >= _bulletsPool.Length)
            {
                _currentBullet = 0;
            }

            TurrelBullet currentBullet = _bulletsPool[_currentBullet];

            currentBullet.gameObject.SetActive(true);
            currentBullet.transform.position = (Vector2)transform.position + _firePos;

            float bulletRotZ = Mathf.Atan2(_fireDir.y, _fireDir.x) * Mathf.Rad2Deg;

            currentBullet.transform.rotation = Quaternion.Euler(0, 0, bulletRotZ);
            currentBullet.Rig.velocity = _fireDir * _bulletSpeed;

            _currentBullet++;
            yield return new WaitForSeconds(_fireRate);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + _firePos, .5f);
    }
}
