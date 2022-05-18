using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworkController : MonoBehaviour
{
    //通常花火を打ち上げるのに使う
    public ParticleSystem normalLaunch;
    //サブ通常花火を打ち上げるのに使う
    public ParticleSystem[] subNormalLaunch;
    //通常花火のエフェクトを入れる
    public ParticleSystem normalFireworkEffect;
    //サブ通常花火のエフェクトを入れる
    public ParticleSystem[] subNormalFireworkEffect;
    //形の変わった花火を打ち上げる
    public ParticleSystem shapeLaunch;
    //サブ通常花火を打ち上げるのに使う
    public ParticleSystem[] subShapeLaunch;
    //形が変わった花火のエフェクトを入れる
    public ParticleSystem shapeFireworkEffect;
    //サブ通常花火のエフェクトを入れる
    public ParticleSystem[] subShapeFireworkEffect;
    //マスクのオブジェクトを入れる
    public GameObject fireworkMask;
    //サブマスクのオブジェクトを入れる
    public GameObject[] subFireworkMask;

    //マスクを格納しておく箱
    public Sprite[] fireworkMasks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 花火の持っている要素からその花火のエフェクトにするためのセッティングを行う
    /// </summary>
    /// <param name="fireworkSize">花火の大きさ</param>
    /// <param name="changeColor">花火の色</param>
    /// <param name="foamNum">花火の形</param>
    /// <param name="subSelectNum">どのサブに入れるか</param>
    public void FireworkSorting
        (int fireworkSize, Color changeColor, int foamNum, int subSelectNum)
    {
        //花火のエフェクトを入れる箱
        ParticleSystem fireworkEffect;
        
        //丸形の花火であるかどうか
        if(foamNum < 2)
        {
            //サブの花火じゃないなら
            if (subSelectNum == -1)
            {
                fireworkEffect = normalFireworkEffect;
            }
            else
            {
                fireworkEffect = subNormalFireworkEffect[subSelectNum]; 
            }

            
        }
        else
        {
            //花火のマスクを入れる箱
            GameObject fireworkEffectMask;

            //サブの花火じゃないなら
            if (subSelectNum == -1)
            {
                fireworkEffect = shapeFireworkEffect;
                fireworkEffectMask = fireworkMask;
            }
            else
            {
                fireworkEffect = subShapeFireworkEffect[subSelectNum];
                fireworkEffectMask = subFireworkMask[subSelectNum];
            }

            ChangeEffectFoam(fireworkEffectMask, foamNum);
        }

        FireTypeControl(fireworkSize, fireworkEffect, subSelectNum);
        ChangeEffectColor(changeColor, fireworkEffect);
        ChangeEffectSize(fireworkSize, fireworkEffect, subSelectNum);
    }

    /// <summary>
    /// 花火のアニメーションを実行する
    /// </summary>
    /// <param name="foamNum">花火の形</param>
    /// <param name="subSelectNum">どのサブに入れるか</param>
    /// <param name="fireworkSize">花火の大きさ</param>
    public void FireworkLaunch(int foamNum, int subSelectNum, int fireworkSize)
    {
        //形がないor丸型
        if(foamNum < 2)
        {
            //メインかどうか
            if(subSelectNum == -1)
            {
                normalLaunch.Play();
            }
            else
            {
                subNormalLaunch[subSelectNum].Play();
            }
        }
        else
        {
            //花火のマスクを入れる箱
            GameObject fireworkEffectMask;
            ParticleSystem fireworkLaunch;

            //メインかどうか
            if (subSelectNum == -1)
            {
                fireworkEffectMask = fireworkMask;
                fireworkLaunch = shapeLaunch;
            }
            else
            {
                fireworkEffectMask = subFireworkMask[subSelectNum];
                fireworkLaunch = subShapeLaunch[subSelectNum];
            }

            fireworkLaunch.Play();
            float magnificationRate = MagnificationRateSet(foamNum, subSelectNum);
            StartCoroutine(MaskSizeControl(fireworkSize, fireworkLaunch, fireworkEffectMask, magnificationRate));
        }
    }

    /// <summary>
    /// マスクが花火に寄り添うように
    /// </summary>
    /// <param name="fireworkSize">花火のサイズ</param>
    /// <param name="fireworkLaunch">種の最終的な位置</param>
    /// <param name="fireworkEffectMsk">マスク</param>
    /// <param name="magnificationRate">マスクの基本サイズ</param>
    /// <returns></returns>
    IEnumerator MaskSizeControl
        (int fireworkSize, ParticleSystem fireworkLaunch, GameObject fireworkEffectMsk, float magnificationRate)
    {
        //種の最終位置が固定されるまでの待機時間
        yield return new WaitForSeconds(1.4f);

        //エフェクトの位置を入手する
        ParticleSystem.Particle[] particle = new ParticleSystem.Particle[1];
        fireworkLaunch.GetParticles(particle);

        //座標系が違うので変換をかけたのちマスクをその位置に持っていく
        var particleLocalPosition = particle[0].position;
        var particleWorldPosition = fireworkLaunch.transform.TransformPoint(particleLocalPosition);
        fireworkEffectMsk.transform.position = particleWorldPosition;

        //種が爆発するまでの待機時間
        yield return new WaitForSeconds(0.2f);
        //何回回ってその結果に到達するか
        float roopNum = 125;

        //花火がはじけるのに合わせてだんだんマスクを大きくしていく
        for (int i = 0; i < roopNum; i++)
        {
            fireworkEffectMsk.transform.localScale +=
                new Vector3(
                    magnificationRate * fireworkSize / roopNum + 2.0f / roopNum,
                    magnificationRate * fireworkSize / roopNum + 2.0f / roopNum,
                    0);

            yield return new WaitForSeconds(0.5f / roopNum);
        }
        //最大まで広がったあとの対空時間
        yield return new WaitForSeconds(0.5f);
        //何回回ってその結果に到達するか
        roopNum = 100;

        //花火が爆発しきったあとに落ちていくのについていくように
        for(int i = 0; i < roopNum; i++)
        {
            fireworkEffectMsk.transform.position -=
                new Vector3(0, (0.8f + fireworkSize * 0.1f) / roopNum, 0);

            yield return new WaitForSeconds(1.0f / roopNum);
        }
        //座標とサイズをリセット
        fireworkEffectMsk.transform.localPosition = Vector3.zero;
        fireworkEffectMsk.transform.localScale = new Vector3(0, 0, 1);
    }

    /// <summary>
    /// 花火の形ごとにマスクの基本サイズを設定する
    /// </summary>
    /// <param name="foamNum">花火の形</param>
    /// <param name="subSelectNum">どのサブに入れるか</param>
    /// <returns>上昇値</returns>
    float MagnificationRateSet(int foamNum, int subSelectNum)
    {
        //基本サイズを入れる
        float magnificationRate = 0.0f;
        //形ごとに振り分け
        switch (foamNum)
        {
            case 2:
                magnificationRate = 0.9f;
                break;
            case 3:

                break;
            case 4:

                break;
            case 5:
                magnificationRate = 1.25f;
                break;
        }

        //サブならば
        if(subSelectNum != -1)
        {
            magnificationRate /= 2.2f;
        }

        return magnificationRate;
    }

    /// <summary>
    /// 火の粉の量を調整する  
    /// </summary>
    /// <param name="fireworkSize">花火のサイズ</param>
    /// <param name="foamNum">形の番号</param>
    public void FireTypeControl(int fireworkSize, ParticleSystem fireworkEffect, int subSelectNum)
    {
        ParticleSystem.EmissionModule fireworkFireTypeVolume = fireworkEffect.emission;
        float time = 0.0f;

        //サブエフェクトかどうかを判断
        if(subSelectNum == -1)
        {
            //エフェクトの生成時間と生成量を調整
            fireworkFireTypeVolume.SetBursts(
                new ParticleSystem.Burst[]
                {
                new ParticleSystem.Burst(time, (fireworkSize + 1) * 125)
                });
        }
        else
        {
            fireworkFireTypeVolume.SetBursts(
                new ParticleSystem.Burst[]
                {
                new ParticleSystem.Burst(time, (fireworkSize + 1) * 70)
                });
        }
    }

    /// <summary>
    /// エフェクトの色を変更する
    /// </summary>
    /// <param name="changeColor">変更する色</param>
    /// <param name="foamNum">形の番号</param>
    public void ChangeEffectColor(Color changeColor, ParticleSystem fireworkEffect)
    {
        ParticleSystem.MainModule fireworkColor = fireworkEffect.main;

        fireworkColor.startColor = changeColor;
    }

    /// <summary>
    /// エフェクトのサイズを変更する
    /// </summary>
    /// <param name="changeSize">変更するサイズ</param>
    /// <param name="foamNum">形の番号</param>
    public void ChangeEffectSize(int changeSize, ParticleSystem fireworkEffect, int selectNum)
    {
        ParticleSystem.MainModule fireworkSize = fireworkEffect.main;

        //サブエフェクトかどうかを判断
        if (selectNum == -1)
        {
            fireworkSize.startSpeed = 
                new ParticleSystem.MinMaxCurve(4 * (changeSize + 1), 4 * (changeSize + 2));
        }
        else
        {
            fireworkSize.startSpeed = 
                new ParticleSystem.MinMaxCurve(1.8f * (changeSize + 1), 1.8f * (changeSize + 2));
        }
    }

    /// <summary>
    /// 花火の形を変更する
    /// </summary>
    /// <param name="changefoam">どの形か</param>
    public void ChangeEffectFoam(GameObject fireworkEffectMask ,int changefoam)
    {
        fireworkEffectMask.GetComponent<SpriteMask>().sprite = fireworkMasks[changefoam - 2];
    }
}
