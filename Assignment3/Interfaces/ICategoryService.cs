using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment3.Interfaces
{
    internal interface ICategoryService
    {
        public List<Category> GetCategories();
        public Category? GetCategory(int cid);
        public bool UpdateCategory(int id, string newName);
        public bool DeleteCategory(int id);
        public bool CreateCategory(int id, string name);
    }
}
