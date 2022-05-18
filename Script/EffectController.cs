using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    //窯を冷やしたときのエフェクト
    public ParticleSystem snowCrystalEffect;
    //窯を温めたときのエフェクト
    public ParticleSystem steamEffect;
    //アイテム合成時のエフェクト
    public ParticleSystem syntheticEffect;

    //エフェクトの再生速度のデータを取ってくる
    public ItemDateController itemDateController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 冷却エフェクトを再生する
    /// </summary>
    public IEnumerator EffectSnowCrystalOn()
    {
        yield return StartCoroutine(EffectSpeedChange(1, itemDateController.kilnMoveSpeed));
    }

    /// <summary>
    /// 加熱エフェクトを再生する
    /// </summary>
    public IEnumerator EffectSteamOn()
    {
        yield return StartCoroutine(EffectSpeedChange(2, itemDateController.kilnMoveSpeed));
    }

    /// <summary>
    /// 合成時エフェクトを再生する
    /// </summary>
    public IEnumerator EffectSyntheticOn()
    {
        yield return StartCoroutine(EffectSpeedChange(3, itemDateController.kilnMoveSpeed));
    }

    /// <summary>
    /// 各エフェクトの再生時間を調整する
    /// </summary>
    /// <param name="selectEffectNum">どのエフェクトなのか</param>
    /// <param name="speed">再生速度の倍数</param>
    /// <returns></returns>
    IEnumerator EffectSpeedChange(int selectEffectNum, float speed)
    {
        //どのエフェクトでも変更するところは同じなので箱だけ用意する
        ParticleSystem playEffect = new ParticleSystem();
        ParticleSystem.MainModule chengeMineEffect;
        ParticleSystem.EmissionModule chengeEmissonEffect;

        //どのエフェクトか
        switch (selectEffectNum)
        {
            case 1:
                playEffect = snowCrystalEffect;
                break;
            case 2:
                playEffect = steamEffect;
                break;
            case 3:
                playEffect = syntheticEffect;
                break;
        }
        chengeMineEffect = playEffect.main;
        chengeEmissonEffect = playEffect.emission;

        //エフェクトの生成時間
        chengeMineEffect.duration = chengeMineEffect.duration / speed;
        //エフェクトの生存時間
        chengeMineEffect.startLifetime = chengeMineEffect.startLifetime.constant / speed;
        //時間あたりの生成量
        chengeEmissonEffect.rateOverTime = chengeEmissonEffect.rateOverTime.constant * speed;
        //再生
        playEffect.Play();
        //エフェクト再生中コードを停止
        yield return new WaitForSeconds(chengeMineEffect.duration + chengeMineEffect.startLifetime.constant);

        //値をもとに戻す
        chengeMineEffect.duration = chengeMineEffect.duration * speed;
        chengeMineEffect.startLifetime = chengeMineEffect.startLifetime.constant * speed;
        chengeEmissonEffect.rateOverTime = chengeEmissonEffect.rateOverTime.constant / speed;
    }
}
