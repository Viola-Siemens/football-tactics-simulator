/**@file     Ball.cs
 * @brief    足球信息与控制
 * @details  记录足球信息，同时可以根据踢球部位与足球旋转更新运行轨迹。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;

/**@class    Ball
 * @brief    足球信息与控制类
 * @details  包含了足球动态信息与控制方法。
 */
public class Ball : MonoBehaviour {
	[HideInInspector]
	public Rigidbody rig;           ///< 足球刚体引用

	[HideInInspector]
	public Player preTouch = null;	///< 上一个接触者
	[HideInInspector]
	public Player curTouch = null;  ///< 最后一个接触者
	[HideInInspector]
	public bool isKick;             ///< 攻方门将是否无法用手接球

	[HideInInspector]
	public float kickBallTime;      ///< 最后一次被踢出的时间

	//	public Vector3 debugForce;

	public bool isGo;               ///< 发球指令
	public bool hasGo;              ///< 已经发球
	public bool GoalReset;          ///< 中圈开球

	/**
	 * @brief 唤醒时调用
	 * 插件被唤醒时，重设发球信号，并获取足球刚体的引用。
	 */
	void Awake() {
		rig = GetComponent<Rigidbody>();

	//	rig.AddForce(debugForce);

		isGo = true;
		hasGo = false;
		GoalReset = false;
	}

	/**
	 * @brief 每帧调用
	 * 程序运行时的每一帧根据足球旋转更新足球运动。
	 */
	void Update() {
		rig.AddForce(Vector3.Cross(rig.angularVelocity, rig.velocity) * 0.0001f);
		rig.velocity *= 0.9996f;
		rig.angularVelocity *= 0.9995f;
	}
}
