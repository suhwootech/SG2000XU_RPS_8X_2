using System;
using System.Reflection;

/// <summary>
/// Single tone
/// </summary>
/// <typeparam name="T"></typeparam>
public class CStn<T> where T : class
{
    private static volatile T m_Instance = null;
    private static readonly object m_SyncRoot = new object();

    /// <summary>
    /// Instance 
    /// </summary>
    public static T It
    {
        get
        {
            if (m_Instance == null)
                _CreateInstance();

            return m_Instance;
        }
    }

    private static void _CreateInstance()
    {
        lock (m_SyncRoot)
        {
            if (m_Instance == null)
            {
                Type mType = typeof(T);

                ConstructorInfo[] ctors = mType.GetConstructors();
                if (ctors.Length > 0)
                {
                    throw new InvalidOperationException(string.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", mType.Name));
                }

                m_Instance = (T)Activator.CreateInstance(mType, true);

            }
        }
    }
}

