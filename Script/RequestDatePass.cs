using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestDatePass : MonoBehaviour
{
    //クエストのID
    public int requestID = 0;
    //選択された素材
    public int[] materialID = { 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        //シーンをまたいでも消えないように
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 二つにならないように
    /// </summary>
    /// <returns></returns>
    protected bool CheckInstance()
    {
        Destroy(gameObject);
        return false;
    }
}
