﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharedLibrary.Dtos;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity,TDto>where TEntity : class where TDto:class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync();
        Task <Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);
        Task<Response<TDto>> AddAsync(TDto entitiy);
        Task<Response<NoDataDto>> Remove(int id);
        Task<Response<NoDataDto>> Update(TDto entitiy, int id);
    }
}
