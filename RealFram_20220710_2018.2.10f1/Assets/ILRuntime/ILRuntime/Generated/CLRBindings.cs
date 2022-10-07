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
            System_String_Binding.Register(app);
            UnityEngine_Debug_Binding.Register(app);
            Test_CLRBindingClass_Binding.Register(app);
            Delegate_Void_Binding.Register(app);
            Delegate_String_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            Singleton_1_ILRuntimeMgr_Binding.Register(app);
            ILRuntimeMgr_Binding.Register(app);
            System_Int32_Binding.Register(app);
            System_Object_Binding.Register(app);
        }
    }
}
