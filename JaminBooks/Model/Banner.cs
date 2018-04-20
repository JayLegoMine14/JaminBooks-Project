using JaminBooks.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static JaminBooks.Model.SQL;

namespace JaminBooks.Model
{
    public class Banner
    {
        public int BannerID { private set; get; } = -1;
        public string URL;
        public int Order;
        public byte[] Image { set; private get; }

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

        public Banner() { }
        private Banner(int BannerID, string URL, byte[] Image, int Order) {
            this.BannerID = BannerID;
            this.URL = URL;
            this.Image = Image;
            this.Order = Order;
        }

        public void Save()
        {
            DataTable results = SQL.Execute("uspAddBanner", 
                                        new Param("Image", Image),
                                        new Param("URL", URL ?? SqlString.Null),
                                        new Param("Order", Order));
            if (results.Rows.Count > 0)
                BannerID = (int)results.Rows[0]["BannerID"];
        }

        public static void Delete(int BannerID)
        {
            SQL.Execute("uspRemoveBanner", new Param("BannerID", BannerID));
        }

        public static void SetOrder(int BannerID, int Order)
        {
            SQL.Execute("uspOrderBanner", new Param("BannerID", BannerID), new Param("Order", Order));
        }

        public static List<Banner> GetBanners()
        {
            DataTable result = SQL.Execute("uspGetBanners");
            List<Banner> banners = new List<Banner>();
            foreach(DataRow dr in result.Rows)
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
