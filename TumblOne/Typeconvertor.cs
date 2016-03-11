using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace TumblOne
{
    sealed class Typeconvertor : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            if (assemblyName ==
                "TumblOne, Version=1.0.4.0, Culture=neutral, PublicKeyToken=null")
            {
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }

            if (typeName ==
                "System.Collections.Generic.List`1[[TumblOne.Post, TumblOne, Version=1.0.4.0, Culture=neutral, PublicKeyToken=null]]")
            {
                typeName =
                    typeName.Replace("TumblOne, Version=1.0.4.0, Culture=neutral, PublicKeyToken=null", Assembly.GetExecutingAssembly().FullName);
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }

            if (assemblyName ==
                "TumblOne, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }

            if (typeName ==
                "System.Collections.Generic.List`1[[TumblOne.Post, TumblOne, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]")
            {
                typeName =
                    typeName.Replace("TumblOne, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", Assembly.GetExecutingAssembly().FullName);
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }
            return returntype;
       }
    }
}