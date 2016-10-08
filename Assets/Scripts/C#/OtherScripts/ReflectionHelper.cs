using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEditor;
using System.IO;


class ReflectionHelper
{
    #region fields for hack
    public static Assembly[] assemblies = null;
    public static Assembly unityEditorAssembly = null;
    public static Assembly unityEngineAssembly = null;
    public static bool init = false; 
    #endregion


    #region functions
    static ReflectionHelper()
    {
    }

    public static void Init()
    {
        if (init)
            return;
        init = true;
        assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; ++i)
        {
            if (assemblies[i].GetName().Name == "UnityEditor")
                unityEditorAssembly = assemblies[i];
            if (assemblies[i].GetName().Name == "UnityEngine")
                unityEngineAssembly = assemblies[i];
            if (unityEngineAssembly != null && unityEditorAssembly != null)
                break;
        }
    }

    public static Assembly GetAssembly(string asmName)
    {
        Init();
        for (int i = 0; i < assemblies.Length; ++i)
        {
            if (assemblies[i].GetName().Name == asmName)
                return assemblies[i];
        }
        return null;
    }

    public static Assembly GetExecutingAssembly()
    {
        Assembly asm = Assembly.GetExecutingAssembly();
        return asm;
    }

    public static Assembly GetEntryAssembly()
    {
        Assembly asm = Assembly.GetEntryAssembly();
        return asm;
    }

    public static Type GetTypeByName(string clsName)
    {
        System.Type type = System.Type.GetType(clsName);
        return type;
    }

    public static Type GetTypeByName(Assembly asm, string clsName)
    {
        System.Type type = asm.GetType(clsName);
        return type;
    }

    public static Type GetTypeByName(string asmName, string clsName)
    {
        Assembly asm = GetAssembly(asmName);
        if (asm != null)
            return GetTypeByName(asm, clsName);
        return null;
    }

    // Create an instance of type, using default constructor
    public static object CreateInstance(Type type)
    {
        object obj = Activator.CreateInstance(type);
        return obj;
    }

    // Search the type from specified assembly and create an instance, using default constructor
    public static object CreateInstance(string assemblyName, string typeName)
    {
        ObjectHandle handler = Activator.CreateInstance(assemblyName, typeName);
        return handler.Unwrap();
    }

    // Create an instance of type in assembly, using explicit constructor
    public static object CreateInstance(Assembly asm, Type type, object[] parameters)
    {
        object inst = asm.CreateInstance(type.ToString(), true, BindingFlags.Default, null, parameters, null, null);
        return inst;
    }

    // Create an instance of type in assembly, using explicit constructor
    public static object CreateInstance(Assembly asm, string type, object[] parameters)
    {
        object inst = asm.CreateInstance(type, true, BindingFlags.Default, null, parameters, null, null);
        return inst;
    }

    // Create an instance of type in assembly, using explicit constructor
    public static object CreateInstance(string assemblyName, string typeName, object[] parameters)
    {
        Assembly asm = GetAssembly(assemblyName);
        if (asm != null)
            return CreateInstance(asm, typeName, parameters);
        return null;
    }

    // Create an instance array of type
    public static object CreateInstanceArray(Type type, int length)
    {
        Array ar = Array.CreateInstance(type, length);
        return ar;
    }

    // Create an instance array of type in assembly
    public static object CreateInstanceArray(string assemblyName, string typeName, int length)
    {
        Type type = GetTypeByName(assemblyName, typeName);
        return CreateInstanceArray(type, length);
    }

    // Create an instance array of type in assembly
    public static object CreateInstanceArray(Assembly asm, string typeName, int length)
    {
        Type type = GetTypeByName(asm, typeName);
        return CreateInstanceArray(type, length);
    }

    public static Type GetDelegateType(Type delegateObjType, string delegateName)
    {
        Type type = delegateObjType.GetNestedType(delegateName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        return type;
    }

    public static Delegate CreateDelegateInstance(Type delegateType, Type objType, string methodName)
    {
        MethodInfo mi = objType.GetMethod(methodName);
        Delegate del = Delegate.CreateDelegate(delegateType, mi);
        return del;
    }

    public static Delegate CreateDelegateInstance(Type delegateObjType, string delegateName, Type objType, string methodName)
    {
        Type delegateType = GetDelegateType(delegateObjType, delegateName);
        Delegate del = CreateDelegateInstance(delegateType, objType, methodName);
        return del;
    }

    public static object InvokMethod(object obj, string methodName, object[] parameters)
    {
        Type objType = obj.GetType();
        object ret = objType.InvokeMember(
            methodName,
            BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod | BindingFlags.Public |
            BindingFlags.Static,
            null,
            obj,
            parameters);
        return ret;
    }

    public static object InvokMethod(object obj, string methodName, object[] parameters, bool incParentMethod)
    {
        BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
        if (!incParentMethod)
            bf |= BindingFlags.DeclaredOnly;
        Type objType = obj.GetType();
        object ret = objType.InvokeMember(methodName, bf, null, obj, parameters);
        return ret;
    }

    public static object InvokMethod(object obj, string methodName, /*Types[]*/object paramTypes, object[] paramValues)
    {
        MethodInfo methodInfo = obj.GetType().GetMethod(methodName,BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,null,(Type[])paramTypes,null);
        object ret = methodInfo.Invoke(obj, paramValues);
        return ret;
    }

    public static object InvokMethodWithRefParam<T>(object obj, string methodName, object[] parameters, int refIndex, ref T refValue)
    {
        object ret = InvokMethod(obj, methodName, parameters);
        refValue = (T)parameters[refIndex];
        return ret;
    }

    public static object InvokStaticMethodWithRefParam<T>(Type type, string methodName, object[] parameters, int refIndex, ref T refValue)
    {
        object ret = InvokStaticMethod(type, methodName, parameters);
        refValue = (T)parameters[refIndex];
        return ret;
    }

    public static object InvokStaticMethod(Type type, string methodName, object[] parameters)
    {
        object ret = type.InvokeMember(
            methodName,
            BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod | BindingFlags.Public |
            BindingFlags.Static,
            null,
            type,
            parameters);
        return ret;
    }

    public static object InvokStaticMethodWithParams(Type type, string methodName, object[] parameters)
    {
        object ret = null;
        MethodInfo mi = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
        if (mi != null)
        {
            ret = mi.Invoke(null, parameters);
        }
        return ret;
    }

    public static object InvokStaticMethodWithParams(Type type, string methodName, Type[] types, object[] parameters)
    {
        object ret = null;
        MethodInfo mi = type.GetMethod(
            methodName, 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static,
            null,
            types,
            null);
        if (mi != null)
        {
            ret = mi.Invoke(null, parameters);
        }
        return ret;
    }

    // Get instance property by property name
    public static object GetProperty(object obj, string propertyName)
    {
        object ret = obj.GetType().InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, obj, new object[] { });
        return ret;
    }

    // Set instance property by property name
    public static void SetProperty(object obj, string propertyName, object parameter)
    {
        obj.GetType().InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty, null, obj, new object[] { parameter });
    }
        
    // Get class static property by property name
    public static object GetStaticProperty(Type type, string propertyName)
    {
        object ret = type.InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, new object[] { });
        return ret;
    }

    // Set class static property by property name
    public static void SetStaticProperty(Type type, string propertyName, object parameter)
    {
        type.InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetField | BindingFlags.SetProperty, null, type, new object[] { parameter });
    }

    // Get the element of an array property of an instance
    public static object GetArrayElemProperty(object obj, string propertyName, int index)
    {
        object ret = obj.GetType().InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, obj, new object[] { });
        if (ret != null)
        {
            Array arProp = (Array)ret;
            if (arProp != null && index >= 0 && index < arProp.Length)
                return arProp.GetValue(index);
        }
        return null;
    }

    // Set the element of an array property of an instance
    public static void SetArrayElemProperty(object obj, string propertyName, object parameter, int index)
    {
        object ret = obj.GetType().InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, obj, new object[] { });
        if (ret != null)
        {
            Array arProp = (Array)ret;
            if (arProp != null && index >= 0 && index < arProp.Length)
                arProp.SetValue(parameter, index);
        }
    }

    // Get the element of a static array property of the class
    public static object GetStaticArrayElemProperty(Type type, string propertyName, int index)
    {
        object ret = type.InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, new object[] { });
        if (ret != null)
        {
            Array arProp = (Array)ret;
            if (arProp != null && index >= 0 && index < arProp.Length)
                return arProp.GetValue(index);
        }
        return null;
    }

    // Set the element of a static array property of the class
    public static void SetStaticArrayElemProperty(Type type, string propertyName, object parameter, int index)
    {
        object ret = type.InvokeMember(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField | BindingFlags.GetProperty, null, type, new object[] { });
        if (ret != null)
        {
            Array arProp = (Array)ret;
            if (arProp != null && index >= 0 && index < arProp.Length)
                arProp.SetValue(parameter, index);
        }  
    }

    #endregion
}
