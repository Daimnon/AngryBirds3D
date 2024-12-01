using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdType
{
    Regular
}

/// <summary>
/// This should act as the anchor for the birds
/// and should have a Rigidbody attached to it
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BirdManager : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private Bird[] _prefabs;
    [SerializeField] private int _initialPoolSize = 20;
    [SerializeField] private Rigidbody _anchorRb;
    private List<Bird> _pool;

    [Header("Spawn")]
    [SerializeField] private Bird _readyBird = null;
    [SerializeField] private BirdType _startingBird = BirdType.Regular;

    private void Awake()
    {
        _pool = new List<Bird>();
    }
    private void Start()
    {
        InitializePool();
        SpawnBird(_startingBird);
    }

    #region Pool Management
    private void InitializePool()
    {
        if (_prefabs == null)
            return;

        for (int i = 0; i < _prefabs.Length; i++)
        {
            for (int j = 0; j < _initialPoolSize; j++)
            {
                Bird newBird = Instantiate(_prefabs[i], transform);
                newBird.gameObject.SetActive(false);
                _pool.Add(newBird);
            }
        }
    }

    public Bird GetBirdFromPool(int birdType)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].BirdType != (BirdType)birdType)
                continue;

            Bird bird = _pool[i];
            if (!bird.gameObject.activeSelf)
            {
                // set the anchor for the bird.
                bird.AnchorGO = gameObject; 
                bird.AnchorJoint.connectedBody = _anchorRb; // set the anchor for the bird.
                bird.gameObject.SetActive(true);
                bird.transform.SetParent(null);
                return bird;
            }
        }

        // if no bird in pool, create a new one
        Bird newBird = Instantiate(_prefabs[birdType], transform);
        _pool.Add(newBird);
        newBird.gameObject.SetActive(true);
        return newBird;
    }
    public Bird GetBirdFromPool(BirdType birdType)
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].BirdType != birdType)
                continue;

            Bird bird = _pool[i];
            if (!bird.gameObject.activeSelf)
            {
                // set the anchor for the bird.
                bird.AnchorGO = gameObject;
                bird.AnchorJoint.connectedBody = _anchorRb; // set the anchor for the bird.
                bird.gameObject.SetActive(true);
                bird.transform.SetParent(null);
                return bird;
            }
        }

        // if no bird in pool, create a new one
        Bird newBird = Instantiate(_prefabs[(int)birdType], transform);
        _pool.Add(newBird);
        newBird.gameObject.SetActive(true);
        return newBird;
    }

    public void ReturnBirdToPool(Bird bird)
    {
        bird.gameObject.SetActive(false);
        bird.transform.SetParent(transform);
        bird.transform.position = Vector3.zero;
        bird.AnchorGO = null;
        bird.AnchorJoint.connectedBody = null;
        _pool.Add(bird);
    }
    #endregion

    #region Spawn Management
    private void SpawnBird(int birdType)
    {
        Bird bird = GetBirdFromPool(birdType);
        Vector3 position = _anchorRb.position;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)); // so birds look to the right
        bird.transform.SetLocalPositionAndRotation(position, quaternion);
    }
    private void SpawnBird(BirdType birdType)
    {
        Bird bird = GetBirdFromPool(birdType);
        Vector3 position = _anchorRb.position;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)); // so birds look to the right
        bird.transform.SetLocalPositionAndRotation(position, quaternion);
    }
    #endregion
}
