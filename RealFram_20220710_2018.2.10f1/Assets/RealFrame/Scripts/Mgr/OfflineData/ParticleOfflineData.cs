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

public class ParticleOfflineData : OfflineData
{
    public ParticleSystem[] m_ParticleSystem;
    /// <summary>拖尾</summary> 
    public TrailRenderer[] m_TrailRenderer;

    public override void BindData()
    {
        base.BindData();

        bool findUnActive =true;
        m_ParticleSystem =gameObject.GetComponentsInChildren<ParticleSystem>(findUnActive);
        m_TrailRenderer =gameObject.GetComponentsInChildren<TrailRenderer>(findUnActive);
    }


    public override void Reset()
    {
        base.Reset();

        foreach (ParticleSystem p in m_ParticleSystem)
        {
            p.Play();
            p.Clear();
        }

        foreach (TrailRenderer t in m_TrailRenderer)
        {
            t.Clear();
        }

    }
}