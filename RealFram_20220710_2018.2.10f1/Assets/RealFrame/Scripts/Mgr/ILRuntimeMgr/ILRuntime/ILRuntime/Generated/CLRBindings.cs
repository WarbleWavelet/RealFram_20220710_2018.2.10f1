using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {
        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Debug_Binding.Register(app);
            Window_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            UnityEngine_Object_Binding.Register(app);
            Singleton_1_SceneMgr_Binding.Register(app);
            SceneMgr_Binding.Register(app);
            Demo16_LoadPanel_Binding.Register(app);
            System_Single_Binding.Register(app);
            UnityEngine_UI_Text_Binding.Register(app);
            UnityEngine_UI_Slider_Binding.Register(app);
            System_String_Binding.Register(app);
            Singleton_1_UIMgr_Binding.Register(app);
            UIMgr_Binding.Register(app);
            Singleton_1_ObjectMgr_Binding.Register(app);
            ObjectMgr_Binding.Register(app);
            Singleton_1_ResourceMgr_Binding.Register(app);
            Demo16_MenuPanel_Binding.Register(app);
            ResourceMgr_Binding.Register(app);
            System_Diagnostics_StackTrace_Binding.Register(app);
            System_Diagnostics_StackFrame_Binding.Register(app);
            Singleton_1_CfgMgr_Binding.Register(app);
            CfgMgr_Binding.Register(app);
            System_Object_Binding.Register(app);
            MonsterData_Binding.Register(app);
            System_Collections_Generic_List_1_MonsterBase_Binding.Register(app);
            Test_CLRBindingClass_Binding.Register(app);
            MonoSingleton_1_GameStart_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            Delegate_Void_Binding.Register(app);
            Delegate_String_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            Singleton_1_ILRuntimeMgr_Binding.Register(app);
            ILRuntimeMgr_Binding.Register(app);
            System_Int32_Binding.Register(app);
        }
    }
}
