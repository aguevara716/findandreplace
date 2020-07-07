using System;
using GalaSoft.MvvmLight.Ioc;

namespace FindAndReplace.Wpf.Ioc
{
    /// <summary>
    /// Adapter/Singleton for the <see cref="SimpleIoc"/> container
    /// </summary>
    public static class IocWrapper
    {
        private static readonly SimpleIoc iocContainer;

        static IocWrapper()
        {
            iocContainer = SimpleIoc.Default;
        }

        // Reset
        public static void ResetIocContainer()
        {
            iocContainer.Reset();
        }

        // Register
        public static void Register<T>()
            where T : class
        {
            iocContainer.Register<T>();
        }

        public static void Register<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            iocContainer.Register<TInterface, TClass>();
        }

        public static void Register<T>(Func<T> factory)
            where T : class
        {
            iocContainer.Register(factory);
        }

        // Get
        public static T Get<T>()
        {
            return iocContainer.GetInstance<T>();
        }

        public static object Get(Type type)
        {
            return iocContainer.GetService(type);
        }

        public static TClass Get<TInterface, TClass>()
            where TInterface : class
            where TClass : class
        {
            var svc = Get(typeof(TInterface)) as TClass;
            if (svc == null)
                throw new InvalidCastException($"Unable to cast {nameof(TInterface)} to {nameof(TClass)}");
            return svc;
        }

        public static T GetWithoutCaching<T>()
        {
            return iocContainer.GetInstanceWithoutCaching<T>();
        }

        public static object GetWithoutCaching(Type type)
        {
            return iocContainer.GetInstanceWithoutCaching(type);
        }

        public static TClass GetWithoutCaching<TInterface, TClass>()
            where TInterface : class
            where TClass : class, TInterface
        {
            var svc = GetWithoutCaching(typeof(TInterface)) as TClass;
            if (svc == null)
                throw new InvalidCastException($"Unable to cast {nameof(TInterface)} to {nameof(TClass)}");
            return svc;
        }

    }

}