/*
 *  Created by bolorundurowb on 2/1/2018
 */

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Shared.Util
{
    public class DeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            var assembly = Assembly.GetExecutingAssembly().FullName;

            var type = Type.GetType(assemblyName.Contains("Shared") ? $"{typeName}, {assembly}" : $"{typeName}, {assemblyName}");

            return type;
        }
    }
}