using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    public class Category
    {
        public int CategoryID { private set; get; } = -1;

        public string CategoryName;
        public bool IsDeleted;

        public Category() { }
        public Category(int CategoryID)
        {
            DataTable dt = SQL.Execute("uspGetCategoryByID", 
                new Param("CategoryID", CategoryID));
            if (dt.Rows.Count > 0)
            {
                this.CategoryID = CategoryID;
                this.CategoryName = (string)dt.Rows[0]["CategoryName"];
                this.IsDeleted = (bool)dt.Rows[0]["CategoryName"];
            }
            else
            {
                throw new Exception("Invalid ID");
            }
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveCategory",
                new Param("CategoryName", CategoryName));

            if (dt.Rows.Count > 0)
            {
                CategoryID = (int)dt.Rows[0]["CategoryID"];
            }
            else
            {
                throw new Exception("Category Not Saved");
            }

        }

        public void SaveCategoryToBook(int BookID)
        {
            DataTable dt = SQL.Execute("uspSaveCategoryToBook",
                new Param("BookID", BookID),
                new Param("CategoryID", CategoryID));
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCategory", 
                new Param("CategoryID", CategoryID));
            if (dt.Rows.Count > 0)
            {
                IsDeleted = true;
            }
        }

        public void GetAllCategories()
        {
            DataTable dt = SQL.Execute("uspGetAllCategories");
        }

        public int GetCategoryIDByName()
        {
            DataTable dt = SQL.Execute("uspGetCategoryIDByName",
                new Param("CategoryName", CategoryName));

            if (dt.Rows.Count == 0)
            {
                SaveCategoryName();
            }
            else
            {
                CategoryID = (int)dt.Rows[0]["CategoryID"];

            }
            return CategoryID;
        }

        public void SaveCategoryName()
        {

            DataTable dt = SQL.Execute("uspSaveCategoryName",
            new Param("CategoryName", CategoryName));
            if (dt.Rows.Count > 0)
            {
                CategoryID = (int)dt.Rows[0]["CategoryID"];

            }
            else
            {
                throw new Exception("Book Not Created");
            }

        }
    }
}


      