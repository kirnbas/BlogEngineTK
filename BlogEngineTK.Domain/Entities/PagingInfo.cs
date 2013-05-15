using BlogEngineTK.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BlogEngineTK.Domain.Entities
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }

        public int ItemsPerPage { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages 
        {
            get { return (int)Math.Ceiling((double)TotalItems / ItemsPerPage); }
        }
    }
}