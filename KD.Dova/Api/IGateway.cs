﻿using KD.Dova.Core;
using System;

namespace KD.Dova.API
{
    /// <summary>
    /// Gateway which is responsible for calling JNI methods.
    /// </summary>
    internal interface IGateway
    {
        /// <summary>
        /// Currently used Java Runtime.
        /// </summary>
        JavaRuntime Runtime { get; }

        /// <summary>
        /// Returns specified Java archive.
        /// In the most cases it will return the JAR file.
        /// </summary>
        /// <param name="pathOrName"></param>
        /// <returns></returns>
        JArchive GetArchive(string pathOrName);
        /// <summary>
        /// Loads specified class.
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        JType LoadClass(string className, params object[] parameters);
        /// <summary>
        /// Returns new Java object by it's name, using the constructor with specified parameters.
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        JObject New(string typeName, params object[] parameters);
        /// <summary>
        /// Invokes method by it's name.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T InvokeMethod<T>(IntPtr objectId, string methodName, params object[] parameters);
        /// <summary>
        /// Invokes static method from specified class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        T InvokeStaticMethod<T>(string className, string methodName, params object[] parameters);
        /// <summary>
        /// Returns value of specified field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        T GetFieldValue<T>(IntPtr objectId, string fieldName);
        /// <summary>
        /// Returns value of the static field.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        T GetStaticFieldValue<T>(string className, string fieldName);
        /// <summary>
        /// Returns reference to specified field.
        /// This should be called when the field is an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        JObject GetFieldRef(IntPtr objectId, string fieldName);
        /// <summary>
        /// Sets the new value of the field.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        void SetFieldValue(IntPtr objectId, string fieldName, object newValue);
        /// <summary>
        /// Sets the new value of specified static field.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="fieldName"></param>
        /// <param name="newValue"></param>
        void SetStaticFieldValue(string className, string fieldName, object newValue);
    }
}
