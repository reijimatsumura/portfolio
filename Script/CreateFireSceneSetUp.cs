using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateFireSceneSetUp : MonoBehaviour
{
    //渡されたデータを入れる変数
    int requestID = 0;
    int[] materialID = { 0, 0 };

    //データを入れている間にゲーム画面を触れないようにするもの
    public GameObject fadeOut;
    //素材データをダウンロードするスクリプト
    public MaterialItemDateDownload itemDateDownload;
    //素材のレシピを入れる
    public MaterialRecipe materialRecipe;
    //評価するときにどのクエストかを参照する
    public ExtractDatePass extractDatePass;

    // Start is called before the first frame update
    void Start()
    {
        //素材データのように変更予定
        var requestDate = GameObject.Find("RequestDate").GetComponent<RequestDatePass>();

        requestID = requestDate.requestID;
        materialID[0] = requestDate.materialID[0];
        materialID[1] = requestDate.materialID[1];

        ////素材同士の合成結果を入手する
        //GetRecipe getRecipe = new GetRecipe();
        //var load = getRecipe.Rapper_LoadData();

        ////ファイルから素材の数をとってくる
        //var guids = 10;
        //if (guids == 0)
        //{
        //    throw new System.IO.FileNotFoundException(" does not found");
        //}
        ////配列に要素数をセット
        //materialRecipe.materialAffinity = new int[guids, guids];
        ////Jsonからのデータをセットする
        //for (var i = 0; i < materialRecipe.materialAffinity.GetLength(0); i++)
        //{
        //    for (var j = 0; j < materialRecipe.materialAffinity.GetLength(1); j++)
        //    {
        //        materialRecipe.materialAffinity[i, j] = load.recipe[i].recipe_get[j];
        //    }
        //}

        extractDatePass.requestID = requestID;

        StartCoroutine(RecipeSetUp());
    }

    IEnumerator RecipeSetUp()
    {
        //素材同士の合成結果を入手する
        GetRecipe getRecipe = new GetRecipe();

        IEnumerator enumerator = getRecipe.LoadSaveData();

        yield return StartCoroutine(enumerator);

        RecipeRapper recipeRapper = (RecipeRapper)enumerator.Current;

        //ファイルから素材の数をとってくる
        var guids = 10;
        if (guids == 0)
        {
            throw new System.IO.FileNotFoundException(" does not found");
        }
        //配列に要素数をセット
        materialRecipe.materialAffinity = new int[guids, guids];
        //Jsonからのデータをセットする
        for (var i = 0; i < materialRecipe.materialAffinity.GetLength(0); i++)
        {
            for (var j = 0; j < materialRecipe.materialAffinity.GetLength(1); j++)
            {
                materialRecipe.materialAffinity[i, j] = recipeRapper.recipe[i].recipe_get[j];
            }
        }

        yield return null;

        StartCoroutine(itemDateDownload.WorkDateDownload(requestID, materialID));
    }

    /// <summary>
    /// データのダウンロードが終了したときに呼ぶ
    /// </summary>
    /// <returns></returns>
    public IEnumerator SetUpComplete()
    {
        //画像のα値を触れるように取り出す
        var fadeOutImage = fadeOut.GetComponent<Image>();

        //だんだん暗転してシーンが明けるようにα値を下げていく
        for(float i = 1.0f; i >= 0; i -= 0.01f)
        {
            fadeOutImage.color = new Vector4(0, 0, 0, i);
            //次の処理が実行される前に0.01f秒の間隔を開ける
            yield return new WaitForSeconds(0.01f);
        }



        //暗転も明けたのでプレイヤーが触れるようにする
        fadeOut.SetActive(false);

        yield return null;
    }
}
