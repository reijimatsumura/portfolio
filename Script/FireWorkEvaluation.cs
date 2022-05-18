using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FireWorkEvaluation : MonoBehaviour
{
    //花火の情報を受け取ったオブジェクトのスクリプトを入れる
    ExtractDatePass extractDatePass;
    //花火の評価の裁定をいれる
    Evaluation evaluation;

    //花火のエフェクトをできたものごとに変えるため
    public FireworkController fireworkController;
    //花火の点数を表示するためのもの
    public ResultSceneTextChange resultSceneTextChange;

    //依頼評価が出てくる紙
    public GameObject questPaper;

    //ユーザーがアニメーションをスキップしたかどうか
    bool[] _animationSkip = new bool[5] { false, false, false, false, false };

    //リザルトでスキップできる演出を入れる
    Coroutine _waitTime = null;

    private void Start()
    {
        //必要なデータを入れる
        extractDatePass = GameObject.Find("ExtractDate").GetComponent<ExtractDatePass>();

        StartCoroutine(EvaluationLoad());
    }
    private void Update()
    {
        // ボタンが押されたかどうか
        if (Input.anyKeyDown)
        {
            //スキップ可能な待機時間になっているかどうか
            if(_waitTime != null)
            {
                //どのあたりまで進んでいるか
                for (int i = 0; i < _animationSkip.Length; i++)
                {
                    //その演出が現在実行されているかどうか
                    if (!_animationSkip[i])
                    {
                        _animationSkip[i] = true;
                        break;
                    }

                    // -シーン遷移-
                    if (i == _animationSkip.Length - 1)
                    {
                        Application.Quit();
                    }
                }
            }
        }
    }

    /// <summary>
    /// アドレスタブルを使ってデータをダウンロード
    /// </summary>
    /// <returns></returns>
    IEnumerator EvaluationLoad()
    {
        //リクエストIDからデータを参照
        var handle = Addressables.LoadAssetAsync<Evaluation>
                        ("Evaluation_" + extractDatePass.requestID);
        yield return handle;
        //ダウンロードできたかどうか
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //データを入れる
            evaluation = handle.Result;
        }
        yield return null;

        StartCoroutine(ResultSceneSetUp());
    }

    /// <summary>
    /// リザルト画面のセットアップをおこなう
    /// </summary>
    /// <returns></returns>
    IEnumerator ResultSceneSetUp()
    {
        StartCoroutine(FireworkEffectSetting());
        Debug.Log(0);
        yield return _waitTime = StartCoroutine(TiemWait(4.0f, 0));

        StartCoroutine(QuestPaperAnimation());

        yield return _waitTime = StartCoroutine(TiemWait(3.0f, 0));

        _animationSkip[0] = true;

        StartCoroutine(CraftFireWorkEvaluation());
    }

    /// <summary>
    /// 花火を飛ばす FireworkEffectStart
    /// </summary>
    /// <returns></returns>
    IEnumerator FireworkEffectSetting()
    {

        for(int i = -1; i < 2; i++)
        {
            fireworkController.FireworkSorting
            (extractDatePass.fireWorkSize, extractDatePass.fireWorkColor, extractDatePass.fireWorkForm, i);
        }

        yield return StartCoroutine(FireworkEffectStart()); ;

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(SubFireworkEffectStart(0));

        yield return new WaitForSeconds(3.0f);
        StartCoroutine(SubFireworkEffectStart(1));
    }

    IEnumerator FireworkEffectStart()
    {
        fireworkController.FireworkLaunch(extractDatePass.fireWorkForm, -1, extractDatePass.fireWorkSize);

        yield return new WaitForSeconds(5.0f);
        StartCoroutine(FireworkEffectStart());
    }

    IEnumerator SubFireworkEffectStart(int subSelectNum)
    {
        fireworkController.FireworkLaunch
            (extractDatePass.fireWorkForm, subSelectNum, extractDatePass.fireWorkSize);

        yield return new WaitForSeconds(5.0f);
        StartCoroutine(SubFireworkEffectStart(subSelectNum));
    }

    /// <summary>
    /// 紙のアニメーションを行う
    /// </summary>
    /// <returns></returns>
    IEnumerator QuestPaperAnimation()
    {
        //紙をアニメーションの開始地点に配置する
        Vector3 questPaperPos = new Vector3(4.4f, 0, 90);
        
        //だんだんと終了位置まで動かしていく
        for (float i = 0.01f; i < 1; i += 0.01f)
        {
            if(i < 0.5f)
            {
                //アニメーションがスキップされたか
                if(_animationSkip[0])
                {
                    i = 0.49f;
                }
                float y = 0.2f / (i * 2);
                float x = 0.2f / (i / 2);

                questPaper.transform.position = questPaperPos + new Vector3(-x, -y, 0);
                
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    /// <summary>
    /// 点数を出しそれをUIに反映する
    /// </summary>
    IEnumerator CraftFireWorkEvaluation()
    {
        
        int colorNum = ColorEvaluation();
        int sizeNum = SizeEvaluation();
        int formNum = FormEvaluation();

        yield return null;

        //だんだん点が増えていくように
        for(int i = 1; i <= colorNum; i++)
        {
            //アニメーションがスキップされたか
            if (_animationSkip[1])
            {
                i = colorNum;
            }

            yield return _waitTime = StartCoroutine(TiemWait(0.05f, 1));
            resultSceneTextChange.ColorEvaluationTextChange(i);
        }
        _animationSkip[1] = true;

        //だんだん点が増えていくように
        for (int i = 1; i <= sizeNum; i++)
        {
            //アニメーションがスキップされたか
            if (_animationSkip[2])
            {
                i = sizeNum;
            }

            yield return _waitTime = StartCoroutine(TiemWait(0.05f, 2));
            resultSceneTextChange.SizeEvaluationTextChange(i);
        }
        _animationSkip[2] = true;

        //だんだん点が増えていくように
        for (int i = 1; i <= formNum; i++)
        {
            //アニメーションがスキップされたか
            if (_animationSkip[3])
            {
                i = formNum;
            }

            yield return _waitTime = StartCoroutine(TiemWait(0.05f, 3));
            resultSceneTextChange.FormEvaluationTextChange(i);
        }
        _animationSkip[3] = true;

        //だんだん点が増えていくように
        for (int i = 1; i <= colorNum + sizeNum + formNum; i++)
        {
            //アニメーションがスキップされたか
            if (_animationSkip[4])
            {
                i = colorNum + sizeNum + formNum;
            }

            yield return _waitTime = StartCoroutine(TiemWait(0.05f, 4));
            resultSceneTextChange.ComprehensiveEvaluationTextChange(i);
        }
        _animationSkip[4] = true;

        //リザルト_ランク


    }

    /// <summary>
    /// 指定された時間の間だけ処理を止めるがスキップすることが可能
    /// </summary>
    /// <param name="stopTiemNum">指定された時間</param>
    /// <param name="animationType">何番目の演出か</param>
    /// <returns></returns>
    IEnumerator TiemWait(float stopTiemNum, int animationType)
    {
        for(float i = 0; i < stopTiemNum; i += 0.01f)
        {
            //この演出がスキップされたか
            if(_animationSkip[animationType])
            {
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// 色の評価点を出す
    /// </summary>
    /// <returns>算出された色評価点</returns>
    int ColorEvaluation()
    {
        //カラーの差分を格納する
        float colorDifferenceNum = -1;

        //複数色に対応するために
        for(int i = 0; i < evaluation.colorPoint.Length; i++)
        {
            //一時的に差分を保管
            float differenceNum = 0;

            //この要素には色が入っているかどうか
            if (evaluation.colorPoint[i].a != 0)
            {
                //各色で計算し差分を知らべる
                differenceNum = NumSignChange(evaluation.colorPoint[i].r - extractDatePass.fireWorkColor.r);
                differenceNum += NumSignChange(evaluation.colorPoint[i].g - extractDatePass.fireWorkColor.g);
                differenceNum += NumSignChange(evaluation.colorPoint[i].b - extractDatePass.fireWorkColor.b);
            }
            else
            {
                //比較することができないからそれを示す値にする
                differenceNum = -1;
            }

            //差分の値が前に入れてある値よりも小さいかつ入れる値が比較できない値が入っていない
            //もしくは前に入れてある値が比較できない値
            if (colorDifferenceNum > differenceNum && differenceNum != -1
                    || colorDifferenceNum == -1)
            {
                colorDifferenceNum = differenceNum;
            }
        }

        //差の値によって評価点が変化
        if(colorDifferenceNum == -1)
        {
            return 0;
        }
        else if (colorDifferenceNum < 0.8f)
        {
            return 40;
        }
        else if (colorDifferenceNum < 1.0f)
        {
            return 30;
        }
        else if (colorDifferenceNum < 1.4f)
        {
            return 20;
        }
        else if (colorDifferenceNum < 1.8f)
        {
            return 10;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 符号がマイナスになったときに逆にする
    /// </summary>
    /// <param name="changeColorNum">値</param>
    /// <returns></returns>
    float NumSignChange(float changeNum)
    {
        if (changeNum < 0)
        {
            return changeNum * -1;
        }
        else
        {
            return changeNum;
        }
    }

    /// <summary>
    /// 大きさの評価点を出す
    /// </summary>
    /// <returns>大きさ評価点</returns>
    int SizeEvaluation()
    {
        if(extractDatePass.fireWorkSize != 0)
        {
            return evaluation.scalePoint[extractDatePass.fireWorkSize - 1];
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 形の評価点を出す
    /// </summary>
    /// <returns>形評価点</returns>
    int FormEvaluation()
    {
        if(extractDatePass.fireWorkForm != 0)
        {
            return evaluation.naturePoint[extractDatePass.fireWorkForm - 1];
        }
        else
        {
            return 0;
        }
    }
}
