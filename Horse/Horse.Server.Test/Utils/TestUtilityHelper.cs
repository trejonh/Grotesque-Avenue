using System;
using System.Reflection;

namespace Horse.Server.Test.Utils
{
    /// <summary>
    /// An utility class made to help grab non-public methods and fields using reflection
    /// </summary>
    public static class TestUtilityHelper
    {
        /// <summary>
        /// Calls an objects private method
        /// </summary>
        /// <param name="type">Object type</param>
        /// <param name="methodName">Method to call</param>
        /// <param name="invoker">The call to call it on</param>
        /// <param name="args">Arguments to pass into method</param>
        /// <returns>null is the method does not return</returns>
        public static object CallPrivateMethod(Type type, string methodName,object invoker,object[] args)
        {
            var method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic );
            if (method == null)
                throw new ArgumentException("No method " +methodName+" found for "+type);
            return method.Invoke(invoker,args);
        }

        /// <summary>
        /// Get a field from an object
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="invoker">The object to call it on</param>
        /// <returns>The field value</returns>
        public static object GetField(Type type, string fieldName, object invoker)
        {
            return type.InvokeMember(fieldName,
            BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static|
            BindingFlags.Instance | BindingFlags.GetField, null, invoker, null);
        }

        /// <summary>
        /// Get a field from an object
        /// </summary>
        /// <param name="type">The object type</param>
        /// <param name="fieldName">The name of the field</param>
        /// <param name="invoker">The object to call it on</param>
        /// <param name="value">Thee value to set the field</param>
        /// <returns>The field value</returns>
        public static object SetField(Type type, string fieldName, object invoker, object[] value)
        {
            return type.InvokeMember(fieldName,
            BindingFlags.DeclaredOnly |
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
            BindingFlags.Instance | BindingFlags.SetField, null, invoker, value);
        }
    }
}
