using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BakeryMVCCore.Models
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BakeryDBContext _ctx;

        public CategoryRepository(BakeryDBContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Category> AllCategories => _ctx.Categories;
    }
}
