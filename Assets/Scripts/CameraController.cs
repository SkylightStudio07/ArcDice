using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    // 백그라운드 이미지 범위 따라가야 함. 별 수 없음...
    [SerializeField] private SpriteRenderer background;

    private Camera cam;
    private Vector3 dragParam;
    private Bounds bounds; // 바운드 따라서 못 넘어가게 세팅용

    private void Awake()
    {
        cam = GetComponent<Camera>();
        bounds = background.bounds;
    }

    private void Update()
    {
        HandleDrag();
    }

    private void HandleDrag()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mousePos = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame)
            dragParam = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));

        if (mouse.leftButton.isPressed)
        {
            Vector3 delta = dragParam - cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
            transform.position = Clamp(transform.position + delta);
        }
    }

    private Vector3 Clamp(Vector3 pos)
    {
        float halfH = cam.orthographicSize; // 카메라 높이의 절반
        // orthographicSize는 카메라의 절반 높이이므로, 너비의 절반은 높이에 종횡비를 곱한 값...
        // 종횡비 : 화면의 너비를 높이로 나눈 값. 즉, orthographicSize * aspect = 카메라 너비의 절반.
        // ㅅㅂ. 몇십분을 골치썩였는데 AI 똑똑하네
        float halfW = cam.orthographicSize * cam.aspect;

        pos.x = Mathf.Clamp(pos.x, bounds.min.x + halfW, bounds.max.x - halfW);
        pos.y = Mathf.Clamp(pos.y, bounds.min.y + halfH, bounds.max.y - halfH);

        return pos;
    }
}
