using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public event Action OnReachesTargetEvnet;

    public static CameraManager Instance;
    GameObject _target;
    public Camera camera;
    public float cameraSizeLimit = 11f;
    public Vector2 clamp;

    [Space]
    public Vector2 characterCameraShift = new Vector2(0, 2);
    public float characterCameraSize = 5;

    private const float StandartWidth = 1280f;
    private const float StandartHight = 720f;

    private float secondsForMoving;
    private float toCameraSize;
    private float currentTime;
    private Vector3 currentPosition;
    private float currentCameraSize;
    private Vector3 cameraShift;

    private float standartCameraSize;


    private void Awake()
    {
        Instance = this;
        var standartAspectRatio = StandartHight / StandartWidth;
        var currentAspectRation = (float)Screen.height / Screen.width;
        camera.orthographicSize = camera.orthographicSize / standartAspectRatio * currentAspectRation;
        standartCameraSize = Mathf.Clamp(camera.orthographicSize, 0, cameraSizeLimit);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            if (secondsForMoving == -1)
            {
                var cameraPosition = new Vector3(
                    _target.transform.position.x,
                    Mathf.Clamp(_target.transform.position.y, -clamp.y + camera.orthographicSize * camera.aspect, clamp.y - camera.orthographicSize * camera.aspect), -10);
                cameraPosition += cameraShift;
                transform.position = cameraPosition;
            }
            else if (secondsForMoving > currentTime)
            {
                currentTime += Time.deltaTime;
                var k = currentTime / secondsForMoving;
                var cameraSize = Mathf.Lerp(currentCameraSize, toCameraSize, k);
                var cameraPosition = Vector3.Lerp(currentPosition, _target.transform.position + cameraShift, k);
                cameraPosition = new Vector3(cameraPosition.x, cameraPosition.y, -10);
                transform.position = cameraPosition;
                camera.orthographicSize = cameraSize;
            }
            else
            {
                var cameraPosition = new Vector3(_target.transform.position.x, _target.transform.position.y, -10);
                transform.position = cameraPosition;
                _target = null;
                OnReachesTargetEvnet?.Invoke();
                OnReachesTargetEvnet = null;
            }
        }

    }

    public void CraracterCamera()
    {
        var caracter = FindObjectOfType<Character>();
        SetTarget(caracter.gameObject, characterCameraSize, -1, characterCameraShift);
    }

    public void SetTarget(GameObject target, float time, Vector3 cameraShift)
    {
        SetTarget(target, standartCameraSize, time, cameraShift);
    }

    public void SetTarget(GameObject target, float size, float time, Vector3 cameraShift)
    {
        currentPosition = transform.position;
        currentCameraSize = camera.orthographicSize;
        secondsForMoving = time;
        _target = target;
        this.cameraShift = cameraShift;
        toCameraSize = size;
        currentTime = 0;
    }
}
