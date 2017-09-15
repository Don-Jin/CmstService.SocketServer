using System;

namespace CmstService.SocketServer.ConfigurationHelper
{
    public interface ICommonMethod
    {
        void BeforeOpen();

        void BeforeSave();
    }

    public interface ICommonMethod<T> : ICommonMethod
    {
        T GetObject();
    }
}
