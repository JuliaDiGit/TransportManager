using System;
using System.Collections.Generic;
using System.Linq;
using TransportManager.DataXML.Serializer;
using TransportManager.Entities.Abstract;

namespace TransportManager.DataXML.Repositories.Abstract
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly XmlSet<TEntity> _serializer;

        protected BaseRepository(string path)
        {
            _serializer = new XmlSet<TEntity>(path);
        }

        public TEntity AddOrUpdate(TEntity entity)
        {
            List<TEntity> listTDto = _serializer.Data;
            
            List<IBaseEntity> listIDto = _serializer.Data.Select(obj => (IBaseEntity)obj)
                                                         .ToList();

            IBaseEntity entityI = entity as IBaseEntity;

            if (Get(entityI.Id) == null)
            {
                entityI.Id = listIDto.Count == 0 ? 1 : listIDto.Max(obj => obj.Id) + 1;
                entityI.CreatedDate = DateTime.Now;
                entity = entityI as TEntity;
                listTDto.Add(entity);
                _serializer.Data = listTDto;

                return entity;
            }

            listIDto.Remove(listIDto.First(obj => obj.Id == entityI.Id));
            listIDto.Add(entityI);

            _serializer.Data = listIDto.OrderBy(obj => obj.Id)
                                       .Select(obj => obj as TEntity)
                                       .ToList();

            return entity;
        }

        public TEntity Get(int id)
        { 
            return _serializer.Data.FirstOrDefault(obj => ((IBaseEntity) obj).Id == id);
        }

        public TEntity Delete(int id)
        {
            try
            {
                List<TEntity> listTDto = _serializer.Data;
                
                TEntity dto = listTDto.First(obj => ((IBaseEntity) obj).Id == id);
                listTDto.Remove(dto);
                _serializer.Data = listTDto;

                return dto;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public List<TEntity> GetAll()
        {
            return _serializer.Data;
        }
    }
}