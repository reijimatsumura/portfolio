using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrowRotate
{
    /// <summary>
    /// どこの矢印を指すか
    /// </summary>
    enum ArrowRotate
    {
        up,
        left,
        rihgt,
        down
    }
}

public class ArrowsController : MonoBehaviour
{
    //矢印の画像
    public GameObject[] arrows;

    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    /// 選択された円から変更する矢印を決める
    /// </summary>
    /// <param name="selecCircle">選択された円</param>
    /// <param name="active">矢印はアクティブになるかどうか</param>
    public void DecideChangeArrow(int selecCircle, bool active)
    {
        if(active)
        {
            switch (selecCircle)
            {
                case (int)CircleDirection.CircleDirection.LeftUp:
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.up, true);
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.left, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.up, true);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.left, true);
                    break;
                case (int)CircleDirection.CircleDirection.LeftUnder:
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.left, true);
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.down, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.left, true);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.down, true);
                    break;
                case (int)CircleDirection.CircleDirection.RightUp:
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.rihgt, true);
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.up, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.rihgt, true);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.up, true);
                    break;
                case (int)CircleDirection.CircleDirection.RightUnder:
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.down, true);
                    ChangeRotateArrow((int)ArrowRotate.ArrowRotate.rihgt, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.down, true);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.rihgt, true);
                    break;
            }
        }
        else
        {
            switch (selecCircle)
            {
                case (int)CircleDirection.CircleDirection.LeftUp:
                    ActiveArrow((int)ArrowRotate.ArrowRotate.up, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.left, false);
                    break;
                case (int)CircleDirection.CircleDirection.LeftUnder:
                    ActiveArrow((int)ArrowRotate.ArrowRotate.left, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.down, false);
                    break;
                case (int)CircleDirection.CircleDirection.RightUp:
                    ActiveArrow((int)ArrowRotate.ArrowRotate.rihgt, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.up, false);
                    break;
                case (int)CircleDirection.CircleDirection.RightUnder:
                    ActiveArrow((int)ArrowRotate.ArrowRotate.down, false);
                    ActiveArrow((int)ArrowRotate.ArrowRotate.rihgt, false);
                    break;
            }
        }
    }

    /// <summary>
    /// 矢印の向きを変える
    /// </summary>
    /// <param name="selectArrows">どの矢印か</param>
    /// <param name="right">時計回りか</param>
    void ChangeRotateArrow(int selectArrows, bool right)
    {
        switch(selectArrows)
        {
            case (int)ArrowRotate.ArrowRotate.up:
                if(right)
                {
                    arrows[selectArrows].transform.rotation = 
                        Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(0, 180, 90));
                }
                break;
            case (int)ArrowRotate.ArrowRotate.left:
                if (right)
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(0, 0, 180));
                }
                else
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(180, 0, 180));
                }
                break;
            case (int)ArrowRotate.ArrowRotate.rihgt:
                if (right)
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(0, 0, 0));
                }
                else
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(180, 0, 0));
                }
                break;
            case (int)ArrowRotate.ArrowRotate.down:
                if (right)
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(0, 0, 270));
                }
                else
                {
                    arrows[selectArrows].transform.rotation =
                        Quaternion.Euler(new Vector3(0, 180, 270));
                }
                break;
        }
    }

    /// <summary>
    /// 矢印の可視状態を変更する
    /// </summary>
    /// <param name="selectArrows">どの矢印か</param>
    /// <param name="active">見えるか</param>
    void ActiveArrow(int selectArrows, bool active)
    {
        arrows[selectArrows].SetActive(active);
    }
}
