using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Tools.SQL;
using JaminBooks.Tools;

namespace JaminBooks.Model
{
    /// <summary>
    /// Models a home page banner
    /// </summary>
    public class Banner
    {
        /// <summary>
        /// The unique number that identifies this banner.
        /// </summary>
        public int BannerID { private set; get; } = -1;
        /// <summary>
        /// The url to which this banner's button links.
        /// </summary>
        public string URL;
        /// <summary>
        /// An integer reflecting the position of this banner.
        /// </summary>
        public int Order;
        /// <summary>
        /// A byte representation of this banner's image.
        /// </summary>
        public byte[] Image { set; private get; }

        /// <summary>
        /// Cache this banner's image if the image has not yet been cached. If the image has been cached, return the path to the image. 
        /// </summary>
        public string LoadImage
        {
            get
            {
                using (MemoryStream ms = new MemoryStream(Image))
                {
                    var filename = Authentication.Hash(Convert.ToBase64String(Image)) + ".png";
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "temp");
                    Directory.CreateDirectory(path);

                    path = Path.Combine(path, filename);

                    if (!File.Exists(path))
                    {
                        using (FileStream fs = new FileStream(path, FileMode.Create, System.IO.FileAccess.Write))
                        {
                            ms.CopyTo(fs);
                            fs.Flush();
                        }
                    }

                    return "/images/temp/" + filename;
                }
            }
        }

        /// <summary>
        /// Instantiates an empty Banner with all of the default values.
        /// </summary>
        public Banner() { }

        /// <summary>
        /// Instantiate a banner with the fields set to the give parameters.
        /// </summary>
        /// <param name="BannerID">The banner's id number</param>
        /// <param name="URL">The banner's link</param>
        /// <param name="Image">The banner's image</param>
        /// <param name="Order">The banner's position</param>
        private Banner(int BannerID, string URL, byte[] Image, int Order)
        {
            this.BannerID = BannerID;
            this.URL = URL;
            this.Image = Image;
            this.Order = Order;
        }

        /// <summary>
        /// Save the banner to the database and update its id
        /// </summary>
        public void Save()
        {
            DataTable results = SQL.Execute("uspAddBanner",
                                        new Param("Image", Image),
                                        new Param("URL", URL ?? SqlString.Null),
                                        new Param("Order", Order));
            if (results.Rows.Count > 0)
                BannerID = (int)results.Rows[0]["BannerID"];
        }

        /// <summary>
        /// Delete the given banner from the database.
        /// </summary>
        /// <param name="BannerID">The banner's id number</param>
        public static void Delete(int BannerID)
        {
            SQL.Execute("uspRemoveBanner", new Param("BannerID", BannerID));
        }

        /// <summary>
        /// Set the position of the given banner.
        /// </summary>
        /// <param name="BannerID">The banner's id number</param>
        /// <param name="Order">The new position of the banner.</param>
        public static void SetOrder(int BannerID, int Order)
        {
            SQL.Execute("uspOrderBanner", new Param("BannerID", BannerID), new Param("Order", Order));
        }

        /// <summary>
        /// Get a list of all banners.
        /// </summary>
        /// <returns>A list of all banners.</returns>
        public static List<Banner> GetBanners()
        {
            DataTable result = SQL.Execute("uspGetBanners");
            List<Banner> banners = new List<Banner>();
            foreach (DataRow dr in result.Rows)
            {
                banners.Add(new Banner(
                    (int)dr["BannerID"],
                    dr["URL"] == DBNull.Value ? null : dr["URL"].ToString(),
                    (byte[])dr["Image"],
                    (int)dr["Order"]
                ));
            }
            return banners;
        }
    }
}
