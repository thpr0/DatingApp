using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public PagedList(List<T> Items,int count,int PageNumber,int PageSize)
        {   
            TotalCount=count;
            this.PageSize=PageSize;
            CurrentPage=PageNumber;
            TotalPages=(int)Math.Ceiling(count/(double)PageSize);
            this.AddRange(Items);
        }


        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int pageSize)
        {
            var count=await source.CountAsync();
            var items=await source.Skip((pageNumber-1)* pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items,count,pageNumber,pageSize);
        }
        
    }
}