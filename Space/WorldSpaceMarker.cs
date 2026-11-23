using UnityEngine;

public class WorldSpaceMarker : MonoBehaviour
{
    [SerializeField] private GameObject _markerPrefab;

    private GameObject _markerInstance;
    private RectTransform _markerRect;
    private Camera _mainCamera;
    private Canvas _canvas;
    private RectTransform _canvasRect;

    void Start()
    {
        _mainCamera = Camera.main;
        _canvas = FindObjectOfType<Canvas>();

        if (_markerPrefab != null && _canvas != null)
        {
            _canvasRect = _canvas.GetComponent<RectTransform>();
            _markerInstance = Instantiate(_markerPrefab, _canvas.transform);
            _markerRect = _markerInstance.GetComponent<RectTransform>();
        }
    }

    void LateUpdate()
    {
        if (_markerInstance == null) return;

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(transform.position);

        if (screenPos.z < 0)
        {
            screenPos *= -1f;
        }
        screenPos.x = Mathf.Clamp(screenPos.x, 50f, Screen.width - 50f);
        screenPos.y = Mathf.Clamp(screenPos.y, 50f, Screen.height - 50f);
        screenPos.z = 0f;

        _markerRect.position = screenPos;
    }

    void OnDestroy()
    {
        if (_markerInstance != null)
            Destroy(_markerInstance);
    }
}