using System.Collections;
using UnityEngine;

public class SimulatedThumbStick : MonoBehaviour {

    [Header("Parameters")]

    [SerializeField] private RectTransform _thumbStickRectTransform;
    private GameObject _thumbStickContainerRectTransform;
    private Vector2 _thumbStickContainerPos;
    [SerializeField] private float _thumbStickDragMin;
    [SerializeField] private float _thumbStickDragMax;
    [SerializeField] private float _thumbStickRecognitionMax;

    [Header("Cache")]

    private Touch _touch;
    private Vector2 _touchDirection;

    private void Awake() {
        _thumbStickContainerRectTransform = _thumbStickRectTransform.parent.gameObject;
        _thumbStickContainerPos = _thumbStickRectTransform.position;
    }

    private void Start() {
        FindObjectOfType<HUD>().onPause.AddListener(ToggleInput);
        Goal.onGoal.AddListener(HideInput);
        FindObjectOfType<LevelManager>().onLevelStart.AddListener(ShowInput);
    }

    private void Update() {
        if (_thumbStickContainerRectTransform.activeSelf) ThumbStickInput();

#if UNITY_EDITOR
        if (_thumbStickContainerRectTransform.activeSelf) {
            if (Input.GetMouseButton(0)) {
                _touchDirection = (Vector2)Input.mousePosition - _thumbStickContainerPos;
                _thumbStickRectTransform.anchoredPosition = IsInRange(_touchDirection.magnitude, _thumbStickDragMin, _thumbStickRecognitionMax) ?
                                                            Vector2.ClampMagnitude(_touchDirection, _thumbStickDragMax) : Vector2.zero;
            }
            else if (Input.GetMouseButtonUp(0)) {
                if (IsInRange(_touchDirection.magnitude, _thumbStickDragMin, _thumbStickRecognitionMax)) PlayerDash.instance.Dash(_touchDirection);
                _thumbStickRectTransform.anchoredPosition = Vector2.zero;
            }
        }
#endif
    }

    private void ThumbStickInput() {
        if (Input.touchCount > 0) {
            _touch = Input.GetTouch(0);
            _touchDirection = _touch.position - _thumbStickContainerPos;
            if (IsInRange(_touchDirection.magnitude, _thumbStickDragMin, _thumbStickRecognitionMax)) {
                if (_touch.phase == TouchPhase.Ended) {
                    PlayerDash.instance.Dash(_touchDirection);
                    _thumbStickRectTransform.anchoredPosition = Vector2.zero;
                }
                else _thumbStickRectTransform.anchoredPosition = IsInRange(_touchDirection.magnitude, _thumbStickDragMin, _thumbStickRecognitionMax) ?
                                                                 Vector2.ClampMagnitude(_touchDirection, _thumbStickDragMax) : Vector2.zero;
            }
        }
    }

    private bool IsInRange(float value, float min, float max) {
        return min <= value && value <= max;
    }

    private void ToggleInput(bool isTrue) {
        _thumbStickContainerRectTransform.SetActive(isTrue);
        _thumbStickRectTransform.gameObject.SetActive(isTrue);
    }

    private void ShowInput() {
        // Putting in a Routine to prevent Input on the same event as closing a Menu
        StartCoroutine(ShowInputRoutine());
    }

    private IEnumerator ShowInputRoutine() {
        yield return null;

        ToggleInput(true);
    }

    private void HideInput() {
        ToggleInput(false);
    }

}
