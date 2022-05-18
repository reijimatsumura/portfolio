using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneRoadController : MonoBehaviour
{
    //シーンをロードするはこ
    AsyncOperation resultScene;

    /// <summary>
    /// シーンを事前にロードしておく
    /// </summary>
   　public void SceneRoad()
    {
        resultScene = SceneManager.LoadSceneAsync("ResultScene");

        //シーン移動を任意のタイミングにするために移動を一時的に無効
        resultScene.allowSceneActivation = false;
    }

    /// <summary>
    /// シーンを移動
    /// </summary>
    public void ResultSceneStart()
    {
        resultScene.allowSceneActivation = true;
    }
}
