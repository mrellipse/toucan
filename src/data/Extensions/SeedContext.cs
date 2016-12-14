using System;
using System.Linq;
using System.Collections.Generic;
using Toucan.Data.Model;

namespace Toucan.Data
{
    public static partial class Extensions
    {
        public static void EnsureSeedData(this ToucanContext context)
        {

            if (!context.Blogs.Any())
            {
                var blog = new Blog()
                {
                    Name = "Toucan Sam",
                    Posts = new List<Post>()
                    {
                        new Post{ Title = "Breakfast Cereals", Content = "Fruit loops is a good choice most mornings for breakfast." }
                    }
                };

                context.Blogs.Add(blog);
                context.SaveChanges();
            }
        }
    }
}