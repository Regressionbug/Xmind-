using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Application = Microsoft.Office.Interop.Word.Application;
using Microsoft.Office.Interop.Word;
using System.Windows.Documents;

namespace WpfApp1
{
    
        public class FileOperation
        {
            //创建一个Document实例
            static Spire.Doc.Document doc = new Spire.Doc.Document();
            static Spire.Doc.Section section1;
            static int n = 0;
            static int paraLength = 0;

            private string filePath;
            private MyNode myNode;
            private MyNode currentNode;

            //创建文件的构造函数
            public FileOperation(string filePath)
            {
            }

            //读取文件的构造函数
            public FileOperation(MyNode myNode, string filePath)
            {
                //赋值
                this.myNode = myNode;
                currentNode = myNode;
                this.filePath = filePath;
                //文件处理
                readFile();
            }

            public void readFile()
            {
                //加载一个现有Word文档
                doc.LoadFromFile(filePath);
                if (doc.Sections[0].Paragraphs[0].Text
                    .Equals("Evaluation Warning: The document was created with Spire.Doc for .NET."))
                {
                    doc.Sections[0].Paragraphs.RemoveAt(0);
                }

                int sectionLength = 0;
                foreach (Spire.Doc.Section section in doc.Sections)
                {
                    sectionLength++;
                }

                for (int i = 0; i < sectionLength; i++)
                {

                    //每一个Section的每一个段落
                    section1 = doc.Sections[i];
                    paraLength = 0;
                    foreach (Spire.Doc.Documents.Paragraph paragraph in section1.Paragraphs)
                    {
                        paraLength++;
                    }
                    for (n = 0; n < paraLength;)
                    {
                        generateNode(null, 0);
                    }
                }
            }


            public bool generateNode(String name, int type)
            {
                if (currentNode == null)
                {
                    return false;
                }

                MyNode tempNode;
                if (n >= paraLength)
                {
                    return false;
                }
                int current = n;
                if (name == null)
                {
                    if (!IsNumberic(section1.Paragraphs[current].StyleName))
                    {
                        //不是数字，是个文本
                        tempNode = new MyNode(section1.Paragraphs[current].Text, currentNode.grid, this.currentNode);
                        this.currentNode.Child.Add(tempNode);
                        n++;
                        return true;
                    }
                    tempNode = new MyNode(section1.Paragraphs[current].Text, currentNode.grid, this.currentNode);
                    this.currentNode.Child.Add(tempNode);
                    currentNode = tempNode;
                    n++;
                    while (generateNode(section1.Paragraphs[current].Text, int.Parse(section1.Paragraphs[current].StyleName)))
                    {

                    }
                    return false;
                }
                else
                {
                    if (!IsNumberic(section1.Paragraphs[current].StyleName))
                    {
                        //文本
                        tempNode = new MyNode(section1.Paragraphs[current].Text, currentNode.grid, this.currentNode);
                        this.currentNode.Child.Add(tempNode);
                        n++;
                        return true;
                    }
                    if (int.Parse(section1.Paragraphs[current].StyleName) > type)
                    {
                        //比上一个节点更小【子节点】
                        tempNode = new MyNode(section1.Paragraphs[current].Text, currentNode.grid, this.currentNode);
                        this.currentNode.Child.Add(tempNode);
                        currentNode = tempNode;
                        n++;
                        while (generateNode(section1.Paragraphs[current].Text, int.Parse(section1.Paragraphs[current].StyleName)))
                        {
                        }
                        return true;
                    }
                    else
                    {
                        //比上一个节点大
                        currentNode = currentNode.father;
                        return false;
                    }
                }
            }

            public bool IsNumberic(string oText)
            {
                try
                {
                    int var1 = Convert.ToInt32(oText);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            #region 新建Word文档
            /// <summary>
            /// 动态生成Word文档并填充内容 
            /// </summary>
            /// <param name="dir">文档目录</param>
            /// <param name="fileName">文档名</param>
            /// <returns>返回自定义信息</returns>
            public static bool CreateWordFile(string dir, string fileName)
            {
                try
                {
                    Object oMissing = System.Reflection.Missing.Value;

                    if (!Directory.Exists(dir))
                    {
                        //创建文件所在目录
                        Directory.CreateDirectory(dir);
                    }

                    //创建Word文档(Microsoft.Office.Interop.Word)
                    _Application WordApp = new Application();
                    //WordApp.Visible = true;
                    _Document WordDoc = WordApp.Documents.Add(
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    //保存
                    object filename = dir + fileName;
                    WordDoc.SaveAs(ref filename, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                    WordDoc.Close(ref oMissing, ref oMissing, ref oMissing);
                    WordApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return false;
                }
            }
            #endregion 新建Word文档


            public static bool SaveFile1(string filePath, MyNode myNode)
            {
                try
                {
                    Spire.Doc.Document doc = new Spire.Doc.Document();
                    doc.LoadFromFile(filePath);
                    while (doc.Sections[0].Paragraphs.Count != 0)
                    {
                        doc.Sections[0].Paragraphs.RemoveAt(0);
                    }
                    saveNode(doc, myNode, 1);
                    doc.SaveToFile(filePath);
                    return true;
                }
                catch (Exception e)
                {

                    return false;
                }

            }

            //保存文件
            public static bool SaveFile(string filePath, MyNode myNode)
            {
                try
                {
                    //初始化
                    object fileobj = filePath;
                    object unknow = System.Reflection.Missing.Value;
                    //打开word程序，创建一个新的word文档，但是还没有保存到硬盘中
                    ApplicationClass wordApp = new ApplicationClass();
                    _Document doc = wordApp.Documents.Add(ref unknow, ref unknow, ref unknow, ref unknow);

                    //这里开始写文件
                    //深度优先遍历
                    //saveNode(doc, myNode,1);


                    //保存word文档
                    doc.SaveAs(ref fileobj, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow, ref unknow);
                    doc.Close(ref unknow, ref unknow, ref unknow);
                    wordApp.Documents.Save(ref unknow, ref unknow);
                    wordApp.Quit(ref unknow, ref unknow, ref unknow);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }

            }


            public static void saveNode(Spire.Doc.Document doc, MyNode myNode, int depth)
            {
                if (depth != 1)
                {
                    //不保存最大的节点
                    Spire.Doc.Documents.Paragraph paraInserted = doc.Sections[0].AddParagraph();
                Spire.Doc.Fields.TextRange textRange1 = paraInserted.AppendText(myNode.Content + "");
                    paraInserted.ApplyStyle(numberToBuitinStyle[depth - 1]);
                }

                for (int i = 0; i < myNode.Child.Count; i++)
                {
                    saveNode(doc, myNode.Child[i], depth + 1);
                }
            }


            public static BuiltinStyle[] numberToBuitinStyle =
            {
            BuiltinStyle.Heading1,
            BuiltinStyle.Heading2,
            BuiltinStyle.Heading3,
            BuiltinStyle.Heading4,
            BuiltinStyle.Heading5,
            BuiltinStyle.Heading6,
            BuiltinStyle.Heading7,
            BuiltinStyle.Heading8,
            BuiltinStyle.Heading9
        };


        }
    }

