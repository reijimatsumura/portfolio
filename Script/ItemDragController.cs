using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//関数 動詞
public class ItemDragController : MonoBehaviour
{
    //マウスと画像の中心のずれ
    private Vector3 _itemslippage = new Vector3(0, 0, 0);
    
    //レイを飛ばすときに使う
    private PointerEventData _eventData;

    //仕切り
    public GameObject partition;

    //窯の素材加工をするキャンバス
    public GameObject processingFireCanvas;

    //どのオブジェクトをどこにドロップしたかによって処理を変えるため
    public ImageDropSwitch imageDropSwitch;

    // Start is called before the first frame update
    void Start()
    {
        //現在のマウスイベントポイントを入れる
        _eventData = new PointerEventData(EventSystem.current);
    }

    /// <summary>
    /// オブジェクトがドラックされるときに呼ばれる
    /// </summary>
    /// <param name="item">ドラックしようとしているオブジェクト</param>
    public void ItemPointerDown(GameObject item)
    {
        //UI座標に変換したマウスの座標
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //オブジェクトの座標を取れるように
        RectTransform itemPosition = item.GetComponent<RectTransform>();
        //動かす前の座標の記録
        Vector3 _pastItemPos = new Vector3(itemPosition.position.x, itemPosition.position.y, 0);

        imageDropSwitch.ItemReturnPosSet(_pastItemPos);

        //マウスと画像の中心のずれている距離を記録
        _itemslippage = mousePos - _pastItemPos;
        //このオブジェクトを最前面に持ってくる
        item.transform.SetSiblingIndex(10);
    }

    /// <summary>
    /// オブジェクトをドラックしているときに呼ばれる
    /// </summary>
    /// <param name="item">ドラックされているオブジェクト</param>
    public void ItemDrag(GameObject item)
    {
        //UI座標に変換したマウスの座標
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //マウスのオブジェクト中央とのずれ分を引きつつオブジェクトの位置をマウスの場所に移動させる
        item.transform.position = mousePos - _itemslippage;
    }

    /// <summary>
    /// オブジェクトをドラックし終わったときに呼ばれる
    /// </summary>
    /// <param name="item">ドラックされていたオブジェクト</param>
    public void ItemDragEnd(GameObject item)
    {
        //レイを飛ばしたときに当たったオブジェクトを入れる
        var raycastResults = new List<RaycastResult>();
        //UI座標に変換したマウスの座標
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //マウスの位置ではオブジェクトの中央ではなくなってしまうためそのずれを修正する
        Vector3 itemCenterpos = mousePos - _itemslippage;
        //マウスの座標に変換した修正されたUI座標
        _eventData.position = Camera.main.WorldToScreenPoint(itemCenterpos);
        //入れたマウス座標のところからレイを飛ばしヒットしたオブジェクトを格納していく
        EventSystem.current.RaycastAll(_eventData, raycastResults);
        //ドラックしたオブジェクトがどこに当たったかを確認しものによっては処理を行う
        foreach (var hit in raycastResults)
        {
            //処理を行う必要があるものに当たったかどうか
            if (imageDropSwitch.ImageDropContact(item, hit.gameObject))
            {
                break;
            }
        }
    }

    /// <summary>
    /// 仕切りを指定した座標の場所に動かす
    /// </summary>
    /// <param name="position">指定座標</param>
    public void PartitionMove(Vector3 position)
    {
        partition.transform.position = position;
    }

    /// <summary>
    /// 窯に蓋をしたときにUIを変えるため呼ばれる
    /// </summary>
    public void KilnCover()
    {
        //窯の素材加工をするキャンバスを有効化
        processingFireCanvas.SetActive(true);
    }

    /// <summary>
    /// 窯の蓋を外したときにUIを変えるため呼ばれる
    /// </summary>
    public void LidOpen()
    {
        //窯の素材加工をするキャンバスを非有効化
        processingFireCanvas.SetActive(false);
    }
}
