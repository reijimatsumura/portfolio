using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotationKilnCircleController : MonoBehaviour
{
    //色を変更する円
    public Image[] rotationKilnCircles;
    //ドラックされるオブジェクトのパスを入れる物
    GameObject _dragCircles;

    /// <summary>
    /// 選択された円からどのように色を変えるかを決める
    /// </summary>
    /// <param name="circleNum">選択された円</param>
    public void ChangeRotationKilnCircleColorStart(int circleNum)
    {
        //どの円が触られたかで色変更
        switch (circleNum)
        {
            case (int)CircleDirection.CircleDirection.LeftUp:
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUp].color = Color.red;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUnder].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUp].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUnder].color = Color.gray;
                break;
            case (int)CircleDirection.CircleDirection.LeftUnder:
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUnder].color = Color.red;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUp].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUnder].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUp].color = Color.gray;
                break;
            case (int)CircleDirection.CircleDirection.RightUp:
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUp].color = Color.red;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUnder].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUp].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUnder].color = Color.gray;
                break;
            case (int)CircleDirection.CircleDirection.RightUnder:
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUnder].color = Color.red;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.RightUp].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUnder].color = Color.green;
                rotationKilnCircles[(int)CircleDirection.CircleDirection.LeftUp].color = Color.gray;
                break;
        }
    }

    /// <summary>
    /// ドラックして窯を動かす状態から戻す
    /// </summary>
    public void ChangeRotationKilnCircleColorEnd()
    {
        //色を最初の色に戻す
        foreach (Image circle in rotationKilnCircles)
        {
            circle.color = Color.white;
        }
    }

    /// <summary>
    /// ドラックによって動くオブジェクトのパスを取得しておく
    /// </summary>
    /// <param name="selectCircle">オブジェクトのパス</param>
    public void SetDragCircle(GameObject selectCircle)
    {
        _dragCircles = selectCircle;
    }

    /// <summary>
    /// マウスの位置にオブジェクトを動かす
    /// </summary>
    /// <param name="mousePos">マウスの位置</param>
    public void MoveDragCircle(Vector3 mousePos)
    {
        _dragCircles.transform.position = new Vector3(mousePos.x, mousePos.y);
    }
}
