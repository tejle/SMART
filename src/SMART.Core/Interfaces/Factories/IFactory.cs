using System;

namespace SMART.Core.Interfaces.Factories
{
    public interface IFactory<T> {
        T Create(Type type);
    }
}