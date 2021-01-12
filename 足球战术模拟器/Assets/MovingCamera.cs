/**@file     MainCamera.cs
 * @brief    视角行为（需手动操控）
 * @details  用户可以手动操控摄像机的视角移动与切换。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;

/**@class    MovingCamera
 * @brief    视角行为类（需手动操控）
 * @details  包含了摄像机移动、视角调整、视角切换等函数。
 */
public class MovingCamera : MonoBehaviour {
	static readonly float MouseTurn = 12.0f;    ///< 视角调整速度
	static readonly float momentum = 0.5f;      ///< 移动动量
	static readonly float MoveSpeed = 2.5f;     ///< 移动速度

	Vector3 camAng;     ///< 摄像机角度

	/**
	 * @brief 唤醒时调用
	 * 插件被唤醒时，重设摄像机角度为当前角度。
	 */
	private void Awake() {
		camAng = transform.eulerAngles;
	}

	/**
	 * @brief 调整视角
	 * @param[in] cameraRotation	俯仰调整幅度（角度制），俯仰角不超过80°
	 */
	private void RotateCamera(float cameraRotation) {
		camAng.x -= cameraRotation;
		if (camAng.x > 80.0f) camAng.x = 80.0f;
		if (camAng.x < -80.0f) camAng.x = -80.0f;
		camAng.y = transform.eulerAngles.y;
		transform.eulerAngles = camAng;
	}

	/**
	 * @brief 调整父对象角度
	 * @param[in] cameraRotation	俯仰调整幅度（角度制），俯仰角不超过80°
	 */
	private void RotateParent(float cameraRotation) {
		camAng.x -= cameraRotation;
		if (camAng.x > 80.0f) camAng.x = 80.0f;
		if (camAng.x < -80.0f) camAng.x = -80.0f;
		camAng.y = transform.parent.eulerAngles.y;
		transform.parent.eulerAngles = camAng;
	}

	/**
	 * @brief 每帧调用
	 * 程序运行时的每一帧动态检测有无输入信号，并根据输入信号调整摄像机位置与视角。
	 * 用户输入上下左右移动键可以移动摄像机，而移动鼠标时只有在按下鼠标右键时才能调整摄像机视角。
	 */
	void Update() {
		Vector3 moveVector = Vector3.zero;	///< 移动距离

		moveVector.x = moveVector.x * momentum + Input.GetAxis("Horizontal") * (1.0f - momentum) * MoveSpeed;
		moveVector.y *= momentum;
		moveVector.z = moveVector.z * momentum + Input.GetAxis("Vertical") * (1.0f - momentum) * MoveSpeed;
		moveVector = transform.TransformDirection(moveVector);
		/// 根据有无父对象采取不同的视角调整策略
		if (transform.parent) {
			moveVector.y = 0;

			if (Input.GetMouseButton(1)) {
				transform.parent.Rotate(0.0f, Input.GetAxis("Mouse X") * MouseTurn, 0.0f);
				RotateParent(Input.GetAxis("Mouse Y") * MouseTurn);
			}

			transform.parent.position += moveVector;
		} else {
			moveVector += Input.GetAxis("Jump") * (1.0f - momentum) * MoveSpeed * Vector3.up;

			if (Input.GetMouseButton(1)) {
				transform.Rotate(0.0f, Input.GetAxis("Mouse X") * MouseTurn, 0.0f);
				RotateCamera(Input.GetAxis("Mouse Y") * MouseTurn);
			}
			transform.position += moveVector;
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (transform.parent) {
				/// 从父对象上移除
				transform.parent = null;
				transform.SetPositionAndRotation(
					transform.position, Quaternion.identity
				);
				camAng = transform.eulerAngles;
			} else {
				/// 退出程序
				Application.Quit();
			}
		}

		if (Input.GetMouseButtonDown(0)) {
			/// 试探选定的父对象并附着
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hit) && (hit.transform.CompareTag("Player") || hit.transform.CompareTag("Ball"))) {
				if (hit.transform.CompareTag("Ball")) {
					transform.SetPositionAndRotation(
						hit.transform.position + hit.transform.localScale.z * 0.1f * hit.transform.forward,
						hit.transform.rotation
					);
				} else {
					transform.SetPositionAndRotation(
						hit.transform.position +
							hit.transform.localScale.y * 0.9f * hit.transform.up +
							hit.transform.localScale.z * 0.1f * hit.transform.forward,
						hit.transform.rotation
					);
				}
				
				transform.parent = hit.transform;
				camAng = transform.parent.eulerAngles;
			}
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			/// 附着在足球上
			GameObject hit = GameObject.Find("Football");
			transform.SetPositionAndRotation(
				hit.transform.position + hit.transform.localScale.z * 0.1f * hit.transform.forward,
				hit.transform.rotation
			);
			transform.parent = hit.transform;
			camAng = transform.parent.eulerAngles;
		}
	}
}
