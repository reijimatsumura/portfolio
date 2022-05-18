using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MaterialItemDateDownload : MonoBehaviour
{
    //ダウンロードしたデータを取ってこれるようにする
    public ItemDateController itemDateController;
    //ワークベンチに置かれている素材
    public MaterialSlotController[] workBenchMaterial;
    //元のシーンのコルーチンを呼ぶため
    public CreateFireSceneSetUp createFireSceneSetUp;

    //仮置き
    public Request request;

    /// <summary>
    /// 窯の中身をそのクエスト用に素材をセッティングし選ばれた素材をワークベンチに入れる
    /// </summary>
    /// <param name="requestID">どのクエストか</param>
    /// <param name="selectMaterialID">どの素材が選ばれたか</param>
    /// <returns></returns>
    public IEnumerator WorkDateDownload(int requestID, int[] selectMaterialID)
    {
        for(int i = 0; i < selectMaterialID.Length; i++)
        {
            //素材が入っているかどうか
            if (selectMaterialID[i] != 0)
            {
                //戻り値を取り出すためコルーチンの型のなかに戻り値が欲しいコルーチンを入れる
                IEnumerator iEnumerator = MaterialDateDownload(selectMaterialID[i]);
                //コルーチンを動かす
                yield return StartCoroutine(iEnumerator);
                //戻り値を取り出し入れる
                MaterialItem materialHandle = (MaterialItem)iEnumerator.Current;
                //ワークベンチにおいてあるオブジェクトに戻り値に入っている素材データを入れる
                workBenchMaterial[i].materialItem = materialHandle;
                workBenchMaterial[i].ImageChange();
            }
            else
            {
                workBenchMaterial[i].materialItem = null;
                workBenchMaterial[i].ImageChange();
            }
        }

        

        //ループの回数を数える
        int roopNum = 0;

        //クエストに入っている2次配列を順番通り窯のスロットにいれるために
        for(int i = 2; i > -1; i--)
        {
            for(int j = 0; j < request.materialItemID.GetLength(0); j++)
            {
                //クエストの配列に入っている値が0でなかったときにそのIDの素材を窯のスロットに入れる
                if(request.materialItemID[i, j] != 0)
                {
                    //戻り値を取り出すためコルーチンの型のなかに戻り値が欲しいコルーチンを入れる
                    IEnumerator iEnumerator = MaterialDateDownload(request.materialItemID[i, j]);
                    //コルーチンを動かす
                    yield return StartCoroutine(iEnumerator);
                    //戻り値を取り出し入れる
                    MaterialItem materialKilnHandle = (MaterialItem)iEnumerator.Current;
                    //窯の一つのスロットに戻り値に入っている素材データを入れる
                    itemDateController.dateSlot[roopNum].materialItem = materialKilnHandle;
                    itemDateController.dateSlot[roopNum].ImageChange();
                }
                //ループの区切りになるのでループ回数を増やす
                roopNum++;
            }
        }
        
        yield return StartCoroutine(createFireSceneSetUp.SetUpComplete());
    }

    /// <summary>
    /// 指定したIDの素材データをダウンロードし戻り値に入れる
    /// </summary>
    /// <param name="materialItemID">指定ID</param>
    /// <returns>素材データ</returns>
    public IEnumerator MaterialDateDownload(int materialItemID)
    {
        //素材データに書かれているパスからデータを参照
        var materialKilnHandle = Addressables.LoadAssetAsync<MaterialItem>
                        ("MaterialItem_" + materialItemID);
        yield return materialKilnHandle;
        //ダウンロードできたかどうか
        if (materialKilnHandle.Status == AsyncOperationStatus.Succeeded)
        {
            //戻り値にデータを入れる
            yield return materialKilnHandle.Result;
        }
    }
}
