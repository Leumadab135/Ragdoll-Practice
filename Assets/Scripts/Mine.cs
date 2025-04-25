using UnityEngine;
using System;

public class Explosion : MonoBehaviour
{
    #region Properties
    #endregion

    private GameObject _playerHips;


    #region Fields
    [Header("Camera Controller")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _explosionCamera;

    [Header("Explosion")]
    [SerializeField] private float _explosionArea;
    [SerializeField] private bool _cameraShouldFollow;
    [SerializeField] private float _explosionForce = 1000;
    [SerializeField] private GameObject _effect;
    #endregion

    #region Unity Callbacks

    private void Update()
    {
        if (_playerHips != null)
        {
            _explosionCamera.transform.LookAt(_playerHips.transform.position);

            if (Vector3.Distance(_playerHips.transform.position, _explosionCamera.transform.position) < 7 || Input.GetKeyDown(KeyCode.R))
            {
                //Explosion camera position
                _explosionCamera.transform.position = new Vector3(9, 9, -10);

                // Change Cameras
                _explosionCamera.enabled = false;
                _mainCamera.enabled = true;

                _playerHips.transform.parent.position = new Vector3(4, 1, 0);
                // Restaurar control del personaje
                _playerHips.transform.parent.GetComponent<Animator>().enabled = true;
                _playerHips.transform.parent.GetComponent<CharacterController>().enabled = true;


                _cameraShouldFollow = false;
            }

        }

        if (_cameraShouldFollow)
        {
            _explosionCamera.transform.Translate(_explosionCamera.transform.forward * Time.deltaTime * 10, Space.Self);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        //Cameras
        _mainCamera.enabled = false;
        _explosionCamera.enabled = true;
        _cameraShouldFollow = true;

        //Explosion
        Instantiate(_effect, transform.position, Quaternion.identity);

        Animator playerAnim = other.GetComponentInParent<Animator>();
        _playerHips = playerAnim.transform.GetChild(0).gameObject;

        _playerHips.transform.parent.GetComponent<CharacterController>().enabled = false;

        if (playerAnim != null)
            playerAnim.enabled = false;

        ExplosionForce();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _explosionArea);
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void ExplosionForce()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, _explosionArea);
        for (int i = 0; i < objects.Length; i++)
        {
            Rigidbody objectRB = objects[i].GetComponent<Rigidbody>();
            if (objectRB != null)
                objectRB.AddExplosionForce(_explosionForce, transform.position, _explosionArea);
        }
    }


    #endregion
}

