using System.Collections.Generic;
using System.Linq;
using Assignment3.Interfaces;

namespace Assignment3;

public class CategoryService : ICategoryService
{
    private readonly List<Category> _categories =
    [
        new() { Id = 1, Name = "Beverages" },
        new() { Id = 2, Name = "Condiments" },
        new() { Id = 3, Name = "Confections" }
    ];

    public List<Category> GetCategories()
    {
        return _categories;
    }

    public Category? GetCategory(int cid)
    {
        return _categories.FirstOrDefault(c => c.Id == cid);
    }

    public bool UpdateCategory(int id, string newName)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return false;
        category.Name = newName;
        return true;
    }

    public bool DeleteCategory(int id)
    {
        var category = _categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return false;
        _categories.Remove(category);
        return true;
    }

    public bool CreateCategory(int id, string name)
    {
        if (_categories.Any(c => c.Id == id)) return false;
        _categories.Add(new Category { Id = id, Name = name });
        return true;
    }
}