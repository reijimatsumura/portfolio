using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageDropSwitch : MonoBehaviour
{
    //蓋を開け閉めしたときの処理を呼ぶ部分
    public ItemDragController itemDrogController;

    //窯の中の素材のデータを変える処理をするために呼ばれる
    public ItemDateController itemDateController;

    //抽出(完成)を開始する処理をするために呼ばれる
    public ExtractDateController extractDateController;

    //シーンの読み込みを行う
    public ResultSceneRoadController resultSceneRoad;

    //ドラックしたアイテムが最初においてあった位置を入れる
    private Vector3 _pastItemPos = new Vector3(0, 0, 0);

    /// <summary>
    /// ドラックする前の座標記録のために呼ばれる
    /// </summary>
    /// <param name="itemPos">ドラック前の座標</param>
    public void ItemReturnPosSet(Vector3 itemPos)
    {
        _pastItemPos = itemPos;
    }

    /// <summary>
    /// どのオブジェクトがどのオブジェクトに触れているかを分岐させて調べる
    /// その後各処理を実行したりするためのものを読んだりする
    /// </summary>
    /// <param name="moveItem">動かしていたオブジェクト</param>
    /// <param name="hitItem">動かしたオブジェクトの下にあるオブジェクト</param>
    /// <returns>処理のあるオブジェクトに当たったかどうか</returns>
    public bool ImageDropContact(GameObject moveItem, GameObject hitItem)
    {
        //動かしているオブジェクトのタグはなにか
        switch (moveItem.gameObject.tag)
        {
            case "MaterialItem": //ワークベンチに置かれている素材
                //窯の中に入れたかどうか
                if (hitItem.CompareTag("C_Kiln"))
                {
                    //選んだスロットが空いているかどうか
                    if (hitItem.GetComponent<MaterialSlotController>().materialItem == null 
                        && moveItem.GetComponent<MaterialSlotController>().materialItem != null)
                    {
                        var hitItemMaterialSlot = hitItem.GetComponent<MaterialSlotController>();
                        //ワークベンチの素材を窯のスロットに入れる
                        hitItemMaterialSlot.materialItem 
                            = moveItem.GetComponent<MaterialSlotController>().materialItem;
                        hitItemMaterialSlot.ImageChange();

                        ItemRemove(moveItem);
                        //ワークベンチの素材を非表示にする
                        moveItem.SetActive(false);

                        return true;
                    }
                }
                //ワークベンチに置かれた場合
                else if (hitItem.CompareTag("C_WorkBench"))
                {
                    return true;
                }
                break;
            case "PartitionItem": //ワークベンチに置かれている囲い
                //窯の中に入れたかどうか
                if (hitItem.CompareTag("C_Kiln"))
                {
                    itemDrogController.PartitionMove(hitItem.transform.position);
                    itemDateController.SlotUnLock();
                    //ドロップしたスロットの囲いの状態を有効化
                    hitItem.GetComponent<MaterialSlotController>().partition = true;

                    return true;
                }
                //ワークベンチに置かれた場合
                else if (hitItem.CompareTag("C_WorkBench"))
                {
                    itemDateController.SlotUnLock();
                    return true;
                }
                break;
            case "LidItem": //ワークベンチに置かれている蓋
                //窯の中に入れたかどうか
                if (hitItem.CompareTag("C_Kiln"))
                {
                    itemDrogController.KilnCover();

                    itemDateController.UIUnLock();

                    itemDateController.P_SlotMaterialItemChange();

                    ItemRemove(moveItem);

                    return true;
                }
                //ワークベンチに置かれた場合
                else if (hitItem.CompareTag("C_WorkBench"))
                {
                    return true;
                }
                break;
            case "ExtractItem": //ワークベンチに置かれている花火完成の瓶
                //抽出設置場所に設置したかどうか
                if (hitItem.CompareTag("Extract"))
                {
                    resultSceneRoad.SceneRoad();

                    extractDateController.ExtractProcess();

                    resultSceneRoad.ResultSceneStart();

                    return true;
                }
                //ワークベンチに置かれた場合
                else if (hitItem.CompareTag("C_WorkBench"))
                {
                    return true;
                }
                break;
            case "P_Lid": //窯の上の蓋
                //ワークベンチに置いたかどうか
                if (hitItem.CompareTag("P_WorkBench"))
                {
                    itemDrogController.LidOpen();

                    Vector3 partitionPos = itemDateController.C_SlotMaterialItemChange();
                    //なにもない座標でないかどうか
                    if(partitionPos != Vector3.zero)
                    {
                        itemDrogController.PartitionMove(partitionPos);
                    }

                    RemoveLid(moveItem);

                    return true;
                }
                break;
            case "Cooling": //窯の冷却
                //窯の上に置いたかどうか
                if (hitItem.CompareTag("P_Kiln"))
                {
                    itemDateController.UILock((int)StatusKiln.StatusKiln.Cooling);

                    ItemRemove(moveItem);

                    return true;
                }
                break;
            case "Heating": //窯の加熱
                //窯の上に置いたかどうか
                if (hitItem.CompareTag("P_Kiln"))
                {
                    itemDateController.UILock((int)StatusKiln.StatusKiln.Heating);

                    ItemRemove(moveItem);

                    return true;
                }
                break;
            default:
                Debug.Log("what`s fuck");
                break;
        }

        //背景に置かれた場合
        if (hitItem.CompareTag("Back"))
        {
            ItemRemove(moveItem);

            return true;
        }

        return false;
    }

    /// <summary>
    /// オブジェクトを元の位置に戻す
    /// </summary>
    /// <param name="removeItem">元に戻すオブジェクト</param>
    void ItemRemove(GameObject removeItem)
    {
        //オブジェクトにドラック前の座標を入れる
        removeItem.transform.position = _pastItemPos;
    }

    /// <summary>
    /// 蓋の座標を戻す
    /// </summary>
    /// <param name="lid">蓋</param>
    void RemoveLid(GameObject lid)
    {
        lid.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
}
