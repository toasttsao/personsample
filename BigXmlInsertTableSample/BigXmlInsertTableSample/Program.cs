using BigXmlInsertTableSample.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace BigXmlInsertTableSample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int TotalInsrtData = 100000;

            string XMLData = FakeData(TotalInsrtData);

            UsingXMLtoSp(XMLData);

           // OldInsert(XMLData, TotalInsrtData);

            Console.Read();
        }

        /// <summary>直接透過EF方式逐筆新增資料 </summary>
        /// <param name="XMLData"></param>
        /// <param name="InsertTotalCnt"></param>
        private static void OldInsert(string XMLData, int InsertTotalCnt)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(bookstory));

            using (StringReader rdr = new StringReader(XMLData))
            {
                //將XML資料做反序列化
                bookstory resultingMessage = (bookstory)serializer.Deserialize(rdr);

                List<book_story> booksList = resultingMessage.books.Select(s => new book_story
                {
                    Amount = s.Amount.ToString(),
                    BookId = s.BookId,
                    BookName = s.BookName,
                    CreateDate = Convert.ToDateTime(s.CreateDate)
                }).ToList();


                try
                {
                    Console.WriteLine(string.Format("原有方法(新增資料)-開始時間{0}!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    using (SampledataEntities db = new SampledataEntities())
                    {
                        //逐一比對物件裡面是否有相同資料,沒有則新增,有則更新
                        Action<book_story> RYH = (s) =>
                        {
                            if (db.book_story.Any(a => a.BookId == s.BookId))
                            {
                                var book_storyCollect = db.book_story.Where(f => f.BookId == s.BookId).ToList();
                                book_storyCollect.ForEach(a =>
                                {
                                    a.BookName = s.BookName;
                                    a.Amount = s.Amount;
                                });
                            }
                            else
                            {
                                db.book_story.Add(s);
                            }
                        };
                        booksList.ForEach(RYH);
                        db.SaveChanges();
                        Console.WriteLine(string.Format("原有方法(新增資料)-完成時間{0}!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
                    }
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    throw;
                }
            };
        }

        /// <summary> 透過SP大量新增資料</summary>
        /// <param name="XMLData">接收API回來XML資料</param>
        private static void UsingXMLtoSp(string XMLData)
        {
            int InsertTotalCnt;
            Console.WriteLine(string.Format("XML_to_sp開始時間:{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            try
            {
                using (SampledataEntities db = new SampledataEntities())
                {
                    //直接將XML放入SP中處理
                    ObjectResult<SP_Storybook_Result> CallInsertSp = db.SP_Storybook(XMLData);
                    InsertTotalCnt = CallInsertSp.FirstOrDefault().cnt.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex.GetBaseException();
            }

            Console.WriteLine(string.Format("XML_to_sp完成時間{0},共新增{1}筆資料!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), InsertTotalCnt));
        }

        /// <summary>產生假資料</summary>
        /// <param name="recordNum">欲產生筆數</param>
        /// <returns></returns>
        private static string FakeData(int recordNum)
        {
            Console.WriteLine(string.Format("產生XML假資料開始時間{0}!", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")));
            string Xmlstr = "";
            //產生假資料
            bookstory BookStoryobj = new bookstory();
            BookStoryobj.books = new List<book>();

            for (int i = 0; i < recordNum; i++)
            {
                book obj = new book()
                {
                    BookId = Guid.NewGuid().ToString("N"),
                    CreateDate = "2016/06/13 12:14:38",
                    Amount = 100,
                    BookName = string.Format("［東Y］真經第{0}集", i)
                };
                BookStoryobj.books.Add(obj);
            }

            //轉換為XML
            XmlSerializer xsSubmit = new XmlSerializer(typeof(bookstory));

            using (StringWriter sww = new StringWriter())
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, BookStoryobj);
                Xmlstr = sww.ToString();
            }
            Console.WriteLine(string.Format("產生XML假資料完成時間{0}!,共{1}筆", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), recordNum));
            return Xmlstr;
        }
    }
}