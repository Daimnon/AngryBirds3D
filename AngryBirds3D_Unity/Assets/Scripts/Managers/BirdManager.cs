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
    [Header("Pool")]
    [SerializeField] private Bird[] _prefabs;
    public Bird[] Prefabs => _prefabs;

    [SerializeField] private int _initialPoolSize = 20;
    [SerializeField] private Rigidbody _anchorRb;
    private List<Bird> _pool;

    [Header("Spawn")]
    [SerializeField] private BirdType _startingBird = BirdType.Red;
    [SerializeField] private CameraController _camController;
    [SerializeField] private float _birdsToSpawn;
    private float _birdsToUse = 5;

    private void OnEnable()
    {
        EventManager.OnPrepareGame += OnPrepareGame;
    }

    private void OnDisable()
    {
        EventManager.OnPrepareGame -= OnPrepareGame;
    }

    private Bird _readyBird = null;
    public Bird ReadyBird => _readyBird;

    private void Awake()
    {
        _pool = new List<Bird>();
    }
    private void Start()
    {
        InitializePool();
        _birdsToUse = _birdsToSpawn;
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
                newBird.BirdManager = this;
                newBird.AnchorGO = gameObject;
                newBird.AnchorJoint.connectedBody = _anchorRb;
                newBird.Rb.isKinematic = true;
                _pool.Add(newBird);
            }
        }
    }

    private void TryAddSpringJoint(Bird bird)
    {
        if (!bird.AnchorJoint)
        {
            SpringJoint newJ = bird.gameObject.AddComponent<SpringJoint>();
            newJ.spring = 120.0f;
            bird.AnchorJoint = newJ;
            bird.AnchorJoint.connectedBody = _anchorRb;
            //newJ.connectedAnchor = Vector3.zero;
        }
    }
    public Bird GetBirdFromPool(int birdType)
    {
        if (_birdsToUse <= 0)
        {
            EventManager.InvokeGameOver();
            return null;
        }
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i].BirdType != (BirdType)birdType)
                continue;

            Bird bird = _pool[i];
            if (!bird.gameObject.activeSelf)
            {
                bird.gameObject.SetActive(true);
                bird.transform.SetParent(null);
                bird.BirdManager = this;
                bird.AnchorGO = gameObject; // set the anchor for the bird.
                bird.Rb.isKinematic = true;
                _birdsToUse--;
                _pool.Remove(bird);
                return bird;
            }
        }

        // if no bird in pool, create a new one
        Bird newBird = Instantiate(_prefabs[birdType], transform);
        newBird.gameObject.SetActive(true);
        newBird.transform.SetParent(null);
        newBird.BirdManager = this;
        newBird.AnchorGO = gameObject; // set the anchor for the bird.
        newBird.Rb.isKinematic = true;
        _birdsToUse--;
        return newBird;
    }
    public Bird GetBirdFromPool(BirdType birdType)
    {
        return GetBirdFromPool((int)birdType);
    }

    public void ReturnBirdToPool(Bird bird)
    {
        bird.Rb.isKinematic = true;
        bird.gameObject.SetActive(false);
        bird.transform.SetParent(transform);
        bird.transform.position = Vector3.zero;
        /*if (!bird.AnchorJoint)
        {
            SpringJoint newJ = bird.gameObject.AddComponent<SpringJoint>();
            newJ.spring = 120.0f;
            bird.AnchorJoint = newJ;
            newJ.connectedBody = _anchorRb;
            newJ.connectedAnchor = Vector3.zero;
        }*/
        bird.BirdManager = this;
        _pool.Add(bird);

        int prefabIndex = UnityEngine.Random.Range(0, _prefabs.Length);
        _camController.SetNewFollowTarget(SpawnBird(prefabIndex));
    }
    #endregion

    #region Spawn Management
    public Bird SpawnBird(int birdType)
    {
        Bird bird = GetBirdFromPool(birdType);
        if (bird == null) return null;

        bird.Rb.isKinematic = true;

        Vector3 position = _anchorRb.position;
        position.y += 0.5f;
        Quaternion quaternion = Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f)); // so birds look to the right
        bird.transform.SetLocalPositionAndRotation(position, quaternion);

        TryAddSpringJoint(bird);

        _readyBird = bird;
        return bird;
    }
    public Bird SpawnBird(BirdType birdType)
    {
        return SpawnBird((int)birdType);
    }
    #endregion

    private void OnPrepareGame()
    {
        SpawnBird(_startingBird);
    }
}
