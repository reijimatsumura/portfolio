using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtractDateController : MonoBehaviour
{
    //花火の色がRGBでどの数値になるかの参照
    public FireWorkColorRecipe fireWorkColorRecipe;

    //スロットの素材のデータを持ってくるため
    public ItemDateController itemDateController;

    //決まったデータをリザルトに送るために使う
    public ExtractDatePass extractDatePass;

    /// <summary>
    /// 抽出を行い素材の花火の特性を集めて色と大きさと形を決める
    /// </summary>
    public void ExtractProcess()
    {
        extractDatePass.fireWorkColor = FireWorkColorDecision();
        extractDatePass.fireWorkSize = FireWorkSizeDecision();
        extractDatePass.fireWorkForm = FireWorkFormDecision();
    }

    /// <summary>
    /// スロットの素材から花火の色特性を決める
    /// </summary>
    /// <returns>花火の色特性</returns>
    Color FireWorkColorDecision()
    {
        float red = 0;
        float green = 0;
        float blue = 0;

        //窯の横幅分回す
        for (int i = 0; i < Math.Pow(itemDateController.dateSlot.Count, 1.0 / 2.0); i++)
        {
            Color slotA = new Color(0, 0, 0);

            //素材が入っていて素材が色を持っている
            if (itemDateController.dateSlot[i].materialItem != null && 
                itemDateController.dateSlot[i].materialItem.fireworkNature[0] > 0)
            {
                slotA = fireWorkColorRecipe.colorRecipe
                    [itemDateController.dateSlot[i].materialItem.fireworkNature[0] - 1];
                
                //黒だった場合
                if (red == 0 && green == 0 && blue == 0)
                {
                    red = slotA.r;
                    green = slotA.g;
                    blue = slotA.b;
                }
                else
                {
                    red = ColorRatioNumCalculation(red, slotA.r);
                    green = ColorRatioNumCalculation(green, slotA.g);
                    blue = ColorRatioNumCalculation(blue, slotA.b);
                }
            }
        }

        return new Color(red, green, blue);
    }

    /// <summary>
    /// 2つの値を足して割った数(ゼロがある場合例外)
    /// </summary>
    /// <param name="colorNumA">2つの数の一つ</param>
    /// <param name="colorNumB">2つの数の一つ</param>
    /// <returns>割った数</returns>
    float ColorRatioNumCalculation(float colorNumA, float colorNumB)
    {

        if(colorNumA == 0 || colorNumB == 0)
        {
            return (colorNumA + colorNumB) / 4;
        }
        else
        {
            return (colorNumA + colorNumB) / 2;
        }
    }

    /// <summary>
    /// スロットの素材から花火の大きさ特性を決める
    /// </summary>
    /// <returns>花火の大きさ特性</returns>
    int FireWorkSizeDecision()
    {
        int sizeNum = 0;

        //窯の横幅分回す
        for (int i = 0; i < Math.Pow(itemDateController.dateSlot.Count, 1.0 / 2.0); i++)
        {
            //素材が入っている
            if(itemDateController.dateSlot[i].materialItem != null)
            {
                sizeNum += itemDateController.dateSlot[i].materialItem.fireworkNature[1];
            }
        }

        return sizeNum;
    }

    /// <summary>
    /// スロットの素材から花火の形特性を決める
    /// </summary>
    /// <returns>花火の形特性</returns>
    int FireWorkFormDecision()
    {
        int sizeNum = 0;
        int formNum = 0;

        //窯の横幅分回す
        for (int i = 0; i < Math.Pow(itemDateController.dateSlot.Count, 1.0 / 2.0); i++)
        {
            //比較するためのデータが入っているかどうか
            if (itemDateController.dateSlot[i].materialItem != null)
            {
                //もともと入っているサイズと比べて新しいほうがでかいかどうか
                if (sizeNum < itemDateController.dateSlot[i].materialItem.fireworkNature[1])
                {
                    sizeNum = itemDateController.dateSlot[i].materialItem.fireworkNature[1];
                    formNum = itemDateController.dateSlot[i].materialItem.fireworkNature[2];
                }
                //サイズがおなじかどうか
                else if(sizeNum == itemDateController.dateSlot[i].materialItem.fireworkNature[1])
                {
                    //形の特性№が新しいほうが若いかどうか
                    if(formNum < itemDateController.dateSlot[i].materialItem.fireworkNature[2])
                    {
                        formNum = itemDateController.dateSlot[i].materialItem.fireworkNature[2];
                    }
                }
            }
        }

        return formNum;
    }
}
