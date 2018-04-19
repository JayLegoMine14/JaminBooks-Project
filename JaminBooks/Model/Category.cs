using System;
using System.Data;
using static JaminBooks.Model.SQL;
using System.Data.SqlTypes;
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
            DataTable dt = SQL.Execute("uspGetCategoryByID", new Param("CategoryID", CategoryID));

            if (dt.Rows.Count > 0)
            {
                this.CategoryID = CategoryID;
                this.CategoryName = (String)dt.Rows[0]["CategoryName"];
                this.IsDeleted = (bool)dt.Rows[0]["IsDeleted"];
            }
            else
            {
                throw new Exception("Invalid Category ID");
            }
        }

        private Category(int CategoryID, string CategoryName, bool IsDeleted)
        {
            this.CategoryID = CategoryID;
            this.CategoryName = CategoryName;
            this.IsDeleted = IsDeleted;
        }

        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveCategory",
                new Param("CategoryID", CategoryID),
                new Param("CategoryName", CategoryName),
                new Param("IsDeleted", IsDeleted)
                );
        }

        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCategory", new Param("CategoryID", CategoryID));
            IsDeleted = true;
        }

        public void AddCategory(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddCategory",
                new Param("CategoryID", CategoryID),
                new Param("BookID", BookID)
                );
        }

        public int getBookID()
        {
            DataTable dt = SQL.Execute("uspGetCategoryByID",
                new Param("CategoryID", CategoryID));
            return (int)dt.Rows[0]["BookID"];
        }

        public static List<Category> GetCategories(int BookID)
        {
            DataTable dt = SQL.Execute("uspGetCategoriesByBookID", new Param("BookID", BookID));
            List<Category> categories = new List<Category>();
            foreach (DataRow dr in dt.Rows)
                categories.Add(new Category(
                    (int)dr["CategoryID"],
                    (String)dr["CategoryName"],
                    (bool)dr["IsDeleted"]
                    ));
            return categories;


        }

        public static List<Category> GetAllCategories()
        {
            DataTable dt = SQL.Execute("uspGetCategories");
            List<Category> categories = new List<Category>();
            foreach (DataRow dr in dt.Rows)
                categories.Add(new Category(
                    (int)dr["CategoryID"],
                    (String)dr["CategoryName"],
                    (bool)dr["IsDeleted"]
                    ));
            return categories;
        }


    }
}
