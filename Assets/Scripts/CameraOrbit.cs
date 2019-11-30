using UnityEngine;

public class CameraOrbit : MonoBehaviour {
  private Transform _XForm_Camera;
  private Transform _XForm_Parent;

  private Vector3 _LocalRotation;
  private float _CameraDistance = 25f;

  private int angle_count = 90;

  public float MouseSensitivity = 4f;
  public float ScrollSensitivity = 2f;
  public float OrbitDampening = 5f;
  public float ScrollDampening = 6f;

  public bool CameraDisabled = false;

  void Start() {
    this._XForm_Camera = this.transform;
    this._XForm_Parent = this.transform.parent;
  }

  void LateUpdate() {
    if (Input.GetKeyDown(KeyCode.LeftShift)) {
      CameraDisabled = !CameraDisabled;
    }

    if (!CameraDisabled) {
      if (Input.GetMouseButton(0)) {
        _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
        _LocalRotation.y -= Input.GetAxis("Mouse Y") * MouseSensitivity;

        _LocalRotation.y = Mathf.Clamp(_LocalRotation.y, -90f, 90f);
      } else {
        _LocalRotation.x -= 0.5f;
        _LocalRotation.y -= 0.25f * Mathf.Sin(Mathf.Deg2Rad * angle_count);
      }

      angle_count++;

      if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
        float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitivity;
        ScrollAmount *= (this._CameraDistance * 0.3f);
        this._CameraDistance += ScrollAmount * -1f;
        this._CameraDistance = Mathf.Clamp(this._CameraDistance, 0f, 50f);
      }
    }

    Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
    this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

    if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f) {
      this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
    }
  }
}