using System;
using System.Collections.Generic;

namespace MNF
{
    internal class DispatchExportorCollection : Singleton<DispatchExportorCollection>
    {
        private Dictionary<Type, IDispatchHelper> messageDispatchExporters = null;

        public DispatchExportorCollection()
        {
            messageDispatchExporters = new Dictionary<Type, IDispatchHelper>();
        }

        public bool Add(Type type)
        {
            IDispatchHelper dispatchExporter = null;
            if (messageDispatchExporters.TryGetValue(type, out dispatchExporter) == true)
                return true;

            dispatchExporter = Utility.GetInstance(type) as IDispatchHelper;
            if (dispatchExporter == null)
                return false;

            messageDispatchExporters.Add(type, dispatchExporter);
            return true;
        }

        public IDispatchHelper Get(Type type)
        {
            IDispatchHelper messageDipatchExporter = null;
            if (messageDispatchExporters.TryGetValue(type, out messageDipatchExporter) == false)
            {
                if (Add(type) == false)
                    return null;
                messageDispatchExporters.TryGetValue(type, out messageDipatchExporter);
            }

            return messageDipatchExporter;
        }
    }
}
