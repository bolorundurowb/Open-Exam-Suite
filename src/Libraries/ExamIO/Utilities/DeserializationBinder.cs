/*
 *  Created by bolorundurowb on 2/1/2018
 */

using System.Reflection;
using System.Runtime.Serialization;
using OpenExamSuite.Shared;

namespace OpenExamSuite.Shared.Utilities;

public class DeserializationBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        var coreAssembly = typeof(Exam).Assembly.FullName;

        var type = Type.GetType(assemblyName.Contains("OpenExamSuite") ? $"{typeName}, {coreAssembly}" : $"{typeName}, {assemblyName}");

        return type;
    }
}