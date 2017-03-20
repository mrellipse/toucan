using System;
using System.Collections.Generic;
using System.Linq;
using Toucan.Contract;

namespace Toucan.Service.Model
{
    public class SearchResult<TOut> : ISearchResult<TOut>
    {
        private readonly Func<object, TOut> mapper;
        private readonly IQueryable<object> query;
        private IEnumerable<TOut> results = null;
        public SearchResult(IQueryable<object> query, Func<object, TOut> mapper, int page, int pageSize)
        {
            this.mapper = mapper;
            this.Page = page;
            this.PageSize = pageSize;
            this.query = query;
        }

        public IEnumerable<TOut> Items
        {
            get
            {
                if (results == null)
                {
                    int page = this.Page < 1 ? 1 : this.Page;
                    int pageSize = this.PageSize < 1 ? 1 : this.PageSize;
                    var q = this.query;

                    if(page > 1)
                        q = q.Skip((page -1) * pageSize);
                    
                    results = q.Take(pageSize).ToList().Select(o => mapper(o));
                }

                return results;
            }
        }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public long Total
        {
            get
            {
                return this.query.Count();
            }
        }
    }
}
