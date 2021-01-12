/**@mainpage  足球战术模拟器
 * <table>
 * <tr><th>Project</th>  <td>足球战术模拟器</td><tr>
 * <tr><th>Author</th>   <td>刘冬煜</td><tr>
 * <tr><th>Version</th>  <td>1.0.1</td><tr>
 * </table>
 * @section   项目详细描述
 * 基于统计的球员行为分类模型
 *
 * @section   功能描述  
 * -# 模拟球员行为，如传球、施压、扑救等
 * -# 根据球员不同比赛风格做出不同决策
 * -# 自由的裁判判罚
 */

/**@file     MainJudge.cs
 * @brief    主裁判行为（需手动操控）
 * @details  包含了主裁判的判罚功能，如暂停比赛、终止比赛、任意球、界外球、进球有效等。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;
using UnityEngine.UI;


/**@class    MainJudge
 * @brief    主裁判行为类（需手动操控）
 * @details  包含了主裁判的判罚函数，如暂停比赛、终止比赛、任意球、界外球、进球有效等。
 */
public class MainJudge : MonoBehaviour {
	public Team blueTeam;       ///< 蓝队引用
	public Team redTeam;		///< 红队引用
	public Ball ball;           ///< 球引用
	public Text redScoreText;	///< 红队得分板
	public Text blueScoreText;  ///< 蓝队得分板
	public Timer judgeTimer;    ///< 计时器

	public Button blueGoalButton;   ///< 蓝队进球按钮
	public Button redGoalButton;    ///< 红队进球按钮

	int redScore = 0;			///< 红队得分
	int blueScore = 0;			///< 蓝队得分

	/**
	 * @brief 蓝队界外球
	 * 若红方球员将球碰出边线以外，应判罚由蓝队球员在出界位置手抛球掷入界内。
	 */
	public void BlueThrowIn() {
		/// 寻找最靠近出界位置的球员来投掷界外球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		float BallXPosi = ball.transform.position.x;

		/// 出界位置不能在底线以外
		if (BallXPosi > redTeam.GoalX) {
			BallXPosi = redTeam.GoalX;
		}
		if (BallXPosi < blueTeam.GoalX) {
			BallXPosi = blueTeam.GoalX;
		}

		/// 将球放置在界外出界位置处
		ball.transform.SetPositionAndRotation(
			BallXPosi * 0.99f * Vector3.right + 6.0f * Vector3.up +
			Border.BorderZ * Mathf.Sign(ball.transform.position.z) * Vector3.forward,
			Quaternion.identity
		);

		/// 寻找蓝队中与足球距离最小的球员
		foreach (Team.Member member in blueTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		blueTeam.BallHander = hander;
		redTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 红队界外球
	 * 若蓝方球员将球碰出边线以外，应判罚由红队球员在出界位置手抛球掷入界内。
	 */
	public void RedThrowIn() {
		/// 寻找最靠近出界位置的球员来投掷界外球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		float BallXPosi = ball.transform.position.x;

		/// 出界位置不能在底线以外
		if (BallXPosi > redTeam.GoalX) {
			BallXPosi = redTeam.GoalX;
		}
		if (BallXPosi < blueTeam.GoalX) {
			BallXPosi = blueTeam.GoalX;
		}

		/// 将球放置在界外出界位置处
		ball.transform.SetPositionAndRotation(
			BallXPosi * 0.99f * Vector3.right + 6.0f * Vector3.up +
			Border.BorderZ * Mathf.Sign(ball.transform.position.z) * Vector3.forward,
			Quaternion.identity
		);

		/// 寻找红队中与足球距离最小的球员
		foreach (Team.Member member in redTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		redTeam.BallHander = hander;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 蓝队任意球
	 * 在正常比赛时间内，红队球员在本方禁区外犯规，应判罚蓝队任意球开球。
	 */
	public void BlueFreeKick() {
		/// 寻找最靠近犯规位置的球员来发任意球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在犯规位置
		ball.transform.SetPositionAndRotation(
			ball.transform.position,
			Quaternion.identity
		);

		/// 寻找蓝队中与足球距离最小的球员
		foreach (Team.Member member in blueTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		blueTeam.BallHander = hander;
		redTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 红队任意球
	 * 在正常比赛时间内，蓝队球员在本方禁区外犯规，应判罚红队任意球开球。
	 */
	public void RedFreeKick() {
		/// 寻找最靠近犯规位置的球员来发任意球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在犯规位置
		ball.transform.SetPositionAndRotation(
			ball.transform.position,
			Quaternion.identity
		);

		/// 寻找红队中与足球距离最小的球员
		foreach (Team.Member member in redTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		redTeam.BallHander = hander;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 蓝队球门球
	 * 若红方球员将球碰出蓝方底线外，或红队间接任意球、界外球直接发入蓝方门线以内，应判罚由蓝队球门球开球。
	 */
	public void BlueGoalKick() {
		/// 将球放置在蓝方小禁区内
		ball.transform.SetPositionAndRotation(
			blueTeam.GoalX * 0.88f * Vector3.right + 6.0f * Vector3.up + Random.Range(-100.0f, 100.0f) * Vector3.forward,
			Quaternion.identity
		);

		/// 设置持球者并固定球
		blueTeam.BallHander = blueTeam.members[blueTeam.members.Length - 1].player;
		redTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 红队球门球
	 * 若蓝方球员将球碰出红方底线外，或蓝队间接任意球、界外球直接发入红方门线以内，应判罚由红队球门球开球。
	 */
	public void RedGoalKick() {
		/// 将球放置在红方小禁区内
		ball.transform.SetPositionAndRotation(
			redTeam.GoalX * 0.88f * Vector3.right + 6.0f * Vector3.up + Random.Range(-100.0f, 100.0f) * Vector3.forward,
			Quaternion.identity
		);

		/// 设置持球者并固定球
		redTeam.BallHander = redTeam.members[redTeam.members.Length - 1].player;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 蓝队角球
	 * 若红方球员将球碰出红方底线外，或红队间接任意球、界外球直接发入红方门线以内，应判罚蓝队角球开球。
	 */
	public void BlueCornerKick() {
		/// 寻找最靠近角旗区的球员来发角球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在对应一侧的角旗区
		ball.transform.SetPositionAndRotation(
			redTeam.GoalX * 0.995f * Vector3.right + 6.0f * Vector3.up +
			Border.BorderZ * Mathf.Sign(ball.transform.position.z) * Vector3.forward,
			Quaternion.identity
		);

		/// 寻找蓝队中与足球距离最小的球员
		foreach (Team.Member member in blueTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		blueTeam.BallHander = hander;
		redTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 红队角球
	 * 若蓝方球员将球碰出蓝方底线外，或蓝队间接任意球、界外球直接发入蓝方门线以内，应判罚红队角球开球。
	 */
	public void RedCornerKick() {
		/// 寻找最靠近角旗区的球员来发角球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在对应一侧的角旗区
		ball.transform.SetPositionAndRotation(
			blueTeam.GoalX * 0.995f * Vector3.right + 6.0f * Vector3.up +
			Border.BorderZ * Mathf.Sign(ball.transform.position.z) * Vector3.forward,
			Quaternion.identity
		);

		/// 寻找红队中与足球距离最小的球员
		foreach (Team.Member member in redTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		redTeam.BallHander = hander;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 蓝队点球
	 * 在正常比赛时间内，红方球员在本方禁区内犯规，应判罚蓝队点球。
	 */
	public void BluePenaltyKick() {
		/// 寻找最靠近点球点的球员来发点球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在点球点
		ball.transform.SetPositionAndRotation(
			blueTeam.AdvanceDirect.x * Border.PenaltyX * Vector3.right + 6.0f * Vector3.up,
			Quaternion.identity
		);

		/// 寻找蓝队中与足球距离最小的球员
		foreach (Team.Member member in blueTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		blueTeam.BallHander = hander;
		redTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 红队点球
	 * 在正常比赛时间内，蓝方球员在本方禁区内犯规，应判罚红队点球。
	 */
	public void RedPenaltyKick() {
		/// 寻找最靠近点球点的球员来发点球
		Player hander = null;
		float handerBallDist = float.MaxValue;

		/// 将球放置在点球点
		ball.transform.SetPositionAndRotation(
			redTeam.AdvanceDirect.x * Border.PenaltyX * Vector3.right + 6.0f * Vector3.up,
			Quaternion.identity
		);

		/// 寻找红队中与足球距离最小的球员
		foreach (Team.Member member in redTeam.members) {
			/// 球员member和足球的距离
			float curBallDist = (member.player.transform.position - ball.transform.position).magnitude;
			if (handerBallDist > curBallDist) {
				hander = member.player;
				handerBallDist = curBallDist;
			}
		}

		/// 设置持球者并固定球
		redTeam.BallHander = hander;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 比赛暂停
	 * 主裁判可在任意比赛时间内暂时终止比赛，由任意一方开球继续比赛。
	 */
	public void Pause() {
		redTeam.BallHander = null;
		blueTeam.BallHander = null;
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
	}

	/**
	 * @brief 比赛结束
	 * 在常规时间和补时结束后，主裁判可以终止比赛，比赛的记录数据随之存储。
	 */
	public void End() {
		Pause();

		/// 存储比赛记录数据
		judgeTimer.End();
	}

	/**
	 * @brief 放置球
	 * 暂停比赛时，裁判可以根据判罚将足球放置在身边并指定球队开球。
	 */
	public void PlaceBall() {
		ball.transform.SetPositionAndRotation(
			transform.position.x * Vector3.right + 6.0f * Vector3.up + transform.position.z * Vector3.forward,
			Quaternion.identity
		);
	}

	/**
	 * @brief 开球
	 * 判罚任意球、点球、球门球、角球、界外球等后，由裁判发出开球信号开球。
	 */
	public void Go() {
		/// 解除球的固定状态
		ball.GoalReset = false;
		ball.isGo = true;
		ball.hasGo = false;
		ball.rig.isKinematic = false;
		ball.rig.useGravity = true;

		/// 重设球员拿到球的时间
		float maxTeamGetBallTime = Mathf.Max(redTeam.getBallTime, blueTeam.getBallTime);
		if(redTeam.BallHander != null) {
			redTeam.getBallTime = (maxTeamGetBallTime + Time.time) / 2.0f;
			redTeam.BallHander.getBallTime = Time.time;
		}
		if (blueTeam.BallHander != null) {
			blueTeam.getBallTime = (maxTeamGetBallTime + Time.time) / 2.0f;
			blueTeam.BallHander.getBallTime = Time.time;
		}
	}

	/**
	 * @brief 进球有效后球员归位
	 * 裁判判定进球有效后，球员回到自己的半场并准备开球。
	 */
	void resetPlayers() {
		/// 重设蓝队重心与持球时间
		blueTeam.heart.x = -500.0f;
		blueTeam.getBallTime = Time.time;
		/// 重设红队重心与持球时间
		redTeam.heart.x = 500.0f;
		redTeam.getBallTime = Time.time;

		/// 将球放置在中圈并固定
		ball.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
		ball.rig.velocity = Vector3.zero;
		ball.rig.angularVelocity = Vector3.zero;
		ball.rig.useGravity = false;
		ball.isGo = false;
		ball.rig.isKinematic = true;
		ball.GoalReset = true;
	}

	/**
	 * @brief 蓝队进球有效
	 * 足球进入红队门线以内后，裁判可判定蓝队进球有效。
	 */
	public void BlueGoal() {
		/// 得分并更新得分板
		blueScore += 1;
		blueScoreText.text = blueScore.ToString();
		resetPlayers();

		blueGoalButton.enabled = false;

		redTeam.BallHander = redTeam.members[0].player;
		blueTeam.BallHander = null;
	}

	/**
	 * @brief 红队进球有效
	 * 足球进入蓝队门线以内后，裁判可判定红队进球有效。
	 */
	public void RedGoal() {
		/// 得分并更新得分板
		redScore += 1;
		redScoreText.text = redScore.ToString();
		resetPlayers();

		redGoalButton.enabled = false;

		redTeam.BallHander = null;
		blueTeam.BallHander = blueTeam.members[0].player;
	}
}
