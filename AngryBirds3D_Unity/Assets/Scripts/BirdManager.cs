using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BirdType
{
    Red,
    Blue,
    Black
}

/// <summary>
/// This should act as the anchor for the birds
/// and should have a Rigidbody attached to it
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BirdManager : MonoBehaviour
{
    private static BirdManager _instance = null;
    public static BirdManager Instance => _instance;

    [Header("Pool")]
    [SerializeField] private Bird[] _prefabs;
    public Bird[] Prefabs => _prefabs;

    [SerializeField] private int _initialPoolSize = 20;
    [SerializeField] private Rigidbody _anchorRb;
    private List<Bird> _pool;

    [Header("Spawn")]
    [SerializeField] private BirdType _startingBird = BirdType.Red;
    private Bird _readyBird = null;

    private void Awake()
    {
        if (!_instance) _instance = this;
        else Destroy(gameObject);

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
                newBird.AnchorGO = gameObject;
                newBird.AnchorJoint.connectedBody = _anchorRb;
                newBird.Rb.isKinematic = true;
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
                bird.gameObject.SetActive(true);
                bird.transform.SetParent(null);
                bird.AnchorGO = gameObject; // set the anchor for the bird.
                if (!bird.AnchorJoint)
                {
                    SpringJoint newJ = gameObject.AddComponent<SpringJoint>();
                    newJ.spring = 120.0f;
                    bird.AnchorJoint = newJ;
                }
                bird.Rb.isKinematic = true;

                return bird;
            }
        }

        // if no bird in pool, create a new one
        Bird newBird = Instantiate(_prefabs[birdType], transform);
        _pool.Add(newBird);
        newBird.gameObject.SetActive(true);
        newBird.transform.SetParent(null);
        newBird.AnchorGO = gameObject; // set the anchor for the bird.
        if (!newBird.AnchorJoint)
        {
            SpringJoint newJ = gameObject.AddComponent<SpringJoint>();
            newJ.spring = 120.0f;
            newBird.AnchorJoint = newJ;
        }
        newBird.Rb.isKinematic = true;
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
                bird.gameObject.SetActive(true);
                bird.transform.SetParent(null);
                bird.AnchorGO = gameObject; // set the anchor for the bird.
                if (!bird.AnchorJoint)
                {
                    SpringJoint newJ = gameObject.AddComponent<SpringJoint>();
                    newJ.spring = 120.0f;
                    bird.AnchorJoint = newJ;
                }
                bird.Rb.isKinematic = true;

                return bird;
            }
        }

        // if no bird in pool, create a new one
        Bird newBird = Instantiate(_prefabs[(int)birdType], transform);
        _pool.Add(newBird);
        newBird.gameObject.SetActive(true);
        newBird.transform.SetParent(null);
        newBird.AnchorGO = gameObject; // set the anchor for the bird.
        if (!newBird.AnchorJoint)
        {
            SpringJoint newJ = gameObject.AddComponent<SpringJoint>();
            newJ.spring = 120.0f;
            newBird.AnchorJoint = newJ;
        }
        newBird.Rb.isKinematic = true;
        return newBird;
    }

    public void ReturnBirdToPool(Bird bird)
    {
        bird.Rb.isKinematic = true;
        bird.gameObject.SetActive(false);
        bird.transform.SetParent(transform);
        bird.transform.position = Vector3.zero;
        if (!bird.AnchorJoint)
        {
            SpringJoint newJ = bird.gameObject.AddComponent<SpringJoint>();
            newJ.spring = 120.0f;
            bird.AnchorJoint = newJ;
            newJ.connectedBody = _anchorRb;
        }
        
        _pool.Add(bird);

        int prefabIndex = UnityEngine.Random.Range(0, _prefabs.Length);
        SpawnBird(prefabIndex);
    }
    #endregion

    #region Spawn Management
    public void SpawnBird(int birdType)
    {
        Bird bird = GetBirdFromPool(birdType);
        bird.Rb.isKinematic = true;

        Vector3 position = _anchorRb.position;
        position.y += 0.5f;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)); // so birds look to the right
        bird.transform.SetLocalPositionAndRotation(position, quaternion);
        _readyBird = bird;
    }
    public void SpawnBird(BirdType birdType)
    {
        Bird bird = GetBirdFromPool(birdType);
        bird.Rb.isKinematic = true;

        Vector3 position = _anchorRb.position;
        position.y += 0.5f;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)); // so birds look to the right
        bird.transform.SetLocalPositionAndRotation(position, quaternion);
        _readyBird = bird;
    }
    #endregion
}
