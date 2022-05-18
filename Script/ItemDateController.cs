using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StatusKiln
{
    /// <summary>
    /// 窯がどんな状態かを示す
    /// </summary>
    public enum StatusKiln
    {
        Heating,
        Cooling,
        Right,
        Left,
    }
}

public class ItemDateController : MonoBehaviour
{
    //スロットのデータを格納する
    public List<MaterialSlotController> dateSlot;
    //表示されるスロットの画像
    public List<Image> imageSlot;

    //窯の中が変わっている最中にGUIに触れないようにする
    public GameObject operationLock;

    //素材のデータをダウンロードするときに使う
    public MaterialItemDateDownload itemDateDownload;

    //蓋を閉めているときに表示される囲い
    public GameObject p_Partition;

    //素材同士が合成した際の合成ができるかどうか合成先は何かを取得できる
    public MaterialRecipe materialRecipe;
    //素材を冷やした際になにができるかを取得できる
    public CoolingRecipe coolingRecipe;
    //素材を熱した際になにができるかを取得できる
    public HeatingRecipe heatingRecipe;

    //素材が入っているオブジェクト
    public List<GameObject> objSlot;
    //素材を表示しているオブジェクト
    public List<RectTransform> imageRotationSlot;
    //窯のオブジェクト
    public RectTransform kiln;
    //
    float _pastkilnRotationZ = 0.0f;

    public EffectController effectController;

    //
    public float kilnMoveSpeed = 1.0f;

    //
    bool _syntheticMaterial = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// UIを蓋が開いている状態のものにする
    /// </summary>
    /// <returns>閉じている状態のときに囲いが置いてある座標</returns>
    public Vector3 C_SlotMaterialItemChange()
    {
        //どのスロットに置かれているか
        int truePartitionNum = -1;

        //すべてのスロットを調べて囲いがどこにあるか
        for (int i = 0; i < dateSlot.Count; i++)
        {
            //このスロットに囲いがあるかどうか
            if (dateSlot[i].partition)
            {
                //囲いのあるスロットの配列番号を記録
                truePartitionNum = i;
                break;
            }
        }

        //囲いのあるスロットがあったかどうか
        if (truePartitionNum != -1)
        {
            //あったスロットの座標を返す
            return dateSlot[truePartitionNum].gameObject.transform.position;
        }
        else
        {
            //なにもない座標を返す
            return Vector3.zero;
        }
    }

    /// <summary>
    /// UIを蓋が閉じている状態のものにする
    /// </summary>
    public void P_SlotMaterialItemChange()
    {
        //蓋を開けているときのスロット変化の更新
        for (int i = 0; i < imageSlot.Count; i++)
        {
            ImageDateChange(i);
        }

        //囲いがあるかどうかをすべてのスロットから調べる
        for(int i = 0; i < dateSlot.Count; i++)
        {
            //囲いがあるかどうか
            if (dateSlot[i].partition)
            {
                //その場所に蓋が閉まってる状態も合わせる
                p_Partition.transform.position = imageSlot[i].transform.position;
                PartitionImageChange(1);

                break;
            }
            //ループの最後かどうか
            if(i == dateSlot.Count - 1)
            {
                PartitionImageChange(0);
            }
        }
    }

    /// <summary>
    /// 素材変化の処理を行っている間UIをロックしつつどのUIの処理にいかせるか
    /// </summary>
    /// <param name="statusSwitchKiln">どの処理にいくか</param>
    public void UILock(int statusSwitchKiln)
    {
        //ロックを有効化
        operationLock.SetActive(true);
        
        //どこの処理を実行するか
        switch (statusSwitchKiln)
        {
            case (int)StatusKiln.StatusKiln.Heating: //熱したとき
                StartCoroutine(HeatingKiln());
                break;
            case (int)StatusKiln.StatusKiln.Cooling: //凍らせたとき
                StartCoroutine(CoolingKiln());
                break;
            case (int)StatusKiln.StatusKiln.Right:   //右回転されたとき
                StartCoroutine(RotationKiln(true));
                break;
            case (int)StatusKiln.StatusKiln.Left:    //左回転されたとき
                StartCoroutine(RotationKiln(false));
                break;
        }
    }

    /// <summary>
    /// GUIが触れる状態にする
    /// </summary>
     public void UIUnLock()
    {
        operationLock.SetActive(false);
    }

    /// <summary>
    /// すべてのスロットを囲われていない状態にする
    /// </summary>
    public void SlotUnLock()
    {
        foreach(MaterialSlotController i in dateSlot)
        {
            i.partition = false;
        }
    }

    /// <summary>
    /// 熱したとき
    /// </summary>
    /// <returns></returns>
    IEnumerator HeatingKiln()
    {
        //素材のIDを入れておくためのもの
        int materialNum = 0;
        
        //すべてのスロットの素材が変化するかどうか
        for (int i = 0; i < dateSlot.Count; i++)
        {
            //素材が入っているかどうか
            if(dateSlot[i].materialItem != null)
            {
                //配列からどの値に変化するかを調べる
                materialNum = heatingRecipe.heatMaterial[dateSlot[i].materialItem.itemID - 1];
                
                //値が0でないなら
                if (materialNum != 0)
                {
                    //戻り値を取り出すためコルーチンの型のなかに戻り値が欲しいコルーチンを入れる
                    IEnumerator iEnumerator = itemDateDownload.MaterialDateDownload(materialNum);
                    //コルーチンを動かす
                    yield return StartCoroutine(iEnumerator);
                    //戻り値を取り出し入れる
                    MaterialItem materialHandle = (MaterialItem)iEnumerator.Current;
                    //取り出したデータをスロットの素材に入れる
                    dateSlot[i].materialItem = materialHandle;
                    ImageDateChange(i);
                }
            }
        }

        yield return StartCoroutine(effectController.EffectSteamOn());

        UIUnLock();
    }

    /// <summary>
    /// 冷やしたとき
    /// </summary>
    /// <returns></returns>
    IEnumerator CoolingKiln()
    {
        //素材のIDを入れておくためのもの
        int materialNum = 0;

        //すべてのスロットの素材が変化するかどうか
        for (int i = 0; i < imageSlot.Count; i++)
        {
            //素材が入っているかどうか
            if (dateSlot[i].materialItem != null)
            {
                //配列からどの値に変化するかを調べる
                materialNum = coolingRecipe.coolMaterial[dateSlot[i].materialItem.itemID - 1];

                //値が0でないなら
                if (materialNum != 0)
                {
                    //戻り値を取り出すためコルーチンの型のなかに戻り値が欲しいコルーチンを入れる
                    IEnumerator iEnumerator = itemDateDownload.MaterialDateDownload(materialNum);
                    //コルーチンを動かす
                    yield return StartCoroutine(iEnumerator);
                    //戻り値を取り出し入れる
                    MaterialItem materialHandle = (MaterialItem)iEnumerator.Current;
                    //取り出したデータをスロットの素材に入れる
                    dateSlot[i].materialItem = materialHandle;
                    ImageDateChange(i);
                }
            }
        }

        yield return StartCoroutine(effectController.EffectSnowCrystalOn());

        UIUnLock();
    }

    /// <summary>
    /// 窯を回転させる
    /// </summary>
    /// <param name="rotationRight">右回転かどうか</param>
    /// <returns></returns>
    IEnumerator RotationKiln(bool rotationRight)
    {
        //素材のデータを一時的に保管するところ
        MaterialItem material = new MaterialItem();
        //囲いの状態を一時的に保管するところ
        bool partition;

        //窯の大きさに応じて処理を分ける
        switch (dateSlot.Count)
        {
            case 4:
                break;
            case 9: //3かけ3
                //右回転かどうか
                if (rotationRight)
                {
                    //回転したあとの場所に素材の位置を動かす
                    material = dateSlot[0].materialItem;
                    partition = dateSlot[0].partition;
                    MaterialPass(0, dateSlot[2].materialItem, dateSlot[2].partition);
                    MaterialPass(2, dateSlot[8].materialItem, dateSlot[8].partition);
                    MaterialPass(8, dateSlot[6].materialItem, dateSlot[6].partition);
                    MaterialPass(6, material, partition);
                    material = dateSlot[1].materialItem;
                    partition = dateSlot[1].partition;
                    MaterialPass(1, dateSlot[5].materialItem, dateSlot[5].partition);
                    MaterialPass(5, dateSlot[7].materialItem, dateSlot[7].partition);
                    MaterialPass(7, dateSlot[3].materialItem, dateSlot[3].partition);
                    MaterialPass(3, material, partition);
                }
                else
                {
                    //回転したあとの場所に素材の位置を動かす
                    material = dateSlot[0].materialItem;
                    partition = dateSlot[0].partition;
                    MaterialPass(0, dateSlot[6].materialItem, dateSlot[6].partition);
                    MaterialPass(6, dateSlot[8].materialItem, dateSlot[8].partition);
                    MaterialPass(8, dateSlot[2].materialItem, dateSlot[2].partition);
                    MaterialPass(2, material, partition);
                    material = dateSlot[1].materialItem;
                    partition = dateSlot[1].partition;
                    MaterialPass(1, dateSlot[3].materialItem, dateSlot[3].partition);
                    MaterialPass(3, dateSlot[7].materialItem, dateSlot[7].partition);
                    MaterialPass(7, dateSlot[5].materialItem, dateSlot[5].partition);
                    MaterialPass(5, material, partition);
                }

                yield return StartCoroutine(FallMaterial(3));
                break;
            case 16:
                break;
        }

        Vector3 slotPos = objSlot[0].transform.position;

        if (rotationRight)
        {
            foreach (RectTransform someSlot in imageRotationSlot)
            {
                someSlot.rotation *= Quaternion.Euler(0, 0, 90.0f);
            }

            slotPos = objSlot[0].transform.position;
            objSlot[0].transform.position = objSlot[2].transform.position;
            objSlot[2].transform.position = objSlot[8].transform.position;
            objSlot[8].transform.position = objSlot[6].transform.position;
            objSlot[6].transform.position = slotPos;
            slotPos = objSlot[1].transform.position;
            objSlot[1].transform.position = objSlot[5].transform.position;
            objSlot[5].transform.position = objSlot[7].transform.position;
            objSlot[7].transform.position = objSlot[3].transform.position;
            objSlot[3].transform.position = slotPos;
        }
        else
        {
            foreach (RectTransform someSlot in imageRotationSlot)
            {
                someSlot.rotation *= Quaternion.Euler(0, 0, -90.0f);
            }

            slotPos = objSlot[0].transform.position;
            objSlot[0].transform.position = objSlot[6].transform.position;
            objSlot[6].transform.position = objSlot[8].transform.position;
            objSlot[8].transform.position = objSlot[2].transform.position;
            objSlot[2].transform.position = slotPos;
            slotPos = objSlot[1].transform.position;
            objSlot[1].transform.position = objSlot[3].transform.position;
            objSlot[3].transform.position = objSlot[7].transform.position;
            objSlot[7].transform.position = objSlot[5].transform.position;
            objSlot[5].transform.position = slotPos;
        }

        for (int i = 0; i < 9; i++)
        {
            if (dateSlot[i].partition)
            {
                p_Partition.transform.position = objSlot[i].transform.position;
            }
        }

        yield return null;

        UIUnLock();
    }

    /// <summary>
    /// 落下浮遊処理
    /// </summary>
    /// <param name="lineCount">1辺に何スロット必要か</param>
    /// <returns></returns>
    IEnumerator FallMaterial(int lineCount)
    {
        //合成が行われたかどうか
        _syntheticMaterial = false;

        //全スロットを調べる
        for (int i = 0; i < imageSlot.Count; i++)
        {
            //そのスロットに素材が入っているかつ囲われていない状態
            if(dateSlot[i].materialItem != null && !dateSlot[i].partition)
            {
                //素材の特性に浮遊があるかどうか
                if(dateSlot[i].materialItem.itemNature[0] == 12 ||
                    dateSlot[i].materialItem.itemNature[1] == 12 ||
                    dateSlot[i].materialItem.itemNature[2] == 12)
                {
                    //スロットが一番上の段よりも低いか
                    if(i < lineCount * (lineCount - 1))
                    {
                        //上に上がる回数を記録
                        int upCount;
                        //上がれる回数を変数に記録しスロットの位置から上がれる回数分だけ回す
                        for (upCount = 0; upCount < (imageSlot.Count - 1 - i) / lineCount; upCount++)
                        {
                            //指定された上のスロットが囲われているかどうか
                            if(!dateSlot[i + lineCount * (1 + upCount)].partition)
                            {
                                //指定されたスロットに素材が入っているかどうか
                                if(dateSlot[i + lineCount * (1 + upCount)].materialItem != null)
                                {
                                    //素材同士がまざるかどうか
                                    int syntheticMaterialID = MaterialRecipeRefence
                                    (dateSlot[i].materialItem.itemID - 1,
                                    dateSlot[i + lineCount * (1 + upCount)].materialItem.itemID - 1);
                                    if (syntheticMaterialID != 0)
                                    {
                                        IEnumerator iEnumerator = 
                                            itemDateDownload.MaterialDateDownload(syntheticMaterialID);

                                        yield return StartCoroutine(iEnumerator);
                                        //アセットのロード
                                        MaterialItem materialHandle = (MaterialItem)iEnumerator.Current;

                                        //合成するのでもともとあった素材は消す
                                        dateSlot[i].materialItem = null;
                                        ImageDateChange(i);
                                        //合成してできた素材をぶつかった先に入れる
                                        dateSlot[i + lineCount * (1 + upCount)].materialItem = materialHandle;
                                        ImageDateChange(i + lineCount * (1 + upCount));

                                        _syntheticMaterial = true;
                                        
                                        upCount = 0;
                                    }

                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (upCount != 0)
                        {
                            MaterialPass(i + upCount * lineCount, dateSlot[i].materialItem, false);
                            dateSlot[i].materialItem = null;
                            ImageDateChange(i);
                        }
                    }
                }
                else if (i > lineCount - 1)
                {
                    //下がる回数を記録
                    int downCount;
                    //下がれる回数を変数に記録しスロットの位置から上がれる回数分だけ回す
                    for (downCount = 0; downCount < i / lineCount; downCount++)
                    {
                        //指定された下のスロットが囲われているかどうか
                        if (!dateSlot[i - lineCount * (1 + downCount)].partition)
                        {
                            //指定されたスロットに素材が入っているかどうか
                            if (dateSlot[i - lineCount * (1 + downCount)].materialItem != null)
                            {
                                //素材同士がまざるかどうか
                                int syntheticMaterialID = MaterialRecipeRefence
                                    (dateSlot[i].materialItem.itemID - 1,
                                    dateSlot[i - lineCount * (1 + downCount)].materialItem.itemID - 1);
                                if (syntheticMaterialID != 0)
                                {
                                    IEnumerator iEnumerator = 
                                        itemDateDownload.MaterialDateDownload(syntheticMaterialID);

                                    yield return StartCoroutine(iEnumerator);
                                    //アセットのロード
                                    MaterialItem materialHandle = (MaterialItem)iEnumerator.Current;

                                    //合成するのでもともとあった素材は消す
                                    dateSlot[i].materialItem = null;
                                    ImageDateChange(i);
                                    //合成してできた素材をぶつかった先に入れる
                                    dateSlot[i - lineCount * (1 + downCount)].materialItem = materialHandle;
                                    ImageDateChange(i - lineCount * (1 + downCount));
                                    //合成した素材から再検査するため値を戻す
                                    i = i - lineCount * (1 + downCount) - 1;

                                    _syntheticMaterial = true;

                                    downCount = 0;
                                }

                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    //素材の下のスロットが空いていたなら
                    if (downCount != 0)
                    {
                        MaterialPass(i - downCount * lineCount, dateSlot[i].materialItem, false);
                        //元の位置のスロットから素材を消しておく
                        dateSlot[i].materialItem = null;
                        ImageDateChange(i);
                    }
                }
            }
        }

        //合成があったかどうか
        if (_syntheticMaterial)
        {
            yield return StartCoroutine(effectController.EffectSyntheticOn());
        }
    }

    /// <summary>
    /// 窯のZ軸の傾きをセットする
    /// </summary>
    public void SetKilnRotationZ()
    {
        var kilnRotation = kiln.rotation.eulerAngles;
        _pastkilnRotationZ = kilnRotation.z;
    }

    /// <summary>
    /// 窯を動かす
    /// </summary>
    /// <param name="rotationRight">右回転かどうか</param>
    public void MoveRotationKiln(bool rotationRight, float rotationProportion)
    {
        if (rotationRight)
        {
            kiln.rotation = Quaternion.Euler(0, 0, _pastkilnRotationZ - rotationProportion * 90.0f);
        }
        else
        {
            kiln.rotation = Quaternion.Euler(0, 0, _pastkilnRotationZ + rotationProportion * 90.0f);
        }
    }

    /// <summary>
    /// 窯を動かし切った
    /// </summary>
    /// <param name="rotationRight">右回転かどうか</param>
    public void MoveEndRotationKiln(bool rotationRight)
    {
        if (rotationRight)
        {
            kiln.rotation = Quaternion.Euler(0, 0, _pastkilnRotationZ - 90.0f);
        }
        else
        {
            kiln.rotation = Quaternion.Euler(0, 0, _pastkilnRotationZ + 90.0f);
        }
    }

    /// <summary>
    /// 窯を動かすのをやめた
    /// </summary>
    public void CancelRotationKiln()
    {
        kiln.rotation = Quaternion.Euler(0, 0, _pastkilnRotationZ);
    }

    /// <summary>
    /// 素材同士が混ざるかどうか
    /// </summary>
    /// <param name="fallMaterialID">ぶつかってきたきた素材</param>
    /// <param name="contactMaterialID">そこにあった素材</param>
    /// <returns>混ざった場合どの素材になるか</returns>
    int MaterialRecipeRefence(int fallMaterialID, int contactMaterialID)
    {
        //レシピの2次配列に値をいれて値を持ってくる
        return materialRecipe.materialAffinity[fallMaterialID, contactMaterialID];
    }

    /// <summary>
    /// 素材の入れを行う
    /// </summary>
    /// <param name="slotID">入れられるスロット</param>
    /// <param name="handOver">入れる素材のデータ</param>
    /// <param name="partition">枠があるかどうか</param>
    void MaterialPass(int slotID, MaterialItem handOver, bool partition)
    {
        //素材データがあるものが入ってくるかどうか
        if(handOver == null)
        {
            dateSlot[slotID].materialItem = null;
        }
        else
        {
            dateSlot[slotID].materialItem = new MaterialItem();
            dateSlot[slotID].materialItem = handOver;
        }
        
        ImageDateChange(slotID);
        //枠の情報更新
        dateSlot[slotID].partition = partition;
        //枠があるなら見た目の更新
        if(partition)
        {
            p_Partition.transform.position = imageSlot[slotID].transform.position;
        }
    }

    /// <summary>
    /// 蓋が閉じているときの窯の素材画像表示を変える
    /// </summary>
    /// <param name="slotNum">変えるスロット場所</param>
    void ImageDateChange(int slotNum)
    {
        //選択されたスロットに素材が入っているかどうか
        if(dateSlot[slotNum].materialItem != null)
        {
            //素材の画像を表示
            imageSlot[slotNum].sprite = dateSlot[slotNum].materialItem.itemSprite;
            //画像のアルファ値を変更し見えるように
            imageSlot[slotNum].color = new Vector4(1, 1, 1, 1);
        }
        else
        {
            //画像のアルファ値を変更し見えないように
            imageSlot[slotNum].color = new Vector4(1, 1, 1, 0);
        }

        dateSlot[slotNum].ImageChange();
    }

    /// <summary>
    /// 蓋を閉めているときに表示される囲いのアルファ値を調整する
    /// </summary>
    /// <param name="alpha">変更するアルファ値</param>
    void PartitionImageChange(int alpha)
    {
        p_Partition.GetComponent<Image>().color = new Vector4(1, 1, 1, alpha);
    }
}
