/**@file     GoalDetector.cs
 * @brief    判断球是否进入门线以内
 * @details  判断足球是否进入门线以内。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;
using UnityEngine.UI;

/**@class    GoalDetector
 * @brief    进球检测类
 * @details  判断足球是否进入门线以内。
 */
public class GoalDetector : MonoBehaviour {
	public Team team;			///< 被进球的一方
	public Button goalButton;   ///< 被进球一方的进球有效按钮

	/**
	 * @brief 被触发时调用
	 * @param[in] other	碰撞体触发者
	 * 对象作为触发器且被碰撞体other触发时，判断是否为足球，若是则说明进球。
	 */
	private void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Ball")) {
			//print(team.TeamName);
			goalButton.enabled = true;
		}
	}
}
