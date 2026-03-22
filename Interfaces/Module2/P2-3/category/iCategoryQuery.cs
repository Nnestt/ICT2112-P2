using System.Collections.Generic;
using ProRental.Domain.Entities;

namespace ProRental.Interfaces.Module2
{
    public interface ICategoryQuery
    {
        Category GetCategoryById(int categoryId);
        List<Category> GetAllCategories();
        List<Category> GetActiveCategories();
    }
}