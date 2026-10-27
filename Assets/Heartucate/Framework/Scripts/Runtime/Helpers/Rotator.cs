using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float _speed = 10;
    [SerializeField] private bool _localAngles = true;
    [SerializeField] private Vector3 _rotationAngle;

    void Update()
    {
        Vector3 rotation = _rotationAngle * Time.deltaTime * _speed;
        if (_localAngles) transform.localEulerAngles += rotation;
        else transform.eulerAngles += rotation;
    }
}
