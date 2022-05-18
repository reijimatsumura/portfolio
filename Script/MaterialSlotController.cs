using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSlotController : MonoBehaviour
{
    //素材の画像を表示するところを入れる
    public Image materiaItemImage;

    //スロットに入っている素材
    public MaterialItem materialItem = null;

    //囲われているかどうか
    public bool partition = false;

    /// <summary>
    /// オブジェクトの画像変更
    /// </summary>
    public void ImageChange()
    {
        //素材が入っているかどうか
        if(materialItem != null)
        {
            //素材の画像を入れる
            materiaItemImage.sprite = materialItem.itemSprite;
            ImageAlphaChange(1);
        }
        else
        {
            ImageAlphaChange(0);
        }
    }

    /// <summary>
    /// 素材の画像表示場所のアルファ値を変える
    /// </summary>
    /// <param name="alpha">変更するアルファ値</param>
    void ImageAlphaChange(int alpha)
    {
        materiaItemImage.color = new Vector4(1, 1, 1, alpha);
    }
}
