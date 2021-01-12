/**@file     Timer.cs
 * @brief    计时器与比赛信息存储
 * @details  记录和更新比赛时间，并将比赛动态信息记录下来并存盘。
 * @author   刘冬煜
 * @version  1.0.1
 */

using System.IO;
using UnityEngine;
using UnityEngine.UI;

/**@class    Timer
 * @brief    计时器类
 * @details  记录和更新比赛时间，记录比赛中每一时刻球员与足球位置以及持球者信息。
 */
public class Timer : MonoBehaviour {
	public Team redTeam;        ///< 红队引用
	public Team blueTeam;       ///< 蓝队引用
	public Ball ball;           ///< 足球引用

	Text text;              ///< 计时器显示界面

	Vector3[,] redplayersPosition = new Vector3[15000, 11];     ///< 红队每一时刻所有球员的位置
	Vector3[,] blueplayersPosition = new Vector3[15000, 11];    ///< 蓝队所有球员的位置
	Vector3[] ballPosition = new Vector3[15000];                ///< 足球每一时刻的位置
	int[] redHand = new int[15000];                             ///< 红队每一时刻的持球者
	int[] blueHand = new int[15000];                            ///< 蓝队每一时刻的持球者

	/**
	 * @brief 唤醒时调用
	 * 插件被唤醒时，获取计时器显示界面。
	 */
	private void Awake() {
		text = GetComponent<Text>();
	}

	/**
	 * @brief 储存所有球员、足球位置与持球者
	 * @param[in] rank	被记录的时刻号，对应记录数组下标
	 * 记录第rank时刻的球员、足球位置与持球者。
	 */
	void savePositions(int rank) {
	//	Debug.Assert(redTeam.members.Length == 11 && blueTeam.members.Length == 11);

		int redhander = -1;
		int bluehander = -1;
		for (int i = 0; i < 11; ++i) {
			redplayersPosition[rank, i] = redTeam.members[i].player.transform.position;
			if (redTeam.BallHander == redTeam.members[i].player) redhander = i;
		}
		for (int i = 0; i < 11; ++i) {
			blueplayersPosition[rank, i] = blueTeam.members[i].player.transform.position;
			if (blueTeam.BallHander == blueTeam.members[i].player) bluehander = i;
		}
		ballPosition[rank] = ball.transform.position;
		redHand[rank] = redhander;
		blueHand[rank] = bluehander;
	}

	int tt = -1;    ///< 记录从比赛开始时刻过去了多少个时刻

	/**
	 * @brief 每隔0.02s调用一次
	 * 更新tt，每0.2s记录一次位置信息。
	 */
	private void FixedUpdate() {
		tt += 1;
		int second = tt % 3000 / 50;
		int minute = tt / 3000;

		/// 每10个时刻记录一次
		if (tt % 10 == 0) {
			savePositions(tt / 10);
			if (tt % 50 == 0) {
				text.text = string.Format("{0:D2}:{1:D2}", minute, second);
			}
		}
	}

	/**
	 * @brief 记录落盘
	 * 比赛结束时，将记录存盘。
	 */
	public void End() {
		int total_record = tt / 10;

		string[] buffers = new string[total_record + 1];
		buffers[0] = "时间,";
		for (int j = 0; j < 11; ++j) {
			buffers[0] += string.Format("红方{0:D},", j);
		}
		for (int j = 0; j < 11; ++j) {
			buffers[0] += string.Format("蓝方{0:D},", j);
		}
		buffers[0] += "足球,红方持球者,蓝方持球者";
		for (int i = 0; i < total_record; ++i) {
			string curLine = string.Format("{0:D2}'{1:D2}\"{2:D2},", i / 300, i % 300 / 5, i % 5 * 12);
			for (int j = 0; j < 11; ++j) {
				curLine += string.Format(
					"{0:N0} {1:N0},",
					redplayersPosition[i, j].x, redplayersPosition[i, j].z
				);
			}
			for (int j = 0; j < 11; ++j) {
				curLine += string.Format(
					"{0:N0} {1:N0},",
					blueplayersPosition[i, j].x, blueplayersPosition[i, j].z
				);
			}
			curLine += string.Format(
				"{0:N0} {1:N0} {2:N0},",
				ballPosition[i].x, ballPosition[i].y, ballPosition[i].z
			);
			curLine += string.Format(
				"{0:N0},{1:N0}", redHand[i], blueHand[i]
			);
			buffers[i + 1] = curLine;
		}

		//落盘
		string filePath = ".\\timeRecord.csv";
		File.WriteAllLines(filePath, buffers, System.Text.Encoding.UTF8);
	}
}
