using Ertis.Data.Models;

namespace Ertis.Data.Repository;

// ReSharper disable once UnusedType.Global
public interface IRepository<TEntity, in TIdentifier> : IRepositoryBase<TEntity, TIdentifier> where TEntity : IEntity<TIdentifier> where TIdentifier : notnull;