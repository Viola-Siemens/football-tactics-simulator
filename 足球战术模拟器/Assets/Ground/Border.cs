/**@file     Border.cs
 * @brief    全局边界参数
 * @details  包含了边线Z坐标、点球点X坐标的绝对值信息。
 * @author   刘冬煜
 * @version  1.0.1
 */

/**@class    Border
 * @brief    边界静态类
 * @details  包含了边线Z坐标、点球点X坐标的绝对值。
 */
public class Border {
    public static readonly float BorderZ = 400.0f;      ///< 边线Z坐标的绝对值
    public static readonly float PenaltyX = 520.0f;     ///< 点球点X坐标的绝对值
}
