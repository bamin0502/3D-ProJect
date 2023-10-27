using System;
using System.Collections.Generic;

namespace MNF
{
    internal class DispatchExporterCollection : Singleton<DispatchExporterCollection>
    {
        private readonly Dictionary<Type, IDispatchHelper> messageDispatchExporters = null;

        public DispatchExporterCollection()
        {
            messageDispatchExporters = new Dictionary<Type, IDispatchHelper>();
        }

        public bool Add(Type type)
        {
            if (messageDispatchExporters.TryGetValue(type, out var dispatchExporter) == true)
                return true;

            dispatchExporter = Utility.GetInstance(type) as IDispatchHelper;
            if (dispatchExporter == null)
                return false;

            messageDispatchExporters.Add(type, dispatchExporter);
            return true;
        }

        public IDispatchHelper Get(Type type)
        {
            if (messageDispatchExporters.TryGetValue(type, out var messageDipatchExporter) == false)
            {
                if (Add(type) == false)
                    return null;
                messageDispatchExporters.TryGetValue(type, out messageDipatchExporter);
            }

            return messageDipatchExporter;
        }
    }
}
