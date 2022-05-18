using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultSceneTextChange : MonoBehaviour
{
    //色の評価点を表示
    public Text colorEvaluation;
    //大きさの評価点を表示
    public Text sizeEvaluation;
    //形の評価点を表示
    public Text formEvaluation;
    //総合評価点を表示
    public Text comprehensiveEvaluation;

    /// <summary>
    /// 色の評価点の点数変更
    /// </summary>
    /// <param name="colorScore">評価点</param>
    public void ColorEvaluationTextChange(int colorScore)
    {
        if(colorScore < 10)
        {
            colorEvaluation.text = "色彩の評価点: 0" + colorScore;
        }
        else
        {
            colorEvaluation.text = "色彩の評価点: " + colorScore;
        }
    }

    /// <summary>
    /// 大きさの評価点の点数変更
    /// </summary>
    /// <param name="sizeScore">評価点</param>
    public void SizeEvaluationTextChange(int sizeScore)
    {
        if (sizeScore < 10)
        {
            sizeEvaluation.text = "大小の評価点: 0" + sizeScore;
        }
        else
        {
            sizeEvaluation.text = "大小の評価点: " + sizeScore;
        }
    }

    /// <summary>
    /// 形の評価点の点数変更
    /// </summary>
    /// <param name="formScore">評価点</param>
    public void FormEvaluationTextChange(int formScore)
    {
        if (formScore < 10)
        {
            formEvaluation.text = "形状の評価点: 0" + formScore;
        }
        else
        {
            formEvaluation.text = "形状の評価点: " + formScore;
        }
    }

    /// <summary>
    /// 総合評価点の点数変更
    /// </summary>
    /// <param name="comprehensiveScore">評価点</param>
    public void ComprehensiveEvaluationTextChange(int comprehensiveScore)
    {
        if (comprehensiveScore < 10)
        {
            comprehensiveEvaluation.text = "総合評価点: 0" + comprehensiveScore;
        }
        else
        {
            comprehensiveEvaluation.text = "総合評価点: " + comprehensiveScore;
        }
    }
}
