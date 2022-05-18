using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractDatePass : MonoBehaviour
{
    //花火の色
    public Color fireWorkColor = new Color(0, 0, 0);
    //花火の大きさ
    public int fireWorkSize = 0;
    //花火の形
    public int fireWorkForm = 0;
    //クエストのID
    public int requestID = 0;

    // Start is called before the first frame update
    void Start()
    {
        //シーンを変えても消えないように
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 2つ同じオブジェクトがないように
    /// </summary>
    /// <returns></returns>
    protected bool CheckInstance()
    {
        Destroy(gameObject);
        return false;
    }
}
