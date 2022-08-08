/****************************************************
    文件：UIOfflineData.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/20 14:59:13
	功能：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOfflineData : OfflineData
{


    public Vector2[] m_AnchorMax;
    public Vector2[] m_AnchorMin;
    public Vector2[] m_AnchoredPosition;
    public Vector2[] m_SizeDelta;
    public Vector2[] m_Pivot;
    public ParticleSystem[] m_ParticleSystem;

    public override void BindData()
    {
        base.BindData();

        int cnt=m_AlllPoints.Length;
        m_AnchorMax = new Vector2[cnt];
        m_AnchorMin = new Vector2[cnt];
        m_AnchoredPosition = new Vector2[cnt];
        m_SizeDelta = new Vector2[cnt];
        m_Pivot = new Vector2[cnt];
        m_ParticleSystem = new ParticleSystem[cnt];


        for (int i = 0; i < m_AlllPoints.Length; i++)
        {
            Transform t = m_AlllPoints[i] as Transform;
            RectTransform rect = t.gameObject.GetComponent<RectTransform>();
            if (rect != null)
            {
                m_AnchorMax[i] = rect.anchorMax;
                m_AnchorMin[i] = rect.anchorMin;
                m_AnchoredPosition[i] = rect.anchoredPosition;
                m_SizeDelta[i] = rect.sizeDelta;
                m_Pivot[i] = rect.pivot;
                m_ParticleSystem[i] = rect.GetComponentInChildren<ParticleSystem>(true);

            }

        }
    }


    public override void Reset()
    {
        base.Reset();

        for (int i = 0; i < m_AlllPoints.Length; i++)
        {
            Transform t = m_AlllPoints[i] as Transform;
            RectTransform rect = t.GetComponent<RectTransform>();
            if (rect != null)
            {
              rect.anchorMax =   m_AnchorMax[i];
               rect.anchorMin = m_AnchorMin[i];
               rect.anchoredPosition = m_AnchoredPosition[i];
               rect.sizeDelta= m_SizeDelta[i];
               rect.pivot = m_Pivot[i];
            }

        }


        for (int i = 0; i < m_ParticleSystem.Length; i++)
        {


            if (m_ParticleSystem[i] != null)
            {
                ParticleSystem p = m_ParticleSystem[i];
                p.Clear();
                p.Play();
            }


        }
    }
}