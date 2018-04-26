using System;
using System.Data;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a book category
    /// </summary>
    public class Category
    {
        /// <summary>
        /// a unique id number identifying this category. -1 represents an uncreated category.
        /// </summary>
        public int CategoryID { private set; get; } = -1;
        /// <summary>
        /// The name of the category.
        /// </summary>
        public string CategoryName;
        /// <summary>
        /// Whether or not the category has been deleted.
        /// </summary>
        public bool IsDeleted;

        /// <summary>
        /// Instantiate an empty category with default values.
        /// </summary>
        public Category() { }

        /// <summary>
        /// Instantiate a category and set its fields equal to the category in the database with the given id.
        /// </summary>
        /// <param name="CategoryID"></param>
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

        /// <summary>
        /// Instantiate a category and set the fields equal to given parameters.
        /// </summary>
        /// <param name="CategoryID">The category's id</param>
        /// <param name="CategoryName">The name of the category</param>
        /// <param name="IsDeleted">Whether or not the category is deleted</param>
        private Category(int CategoryID, string CategoryName, bool IsDeleted)
        {
            this.CategoryID = CategoryID;
            this.CategoryName = CategoryName;
            this.IsDeleted = IsDeleted;
        }

        /// <summary>
        /// Save the category to the database.
        /// </summary>
        public void Save()
        {
            DataTable dt = SQL.Execute("uspSaveCategory",
                new Param("CategoryID", CategoryID),
                new Param("CategoryName", CategoryName),
                new Param("IsDeleted", IsDeleted)
                );

            if (dt.Rows.Count > 0)
                CategoryID = (int)dt.Rows[0]["CategoryID"];
        }

        public void DeleteCategoryFromBook(int BookID)
        {
            DataTable dt = SQL.Execute("uspDeleteCategoryFromBook",
                new Param("BookID", BookID),
                new Param("CategoryID", CategoryID)
                );
        }


        /// <summary>
        /// Delete the categories not linked to a book from the database.
        /// </summary>
        public void DumpCategories()
        {
            DataTable dt = SQL.Execute("uspDeleteEmptyAuthors");
        }

        /// <summary>
        /// Delete the category from the database.
        /// </summary>
        public void Delete()
        {
            DataTable dt = SQL.Execute("uspDeleteCategory", new Param("CategoryID", CategoryID));
            IsDeleted = true;
        }

        /// <summary>
        /// Add the category to the given book.
        /// </summary>
        /// <param name="BookID">The book's id</param>
        public void AddCategory(int BookID)
        {
            DataTable dt = SQL.Execute("uspBookAddCategory",
                new Param("CategoryID", CategoryID),
                new Param("BookID", BookID)
                );
        }

        /// <summary>
        /// Get a list of categories associated with a given book.
        /// </summary>
        /// <param name="BookID">The id of the book</param>
        /// <returns>A list of categories</returns>
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

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>A list of all categories</returns>
        public static List<Category> GetCategories()
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
