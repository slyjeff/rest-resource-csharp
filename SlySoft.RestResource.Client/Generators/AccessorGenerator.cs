﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SlySoft.RestResource.Client.Generators; 

internal abstract class AccessorGenerator {
    private static ModuleBuilder? _moduleBuilder;

    protected AccessorGenerator(Type interfaceType, Type accessorType) {
        InterfaceType = interfaceType;
    
        if (!interfaceType.IsInterface) {
            throw new CreateAccessorException($"Accessor must be based on an interface. {InterfaceType.Name} is not an interface");
        }

        try {
            var typeName = string.Concat(interfaceType.Name.AsSpan(1), accessorType.Name);
            TypeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Public, accessorType, new[] { interfaceType });
        } catch (Exception e) {
            throw new CreateAccessorException($"Error defining type based in on interface {InterfaceType.Name}", e);
        }
    }

    private static ModuleBuilder ModuleBuilder {
        get {
            if (_moduleBuilder != null) {
                return _moduleBuilder;
            }

            var assemblyName = new AssemblyName { Name = Assembly.GetAssembly(typeof(AccessorGenerator))!.FullName };
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(assemblyName.Name!);

            return _moduleBuilder;
        }
    }

    protected Type InterfaceType { get; }
    protected TypeBuilder TypeBuilder { get; }

    protected void AddProperty(PropertyInfo property) {
        var propertyBuilder = TypeBuilder.DefineProperty(property.Name, PropertyAttributes.None, property.PropertyType, null);
        
        if (property.CanRead) {
            propertyBuilder.SetGetMethod(CreateGetter(property));
        }

        if (property.CanWrite) {
            propertyBuilder.SetSetMethod(CreateSetter(property));
        }
    }

    private MethodBuilder CreateGetter(PropertyInfo property) {
        var methodBuilder = TypeBuilder.DefineMethod("get_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard | CallingConventions.HasThis, property.PropertyType, Type.EmptyTypes);

        if (TypeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {InterfaceType.Name}");
        }

        var getDataMethod = TypeBuilder.BaseType.GetMethod("GetData", BindingFlags.Instance | BindingFlags.NonPublic);
        if (getDataMethod == null) {
            throw new CreateAccessorException($"Could not find 'GetData' method when generating based in on interface {InterfaceType.Name}");
        }

        var typedGetDataMethod = getDataMethod.MakeGenericMethod(property.PropertyType);

        var codeGenerator = methodBuilder.GetILGenerator();

        codeGenerator.Emit(OpCodes.Ldarg_0);                       //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldstr, property.Name);          //push the name of the property onto the stack
        codeGenerator.Emit(OpCodes.Callvirt, typedGetDataMethod);  //call the the "GetData" method
        codeGenerator.Emit(OpCodes.Ret);                           //return
        
        return methodBuilder;
    }

    private MethodBuilder CreateSetter(PropertyInfo property) {
        var methodBuilder = TypeBuilder.DefineMethod("set_" + property.Name, MethodAttributes.Public | MethodAttributes.Virtual, CallingConventions.Standard | CallingConventions.HasThis, typeof(void), new[] { property.PropertyType });

        if (TypeBuilder.BaseType == null) {
            throw new CreateAccessorException($"BaseType of TypeBuilder is null when generating accessor based in on interface {InterfaceType.Name}");
        }

        var setDataMethod = TypeBuilder.BaseType.GetMethod("SetData", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setDataMethod == null) {
            throw new CreateAccessorException($"Could not find 'SetData' method when generating based in on interface {InterfaceType.Name}");
        }

        var typedSetDataMethod = setDataMethod.MakeGenericMethod(property.PropertyType);

        var codeGenerator = methodBuilder.GetILGenerator();

        codeGenerator.Emit(OpCodes.Ldarg_0);                       //push 'this' onto the stack
        codeGenerator.Emit(OpCodes.Ldstr, property.Name);          //push the name of the property onto the stack
        codeGenerator.Emit(OpCodes.Ldarg_1);                       //load the value of the parameter
        codeGenerator.Emit(OpCodes.Callvirt, typedSetDataMethod);  //call the the "SetData" method
        codeGenerator.Emit(OpCodes.Ret);                           //return

        return methodBuilder;
    }
}