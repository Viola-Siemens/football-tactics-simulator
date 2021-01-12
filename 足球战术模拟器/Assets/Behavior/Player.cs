/**@file     Player.cs
 * @brief    球员行为
 * @details  建立模型求出球员各个行为的得分，并根据得分预测球员行为。
 * @author   刘冬煜
 * @version  1.0.1
 */

using UnityEngine;

[System.Serializable]
/**@class	 PositionScore
 * @brief    球员各位置得分类
 * @details  球员在各位置的熟悉程度。
 * 0：没听说过；
 * 1：听说过且几乎没踢过；
 * 2：踢过这一位置但非常不熟悉；
 * 3：偶尔踢这一位置且并不很熟悉；
 * 4：有时踢这一位置且有一定的经验；
 * 5：经常踢这一位置且非常熟悉。
 */
public class PositionScore {
/*  ====门将====  */

	/* 门将 */
	public ushort GoalKeeper;

/*  ====后场====  */

	/* 中后卫 */
	public ushort MidCenterBack;

	/* 左边后卫 */
	public ushort LeftSideBack;

	/* 右边后卫 */
	public ushort RightSideBack;

/*  ====中场====  */

	/* 后腰 */
	public ushort DefenceMidfielder;

	/* 中前卫 */
	public ushort CenterMidfielder;

	/* 左边前卫 */
	public ushort LeftSideMidfielder;

	/* 右边前卫 */
	public ushort RightSideMidfielder;

	/* 前腰 */
	public ushort AttackMidfielder;

/*  ====锋线====  */

	/* 影锋 */
	public ushort ShadowStriker;

	/* 左边锋 */
	public ushort LeftWingForward;

	/* 右边锋 */
	public ushort RightWingForward;

	/* 中锋 */
	public ushort CenterForward;
};

[System.Serializable]
/**@class	 Ability
 * @brief    球员各方面能力
 * @details  球员各方面能力值。除基础属性外，最低为40，最高为100。
 */
public class Ability {
/*  ====球员基础====  */

	/* 年龄 */
	public int Age;

	/* 身高 */
	public int Height;

	/* 体重 */
	public int Weight;

/*  ====攻击能力====  */

	/* 控球能力 */
	public int BallControl;

	/* 地面传球 */
	public int GroundPass;

	/* 空中传球 */
	public int AirPass;

	/* 头球 */
	public int HeadBall;

	/* 力量 */
	public int Strength;

	/* 射门 */
	public int Shot;

/*  ====防守能力====  */

	/* 抢断 */
	public int Steal;

	/* 解围 */
	public int Clear;

	/* 扑救区域 */
	public int SaveMeasure;

	/* 反应速度 */
	public int ReactionSpeed;

	/* 接停球 */
	public int StopBall;

/*  ====均衡能力====  */

	/* 速度 */
	public int RunningSpeed;

	/* 总体力 */
	public int TotalCapacity;

	/* 跳跃高度 */
	public int JumpHeight;

	/* 抗伤 */
	public int InjureResistance;
};

/**@class	 Player
 * @brief    球员类（含预测模型）
 * @details  包含了球员的物理行为，以及模型计算得分的方法。
 */
public class Player : MonoBehaviour {
	[HideInInspector]
	public Rigidbody rig;	///< 球员刚体的引用

	Ball ball;              ///< 足球的引用

	/**@enum	 BehaviorType
	 * @brief    球员比赛风格枚举
	 * @details  球员比赛时的风格特点。
	 * 组织型：善于传球，并容易制造出具有威胁性的机会；
	 * 力量型：进攻意识强，身体强壮；
	 * 技巧型：防守意识强，且有持球突破的趋势；
	 * 速度型：速度快，且有意识回防和高速冲击对方后防线；
	 * 灵活型：位置多变，更容易找到防线破绽和吸引防守；
	 * 其中：
	 * <table>
	 * <tr><th>类型\位置</th>	<th>中锋</th>		<th>影锋</th>		<th>左/右边锋</th>	<th>前腰</th>		<th>左/右边前卫</th>	<th>中前卫</th>		<th>后腰</th>		<th>左/右边后卫</th>	<th>中后卫</th>		<th>门将</th></tr>
	 * <tr><th>组织型</th>		<td>支点中锋</td>	<td>创意指挥官</td>	<td>创意指挥官</td>	<td>创意指挥官</td>	<td>创意指挥官</td>		<td>指挥官</td>		<td>指挥官</td>		<td>后场组织者</td>		<td>后场组织者</td>	<td>门腰</td></tr>
	 * <tr><th>力量型</th>		<td>禁区之狐</td>	<td>禁区之狐</td>	<td>禁区之狐</td>	<td>中场狙击手</td>	<td>中场狙击手</td>		<td>破坏者</td>		<td>破坏者</td>		<td>破坏者</td>			<td>破坏者</td>		<td>门锋</td></tr>
	 * <tr><th>技巧型</th>		<td>魔术大师</td>	<td>古典十号位</td>	<td>传中专家</td>	<td>古典十号位</td>	<td>传中专家</td>		<td>古典十号位</td>	<td>靠山</td>		<td>传中专家</td>		<td>防守型后卫</td>	<td>防守型守门员</td></tr>
	 * <tr><th>速度型</th>		<td>偷猎者</td>		<td>偷猎者</td>		<td>偷猎者</td>		<td>全能中场</td>	<td>全能中场</td>		<td>全能中场</td>	<td>全能中场</td>	<td>清道夫</td>			<td>清道夫</td>		<td>门卫</td></tr>
	 * <tr><th>灵活型</th>		<td>自由中锋</td>	<td>前插攻击手</td>	<td>自由侧翼</td>	<td>前插攻击手</td>	<td>自由侧翼</td>		<td>前插攻击手</td>	<td>前插攻击手</td>	<td>迭瓦终结者</td>		<td>额外中锋</td>	<td>进攻型守门员</td></tr>
	 * </table>
	 */
	public enum BehaviorType {
		Passing = 0,    ///< 组织型
		Strength = 1,   ///< 力量型
		Skillful = 2,   ///< 技巧型
		Speeding = 3,   ///< 速度型
		Light = 4       ///< 灵活型
	}
	public BehaviorType behaviorType;   ///< 球员比赛风格

	public Ability abilities;       ///< 球员能力属性
	public PositionScore positions; ///< 球员处于各位置对球队精神的增益

	[HideInInspector]
	public Team team;               ///< 球员所在的队伍信息

	[HideInInspector]
	public Team enemy;              ///< 球员所在球队的对手队伍信息

	[HideInInspector]
	public Position playPosition;   ///< 球员在比赛中所踢的位置

	public string playerName;       ///< 球员姓名

	public float currentCap;        ///< 球员当前剩余体力 [0, 100]

	[HideInInspector]
	public float getBallTime;       ///< 球员最近拿到球的时间

	Vector3 force = Vector3.zero;   ///< 球员完成传球或射门或解围所需对球施加的力量
	Vector3 noise = Vector3.zero;   ///< 施加力量的噪声力量
	Player nextPlayer = null;		///< 传球给到的球员

	public Vector3 standposi = Vector3.zero;	///< 球员相对球队重心的站位
	Vector3 virtualForce = Vector3.zero;		///< 球员无球进攻或防守时受到的虚拟力

	float actionScore = 0.0f;       ///< 决策得分
	string message;                 ///< 决策名

	float injured;                  ///< 是否受伤，若大于零代表受伤，数值等于其愈合时间。

	static readonly float maxInjuredTime = 15.0f;   ///< 最大伤愈时间

	CapsuleCollider capsuleCollider;                ///< 碰撞体，用于判断是否触地

	//LineRenderer debugLine;			///< 测试阶段的划线


	/**
	 * @brief 唤醒时调用
	 * 插件被唤醒时，获取足球、刚体、碰撞体的引用，更新刚体的质量，并清除球员异常状态。
	 */
	private void Awake() {
		rig = GetComponent<Rigidbody>();
		ball = GameObject.Find("Football").GetComponent<Ball>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		rig.mass = abilities.Weight;
		injured = 0.0f;
		//debugLine = GameObject.Find("Football").GetComponent<LineRenderer>();
	}

	/**
	 * @brief 每帧调用
	 * 每一帧分别根据场上情况，依据模型预测球员行为
	 */
	private void Update() {
		if (injured > 0.0f) {
			injured -= Time.deltaTime;
			if (injured < 0.0f) {
				injured = 0.0f;
				rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			}
		} else {
			if (ball.isGo) {
				Detect_Action();
			} else {
				Detect_FreeKick();
			}
		}
	}

	/**
	 * @brief 第一帧调用
	 * 在Player和Team都被唤醒后，依据球员位置和球队重心更新球员站位和球员尺寸。
	 */
	private void Start() {
		GetComponent<MeshRenderer>().material.color = team.TeamColor;

		float sc = abilities.Height / 20.0f;
		transform.localScale = new Vector3(sc, sc * 1.2f, sc);

		standposi = new Vector3((transform.position.x - team.heart.x) * 1.6f, sc * 1.2f + 0.01f, transform.position.z);
		transform.SetPositionAndRotation(transform.position, Quaternion.identity);

		transform.Rotate(new Vector3(0, -Mathf.Sign(team.heart.x) * 90, 0));
	}

	/**
	 * @brief 【物理】转身并面向球
	 * @param[in] ballDirect	从球员指向球的方向向量
	 * 将球员的面朝方向转向足球。
	 */
	private void turn(Vector3 ballDirect) {
		float rotateSpeed = 120.0f;
		float YAngle = Vector3.SignedAngle(Vector3.forward, ballDirect, Vector3.up);
		Quaternion rotation00 = Quaternion.Euler(0, YAngle, 0);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation00, Time.deltaTime * rotateSpeed);
	}

	/**
	 * @brief 【物理】球员盘带
	 * @param[in] dribble_direct	球员盘带方向
	 * 球员持球沿盘带方向盘带。
	 * @see Player::detect_dribble
	 */
	private void dribble(Vector3 dribble_direct) {
		message = "盘带";

		force = Mathf.Sqrt(120 - abilities.BallControl) * dribble_direct * 1.0f;
		noise = Vector3.zero;
		nextPlayer = this;
	}

	/**
	 * @brief 【物理】球员传球
	 * @param[in] target	球员传球目标
	 * @param[in] smart		是否传直传球
	 * @param[in] byAir		是否传高空球
	 * 球员根据传球信号将球传给目标。
	 * 传球信号包括：
	 * <table>
	 * <tr><th>smart\byAir</th>	<th>False</th>		<th>True</th></tr>
	 * <tr><th>False</th>		<td>地面传球</td>	<td>高空传球</td></tr>
	 * <tr><th>True</th>		<td>地面直塞</td>	<td>过顶球</td></tr>
	 * </table>
	 * @see Player::detect_ground_simple
	 * @see Player::detect_ground_smart
	 * @see Player::detect_air_simple
	 * @see Player::detect_air_smart
	 */
	private void pass(Player target, bool smart, bool byAir) {
		message = "";
		if (byAir) {
			message += "高空";
		}
		if (smart) {
			message += "直";
		}
		message += "传球";

		force = target.transform.position + target.rig.velocity - transform.position;

		force *= 0.75f + 1.0f / ((target.transform.position - transform.position).magnitude / 100.0f + 1.0f);

		if (smart) {
			force += target.rig.velocity + team.AdvanceDirect * force.magnitude * 0.05f;
		}

		noise = new Vector3(Random.value - 0.5f, Random.value * 0.1f - 0.025f, Random.value - 0.5f);
		noise *= Mathf.Pow(force.magnitude, 0.75f) * 0.005f;

		float YForceMag;
		if (byAir) {
			noise *= Mathf.Sqrt(100 - abilities.AirPass);

			YForceMag = force.sqrMagnitude / 250.0f;
			if (YForceMag > 320.0f) {
				YForceMag = 320.0f;
			}
			force *= 0.92f + (YForceMag - 400.0f) / 5000.0f;
		} else {
			force *= 1.08f;

			YForceMag = force.sqrMagnitude / 500.0f;
			if (YForceMag > 40.0f) {
				YForceMag = 40.0f;
			}

			noise *= Mathf.Sqrt(100 - abilities.GroundPass);
		}

		if (force.magnitude > abilities.Strength * 8.0f) {
			force = force.normalized * abilities.Strength * 8.0f;
		}
		force += Vector3.up * YForceMag;

		nextPlayer = target;
	}

	/**
	 * @brief 【物理】球员射门
	 * @param[in] goal		球员射门目标位置
	 * 球员向目标位置射门。
	 * @see Player::detect_shot
	 */
	private void shot(Vector3 goal) {
		message = "射门";

		force = (goal - transform.position) * 2.5f;
		force *= 1.0f + 1.0f / ((goal - transform.position).magnitude / 200.0f + 1.0f);

		force += Vector3.up * (50.0f + goal.y * 1.6f + (goal - transform.position).sqrMagnitude / 1200.0f);

		if (force.magnitude > abilities.Strength * 8.0f) {
			force = force.normalized * abilities.Strength * 8.0f;
		} else if (force.magnitude < abilities.Strength * 5.0f) {
			force = force.normalized * abilities.Strength * 5.0f;
		}

		noise = new Vector3(Random.value - 0.5f, Random.value * 0.32f - 0.4f, Random.value - 0.5f);
		noise *= Mathf.Sqrt(102 - abilities.Shot) * Mathf.Sqrt(force.magnitude) * 0.5f;

		nextPlayer = this;
	}

	/**
	 * @brief 【物理】球员解围
	 * 球员将球大力踢出危险区域。
	 * @see Player::detect_clear
	 */
	private void clear() {
		message = "解围";

		noise = new Vector3(Random.value * 20.0f - 10.0f, Random.value * 20.0f + 5.0f, Random.value * 20.0f - 10.0f);

		Vector3 targetClearDirect;
		if (Vector3.Dot(team.AdvanceDirect, transform.forward) > 0) {
			//解围到中圈
			targetClearDirect = team.AdvanceDirect;
		} else {
			//解围到边线
			targetClearDirect = Mathf.Sign(transform.forward.x) * Vector3.forward;
		}
		float clear_rate = Random.value * 0.25f + Vector3.Dot(transform.forward, targetClearDirect) * 0.75f;
		force = (targetClearDirect + clear_rate * transform.forward) * abilities.Strength * (4.0f - 0.5f * clear_rate) +
				350.0f * Vector3.up;

		nextPlayer = this;
	}

	float catchBallTime = 0;    ///< 多久的将来能够追上足球


	/**
	 * @brief 【模型】分析追球与转向
	 * 分析足球还需多久能够追上
	 * @see Player::turn
	 * @see Player::follow_ball
	 */
	private Vector3 detect_turn() {
		Vector3 ballDist = ball.transform.position - transform.position;
		Vector3 BallVelocity = ball.rig.velocity * Mathf.Exp(-ballDist.magnitude * 0.001f);
		ballDist.y = 0;
		BallVelocity.y = 0;

		float MaxSpeed = abilities.RunningSpeed * 0.8f;

		float para_a = BallVelocity.sqrMagnitude - MaxSpeed * MaxSpeed;
		float para_b = 2 * Vector3.Dot(ballDist, BallVelocity);
		float para_c = ballDist.sqrMagnitude;
		float para_delta;

		if (Mathf.Abs(para_a) < 1e-6) {
			catchBallTime = -para_c / para_b;
		} else {
			para_delta = Mathf.Sqrt(para_b * para_b - 4 * para_a * para_c);

			float catchBallTime1 = (para_delta - para_b) / (2 * para_a);
			float catchBallTime2 = (-para_delta - para_b) / (2 * para_a);
			catchBallTime = Mathf.Min(catchBallTime1, catchBallTime2);
			if (catchBallTime < 0) {
				catchBallTime = Mathf.Max(catchBallTime1, catchBallTime2);
			}
			if (catchBallTime < 0 || float.IsNaN(catchBallTime)) {
				catchBallTime = 10.0f;
			}
		}

		para_a = 0.5f * Physics.gravity.y;
		para_b = ball.rig.velocity.y;
		para_c = ball.transform.position.y;
		para_delta = Mathf.Sqrt(para_b * para_b - 4 * para_a * para_c);

		float BallGroundTime = (-para_delta - para_b) / (2 * para_a);
		if (catchBallTime < BallGroundTime * 1.25f) {
			catchBallTime = BallGroundTime * 1.25f;
		}

		Vector3 ballDirect = ballDist + BallVelocity * catchBallTime;
		ballDirect.y = 0;

		ballDirect = ballDirect.normalized;

		turn(ballDirect);
		return ballDirect;
	}

	/**
	 * @brief 【物理】球员追球
	 * 球员根据试探方向追球。
	 * @see Player::detect_turn
	 */
	private void follow_ball() {
		Vector3 ballDirect = detect_turn();

		float MaxSpeed = abilities.RunningSpeed * 0.8f;
		Vector3 runForce = ballDirect * (1.6f + abilities.RunningSpeed * 0.016f);
		rig.AddForce(runForce, ForceMode.VelocityChange);
		if (rig.velocity.magnitude > MaxSpeed) {
			rig.velocity = rig.velocity.normalized * MaxSpeed;
		}
	}

	/**
	 * @brief 【模型】分析盘带
	 * 分析球员盘带得分与盘带方向。
	 * @see Player::dribble
	 */
	private void detect_dribble() {
		float dribble_score = 10.0f * Mathf.Exp(-(Time.time - getBallTime) / 2.0f);
		Vector3 dribble_direct =
			(enemy.GoalX * Vector3.right - transform.position).normalized * 0.5f +
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +				//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +			//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +			//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect;			//对方门线
		if (playPosition == Position.GoalKeeper) {
			dribble_score *= 0.005f;
		}

		float ballGoalXDist = 1.0f - 0.9f * Mathf.Exp(Mathf.Pow((team.GoalX - ball.transform.position.x) * team.AdvanceDirect.x * 0.001f, 3.0f));

		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 position_diff = en.player.transform.position - transform.position;
			Vector3 forward_diff = en.player.transform.forward - transform.forward;
			Vector3 velocity_diff = en.player.rig.velocity - rig.velocity;
			position_diff.y = 0;
			float curDist = 1.0f - Mathf.Exp((50.0f - (position_diff + forward_diff * 10.0f + velocity_diff * 0.1f).magnitude) * ballGoalXDist * (playPosition == Position.GoalKeeper ? 0.001f : 0.01f));

			dribble_direct -= 0.5f * position_diff.normalized / Mathf.Pow(position_diff.magnitude / 20.0f, 3.0f);

			dribble_score *= Mathf.Max(curDist, 0f);
		}
		dribble_score *= 0.01f * team.tactic.dribblingMoti + 0.1f;

		if (dribble_direct.magnitude > 32f) {
			dribble_direct = dribble_direct.normalized * 32f;
		}
		if (dribble_direct.magnitude < 20f) {
			dribble_direct = dribble_direct.normalized * 20f;
		}

		//惩罚失位
		Vector3 deviate = standposi + team.heart - transform.position;
		dribble_score *= Mathf.Exp(-Mathf.Pow(
			deviate.magnitude / (playPosition == Position.GoalKeeper ? 200.0f : 2000.0f), 2.0f
		));

		if (behaviorType == BehaviorType.Skillful) {
			dribble_score *= 1.25f;
		}

		if (actionScore < dribble_score) {
			actionScore = dribble_score;
			dribble(dribble_direct);
		}

		if (dribble_score < 0) print("dribble_score = " + dribble_score);
	}

	/**
	 * @brief 【模型】分析地面传球
	 * 分析球员地面传球得分与传球对象。
	 * @see Player::pass
	 */
	private void detect_ground_simple() {
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position;
			Vector3 deltaPosition = teamPlayerPosi - transform.position;

			float ground_simple_score = Mathf.Exp(
				Vector3.Dot(
					deltaPosition.normalized,
					(ball.transform.position - transform.position).normalized
				) * 0.1f - 0.1f
			);
			ground_simple_score *= 0.9f - 0.01f * team.tactic.dribblingMoti;
			//过滤掉过短的传球
			ground_simple_score *= 1.0f - Mathf.Exp(-0.005f * deltaPosition.magnitude);
			//推进方向
			ground_simple_score *= 1.0f - 0.2f * Mathf.Exp(
				-Mathf.Pow(deltaPosition.x / 25.0f, 3.0f) * team.AdvanceDirect.x * Mathf.Pow((enemy.GoalX - enemy.heart.x) / 1000.0f, 2.0f) *
					(team.tactic.advanceSpeed - 2) * 0.005f +
				Mathf.Sqrt(Time.time - team.getBallTime) * 0.125f - 0.25f
			);
			
			//下底传中
			ground_simple_score *= 2.0f - 2.0f * Mathf.Exp(-
				Mathf.Pow((Border.BorderZ - Mathf.Abs(teamPlayerPosi.z)) / 100.0f, 4.0f) *
				Mathf.Pow((enemy.GoalX - transform.position.x) / 100.0f, 2.0f)
			);
			//惩罚回传门将
			if (fr.position == Position.GoalKeeper) {
				ground_simple_score *= 0.00001f;
			}

			float Kx = teamPlayerPosi.z - transform.position.z;
			float Ky = transform.position.x - teamPlayerPosi.x;
			float B = -Kx * transform.position.x - Ky * transform.position.z;

			float frGoalXDist = 1.0f - 0.9f * Mathf.Exp(Mathf.Pow((team.GoalX - teamPlayerPosi.x) * team.AdvanceDirect.x * 0.001f, 3.0f));

			foreach (Team.Member en in enemy.members) {
				if (en.player == null) continue;

				Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward + en.player.rig.velocity;

				float line_distance = 0.0f;
				if (Vector3.Dot(deltaPosition, enemyPlayerPosi - teamPlayerPosi) > 0.0f) {
					line_distance = (enemyPlayerPosi - teamPlayerPosi).magnitude - 12.5f;
				} else if (Vector3.Dot(transform.position - enemyPlayerPosi, deltaPosition) > 0.0f) {
					line_distance = deltaPosition.magnitude * 10.0f - 12.5f;
				} else {
					line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 25.0f;
				}

				float curDist = 1.0f - Mathf.Exp(-line_distance * 0.002f * frGoalXDist);
				if (en.player.isWing()) {
					curDist *= 0.7f + (0.6f * team.tactic.wingSideMoti);
				}
				ground_simple_score *= Mathf.Max(curDist, 0f);
			}
			if (actionScore < ground_simple_score) {
				actionScore = ground_simple_score;
				pass(fr.player, false, false);
			}

			//if (ground_simple_score < 0) print("targetName = " + fr.player.playerName + ", ground_simple_score = " + ground_simple_score);
		}
	}

	/**
	 * @brief 【模型】分析地面直塞
	 * 分析球员地面直塞得分与传球对象。
	 * @see Player::pass
	 */
	private void detect_ground_smart() {
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.rig.velocity + team.AdvanceDirect * 20.0f;
			Vector3 deltaPosition = teamPlayerPosi - transform.position;

			float ground_smart_score = Mathf.Exp(
				Vector3.Dot(
					deltaPosition.normalized,
					(ball.transform.position - transform.position).normalized
				) * 0.1f - 0.1f
			);
			ground_smart_score *= 0.9f - 0.01f * team.tactic.dribblingMoti;
			//过滤掉过短的传球
			ground_smart_score *= 1.0f - Mathf.Exp(-0.005f * deltaPosition.magnitude);
			//推进方向
			ground_smart_score *= 1.0f - 0.2f * Mathf.Exp(
				-Mathf.Pow(deltaPosition.x / 25.0f, 3.0f) * team.AdvanceDirect.x * Mathf.Pow((enemy.GoalX - enemy.heart.x) / 1000.0f, 2.0f) *
					(team.tactic.advanceSpeed - 2) * 0.005f +
				Mathf.Sqrt(Time.time - team.getBallTime) * 0.125f - 0.125f
			);
			//下底传中
			ground_smart_score *= 2.0f - 2.0f * Mathf.Exp(-
				Mathf.Pow((Border.BorderZ - Mathf.Abs(teamPlayerPosi.z)) / 100.0f, 4.0f) *
				Mathf.Pow((enemy.GoalX - transform.position.x) / 100.0f, 2.0f)
			);
			//惩罚回传门将
			if (fr.position == Position.GoalKeeper) {
				ground_smart_score *= 0.0000025f;
			}

			if (behaviorType == BehaviorType.Passing) {
				ground_smart_score *= 1.25f;
			}

			float Kx = teamPlayerPosi.z - transform.position.z;
			float Ky = transform.position.x - teamPlayerPosi.x;
			float B = -Kx * transform.position.x - Ky * transform.position.z;

			float frGoalXDist = 1.0f - 0.9f * Mathf.Exp(Mathf.Pow((team.GoalX - teamPlayerPosi.x) * team.AdvanceDirect.x * 0.001f, 3.0f));

			foreach (Team.Member en in enemy.members) {
				if (en.player == null) continue;

				Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward + en.player.rig.velocity;

				float line_distance = 0.0f;
				if (Vector3.Dot(deltaPosition, enemyPlayerPosi - teamPlayerPosi) > 0.0f) {
					line_distance = (enemyPlayerPosi - teamPlayerPosi).magnitude * 0.5f - 6.25f;
				} else if (Vector3.Dot(transform.position - enemyPlayerPosi, deltaPosition) > 0.0f) {
					line_distance = deltaPosition.magnitude * 5.0f - 6.25f;
				} else {
					line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 25.0f;
				}

				float curDist = 1.0f - Mathf.Exp(-line_distance * 0.0032f * frGoalXDist);
				ground_smart_score *= Mathf.Max(curDist, 0f);
			}
			if (actionScore < ground_smart_score) {
				actionScore = ground_smart_score;
				pass(fr.player, true, false);
			}

			//if (ground_smart_score < 0) print("targetName = " + fr.player.playerName + ", ground_smart_score = " + ground_smart_score);
		}
	}

	/**
	 * @brief 【模型】分析高空传球
	 * 分析球员高空传球得分与传球对象。
	 * @see Player::pass
	 */
	private void detect_air_simple() {
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position;
			Vector3 deltaPosition = teamPlayerPosi - transform.position;

			float air_simple_score = Mathf.Exp(
				Vector3.Dot(
					deltaPosition.normalized,
					(ball.transform.position - transform.position).normalized
				) * 0.1f - 0.1f
			);
			air_simple_score *= 0.9f - 0.01f * team.tactic.dribblingMoti;
			//过滤掉过短的传球
			air_simple_score *= 1.0f - Mathf.Exp(-0.0001f * deltaPosition.magnitude);
			//推进方向
			air_simple_score *= 1.0f - 0.8f * Mathf.Exp(
				-Mathf.Pow(deltaPosition.x / 25.0f, 3.0f) * team.AdvanceDirect.x * Mathf.Pow((enemy.GoalX - enemy.heart.x) / 1000.0f, 2.0f) *
					(team.tactic.advanceSpeed - 2) * 0.005f +
				Mathf.Sqrt(Time.time - team.getBallTime) * 0.125f
			);
			//长传战术加成
			air_simple_score *= 0.025f + 0.005f * team.tactic.longPassMoti;
			//边路传递
			air_simple_score *= Mathf.Exp(-(Border.BorderZ - Mathf.Abs(transform.position.z)) / 500.0f);
			//下底传中
			air_simple_score *= 1.25f - 1.25f * Mathf.Exp(-
				Mathf.Pow((Border.BorderZ - Mathf.Abs(teamPlayerPosi.z)) / 100.0f, 4.0f) *
				Mathf.Pow((enemy.GoalX - transform.position.x) / 100.0f, 2.0f)
			);
			//惩罚回传门将
			if (fr.position == Position.GoalKeeper) {
				air_simple_score *= 0.0000025f;
			}

			float frGoalXDist = 1.0f - 0.9f * Mathf.Exp(Mathf.Pow((team.GoalX - teamPlayerPosi.x) * team.AdvanceDirect.x * 0.001f, 3.0f));

			foreach (Team.Member en in enemy.members) {
				if (en.player == null) continue;

				Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward + en.player.rig.velocity;

				float line_distance = (enemyPlayerPosi - teamPlayerPosi).magnitude - 8.0f;

				float curDist = 1.0f - 1.0f * Mathf.Exp(-line_distance * frGoalXDist * (0.0032f + team.tactic.longPassMoti * 0.00024f));
				air_simple_score *= Mathf.Max(curDist, 0f);
			}
			if (actionScore < air_simple_score) {
				actionScore = air_simple_score;
				pass(fr.player, false, true);
			}
			//if(air_simple_score < 0) print("targetName = " + fr.player.playerName + ", air_simple_score = " + air_simple_score);
		}
	}

	/**
	 * @brief 【模型】分析过顶球
	 * 分析球员过顶传球得分与传球对象。
	 * @see Player::pass
	 */
	private void detect_air_smart() {
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.rig.velocity + team.AdvanceDirect * 25.0f;
			Vector3 deltaPosition = teamPlayerPosi - transform.position;

			float air_smart_score = Mathf.Exp(
				Vector3.Dot(
					deltaPosition.normalized,
					(ball.transform.position - transform.position).normalized
				) * 0.1f - 0.1f
			);
			air_smart_score *= 0.9f - 0.01f * team.tactic.dribblingMoti;
			//过滤掉过短的传球
			air_smart_score *= 1.0f - Mathf.Exp(-0.0001f * deltaPosition.magnitude);
			//推进方向
			air_smart_score *= 1.0f - 0.8f * Mathf.Exp(
				-Mathf.Pow(deltaPosition.x / 25.0f, 3.0f) * team.AdvanceDirect.x * Mathf.Pow((enemy.GoalX - enemy.heart.x) / 1000.0f, 2.0f) *
					(team.tactic.advanceSpeed - 2) * 0.005f +
				Mathf.Sqrt(Time.time - team.getBallTime) * 0.125f
			);
			//长传战术加成
			air_smart_score *= 0.02f + 0.004f * team.tactic.longPassMoti;
			//边路传递
			air_smart_score *= Mathf.Exp(-(Border.BorderZ - Mathf.Abs(transform.position.z)) / 500.0f);
			//下底传中
			air_smart_score *= 1.25f - 1.25f * Mathf.Exp(-
				Mathf.Pow((Border.BorderZ - Mathf.Abs(teamPlayerPosi.z)) / 100.0f, 4.0f) *
				Mathf.Pow((enemy.GoalX - transform.position.x) / 100.0f, 2.0f)
			);
			//惩罚回传门将
			if (fr.position == Position.GoalKeeper) {
				air_smart_score *= 0.000001f;
			}

			if (behaviorType == BehaviorType.Passing) {
				air_smart_score *= 1.25f;
			}

			float frGoalXDist = 1.0f - 0.9f * Mathf.Exp(Mathf.Pow((team.GoalX - teamPlayerPosi.x) * team.AdvanceDirect.x * 0.001f, 3.0f));

			foreach (Team.Member en in enemy.members) {
				if (en.player == null) continue;

				Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward + en.player.rig.velocity;

				float line_distance = (enemyPlayerPosi - teamPlayerPosi).magnitude - 8.0f;

				float curDist = 1.0f - 1.0f * Mathf.Exp(-line_distance * frGoalXDist * (0.0036f + team.tactic.longPassMoti * 0.00036f));
				air_smart_score *= Mathf.Max(curDist, 0f);
			}
			if (actionScore < air_smart_score) {
				actionScore = air_smart_score;
				pass(fr.player, true, true);
			}
			//if (air_smart_score < 0) print("targetName = " + fr.player.playerName + ", air_smart_score = " + air_smart_score);
		}
	}

	/**
	 * @brief 【模型】分析射门
	 * 分析球员射门得分与射门方向。
	 * @see Player::shot
	 */
	private void detect_shot() {
		float[] Zposi = {
			enemy.LeftBound,
			(enemy.LeftBound + enemy.RightBound) / 2.0f,
			enemy.RightBound
		};

		float visibleGoalCos = Vector3.Dot(((enemy.GoalX + 50.0f * team.AdvanceDirect.x) * Vector3.right - transform.position).normalized, team.AdvanceDirect);
		visibleGoalCos = Mathf.Pow(visibleGoalCos + 1.0f, 4.0f) / 16.0f - 0.125f;

		for (int z = 0; z < Zposi.Length; ++z) {
			float yVal = Random.value + Random.value - 1.0f;
			yVal = Mathf.Sign(yVal) * (1.0f - yVal) / 2.0f + 0.5f;
			Vector3 currentGoal = new Vector3(
				enemy.GoalX,
				yVal * enemy.TopBound + (1.0f - yVal) * enemy.BottomBound,
				Zposi[z]
			);

			float shot_score = 0.01f * visibleGoalCos *
				Mathf.Exp(32.0f - Mathf.Pow((transform.position - currentGoal).magnitude / 64.0f, 2.0f) * 4.0f);
			if (behaviorType == BehaviorType.Strength) {
				shot_score *= 1.25f;
			}

			float Kx = Zposi[z] - transform.position.z;
			float Ky = transform.position.x - enemy.GoalX;
			float B = -Kx * transform.position.x - Ky * transform.position.z;

			foreach (Team.Member en in enemy.members) {
				if (en.player == null) continue;

				Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;

				float line_distance = 0.0f;
				if (Vector3.Dot(currentGoal - transform.position, enemyPlayerPosi - currentGoal) > 0.0f) {
					line_distance = (enemyPlayerPosi - currentGoal).magnitude * 2.5f - 2.5f;
				} else if (Vector3.Dot(enemyPlayerPosi - transform.position, transform.position - currentGoal) > 0.0f) {
					line_distance = (transform.position - currentGoal).magnitude * 0.5f;
				} else {
					line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 5.0f;
				}
				float curDist;
				if (en.position == Position.GoalKeeper) {
					curDist = 1.0f - Mathf.Exp(-line_distance * 0.008f);
				} else {
					curDist = 1.0f - Mathf.Exp(-line_distance * 0.016f);
				}
				shot_score *= Mathf.Max(curDist, 0f);
			}

			//惩罚失位
			Vector3 deviate = standposi + team.heart - transform.position;
			deviate.x *= 0.1f;
			shot_score *= Mathf.Exp(-Mathf.Pow(deviate.magnitude / 160.0f, 2.0f));

			if (actionScore < shot_score) {
				actionScore = shot_score;
				shot(currentGoal);
			}
			
			//if (shot_score < 0) print("shot_score = " + shot_score);
		}
	}

	/**
	 * @brief 【模型】分析解围
	 * 分析球员解围得分。
	 * @see Player::clear
	 */
	private void detect_clear() {
		float clear_score = 0.1f - 0.1f * Mathf.Exp((transform.position.x - team.heart.x) * team.AdvanceDirect.x * 0.001f - 1.0f);
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + 0.25f * en.player.rig.velocity + 5.0f * en.player.transform.forward;

			float line_distance = (en.player.transform.position - ball.transform.position).magnitude - 50.0f;
			float curDist = Mathf.Exp(-line_distance * 0.002f);
			clear_score *= curDist * 0.01f;
		}
		//惩罚失位
		Vector3 deviate = standposi + team.heart - transform.position;
		deviate.x *= 0.1f;
		clear_score *= Mathf.Exp(-Mathf.Pow(deviate.magnitude / 180.0f, 2.0f));
		clear_score *= Mathf.Exp(-Mathf.Pow(Time.time - team.getBallTime, 2.0f));
		if (actionScore < clear_score) {
			actionScore = clear_score;
			clear();
		}
		
		//if (clear_score < 0) print("clear_score = " + clear_score);
	}

	/**
	 * @brief 【模型】持球进攻时试探
	 * 球员持球进攻时，分析球员各种行为的得分与对象。
	 */
	public void Detect_hand_attack() {
		follow_ball();

		if (ball.hasGo) {
			detect_dribble();
		}
		detect_ground_simple();
		detect_ground_smart();
		detect_air_simple();
		detect_air_smart();
	//	detect_clear();
		detect_shot();
	}


	/**
	 * @brief 【模型】分析前插
	 * @param[in] offsideLine	越位线的X坐标
	 * 分析球员前插得分，并计算出前插的虚拟力。
	 */
	private void detect_forward(float offsideLine) {
		Vector3 enemyVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = transform.position - enemyPlayerPosi;
			enemyVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 teamVirtualForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;
			Vector3 posiDiff = transform.position - teamPlayerPosi;
			teamVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position;
		Vector3 forward_VirtualForce =
			standPosi_diff * 0.01f +																//保持阵型
			team.AdvanceDirect * 10.0f +															//前进
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +				//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +			//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +			//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect +			//对方门线
			enemyVirtualForce * 160.0f + teamVirtualForce * 240.0f;									//松散地区

		forward_VirtualForce.y = 0;

		float maxForceMag = 1.5f + abilities.RunningSpeed * 0.015f;
		if (forward_VirtualForce.magnitude > maxForceMag) {
			forward_VirtualForce = forward_VirtualForce.normalized * maxForceMag;
		}

		float Kx = ball.transform.position.z - transform.position.z;
		float Ky = transform.position.x - ball.transform.position.x;
		float B = -Kx * transform.position.x - Ky * transform.position.z;

		Vector3 targetPosition = transform.position +
			team.AdvanceDirect * (ball.transform.position - transform.position).magnitude / abilities.RunningSpeed * 0.5f;
		float forward_score = 1.0f - Mathf.Exp((offsideLine - transform.position.x) * enemy.AdvanceDirect.x * 0.1f);
		if (isWing()) {
			forward_score *= 1.25f;
		}
		if (behaviorType == BehaviorType.Speeding) {
			forward_score *= 1.25f;
		}

		Vector3 deltaPosition = ball.transform.position - transform.position;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			forward_score *= 1.0f - Mathf.Exp(-(targetPosition - en.player.transform.position).magnitude * 0.2f);

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;

			float line_distance = 0.0f;
			if (Vector3.Dot(deltaPosition, enemyPlayerPosi - ball.transform.position) > 0.0f) {
				line_distance = (enemyPlayerPosi - ball.transform.position).magnitude * 0.5f;
			} else if (Vector3.Dot(transform.position - enemyPlayerPosi, deltaPosition) > 0.0f) {
				line_distance = deltaPosition.magnitude * 5.0f;
			} else {
				line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 20.0f;
			}

			float curDist = 1.0f - 1.0f * Mathf.Exp(-line_distance * 0.02f - Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(teamVirtualForce, forward_VirtualForce)) * 0.004f);
			forward_score *= Mathf.Max(curDist, 0f);
		}

		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			forward_score *= 1.0f - Mathf.Exp(-(targetPosition - fr.player.transform.position).magnitude * 0.01f);
		}

		if (actionScore < forward_score) {
			actionScore = forward_score;
			virtualForce = forward_VirtualForce;
			message = "前插";
		}
	}

	/**
	 * @brief 【模型】分析回撤接应
	 * @param[in] offsideLine	越位线的X坐标
	 * 分析球员回撤接应得分，并计算出回撤接应的虚拟力。
	 */
	private void detect_backward(float offsideLine) {
		Vector3 enemyVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = transform.position - enemyPlayerPosi;
			enemyVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 teamVirtualForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;
			Vector3 posiDiff = transform.position - teamPlayerPosi;
			teamVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position;
		Vector3 ballPosi_diff = ball.transform.position - transform.position;
		Vector3 backward_VirtualForce =
			standPosi_diff * 0.2f +																	//保持阵型
			ballPosi_diff * (1.0f - team.tactic.supportDist * 0.025f) +								//随球转移
			enemy.AdvanceDirect * 0.5f +                                                            //后退
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +				//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +			//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +			//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect +			//对方门线
			100.0f * enemy.AdvanceDirect *
				Mathf.Exp(-(transform.position.x - offsideLine) / 10.0f * enemy.AdvanceDirect.x) +  //反越位
			enemyVirtualForce * 320.0f + teamVirtualForce * 160.0f;									//松散地区

		backward_VirtualForce.y = 0;
		backward_VirtualForce += (ballPosi_diff.z + 200.0f * ball.transform.position.z) * Vector3.forward;

		float maxForceMag = 1.2f + abilities.RunningSpeed * 0.012f;
		if (backward_VirtualForce.magnitude > maxForceMag) {
			backward_VirtualForce = backward_VirtualForce.normalized * maxForceMag;
		}

		float Kx = ball.transform.position.z - transform.position.z;
		float Ky = transform.position.x - ball.transform.position.x;
		float B = -Kx * transform.position.x - Ky * transform.position.z;

		Vector3 targetPosition = (standposi + team.heart + ball.transform.position) / 2.0f;
		float backward_score = 2.0f * Mathf.Exp(-(ball.transform.position - transform.position).magnitude * 0.005f);
		if (behaviorType == BehaviorType.Skillful) {
			backward_score *= 1.25f;
		}

		Vector3 deltaPosition = ball.transform.position - transform.position;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			backward_score *= 1.0f - Mathf.Exp(-(targetPosition - en.player.transform.position).magnitude * 0.2f);

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;

			float line_distance = 0.0f;
			if (Vector3.Dot(deltaPosition, enemyPlayerPosi - ball.transform.position) > 0.0f) {
				line_distance = (enemyPlayerPosi - ball.transform.position).magnitude * 0.5f;
			} else if (Vector3.Dot(transform.position - enemyPlayerPosi, deltaPosition) > 0.0f) {
				line_distance = deltaPosition.magnitude * 5.0f;
			} else {
				line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 20.0f;
			}

			float curDist = 1.0f - 1.0f * Mathf.Exp(-line_distance * 0.005f - Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(teamVirtualForce, backward_VirtualForce)) * 0.008f);
			backward_score *= Mathf.Max(curDist, 0f);
		}

		foreach (Team.Member fr in team.members) {
			if (fr.player == null) continue;

			backward_score *= 1.0f - Mathf.Exp(-(targetPosition - fr.player.transform.position).magnitude * 0.01f);
		}

		if (actionScore < backward_score) {
			actionScore = backward_score;
			virtualForce = backward_VirtualForce;
			message = "回撤接应";
		}
	}

	/**
	 * @brief 【模型】分析保持阵型
	 * @param[in] offsideLine	越位线的X坐标
	 * 分析球员保持阵型得分，并计算出保持阵型的虚拟力。
	 */
	private void detect_stand(float offsideLine) {
		Vector3 enemyVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = transform.position - enemyPlayerPosi;
			enemyVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position;

		Vector3 teamVirtualForce = Vector3.zero;
		Vector3 standPosiForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;

			Vector3 posiDiff = transform.position - teamPlayerPosi;
			teamVirtualForce += posiDiff / posiDiff.sqrMagnitude;

			Vector3 standposiDiff = standPosi_diff + transform.position - teamPlayerPosi;
			standPosiForce += standposiDiff / standposiDiff.sqrMagnitude;
		}
		//若有人补位，惩罚回位
		float posiHoldRatio = Mathf.Exp(-standPosiForce.magnitude * 5.0f);


		Vector3 stand_VirtualForce =
			standPosi_diff * 0.64f * posiHoldRatio +												//保持阵型
			team.AdvanceDirect * 0.1f +                                                             //保持推进
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +				//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +			//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +			//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect +			//对方门线
			100.0f * enemy.AdvanceDirect *
				Mathf.Exp(-(transform.position.x - offsideLine) / 10.0f * enemy.AdvanceDirect.x) +  //反越位
			enemyVirtualForce * 240.0f + teamVirtualForce * 160.0f;									//松散地区

		Vector3 ballPosi_diff = ball.transform.position - transform.position;
		if (playPosition == Position.GoalKeeper) {
			stand_VirtualForce += ballPosi_diff * 0.01f +
				-ballPosi_diff / ballPosi_diff.sqrMagnitude * 10.0f;								//随球转移
		} else {
			stand_VirtualForce += ballPosi_diff * 0.1f +
				-ballPosi_diff / ballPosi_diff.sqrMagnitude * 100.0f;								//随球转移
		}

		stand_VirtualForce.y = 0;

		float maxForceMag = 1.2f + abilities.RunningSpeed * 0.012f;
		if (stand_VirtualForce.magnitude > maxForceMag) {
			stand_VirtualForce = stand_VirtualForce.normalized * maxForceMag;
		}

		Vector3 targetPosition = standposi + team.heart;
		float stand_score = 1.0f - Mathf.Exp(-(targetPosition - transform.position).magnitude * 0.01f);
		stand_score *= posiHoldRatio;
		if (behaviorType == BehaviorType.Light) {
			stand_score *= 0.75f;
		}
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			stand_score *= 1.0f - Mathf.Exp(-(targetPosition - en.player.transform.position).magnitude * 0.004f);
		}

		if (actionScore < stand_score) {
			actionScore = stand_score;
			virtualForce = stand_VirtualForce;
			message = "保持阵型";
		}
	}

	/**
	 * @brief 【模型】分析切换持球
	 * 如果自己比持球者早2秒以上拿到球，那么切换到自己持球。
	 */
	private void detect_switch() {
		if (Time.time - team.BallHander.getBallTime > 2.0f - 0.005f * abilities.ReactionSpeed &&
			catchBallTime + 2.0f < team.BallHander.catchBallTime &&
			(ball.transform.position - transform.position).magnitude < (ball.transform.position - team.BallHander.transform.position).magnitude) {
			team.BallHander = this;
			getBallTime = Time.time;

			message = "切换持球";
			print(playerName + " # " + message);
		}
	}

	/**
	 * @brief 【模型】分析拉边
	 * @param[in] offsideLine	越位线的X坐标
	 * 分析球员拉边得分，并计算出拉边的虚拟力。
	 */
	private void detect_wing(float offsideLine) {
		Vector3 enemyVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = transform.position - enemyPlayerPosi;
			enemyVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 teamVirtualForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;
			Vector3 posiDiff = transform.position - teamPlayerPosi;
			teamVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position;
		Vector3 wing_VirtualForce =
			standPosi_diff * 0.25f +																//保持阵型
			Vector3.forward * Mathf.Sign(transform.position.z) * 100.0f +							//拉边
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +				//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +			//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +			//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect +			//对方门线
			enemyVirtualForce * 160.0f + teamVirtualForce * 240.0f;									//松散地区

		wing_VirtualForce.y = 0;

		float maxForceMag = 1.2f + abilities.RunningSpeed * 0.012f;
		if (wing_VirtualForce.magnitude > maxForceMag) {
			wing_VirtualForce = wing_VirtualForce.normalized * maxForceMag;
		}

		float Kx = ball.transform.position.z - transform.position.z;
		float Ky = transform.position.x - ball.transform.position.x;
		float B = -Kx * transform.position.x - Ky * transform.position.z;

		Vector3 targetPosition = transform.position.x * Vector3.right +
			Border.BorderZ * 0.96f * Mathf.Sign(transform.position.z) * Vector3.forward;
		float wing_score = Mathf.Exp(-Mathf.Pow(ball.transform.position.x - transform.position.x, 2.0f) * 0.01f);
		wing_score *= team.tactic.wingSideMoti * 0.1f;
		wing_score *= 1.0f - Mathf.Exp((ball.transform.position.x - enemy.GoalX) * team.AdvanceDirect.x * 0.001f);

		//惩罚失位
		Vector3 deviate = standposi + team.heart - transform.position;
		deviate.x *= 0.1f;
		wing_score *= Mathf.Exp(-Mathf.Pow(deviate.magnitude / 200.0f, 2.0f));

		if (isWing()) {
			wing_score *= 5.0f;
		}

		Vector3 deltaPosition = ball.transform.position - transform.position;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			wing_score *= 1.0f - Mathf.Exp(-(targetPosition - en.player.transform.position).magnitude * 0.2f);

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;

			float line_distance = 0.0f;
			if (Vector3.Dot(deltaPosition, enemyPlayerPosi - ball.transform.position) > 0.0f) {
				line_distance = (enemyPlayerPosi - ball.transform.position).magnitude * 0.5f;
			} else if (Vector3.Dot(transform.position - enemyPlayerPosi, deltaPosition) > 0.0f) {
				line_distance = deltaPosition.magnitude * 5.0f;
			} else {
				line_distance = Mathf.Abs(Kx * enemyPlayerPosi.x + Ky * enemyPlayerPosi.z + B) / Mathf.Sqrt(Kx * Kx + Ky * Ky) - 20.0f;
			}

			float curDist = 1.0f - 1.0f * Mathf.Exp(-line_distance * 0.02f - Mathf.Cos(Mathf.Deg2Rad * Vector3.Angle(teamVirtualForce, wing_VirtualForce)) * 0.004f);
			wing_score *= Mathf.Max(curDist, 0f);
		}

		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			wing_score *= 1.0f - Mathf.Exp(-(targetPosition - fr.player.transform.position).magnitude * 0.01f);
		}

		if (actionScore < wing_score) {
			actionScore = wing_score;
			virtualForce = wing_VirtualForce;
			message = "拉边";
		}
	}

	/**
	 * @brief 【模型】未持球进攻时试探
	 * 球员未持球且进攻时，分析球员各种行为的得分与虚拟力的大小方向。
	 */
	public void Detect_stand_attack() {
		detect_turn();
		detect_switch();

		float offsideLine = 0;
		if (ball.transform.position.x * team.AdvanceDirect.x > enemy.heart.x * team.AdvanceDirect.x) {
			offsideLine = ball.transform.position.x;
		} else if (enemy.heart.x * team.AdvanceDirect.x > 0) {
			offsideLine = enemy.heart.x;
		}

		detect_stand(offsideLine);
		detect_backward(offsideLine);

		if (playPosition == Position.GoalKeeper) {
			detect_save();
		} else {
			detect_forward(offsideLine);
			detect_wing(offsideLine);
		}


		float MaxSpeed = abilities.RunningSpeed * 0.8f;
		rig.AddForce(virtualForce, ForceMode.VelocityChange);
		if (rig.velocity.magnitude > MaxSpeed) {
			rig.velocity = rig.velocity.normalized * MaxSpeed;
		}

		//	if (message != null) {
		//		print(playerName + " # " + message);
		//	}
	}

	/**
	 * @brief 【模型】分析施压
	 * 分析球员施压得分，并计算出施压的虚拟力。
	 */
	private void detect_press() {
		Vector3 pressVirtualForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;
			Vector3 posiDiff = ball.transform.position - teamPlayerPosi;
			pressVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}
		Vector3 supportVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = ball.transform.position - enemyPlayerPosi;
			supportVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position - 0.1f * standposi.x * Vector3.right;
		Vector3 ballPosi_diff = ball.transform.position - transform.position;

		Vector3 ballDirect = detect_turn();

		Vector3 press_VirtualForce =
			standPosi_diff * 0.1f +																			//保持阵型
			ballPosi_diff * (0.1f + 0.5f / Mathf.Pow(0.01f + ballPosi_diff.magnitude / 32.0f, 2.0f)) +		//随球转移
			ballDirect * 1.0f +																				//追球
			enemy.AdvanceDirect * 0.125f +																	//保持后退
			50.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +						//边线
			50.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +					//边线
			100.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +					//本方底线
			50.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect;					//对方门线

		press_VirtualForce.y = 0;

		float maxForceMag = 1.5f + abilities.RunningSpeed * 0.015f;
		if (press_VirtualForce.magnitude > maxForceMag) {
			press_VirtualForce = press_VirtualForce.normalized * maxForceMag;
		}

		float press_score =
			Mathf.Exp(-Mathf.Pow((ball.transform.position - transform.position).sqrMagnitude * 0.0001f, 2.0f)) *
			Mathf.Exp(-Mathf.Pow(pressVirtualForce.sqrMagnitude * 0.001f, 2.0f)) *
			Mathf.Exp(-Mathf.Pow(supportVirtualForce.sqrMagnitude * 0.001f, 2.0f)) *
			Mathf.Exp(-supportVirtualForce.sqrMagnitude * pressVirtualForce.sqrMagnitude * 0.000001f) *
			Mathf.Exp(Mathf.Pow((team.GoalX - ball.transform.position.x) * team.AdvanceDirect.x / 1000.0f, 3.0f) * (1.0f - team.tactic.pressureMoti / 40.0f));
		if (behaviorType == BehaviorType.Strength) {
			press_score *= 1.25f;
		}

		if (playPosition == Position.GoalKeeper) {
			press_score *= Mathf.Exp(-supportVirtualForce.sqrMagnitude * 40.0f);
		}

		if (actionScore < press_score) {
			actionScore = press_score;
			virtualForce = press_VirtualForce;
			message = "施压";
		}
	}

	/**
	 * @brief 【模型】分析盯人
	 * 分析球员盯人得分，并计算出盯人的虚拟力。
	 */
	private void detect_cut() {
		float cut_score = 0.0f;
		Vector3 cut_VirtualForce = Vector3.zero;
		Vector3 standPosi_diff = standposi + team.heart - transform.position - 0.1f * standposi.x * Vector3.right;

		foreach (Team.Member en in enemy.members) {
			if (en.player == null || enemy.BallHander == en.player) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = enemyPlayerPosi - transform.position;

			Vector3 watchVirtualForce = Vector3.zero;
			foreach (Team.Member fr in team.members) {
				if (fr.player == null || fr.player == this) continue;

				Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;
				Vector3 cutPosiDiff = enemyPlayerPosi - teamPlayerPosi;
				watchVirtualForce += cutPosiDiff / cutPosiDiff.sqrMagnitude;
			}

			Vector3 enemyDirect = posiDiff +
				(ball.rig.velocity * (1.0f + ball.transform.position.y / 200.0f) +
				Vector3.Cross(ball.rig.angularVelocity, ball.rig.velocity) * 0.001f) * posiDiff.magnitude / abilities.RunningSpeed * 0.4f -
				force.normalized * 10.0f;
			enemyDirect.y = 0;

			Vector3 curVirtualForce =
				standPosi_diff * 0.5f +                                                                         //保持阵型
				posiDiff * (0.5f + 1.25f / Mathf.Pow(0.01f + posiDiff.magnitude / 32.0f, 2.0f)) +               //随人转移
				enemyDirect * 1.0f +                                                                            //追人
				enemy.AdvanceDirect * 0.25f +                                                                   //保持后退
				500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +						//边线
				500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +					//边线
				1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +					//本方底线
				500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect;					//对方门线

			curVirtualForce.y = 0;

			float maxForceMag = 1.4f + abilities.RunningSpeed * 0.014f;
			if (curVirtualForce.magnitude > maxForceMag) {
				curVirtualForce = curVirtualForce.normalized * maxForceMag;
			}

			float curDist =
				Mathf.Exp(-(en.player.transform.position - transform.position).magnitude * 0.001f) *
				Mathf.Exp(-(en.player.transform.position - ball.transform.position).magnitude * 0.0001f) *
				Mathf.Exp(-watchVirtualForce.sqrMagnitude * 10.0f) *
				Mathf.Exp(Mathf.Pow((team.GoalX - en.player.transform.position.x) * team.AdvanceDirect.x / 1000.0f, 3.0f));

			foreach (Team.Member fr in team.members) {
				if (fr.player == null || fr.player == this) continue;

				curDist *= 1.0f - Mathf.Exp(-(enemyPlayerPosi - fr.player.transform.position).magnitude * 0.01f);
			}

			if (cut_score < curDist) {
				cut_score = curDist;
				cut_VirtualForce = curVirtualForce;
			}
		}
		if (behaviorType == BehaviorType.Light) {
			cut_score *= 1.25f;
		}

		if (actionScore < cut_score) {
			actionScore = cut_score;
			virtualForce = cut_VirtualForce;
			message = "盯人";
		}
	}

	/**
	 * @brief 【模型】分析退防
	 * 分析球员退防得分，并计算出退防的虚拟力。
	 */
	private void detect_keep() {
		Vector3 enemyVirtualForce = Vector3.zero;
		foreach (Team.Member en in enemy.members) {
			if (en.player == null) continue;

			Vector3 enemyPlayerPosi = en.player.transform.position + en.player.transform.forward;
			Vector3 posiDiff = transform.position - enemyPlayerPosi;
			enemyVirtualForce += posiDiff / posiDiff.sqrMagnitude;
		}

		Vector3 standPosi_diff = standposi + team.heart - transform.position - 0.1f * standposi.x * Vector3.right;
		Vector3 ballPosi_diff = ball.transform.position - transform.position;
		ballPosi_diff.x *= 0.1f;

		Vector3 teamVirtualForce = Vector3.zero;
		Vector3 standPosiForce = Vector3.zero;
		foreach (Team.Member fr in team.members) {
			if (fr.player == null || fr.player == this) continue;

			Vector3 teamPlayerPosi = fr.player.transform.position + fr.player.transform.forward;

			Vector3 posiDiff = transform.position - teamPlayerPosi;
			teamVirtualForce += posiDiff / posiDiff.sqrMagnitude;

			Vector3 standposiDiff = standPosi_diff + transform.position - teamPlayerPosi;
			standPosiForce += standposiDiff / standposiDiff.sqrMagnitude;
		}
		//若有人补位，惩罚回位
		float posiHoldRatio = Mathf.Exp(-standPosiForce.magnitude * 5.0f);

		float offsideLine = 0;
		if (ball.transform.position.x * team.AdvanceDirect.x > enemy.heart.x * team.AdvanceDirect.x) {
			offsideLine = ball.transform.position.x;
		} else if (enemy.heart.x * team.AdvanceDirect.x > 0) {
			offsideLine = enemy.heart.x;
		}

		Vector3 keep_VirtualForce =
			standPosi_diff * (2.0f - standposi.x * team.AdvanceDirect.x / 720.0f) * posiHoldRatio +			//保持阵型
			ballPosi_diff * (0.3f + 0.04f * team.tactic.compactness +
							 1.25f / Mathf.Pow(0.01f + ballPosi_diff.magnitude / 32.0f, 2.0f)) +			//随球转移
			enemy.AdvanceDirect * 5.0f +																	//保持后退
			500.0f / Mathf.Abs(transform.position.z - Border.BorderZ) * Vector3.back +						//边线
			500.0f / Mathf.Abs(-transform.position.z - Border.BorderZ) * Vector3.forward +					//边线
			1000.0f / Mathf.Abs(transform.position.x - team.GoalX) * team.AdvanceDirect +					//本方底线
			500.0f / Mathf.Abs(transform.position.x - enemy.GoalX) * enemy.AdvanceDirect +					//对方门线
			100.0f * enemy.AdvanceDirect *
				Mathf.Exp(-(transform.position.x - offsideLine) / 25.0f * enemy.AdvanceDirect.x) +          //造越位
			enemyVirtualForce * 40.0f + teamVirtualForce * (120.0f - 4.0f * team.tactic.compactness);		//松散地区

		if (playPosition == Position.GoalKeeper) {
			keep_VirtualForce += 12.5f * Vector3.forward *
				Mathf.Sign(ballPosi_diff.z) * Mathf.Sqrt(Mathf.Abs(ballPosi_diff.z));                       //随球转移
		} else {
			keep_VirtualForce += enemy.AdvanceDirect * Mathf.Pow(
				2.0f - Mathf.Pow((standposi.x * team.AdvanceDirect.x - 400.0f) / 400.0f, 2.0f), 3.0f
			);                                                                                              //补在后卫线处
			if (isMidField()) {
				keep_VirtualForce += (ballPosi_diff.x - 100.0f * team.AdvanceDirect.x) * Vector3.right * 4.0f + ballPosi_diff;
			}
		}

		keep_VirtualForce.y = 0;

		float keep_score = 1.0f - Mathf.Exp(-(ball.transform.position - transform.position).magnitude * 0.01f);
		keep_score *= posiHoldRatio;

		if (isWing()) {
			keep_score *= 0.75f;
		}

		if(isFrontField()) {
			keep_score *= 0.5f;
		}

		float maxForceMag = 1.2f + abilities.RunningSpeed * 0.012f;
		if (keep_VirtualForce.magnitude > maxForceMag) {
			keep_VirtualForce = keep_VirtualForce.normalized * maxForceMag;
		}

		if (actionScore < keep_score) {
			actionScore = keep_score;
			virtualForce = keep_VirtualForce;
			message = "退防";
		}
	}

	/**
	 * @brief 【模型】分析扑救（仅限守门员）
	 * 分析门将是否需要扑救，并计算出扑救的爆发力。
	 */
	private void detect_save() {
		float maxForceMag = 50.0f + abilities.SaveMeasure * 0.5f;

		Vector3 ballVelo = ball.rig.velocity;

		if (Time.time - ball.kickBallTime < (0.25f - 0.002f * abilities.ReactionSpeed) || ballVelo.magnitude == 0 ||
			(transform.position - ball.transform.position).magnitude / ballVelo.magnitude > 0.4f ||
			Vector3.Dot(transform.position - ball.transform.position, ballVelo) < 0) {
			rig.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			return;
		}

		float Kx = ballVelo.z;
		float Ky = -ballVelo.x;
		float B_ball = -Kx * ball.transform.position.x - Ky * ball.transform.position.z;
		float B_gk = -Kx * transform.position.x - Ky * transform.position.z;

		float crashDist = (B_gk - B_ball) / Mathf.Sqrt(Kx * Kx + Ky * Ky);
		if (Mathf.Abs(crashDist) > abilities.SaveMeasure / 5.0f + 40.0f) {
			return;
		}

		rig.constraints = RigidbodyConstraints.None;

		Vector3 saveForce =
			crashDist * 1.0f * (Kx * Vector3.right + Ky * Vector3.forward);
		if (saveForce.magnitude > maxForceMag) {
			saveForce = saveForce.normalized * maxForceMag;
		}
		saveForce += Vector3.up * (2.0f * ballVelo.y + 6.0f * ball.transform.position.y - 20.0f);

		if (isGrounded() && isJumpable()) {
			//		print(saveForce);
			rig.AddForceAtPosition(saveForce, transform.position + Vector3.up * standposi.y * 0.8f, ForceMode.VelocityChange);
		}

		if (ball.rig.velocity.magnitude + ball.rig.angularVelocity.magnitude * 0.5f > Mathf.Sqrt(abilities.StopBall * 2000.0f)) {
			force = saveForce * 0.001f - transform.forward * 0.1f +
				(ball.transform.position - transform.position).normalized * 0.2f;
			noise = new Vector3(Random.value * 1.0f - 0.5f, Random.value * 1.5f + 0.5f, Random.value * 1.0f - 0.5f);
			injured = Time.deltaTime * 10.0f;
		}
	}

	/**
	 * @brief 【模型】未持球防守时试探
	 * 球员未持球且防守时，分析球员各种行为的得分与虚拟力的大小方向。
	 */
	public void Detect_defence() {
		detect_keep();
		if (ball.hasGo) {
			detect_press();
		} else {
			detect_turn();
		}

		if (playPosition == Position.GoalKeeper) {
			detect_save();
		} else {
			detect_cut();
		}

		float MaxSpeed = abilities.RunningSpeed * 0.8f;
		rig.AddForce(virtualForce, ForceMode.VelocityChange);
		if (rig.velocity.magnitude > MaxSpeed) {
			rig.velocity = rig.velocity.normalized * MaxSpeed;
		}

		//	if (message != null) {
		//		print(playerName + " # " + message);
		//	}
	}

	/**
	 * @brief 【模型】正常比赛试探
	 * 正常比赛过程中，分析球员各种行为的得分、对象与虚拟力的大小方向。
	 */
	public void Detect_Action() {
		actionScore = float.NegativeInfinity;

		if (team.BallHander == this) {
			Detect_hand_attack();
		} else if (team.BallHander != null) {
			Detect_stand_attack();
		} else {
			Detect_defence();
		}
	}

	/**
	 * @brief 【物理】定位球发球者接近球
	 * 计算球员靠近球的虚拟力。
	 */
	private void attach_ball() {
		Vector3 targetPosition = ball.transform.position + 50.0f * enemy.AdvanceDirect;

		Vector3 ballVirtualForce = transform.position - ball.transform.position;
		ballVirtualForce /= ballVirtualForce.sqrMagnitude;

		virtualForce = targetPosition - transform.position + ballVirtualForce * 25.0f;

		float maxForceMag = 1.4f + abilities.RunningSpeed * 0.014f;
		if (virtualForce.magnitude > maxForceMag) {
			virtualForce = virtualForce.normalized * maxForceMag;
		}
	}

	/**
	 * @brief 【物理】非发球者球员离开球
	 * 计算球员远离球的虚拟力。
	 */
	private void leave_ball() {
		Vector3 ballVirtualForce = transform.position - ball.transform.position;
		ballVirtualForce /= ballVirtualForce.sqrMagnitude;
		ballVirtualForce *= 50.0f;

		float maxForceMag = 1.4f + abilities.RunningSpeed * 0.014f;
		if (ballVirtualForce.magnitude > maxForceMag) {
			ballVirtualForce = ballVirtualForce.normalized * maxForceMag;
		}

		if (ballVirtualForce.magnitude < 0.1f) {
			virtualForce = Vector3.zero;
			return;
		}

		float leave_score = 1.0f - Mathf.Exp(-ballVirtualForce.magnitude);
		if (actionScore < leave_score) {
			actionScore = leave_score;
			virtualForce = ballVirtualForce;
		}
	}

	/**
	 * @brief 【物理】准备开中圈球前球员归位
	 * 计算球员归位的虚拟力。
	 */
	private void keep_stand() {
		Vector3 keepStandVirtualForce = new Vector3(
			standposi.x / 1.6f + team.heart.x - transform.position.x,
			standposi.y - transform.position.y,
			standposi.z - transform.position.z
		);

		float maxForceMag = 1.2f + abilities.RunningSpeed * 0.012f;
		if (keepStandVirtualForce.magnitude > maxForceMag) {
			keepStandVirtualForce = keepStandVirtualForce.normalized * maxForceMag;
		}
		actionScore = 1.0f;
		virtualForce = keepStandVirtualForce;
	}

	/**
	 * @brief 【模型】未开球时试探
	 * 比赛进行但暂未开球时，分析球员各种行为的得分、对象与虚拟力的大小。
	 */
	public void Detect_FreeKick() {
		actionScore = float.NegativeInfinity;

		if (team.BallHander == this) {
			attach_ball();
			detect_turn();

			float MaxSpeed = abilities.RunningSpeed * 0.8f;
			rig.AddForce(virtualForce, ForceMode.VelocityChange);
			if (rig.velocity.magnitude > MaxSpeed) {
				rig.velocity = rig.velocity.normalized * MaxSpeed;
			}
		} else if (team.BallHander != null && !ball.GoalReset) {
			Detect_stand_attack();
		} else {
			leave_ball();

			if (ball.GoalReset) {
				keep_stand();
			} else {
				detect_keep();
				if (playPosition != Position.GoalKeeper) {
					detect_cut();
				}
			}

			float MaxSpeed = abilities.RunningSpeed * 0.8f;
			rig.AddForce(virtualForce, ForceMode.VelocityChange);
			if (rig.velocity.magnitude > MaxSpeed) {
				rig.velocity = rig.velocity.normalized * MaxSpeed;
			}
		}
	}

	/**
	 * @brief 碰撞进入时调用
	 * @param[in] collision		包含了碰撞对象、碰撞刚体、碰撞力度等信息
	 * 球员遇到碰撞时，判断碰撞来源，如果是足球，那么根据得分最高的行为力完成对球的处理；同时判断碰撞力度是否过大，过大则球员会受伤。
	 */
	private void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.CompareTag("Ball") && ball.isGo) {
			if (ball.curTouch != this) {
				ball.preTouch = ball.curTouch;
				ball.curTouch = this;
			}
			ball.hasGo = true;
			ball.kickBallTime = Time.time;
			if (team.BallHander != null &&
				Time.time - team.BallHander.getBallTime < 1.25f - 0.005f * abilities.ReactionSpeed &&
				Vector3.Dot(collision.impulse, transform.forward) < -0.5f * collision.impulse.magnitude) {
				print(playerName + "  碰撞");
				ball.isKick = false;
				return;
			}
			ball.isKick = true;

			float keepVeloRate;

			if (team.BallHander == null) {
				//取得球权
				if (enemy.getBallTime - team.getBallTime <= Time.time - enemy.getBallTime) {
					team.getBallTime = Time.time;
				} else {
					team.getBallTime += Time.time - enemy.getBallTime;
				}
				getBallTime = Time.time;
				if (enemy.BallHander != null &&
					(enemy.BallHander.transform.position - transform.position).magnitude < 50 &&
					collision.rigidbody.velocity.magnitude < 150) {
					//抢断
					print(playerName + "  抢断");

					collision.rigidbody.AddForce(transform.forward * (abilities.Strength * 0.05f + 5.0f));
				} else {
					//拦截
					if (playPosition == Position.GoalKeeper && (ball.preTouch.team != team || !ball.isKick)) {
						if (force.magnitude > 1.0f) {
							print(playerName + "  扑救");
							ball.isKick = false;
							collision.rigidbody.AddForceAtPosition(
								force + noise,
								transform.position + transform.forward * abilities.Height * 0.1f
							);
							force = Vector3.zero;
							noise = Vector3.zero;
							keepVeloRate = 0.0f;
						} else {
							print(playerName + "  抱球");
							keepVeloRate = Mathf.Sqrt(abilities.StopBall * 1200.0f);
							injured = Time.deltaTime;
						}
					} else {
						print(playerName + "  拦截");
						keepVeloRate = Mathf.Sqrt(abilities.StopBall * 240.0f);
					}
					float ballSpeed = collision.rigidbody.velocity.magnitude;
					if (keepVeloRate > ballSpeed) {
						collision.rigidbody.velocity = Vector3.zero;
					} else if (ballSpeed != 0) {
						collision.rigidbody.velocity *= (ballSpeed - keepVeloRate) / ballSpeed;
					}
					collision.rigidbody.angularVelocity *= 1.0f - Mathf.Exp(-keepVeloRate / 10.0f);
				}
				team.BallHander = this;
				enemy.BallHander = null;
			} else if (nextPlayer != null) {
				//传球、解围、射门
				print(playerName + ' ' + message + (nextPlayer != this ? ' ' + nextPlayer.playerName : ""));
				keepVeloRate = Mathf.Sqrt(abilities.StopBall * 320.0f);
				float ballSpeed = collision.rigidbody.velocity.magnitude;
				if (keepVeloRate > ballSpeed) {
					collision.rigidbody.velocity = Vector3.zero;
				} else if (ballSpeed != 0) {
					collision.rigidbody.velocity *= (ballSpeed - keepVeloRate) / ballSpeed;
				}
				collision.rigidbody.angularVelocity *= 1.0f - Mathf.Exp(-keepVeloRate / 10.0f);

				Vector3 GroundForce = force - Vector3.up * force.y;
				float directNoiseRate = 3.0f - Vector3.Dot(transform.forward, GroundForce) / GroundForce.magnitude;
				noise *= directNoiseRate * directNoiseRate * 0.5f;
				force += ball.transform.position.y * noise.magnitude * 0.1f * Vector3.down;
				force *= 1.0f - Mathf.Pow(ball.transform.position.y * (100.0f - abilities.HeadBall) / transform.localScale.y / 200.0f, 2.0f);

				if (Vector3.Dot(force, ball.transform.position - transform.position) < 0) {
					ball.transform.position += 2.0f * (transform.position - ball.transform.position);
				}

				collision.rigidbody.AddForceAtPosition(
					force + noise,
					transform.position + transform.forward * abilities.Height * 0.1f
				);

				force = Vector3.zero;
				noise = Vector3.zero;
				team.BallHander = nextPlayer;
				if (nextPlayer != this) {
					nextPlayer.getBallTime = Time.time;
				}
				nextPlayer = null;
			} else {
				//接球
				if (playPosition == Position.GoalKeeper && (ball.preTouch.team != team || !ball.isKick)) {
					//扑球
					if (force.magnitude > 1.0f) {
						print(playerName + "  扑救");
						ball.isKick = false;
						collision.rigidbody.AddForceAtPosition(
							force + noise,
							transform.position + transform.forward * abilities.Height * 0.1f
						);
						force = Vector3.zero;
						noise = Vector3.zero;
						keepVeloRate = 0.0f;
						injured = Time.deltaTime;
					} else {
						print(playerName + "  抱球");
						keepVeloRate = Mathf.Sqrt(abilities.StopBall * 1500.0f);
						injured = Time.deltaTime;
					}
				} else if(Time.time - ball.kickBallTime > 1.25f - 0.005f * abilities.ReactionSpeed) {
					print(playerName + "  接球");
					keepVeloRate = Mathf.Sqrt(abilities.StopBall * 300.0f);
				} else {
					return;
				}

				float ballSpeed = collision.rigidbody.velocity.magnitude;
				if (keepVeloRate > ballSpeed) {
					collision.rigidbody.velocity = Vector3.zero;
				} else if (ballSpeed != 0) {
					collision.rigidbody.velocity *= (ballSpeed - keepVeloRate) / ballSpeed;
				}
				collision.rigidbody.angularVelocity *= 1.0f - Mathf.Exp(-keepVeloRate / 10.0f);

				team.BallHander = this;
				enemy.BallHander = null;

				getBallTime = Time.time;
			}
		}
		if (Vector3.Dot(collision.impulse, transform.forward) > Vector3.Dot(collision.impulse, collision.gameObject.transform.forward) &&
			collision.impulse.magnitude > 1024.0f + (256.0f * Mathf.Cos(Mathf.Deg2Rad *
				Vector3.Angle(collision.gameObject.transform.position - transform.position, transform.forward)
			)) + abilities.InjureResistance * 64.0f) {
			//受伤
			rig.constraints = RigidbodyConstraints.None;
			injured = (1.0f - Random.value * (1.0f - Mathf.Exp(3000.0f - collision.impulse.magnitude))) * maxInjuredTime;
			print(playerName + "  受伤");
			if (collision.gameObject.CompareTag("Player")) {
				Player killer = collision.gameObject.GetComponent<Player>();
				if (killer.team == team) {
					print(killer.playerName + "  误伤了  " + playerName);
				} else {
					print(killer.playerName + "  侵犯了  " + playerName);
				}
			}
		}
	}

	/**
	 * @brief 碰撞停留时调用
	 * @param[in] collision		包含了碰撞对象、碰撞刚体、碰撞力度等信息
	 * 球员与足球碰撞后相对静止时，施加一个力让足球远离球员。
	 */
	private void OnCollisionStay(Collision collision) {
		if (collision.collider.CompareTag("Ball") && Time.time - ball.kickBallTime > 0.25f) {
			ball.kickBallTime = Time.time;
			ball.rig.velocity *= 1.5f;
		}
	}

	/**
	 * @brief 判断球员是否着地
	 * 通过胶囊碰撞体判断球员是否着地，以判断球员能否完成加速、蹬地等行为。
	 * @return	若球员与地面接触则返回True，否则返回False。
	 */
	public bool isGrounded() {
		float sc = transform.localScale.y;
		float halfHeight = sc * capsuleCollider.height / 2;
		float radius = sc * capsuleCollider.radius;
		Vector3 pointBottom =
			transform.position + transform.up * radius - transform.up * halfHeight;
		Vector3 pointTop =
			transform.position + transform.up * halfHeight - transform.up * radius;
		LayerMask mask = 1 << LayerMask.NameToLayer("Ground");

		Collider[] colliders = Physics.OverlapCapsule(pointBottom, pointTop, radius, mask);
		//	Debug.DrawLine(pointBottom, pointTop, Color.green);
		if (colliders.Length == 0) {
			return false;
		}
		return true;
	}

	/**
	 * @brief 判断球员是否能跳起来
	 * 通过判断球员重心是否稳定来决定球员能否跳起。
	 * @return	若球员重心稳定则返回True，否则返回False。
	 */
	public bool isJumpable() {
		return Vector3.Angle(Vector3.up, transform.up) < 30.0f;
	}

	/**
	 * @brief 判断球员是否是前场球员
	 * 判断球员是否为中锋、左/右边锋、影锋。
	 * @return	若球员是前场球员则返回True，否则返回False。
	 */
	public bool isFrontField() {
		return playPosition > Position.AttackMidfielder;
	}

	/**
	 * @brief 判断球员是否是中场球员
	 * 判断球员是否为前腰、左/右边前卫、中前卫、后腰。
	 * @return	若球员是中场球员则返回True，否则返回False。
	 */
	public bool isMidField() {
		return playPosition < Position.ShadowStriker && playPosition > Position.RightSideBack;
	}

	/**
	 * @brief 判断球员是否是后场球员
	 * 判断球员是否为左/右边后卫、中后卫、门将。
	 * @return	若球员是后场球员则返回True，否则返回False。
	 */
	public bool isBackField() {
		return playPosition < Position.DefenceMidfielder;
	}

	/**
	 * @brief 判断球员是否是边路球员
	 * 判断球员是否为左/右边后卫、左/右边前卫、左/右边锋。
	 * @return	若球员是边路球员则返回True，否则返回False。
	 */
	public bool isWing() {
		return ((int)playPosition & 0x2) != 0;
	}
}
