using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DoubleAnimation c_daListAnimation;
        private Brush brush1 = (Brush)new BrushConverter().ConvertFromString("#FF0672E5");
        public bool c_bState = true;
        private FileOperation fileOperation;
        private MyNode myNode;
        private bool isMoveTip = false;
        //当前聚焦的点
        private int currentF1ocus = 1;
        //文件名和地址
        private string filePath = "";

        public MainWindow()
        {
            
            //这边需要换一下，任何一个思维到只有一个父主题（后面可以加自由主题，再说）
            InitializeComponent();
            myNode = new MyNode("我是爸爸", grid, null);

            //显示图
            myNode.SetNumber_Numberother(0);
            myNode.ShowChildren(0);
            myNode.CreateRelation();

            //事件
            this.AddClick(myNode);
        }
        private void shortcutKey_Down(object sender, KeyEventArgs e)
        {
            MyNode temp = FocusManager.GetFocusedElement(this) as MyNode;
            MyNode temp1;
            //使用自带的FOUCS属性
            if (e.Key == Key.A)
            {
                //为什么enter键不行有问题？【创建兄弟】
                myNode.removeNode();
                temp1 = new MyNode("创建了一个新节点", grid, myNode);
                temp.Child.Add(temp1);
                myNode.SetNumber_Numberother(0);
                RecoverWantEdit(myNode);
                myNode.ShowChildren(0);
                myNode.CreateRelation();
                temp1.Click += myNode_Click;
                temp1.KeyDown += shortcutKey_Down;
            }
            else if (e.Key == Key.Tab)
            {
                //创建兄弟节点
                if (temp.father != null)
                {
                    myNode.removeNode();
                    temp1 = new MyNode("创建了一个兄弟节点", grid, temp.father);
                    temp.father.Child.Add(temp1);
                    myNode.SetNumber_Numberother(0);
                    RecoverWantEdit(myNode);
                    myNode.ShowChildren(0);
                    myNode.CreateRelation();
                    temp1.Click += myNode_Click;
                    temp1.KeyDown += shortcutKey_Down;
                }
            }
            else if (e.Key == Key.Delete)
            {
                if (temp.father != null)
                {
                    myNode.removeNode();
                    //删除对应的线
                    temp.father.lines.Remove(temp.father.lines[temp.father.Child.IndexOf(temp)]);
                    temp.father.Child.Remove(temp);
                    myNode.SetNumber_Numberother(0);
                    RecoverWantEdit(myNode);
                    myNode.ShowChildren(0);
                    myNode.CreateRelation();
                    temp = null;
                }
            }
            else if (e.Key == Key.E)
            {
                //edit键
                myNode.removeNode();
                myNode.SetNumber_Numberother(0);
                RecoverWantEdit(myNode);
                temp.wantEdit = true;
                myNode.ShowChildren(0);
                myNode.CreateRelation();
            }
        }
        public void RecoverWantEdit(MyNode myNode)
        {
            for (int i = 0; i < myNode.Child.Count; i++)
            {
                RecoverWantEdit(myNode.Child[i]);
            }
            myNode.Content = myNode.textBox.Text;
            myNode.wantEdit = false;
        }
        private void myNode_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("略略略");
            MyNode temp = sender as MyNode;
            myNode.removeNode();
            myNode.SetNumber_Numberother(0);
            RecoverWantEdit(myNode);
            myNode.ShowChildren(0);
            myNode.CreateRelation();
        }
        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            //需要增加一个逻辑，当前文件是否保存
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Microsoft Office files(*.docx)|*.doc;*.docx";//过滤一下，只要word格式的
            if (openFileDialog.ShowDialog() == true)
            {
                //确定要打开文件
                filePath = openFileDialog.FileName;
                //并且需要将组件去掉
                myNode.removeNode();
                //将当前的思维导图清空
                myNode.Child = new Collection<MyNode>();
                fileOperation = new FileOperation(myNode, filePath);
                //调用之后，需要刷新
                //myNode.removeNode();
                myNode.SetNumber_Numberother(0);
                myNode.ShowChildren(0);
                myNode.CreateRelation();
                //所有节点都需要加入click
                myNode.Click -= myNode_Click;
                myNode.KeyDown -= shortcutKey_Down;
                AddClick(myNode);
            }
        }
        private void saveFile_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //当前文件是新创建的[需要考虑里面的内容]
                if (!filePath.Equals(""))
                {
                    //保存思维导图的内容
                    if (FileOperation.SaveFile1(filePath, myNode))
                    {
                        MessageBox.Show("保存成功");
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception exception)
            {
                //需要给出保存的地址
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Microsoft Office files(*.docx)|*.docx";
                if (saveFileDialog.ShowDialog() == true)
                {
                    //对文件名字的处理
                    filePath = saveFileDialog.FileName;

                    if (!File.Exists(filePath))
                    {
                        string[] temp = filePath.Split('\\');
                        string temp1 = "";
                        for (int i = 0; i < temp.Length - 1; i++)
                        {
                            temp1 += temp[i] + "\\\\";
                        }
                        //不存在就需要创建文件
                        FileOperation.CreateWordFile(temp1, temp[temp.Length - 1].Split('.')[0]);
                    }

                    //保存思维导图的内容
                    MessageBox.Show(FileOperation.SaveFile1(filePath, myNode) ? "保存成功" : "保存失败");

                }
            }


        }
        private void AddClick(MyNode t)
        {
            for (int i = 0; i < t.Child.Count; i++)
            {
                AddClick(t.Child[i]);
            }
            t.Click += myNode_Click;
            t.KeyDown += shortcutKey_Down;
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            c_daListAnimation = new DoubleAnimation();
            c_daListAnimation.BeginTime = TimeSpan.FromSeconds(1);
            c_daListAnimation.FillBehavior = FillBehavior.HoldEnd;
            c_daListAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
            if (c_bState)
            {
                c_daListAnimation.From = 0;
                c_daListAnimation.To = 245;
                c_bState = false;
            }
            else
            {
                c_daListAnimation.From = 245;
                c_daListAnimation.To = 0;
                c_bState = true;
            }

            c_daListAnimation.BeginTime = TimeSpan.FromSeconds(0.01);
            myTranslateTransform.BeginAnimation(TranslateTransform.XProperty, c_daListAnimation);




        }
        private void change_pasterTip(object sender, MouseButtonEventArgs e)
        {

            signTip.Background = Brushes.White;
            signTipWord.Foreground = Brushes.Gray;
            pasterTip.Background = brush1;
            pasterTipWord.Foreground = Brushes.White;
            if (isMoveTip == false)
            {
                moveTip.X += 245;
                movePaster.X -= 245;
                isMoveTip = true;
            }
            

        }
        private void change_signTip(object sender, MouseButtonEventArgs e)
        {
            signTip.Background = brush1;
            signTipWord.Foreground = Brushes.White;
            pasterTip.Background = Brushes.White;
            pasterTipWord.Foreground = Brushes.Gray;
            if (isMoveTip == true)
            {
                moveTip.X -= 245;
                movePaster.X += 245;
                isMoveTip = false;
            }
            
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)

        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_childNode(object sender, RoutedEventArgs e)//zheli

        {

            MyNode temp = FocusManager.GetFocusedElement(this) as MyNode;
            /*if(temp == null)
            {
                return;
            }*/
            MyNode temp1;
            myNode.removeNode();
            temp1 = new MyNode("创建了一个新节点", grid, myNode);
            temp.Child.Add(temp1);
            myNode.SetNumber_Numberother(0);
            RecoverWantEdit(myNode);
            myNode.ShowChildren(0);
            myNode.CreateRelation();
            temp1.Click += myNode_Click;
            temp1.KeyDown += shortcutKey_Down;
        }


    }


}

  
    


    





