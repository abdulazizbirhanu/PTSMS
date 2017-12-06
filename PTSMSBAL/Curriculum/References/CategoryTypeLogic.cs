using PTSMSDAL.Access.Curriculum.References;
using PTSMSDAL.Models.Curriculum.References;
using System.Collections.Generic;

namespace PTSMSBAL.Curriculum.References
{
    public class CategoryTypeLogic
    {
        CategoryTypeAccess categoryTypeAccess = new CategoryTypeAccess();

        public List<CategoryType> List()
        {
            return categoryTypeAccess.List();
        }
        public List<CategoryType> CategoryTypeList()
        {
            return categoryTypeAccess.CategoryTypeList();
        }

        public CategoryType Details(int id)
        {
            return categoryTypeAccess.Details(id);
        }

        public bool Add(CategoryType categoryType)
        {
            categoryType.Status = "Active";
            categoryType.RevisionNo = 1;
            return categoryTypeAccess.Add(categoryType);
        }

        public bool Revise(CategoryType categoryType)
        {
            CategoryType catt = (CategoryType)categoryTypeAccess.Details(categoryType.CategoryTypeId);
            catt.Type = categoryType.Type;
            catt.Description = categoryType.Description;
            return categoryTypeAccess.Revise(catt);
        }

        public bool Delete(int id)
        {
            return categoryTypeAccess.Delete(id);
        }
    }
}