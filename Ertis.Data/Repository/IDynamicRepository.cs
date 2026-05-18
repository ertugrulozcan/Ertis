namespace Ertis.Data.Repository;

// ReSharper disable once UnusedType.Global
public interface IDynamicRepository<in TIdentifier> : IRepositoryBase<object, TIdentifier>;