using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtil
{
    public static Type[] FindAllSubslasses<T>()
    {
        Type baseType = typeof(T);
        Assembly assembly = Assembly.GetAssembly(baseType);

        Type[] types = assembly.GetTypes();
        Type[] subclasses = types.Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract).ToArray();

        return subclasses;
    }

    public static Type[] FindAllImplementations<T>()
    {
        Type interfaceType = typeof(T);
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var implementations = new List<Type>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes();
                implementations.AddRange(types.Where(type =>
                        !type.IsAbstract &&          // Исключаем абстрактные классы
                        interfaceType.IsAssignableFrom(type) // Проверка реализации интерфейса
                ));
            }
            catch (ReflectionTypeLoadException)
            {
                Debug.Log("ErorRegister");
                continue; // Игнорируем ошибки загрузки
            }
        }

        return implementations.ToArray();
    }
}