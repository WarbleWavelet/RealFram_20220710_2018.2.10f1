/****************************************************
    文件：StorageMgr.cs
	作者：lenovo
    邮箱: 
    日期：2022/7/20 0:34:32
	功能：游戏存储Mgr
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OfflineData : MonoBehaviour
{
	public GameObject m_GameObejct;
	public Rigidbody m_Rigidbody;
	public Collider m_Collider;
    /// <summary>所有子节点</summary>
    public Transform[] m_AlllPoints;
    public int[] m_AlllPoints_ChildCount;
    public bool[] m_AlllPoints_Active;
	public Vector3[] m_Position;
	public Quaternion[] m_Rotation;
	public Vector3[] m_Scale;

	public virtual void Reset()
	{

		for (int i = 0; i < m_AlllPoints.Length; i++)
        {
            Transform t = m_AlllPoints[i] as Transform;
			if (t != null)
			{ 
	            m_AlllPoints_ChildCount[i] = t.childCount;
				m_AlllPoints_Active[i] = t.gameObject.activeSelf;
	
				t.gameObject.SetActive(m_AlllPoints_Active[i]);			
				t.localPosition=m_Position[i] ;
				t.localRotation=m_Rotation[i]  ;
				t.localScale=m_Scale[i]   ;

				//删除超出数量
				int childCnt = m_GameObejct.transform.childCount;
				if (childCnt > m_AlllPoints_ChildCount[i])
				{ 
				
					for (int j= m_AlllPoints_ChildCount[i]; j <childCnt ;j++)
					{
						Transform _t = t.GetChild(j);
						if (ObjectMgr.Instance.IsCreateByObjectMgr(_t.gameObject) == true)
						{ 
							GameObject.Destroy(_t.gameObject); }
						}
					}			
				}

			}

		}
	

	public virtual void BindData()
	{
		m_GameObejct = gameObject;
		bool findUnAcive = true;
		m_Collider = m_GameObejct.GetComponentInChildren<Collider>(findUnAcive);
		m_Rigidbody = m_GameObejct.GetComponentInChildren<Rigidbody>(findUnAcive);
		//
		m_AlllPoints = m_GameObejct.GetComponentsInChildren<Transform>(findUnAcive);
		int cnt= m_AlllPoints.Length;
		//
		m_AlllPoints_Active = new bool[cnt];
		m_AlllPoints_ChildCount = new int[cnt];
		m_Position=new Vector3[cnt];
		m_Rotation=new Quaternion[cnt];
		m_Scale= new Vector3[cnt];

		//
		for (int i = 0; i <cnt; i++)
        {
			Transform t = m_AlllPoints[i] as Transform;
			if (t != null)
			{ 
				m_AlllPoints_ChildCount[i] = t.childCount;
				m_AlllPoints_Active[i] = t.gameObject.activeSelf;
				m_Position[i] = t.localPosition;
				m_Rotation[i] = t.localRotation;
				m_Scale[i] = t.localScale;
			}

		}
		
	}
}