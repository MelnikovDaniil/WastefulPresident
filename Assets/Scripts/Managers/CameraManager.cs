using System;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public event Action OnReachesTargetEvnet;

    public static CameraManager Instance;
    GameObject _target;
    public Camera camera;
    public float cameraSizeLimit = 11f;
    public Vector2 clampPoint1;
    public Vector2 clampPoint2;
    public float startCameraMove = 2f;
    public float maxCameraDistance = 5f;

    [Space]
    public Vector2 characterCameraShift = new Vector2(0, 2);
    public float characterCameraSize = 5;

    [Space]
    public bool isFollowMouse = true;

    [Space]
    public bool canMovingByTaps = true;
    public float startMovingDistance = 1f;

    [NonSerialized]
    public bool isMovingByTaps;

    private const float StandartWidth = 1280f;
    private const float StandartHight = 720f;

    private float secondsForMoving;
    private float toCameraSize;
    private float currentTime;
    private Vector3 currentPosition;
    private float currentCameraSize;
    private Vector3 cameraShift;

    private float standartCameraSize;
    private Vector3 toPosition;

    private Vector3 touchStartPosition;

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
                var mouthPos = camera.ScreenToWorldPoint(Input.mousePosition);
                var distance = Vector2.Distance(mouthPos, _target.transform.position);
                toPosition = _target.transform.position;
                if (isFollowMouse && startCameraMove < distance)
                {
                    var cameraVector = mouthPos - _target.transform.position;
                    toPosition = _target.transform.position + (cameraVector.normalized * Mathf.Clamp(distance, 0, maxCameraDistance));
                }

                transform.position = toPosition + cameraShift;
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
        else if (canMovingByTaps)
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                var diraction = touchStartPosition - camera.ScreenToWorldPoint(Input.mousePosition);
                if (isMovingByTaps || diraction.magnitude > startMovingDistance)
                {
                    isMovingByTaps = true;
                    transform.position += diraction;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isMovingByTaps = false;
            }
        }
        ClampCamera();
    }

    public void CharacterCamera()
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

    private void ClampCamera()
    {
        transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, clampPoint1.x + camera.orthographicSize * camera.aspect, clampPoint2.x - camera.orthographicSize * camera.aspect),
                    Mathf.Clamp(transform.position.y, clampPoint1.y + camera.orthographicSize, clampPoint2.y - camera.orthographicSize),
                    -10);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector2.Lerp(clampPoint1, clampPoint2, 0.5f), clampPoint2 - clampPoint1);

        var cameraSize = new Vector3(
                    (clampPoint2.x - clampPoint1.x) - camera.orthographicSize * camera.aspect * 2,
                    (clampPoint2.y - clampPoint1.y) - camera.orthographicSize * 2,
                    -10);


        Gizmos.DrawWireCube(Vector2.Lerp(clampPoint1, clampPoint2, 0.5f), cameraSize);
    }
}
