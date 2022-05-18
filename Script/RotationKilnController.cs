using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CircleDirection
{
    /// <summary>
    /// 円のどこを選択したかを示す
    /// </summary>
    public enum CircleDirection
    {
        LeftUp,
        LeftUnder,
        RightUp,
        RightUnder
    }
}

public class RotationKilnController : MonoBehaviour
{
    //マウスの最初にドラックしたところ
    private Vector3 _pastMousePos = new Vector3(0, 0, 0);
    //ドラックされた
    private Vector3 _pastSelectCircle = new Vector3(0, 0, 0);

    private int _dragCircle = 0;

    //素材のデータの処理を行うところを呼び出す
    public ItemDateController itemDateController;
    //窯の四つ角の円の色を変えたりする
    public RotationKilnCircleController rotationKilnCircleController;
    //ユーザーに対する指示を変える
    public ArrowsController arrowsController;

    //選択した円の値を入れておく
    [SerializeField] int _clickCircleNum = -1;

    //レイを飛ばすときに使う
    private PointerEventData _eventData;

    // Start is called before the first frame update
    void Start()
    {
        //現在のマウスイベントポイントを入れる
        _eventData = new PointerEventData(EventSystem.current);
    }

    /// <summary>
    /// 円をクリックしたときに呼ばれる
    /// </summary>
    /// <param name="circleNum">クリックした円</param>
    public void ClickCircle(int circleNum)
    {
        //どの円も選択されていない
        if (_dragCircle == 0)
        {
            //いま選択された状態の円があるかどうか
            if (_clickCircleNum == -1)
            {
                RotationKilnStart(circleNum);
            }
            else
            {
                //その円とクリックされた円がかぶっているかどうか
                if (circleNum == _clickCircleNum)
                {
                    RotationKilnEnd();
                }
                else
                {
                    arrowsController.DecideChangeArrow(_clickCircleNum, false);
                    RotationKilnStart(circleNum);
                }
            }
        }
        else
        {
            _dragCircle -= 1;
        }
    }

    /// <summary>
    /// ドラックを始めたときに呼ばれる
    /// </summary>
    public void DragStart(GameObject selectCircle)
    {
        //ドラック開始時のマウスの座標変換してを記録
        _pastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //ドラックされる画像の初期地点を記録
        _pastSelectCircle = selectCircle.transform.position;

        rotationKilnCircleController.SetDragCircle(selectCircle);

        itemDateController.SetKilnRotationZ();
    }

    /// <summary>
    /// ドラックされている時に呼ばれる
    /// </summary>
    /// <param name="circleNum">どの円をドラッグしているか</param>
    public void Drag(int circleNum)
    {
        //選択した円をドラッグしているかどうか
        if (_clickCircleNum == circleNum)
        {
            //マウスのポジションをとる
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float rotationProportion = CalculateKilnRotaion(mousePos, true);
            rotationKilnCircleController.MoveDragCircle(mousePos);

            //回転範囲に入って
            if (rotationProportion != -1.0f)
            {
                itemDateController.MoveRotationKiln(true, rotationProportion);
            }

            rotationProportion = CalculateKilnRotaion(mousePos, false);

            if (rotationProportion != -1.0f)
            {
                itemDateController.MoveRotationKiln(false, rotationProportion);
            }
        }
    }

    /// <summary>
    /// ドラックが終わったときに呼ばれる
    /// </summary>
    /// <param name="rotationRight">左右どちらか</param>
    public void DragEnd(GameObject selectCircle)
    {
        if (selectCircle.tag == "CircleLeftUnder" 
            && _clickCircleNum == (int)CircleDirection.CircleDirection.LeftUnder ||
            selectCircle.tag == "CircleLeftUp"
            && _clickCircleNum == (int)CircleDirection.CircleDirection.LeftUp ||
            selectCircle.tag == "CircleRightUnder"
            && _clickCircleNum == (int)CircleDirection.CircleDirection.RightUnder ||
            selectCircle.tag == "CircleRightUp"
            && _clickCircleNum == (int)CircleDirection.CircleDirection.RightUp)
        {
            //レイを飛ばしたときに当たったオブジェクトを入れる
            var raycastResults = new List<RaycastResult>();
            //UI座標に変換したマウスの座標
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //マウスの座標に変換した修正されたUI座標
            _eventData.position = Camera.main.WorldToScreenPoint(mousePos);
            //入れたマウス座標のところからレイを飛ばしヒットしたオブジェクトを格納していく
            EventSystem.current.RaycastAll(_eventData, raycastResults);
            //ドラックしたオブジェクトがどこに当たったかを確認しものによっては処理を行う
            foreach (var hit in raycastResults)
            {
                if (HitObjectProcess(hit.gameObject))
                {
                    break;
                }
            }
            selectCircle.transform.position = _pastSelectCircle;
        }
        //Debug.Log(_dragCircle);
    }

    /// <summary>
    /// マウスの位置から範囲内にいた場合割合を出す
    /// </summary>
    /// <param name="cilckPos">マウスの位置</param>
    /// <param name="rightRotation">右回転か</param>
    /// <returns>割合</returns>
    public float CalculateKilnRotaion(Vector2 cilckPos, bool rightRotation)
    {
        //どの範囲かにおおじてより小さいものも大きいも変わるため箱を作る
        Vector2 largePos = Vector2.zero;
        Vector2 smallPos = Vector2.zero;
        //どの範囲かにおおじて幅の最大値も現在の位置も変わるため箱を作る
        float maxWidths = 0.0f;
        float mosueWidths = 0.0f;
        //右回転かどうか
        if (rightRotation)
        {
            //どの円を動かしているか
            switch (_clickCircleNum)
            {
                case (int)CircleDirection.CircleDirection.LeftUp:
                    largePos = new Vector2(cilckPos.x, cilckPos.y);
                    smallPos = new Vector2(_pastMousePos.x, _pastMousePos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.x - 2.9f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.x - cilckPos.x);
                    break;
                case (int)CircleDirection.CircleDirection.LeftUnder:
                    largePos = new Vector2(_pastMousePos.x, cilckPos.y);
                    smallPos = new Vector2(cilckPos.x, _pastMousePos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.y - 1.9f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.y - cilckPos.y);
                    break;
                case (int)CircleDirection.CircleDirection.RightUp:
                    largePos = new Vector2(cilckPos.x, _pastMousePos.y);
                    smallPos = new Vector2(_pastMousePos.x, cilckPos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.y - 7.8f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.y - cilckPos.y);
                    break;
                case (int)CircleDirection.CircleDirection.RightUnder:
                    largePos = new Vector2(_pastMousePos.x, _pastMousePos.y);
                    smallPos = new Vector2(cilckPos.x, cilckPos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.x - 8.7f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.x - cilckPos.x);
                    break;
            }
        }
        else
        {
            //どの円を動かしているか
            switch (_clickCircleNum)
            {
                case (int)CircleDirection.CircleDirection.LeftUp:
                    largePos = new Vector2(_pastSelectCircle.x, _pastSelectCircle.y);
                    smallPos = new Vector2(cilckPos.x, cilckPos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.y - 7.8f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.y - cilckPos.y);
                    break;
                case (int)CircleDirection.CircleDirection.LeftUnder:
                    largePos = new Vector2(cilckPos.x, _pastSelectCircle.y);
                    smallPos = new Vector2(_pastSelectCircle.x, cilckPos.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.x - 2.9f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.x - cilckPos.x);
                    break;
                case (int)CircleDirection.CircleDirection.RightUp:
                    largePos = new Vector2(_pastSelectCircle.x, cilckPos.y);
                    smallPos = new Vector2(cilckPos.x, _pastSelectCircle.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.x - 8.7f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.x - cilckPos.x);
                    break;
                case (int)CircleDirection.CircleDirection.RightUnder:
                    largePos = new Vector2(cilckPos.x, cilckPos.y);
                    smallPos = new Vector2(_pastSelectCircle.x, _pastSelectCircle.y);
                    maxWidths = Mathf.Abs(_pastSelectCircle.y - 1.9f);
                    mosueWidths = Mathf.Abs(_pastSelectCircle.y - cilckPos.y);
                    break;
            }
        }

        //マウスが範囲内にあるかどうか
        if(largePos.x > smallPos.x && largePos.y > smallPos.y)
        {
            //マウスの位置が割合の最大値よりも小さいか
            if(maxWidths > mosueWidths)
            {
                return mosueWidths / maxWidths;
            }
            else
            {
                return 1.0f;
            }
        }
        else
        {
            return -1.0f;
        }
    }

    /// <summary>
    /// どのオブジェクトと重なったか
    /// </summary>
    /// <param name="hitGameObject">重なったオブジェクト</param>
    /// <returns>指定したオブジェクトと重なったか</returns>
    bool HitObjectProcess(GameObject hitGameObject)
    {
        //このタグなら右に回るように
        string rightTag = "";
        //このタグなら左に回るように
        string leftTag = "";
        //右回転時の誤クリック発生のための調整変数
        int rightAdditionsDragCircle = 0;
        //左回転時の誤クリック発生のための調整変数
        int leftAdditionsDragCircle = 0;

        //どこの円かによって左右の基準になるタグが違うのでそうなるタグを入れる
        switch (_clickCircleNum)
        {
            case (int)CircleDirection.CircleDirection.LeftUp:
                rightTag = "CircleRightUp";
                leftTag = "CircleLeftUnder";
                break;
            case (int)CircleDirection.CircleDirection.LeftUnder:
                rightTag = "CircleLeftUp";
                leftTag = "CircleRightUnder";
                rightAdditionsDragCircle = 1;
                break;
            case (int)CircleDirection.CircleDirection.RightUp:
                rightTag = "CircleRightUnder";
                leftTag = "CircleLeftUp";
                leftAdditionsDragCircle = 1;
                break;
            case (int)CircleDirection.CircleDirection.RightUnder:
                rightTag = "CircleLeftUnder";
                leftTag = "CircleRightUp";
                rightAdditionsDragCircle = 1;
                leftAdditionsDragCircle = 1;
                break;
        }

        //右回転タグとおなじなら
        if (hitGameObject.tag == rightTag)
        {
            RotationKilnEnd();
            itemDateController.UILock((int)StatusKiln.StatusKiln.Right);
            itemDateController.MoveEndRotationKiln(true);
            _dragCircle += rightAdditionsDragCircle;
            return true;
        }
        //左回転タグとおなじなら
        else if (hitGameObject.tag == leftTag)
        {
            RotationKilnEnd();
            itemDateController.UILock((int)StatusKiln.StatusKiln.Left);
            itemDateController.MoveEndRotationKiln(false);
            _dragCircle += leftAdditionsDragCircle;
            return true;
        }
        //背景画像に当たったかどうか
        if (hitGameObject.tag == "Back")
        {
            itemDateController.CancelRotationKiln();
        }

        return false;
    }

    /// <summary>
    /// 窯が回転する状態になったとき呼ばれる
    /// </summary>
    /// <param name="circleNum">どの円を選択したか</param>
    void RotationKilnStart(int circleNum)
    {
        _clickCircleNum = circleNum;
        rotationKilnCircleController.ChangeRotationKilnCircleColorStart(circleNum);
        arrowsController.DecideChangeArrow(circleNum, true);
    }

    /// <summary>
    /// 窯の回転する状態を終了したときに呼ばれる
    /// </summary>
    void RotationKilnEnd()
    {
        arrowsController.DecideChangeArrow(_clickCircleNum, false);
        _clickCircleNum = -1;
        rotationKilnCircleController.ChangeRotationKilnCircleColorEnd();
    }
}
