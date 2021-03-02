using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Line = System.Windows.Shapes.Line;

namespace WpfApp1
{
    public class MyNode:Button
    {
        //孩子节点
        public Collection<MyNode> Child;
        public Panel grid;
        public MyNode father;
        public Collection<Line[]> lines;
        //当前节点从上到下的层数
        public int number = 0;
        //包括其他节点
        public int numberOther = 0;
        //给节点的编号
        public int num;
        public int leftShift = 2, topShift = 2;
        public TextBox textBox;
        public Boolean wantEdit = false;

        public void removeNode()
        {
            for (int i = 0; i < Child.Count; i++)
            {
                Child[i].removeNode();
                if (i < lines.Count)
                {
                    if (grid.Children.Contains(lines[i][0]))
                    {
                        grid.Children.Remove(lines[i][0]);
                    }
                    if (grid.Children.Contains(lines[i][1]))
                    {
                        grid.Children.Remove(lines[i][1]);
                    }
                    if (grid.Children.Contains(lines[i][2]))
                    {
                        grid.Children.Remove(lines[i][2]);
                    }
                }
            }

            if (grid.Children.Contains(this.textBox))
            {
                grid.Children.Remove(this.textBox);
            }

            if (grid.Children.Contains(this))
            {
                grid.Children.Remove(this);
            }
        }

        public MyNode(String me, Panel grid, MyNode father)
        {
            this.father = father;
            this.Child = new Collection<MyNode>();
            this.grid = grid;
            this.Content = me;
            this.Width = 100;
            this.Height = 30;
            this.Margin = new Thickness(0, 0, 0, 0);
            setStyle();
            textBox = new TextBox();
            lines = new Collection<Line[]>();
        }


        //设置竖直方向上的层
        public int SetNumber_Numberother(int other)
        {
            this.number = 0;
            this.numberOther = 0;
            int temp = 0;
            numberOther = other;
            //同级节点带来的变化
            if (Child.Count == 0)
            {
                number = 1;
                numberOther += number;
                //当前没有孩子节点
                //返回当前节点的层数【包括后面的】
                return number;
            }
            else
            {
                //有孩子，是孩子的
                for (int i = 0; i < Child.Count; i++)
                {
                    temp = Child[i].SetNumber_Numberother(other);
                    other += temp;
                    number += temp;
                }
                numberOther += number;
                return number;
            }
            //应该是每一个孩子节点的孩子层数
        }

        //显示节点【最外层的节点个数】
        public void ShowChildren(double leftBase)
        {
            for (int i = 0; i < Child.Count; i++)
            {
                //每个孩子都显示
                Child[i].ShowChildren(leftBase + Width + 30);
            }
            this.Margin = new Thickness(leftBase + 30, (numberOther - number * 0.5 - 0.5) * 0.5 * (Width + 30), 0, 0);
            grid.Children.Add(this);
            showTextbox();
        }

        public void showTextbox()
        {
            textBox.Height = this.Height - topShift * 2;
            textBox.Width = this.Width - leftShift * 2;
            textBox.TextAlignment = TextAlignment.Center;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;
            textBox.Margin = new Thickness(this.Margin.Left + leftShift, this.Margin.Top + topShift, 0, 0);
            textBox.BorderThickness = new Thickness(0);
            textBox.Text = this.Content + "";
            if (this.wantEdit)
            {
                grid.Children.Add(textBox);
            }

            if (this.IsFocused)
            {
                this.setBorder();
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
        }

        public void setStyle()
        {
            FontSize = 12;
            BorderThickness = new Thickness(0);
            Background = Brushes.LightBlue;
        }

        public void setBorder()
        {
            BorderThickness = new Thickness(2);
            BorderBrush = Brushes.Black;
        }

        //创建连线
        public void CreateRelation()
        {
            lines = new Collection<Line[]>();
            Line line;
            Line line1;
            Line line2;
            int type = 2;
            //创建连线
            for (int i = 0; i < Child.Count; i++)
            {
                line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1.0;

                if (type == 1)
                {
                    line.X1 = this.Margin.Left + Width;
                    line.X2 = Child[i].Margin.Left;
                    line.Y1 = Margin.Top + Height / 2;
                    line.Y2 = Child[i].Margin.Top + Child[i].Height / 2;
                }
                else
                {
                    line.X1 = this.Margin.Left + this.Width;
                    line.Y1 = this.Margin.Top + this.Height / 2;
                    line.X2 = (this.Margin.Left + this.Width + Child[i].Margin.Left) / 2;
                    line.Y2 = line.Y1;

                    line1 = new Line();
                    line1.Stroke = new SolidColorBrush(Colors.Black);
                    line1.StrokeThickness = 1.0;
                    line1.X1 = line.X2;
                    line1.Y1 = line.Y2;

                    line1.X2 = line1.X1;
                    line1.Y2 = Child[i].Margin.Top + Child[i].Height / 2;

                    line2 = new Line();
                    line2.Stroke = new SolidColorBrush(Colors.Black);
                    line2.StrokeThickness = 1.0;

                    line2.X1 = line1.X2;
                    line2.Y1 = line1.Y2;

                    line2.X2 = Child[i].Margin.Left;
                    line2.Y2 = line1.Y2;

                    lines.Add(new Line[] { line, line1, line2 });
                    grid.Children.Add(line1);
                    grid.Children.Add(line2);

                }

                grid.Children.Add(line);
                Child[i].CreateRelation();
            }
        }

    }
}

