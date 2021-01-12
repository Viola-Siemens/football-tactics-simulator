/**@file     Team.cs
 * @brief    球队行为
 * @details  球队的战术布置、球员站位、重心等信息及其动态变化。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;

/**@enum	 Position
 * @brief    球员位置枚举
 * @details  球员在球队比赛时所踢的位置，如中锋、边后卫、门将等。
 * 后场特点：枚举值小于4；
 * 中场特点：枚举值大于等于4且小于等于8；
 * 前场特点：枚举值大于8；
 * 中路特点：枚举值与2按位与的值为0；
 * 边路特点：枚举值与2按位与的值为2。
 */
public enum Position {
/*  ====门将====  */

	/* 门将 */
	GoalKeeper = 0,

/*  ====后场====  */

	/* 中后卫 */
	MidCenterBack = 1,

	/* 左边后卫 */
	LeftSideBack = 2,

	/* 右边后卫 */
	RightSideBack = 3,

/*  ====中场====  */

	/* 后腰 */
	DefenceMidfielder = 4,

	/* 中前卫 */
	CenterMidfielder = 5,

	/* 左边前卫 */
	LeftSideMidfielder = 6,

	/* 右边前卫 */
	RightSideMidfielder = 7,

	/* 前腰 */
	AttackMidfielder = 8,

/*  ====锋线====  */

	/* 影锋 */
	ShadowStriker = 9,

	/* 左边锋 */
	LeftWingForward = 10,

	/* 右边锋 */
	RightWingForward = 11,

	/* 中锋 */
	CenterForward = 12
}

/**@class    Tactics
 * @brief    战术
 * @details  包含球队整体、传球、跑位、站位、施压的详细战术。
 */
[System.Serializable]
public class Tactics {
/*  ====整体====  */
	public ushort keepForm;         ///< 保持队形:   [1, 10]

/*  ====传球时====  */
	public ushort longPassMoti;     ///< 长传积极性: [1, 10]
	public ushort wingSideMoti;     ///< 分边积极性: [1, 10]
	public ushort advanceSpeed;     ///< 推进速度:   [1, 10]

/*  ====有球权跑位时====  */
	public ushort defenseLine;      ///< 防线高度:   [1, 10]
	public ushort supportDist;      ///< 支持距离:   [1, 10]
	public ushort dribblingMoti;    ///< 盘球积极性: [1, 10]

/*  ====无球权站位时====  */
	public ushort compactness;      ///< 紧密度:     [1, 10]
	public ushort centerBlock;      ///< 中路围堵:   [1, 10]

/*  ====施压时====  */
	public ushort backwardSpeed;    ///< 回防速度:   [1, 10]
	public ushort pressureMoti;     ///< 施压积极性: [1, 10]

	/**
	 * @brief Tactics默认构造函数
	 */
	public Tactics() {
		keepForm = 5;

		longPassMoti = 5;
		wingSideMoti = 5;
		advanceSpeed = 5;

		defenseLine = 5;
		supportDist = 5;
		dribblingMoti = 5;

		compactness = 5;
		centerBlock = 5;

		backwardSpeed = 5;
		pressureMoti = 5;
	}
}

[System.Serializable]
/**@class    Team
 * @brief    球队类
 * @details  包含了球队的所有基本信息与行为。
 */
public class Team: MonoBehaviour {
	public string TeamName;         ///< 队名
	public Color TeamColor;         ///< 队服颜色

	public Vector3 AdvanceDirect;   ///< 推进方向
	public Team enemy;              ///< 对手球队

	public float GoalX;         ///< 球门X坐标
	public float LeftBound;     ///< 球门Z左边界
	public float RightBound;    ///< 球门Z右边界
	public float TopBound;      ///< 球门Y上边界
	public float BottomBound;   ///< 球门Y下边界

	public Vector3 heart;           ///< 重心（后防线的中点）

	public Tactics tactic;          ///< 战术

	//[HideInInspector]
	public Player BallHander;       ///< 球权持有者
	public float getBallTime;       ///< 最近一次取得球权的时间

	Ball ball;                  ///< 足球引用

	[System.Serializable]
	public class Member {
		public Player player;			///< 球员
		public Position position;		///< 位置
	}

	public Member[] members = new Member[11];   ///< 所有队员

	/**
	 * @brief 唤醒时调用
	 * 插件被唤醒时，重设球队持球时间与队员属性，并获得足球引用。
	 */
	private void Awake() {
		if (BallHander == null) {
			getBallTime = 0;
		} else {
			getBallTime = Time.time;
		}
		
		foreach (Member m in members) {
			if (m.player == null) continue;
			m.player.team = this;
			m.player.enemy = enemy;
			m.player.playPosition = m.position;
		}
		ball = GameObject.Find("Football").GetComponent<Ball>();
	}

	/**
	 * @brief 每帧调用
	 * 程序运行时的每一帧动态调整球队重心以实现推进与退防。
	 */
	private void Update() {
		if (ball.GoalReset) return;
		float ballDiff = ball.transform.position.x - heart.x;
		if (Mathf.Abs(heart.x) < tactic.defenseLine * 20.0f - 10.0f) {
			heart.x = Mathf.Sign(heart.x) * (tactic.defenseLine * 20.0f - 10.0f);
			return;
		}
		if (Mathf.Abs(heart.x) > tactic.defenseLine * 5.0f + 450.0f) {
			heart.x = Mathf.Sign(heart.x) * (tactic.defenseLine * 5.0f + 450.0f);
			return;
		}
		if (BallHander != null) {
			heart += AdvanceDirect * (tactic.advanceSpeed * 0.03f + 0.06f);
			heart.x += (Mathf.Pow(Mathf.Abs(ballDiff), 1.0f) * Mathf.Sign(ballDiff) - 200.0f * AdvanceDirect.x) * 0.002f;
		} else if (enemy.BallHander != null) {
			heart -= AdvanceDirect * (tactic.backwardSpeed * 0.06f + 0.12f);
			heart.x += (Mathf.Pow(Mathf.Abs(ballDiff), 1.0f) * Mathf.Sign(ballDiff) - 200.0f * AdvanceDirect.x) * 0.001f;
		}
	}
}
