﻿using KD.Dova.Proxy.Natives;
using System;

namespace KD.Dova
{
    /// <summary>
    /// Represents Java Environment.
    /// </summary>
    public class JavaEnvironment
    {
        public JNIEnvironment JNIEnv { get; }

        public JavaEnvironment(IntPtr ptr)
        {
            this.JNIEnv = new JNIEnvironment(ptr);
        }

        public int GetVersion()
        {
            IntPtr ret = this.JNIEnv.GetVersion(this.JNIEnv.Environment);
            int version = ret.ToInt32();
            return version;
        }

        public int GetMajorVersion()
        {
            return GetVersion() >> 16;
        }

        public int GetMinorVersion()
        {
            return GetVersion() % 65536;
        }

        public JavaVM NewJavaVM()
        {
            IntPtr jvm;
            this.JNIEnv.GetJavaVM(this.JNIEnv.Environment, out jvm);
            return new JavaVM(jvm);
        }
    }
}
