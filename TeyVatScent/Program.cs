using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeyVatScent.Model;

namespace TeyVatScent
{
    internal class Program
    {
        static string Link = "https://nuochoarosa.com/cua-hang/";
        static DataContext Context = new DataContext();
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var web = new HtmlWeb();
            var doc = web.Load(Link);
            
            var productCategory = doc.DocumentNode.SelectSingleNode("//ul[@class='product-categories']");

            //GetCategory(productCategory);
            GetProduct("https://nuochoarosa.com/cua-hang/");



        }

        static void GetCategory(HtmlNode CategoryNode)
        {
            foreach (var item in CategoryNode.ChildNodes)
            {
                var urlCategory = item.FirstChild.Attributes["href"].Value;
                var categoryName = item.FirstChild.InnerText;
                //Console.WriteLine(categoryName);
                Console.WriteLine($"====================={categoryName}======================");
                Thread.Sleep(1000);
                GetProduct(urlCategory);

            }
        }
        //static void GetProductPages(string url)
        //{
        //    HtmlWeb web = new HtmlWeb();
        //    var doc = web.Load(url);
        //    var page = doc.DocumentNode.SelectSingleNode("//ul[@class='page-numbers']");
        //    var pages = 1;
        //    if(page != null)
        //    {
        //        var lastPage = page.ChildNodes[page.ChildNodes.Count - 2].InnerText;
        //        pages = int.Parse(lastPage);
                
        //    }
        //    for (int i = 1; i <= pages; i++)
        //    {
        //        if (i>1)
        //        {
        //            url = url + $"/page/{i}";
        //        }
        //        GetProduct(url);
        //        Console.WriteLine($"------------------page{i}----------------");
        //    }

           
        //}
        static void GetProduct(string url)
        {
            var web = new HtmlWeb();
            
            for (int i = 1; i <= 3; i++)
            {
                var Categories = new Category()
                {
                    CategoryName = "Danh mục nước hoa " + i,
                    Isdelete = false

                };
                Context.Categories.Add(Categories);
                Context.SaveChanges();
                if (i >= 2)
                {
                    url += $"page/{i}/";
                    
                }
                    
                var doc = web.Load(url);
                var products = doc.DocumentNode.SelectNodes("//h3[@class='wd-entities-title']");
                foreach (var product in products)
                {
                    var urlProduct = product.FirstChild.Attributes["href"].Value;

                    Console.WriteLine(urlProduct);

                    Console.WriteLine("++++++++++++++++++++++++++++++++++++++++");
                    GetProductInfo(urlProduct,Categories.IDCategory);


                }
            }
        }
        static void GetProductInfo(string url, int CategoryId)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url).DocumentNode;
            var name = doc.SelectSingleNode("//h1[@class='product_title entry-title wd-entities-title']").InnerHtml;
            var attr = doc.SelectSingleNode("//div[@class='woocommerce-product-details__short-description']").InnerText ?? string.Empty;
            var description = doc.SelectSingleNode("//div[@id='tab-description']").InnerText;
            var img = doc.SelectSingleNode("//figure[@class='woocommerce-product-gallery__image']").FirstChild.Attributes["href"].Value;
            Console.WriteLine(name);
            Console.WriteLine(description);
            Console.WriteLine(attr);
            Console.WriteLine(img);
            var imgUrl = SaveImage(img);
            var product = new Product()
            {
                Name = name,
                Description = description,
                IDCategory = CategoryId,
                ImageURL = imgUrl,
                Price = 100000,
                IsDelete = false,
                Stock = 100,
                ShortDescription = attr
            };
            Context.Products.Add(product);
            Context.SaveChanges();
            Console.WriteLine($"********************************{name}********************************");
            

        }
        static string SaveImage(string imgUrl)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(imgUrl))
            {
                using (WebClient client = new WebClient())
                {
                    path = Path.GetFileName(imgUrl);
                    client.DownloadFile(imgUrl, "../../Image/" + path);
                }
            }
            return path;
        }
    }
}
