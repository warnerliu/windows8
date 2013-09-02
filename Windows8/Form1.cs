using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;


namespace Windows8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //测试用列表，不影响全局
        List<Point> lineList = new List<Point>();

        //记录画圆过程中的轨迹，用来确定圆的外切正方形
        List<Point> circleList = new List<Point>();
        
        //用于序列化对象的存储
        //数据存储在arrayList文件中，后期数据处理
        ArrayList storeList = new ArrayList();//数据处理

        //单独存储线，将起点和终点作为键值对存储
        //后期数据处理需要
        Dictionary<Point, Point> line = new Dictionary<Point, Point>();//数据处理

        //存储键值对，圆心和半径，实际是圆的外切正方形
        //数据存储在dictionary文件中，后期数据处理需要
        Dictionary<Point, Rectangle> circle = new Dictionary<Point, Rectangle>();//数据处理

        //点误差范围
        static int error = 15;
        //圆误差范围
        static int circleErr = 25;

        Point beginP;
        Point endP;
        private int picTag = 0;
        private bool flag = false;

        private string savePath = System.AppDomain.CurrentDomain.BaseDirectory;

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            beginP = new Point(e.X, e.Y);
            
            //鼠标按下有效
            flag = true;

        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            endP = new Point(e.X, e.Y);

            Graphics g = panel1.CreateGraphics();
            Pen rPen = new Pen(Color.Red, 5);

            //画线和画点
            #region
            if (distanceTest(beginP, endP) > 30)
            {
                Point[] drawLine = { beginP, endP };
                g.DrawLines(rPen, drawLine);

                //起点
                Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                g.DrawEllipse(rPen, bRec);
                //终点
                Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                g.DrawEllipse(rPen, eRec);

                //填充起点和终点
                SolidBrush sb = new SolidBrush(Color.Red);
                g.FillEllipse(sb,bRec);
                g.FillEllipse(sb,eRec);

                //将起点加入列表
                lineList.Add(beginP);
                //将终点加入列表
                lineList.Add(endP);
                //加入起点和终点
                if (!line.ContainsKey(beginP))
                    line.Add(beginP, endP);//后期数据处理

                //storeList.Add(beginP);
                //storeList.Add(endP);
            }

            
            if (beginP.Equals(endP))
            {
                Pen bPen = new Pen(Color.Blue, 2);
                Rectangle rec = new Rectangle(e.X - 10, e.Y - 10, 20, 20);

                //正方形
                //g.DrawRectangle(bPen, e.X - 10, e.Y - 10, 20, 20);

                //空心圆
                //g.DrawEllipse(bPen,rec);

                //实心填充圆
                SolidBrush bsh = new SolidBrush(Color.Red);
                g.FillEllipse(bsh, rec);
                //将起点加入列表
                lineList.Add(beginP);
                storeList.Add(beginP);
            }
            #endregion
            //画圆
            #region
            //起点和终点的距离在0到30之间，切列表里的点数大于40，此时默认画的是圆。
            if (distanceTest(beginP, endP) > 0 && distanceTest(beginP, endP) < 30 && circleList.Count > 40)
            {
                
                //处理circleList中的point数据
                int leftP = circleList[0].X;
                int rightP = circleList[0].X;
                int topP = circleList[0].Y;
                int bottomP = circleList[0].Y;

                foreach (Point p in circleList)
                {
                    if (p.X < leftP)
                        leftP = p.X;
                    if(p.X > rightP)
                        rightP = p.X;

                    if (p.Y < topP)
                        topP = p.Y;
                    if(p.Y > bottomP)
                        bottomP = p.Y;
                    
                }

                Pen bPen = new Pen(Color.Blue,1);

                //确定正方形
                int size = (rightP - leftP + bottomP - topP)/2;
                int xCoordinate = (leftP + rightP) / 2 - size / 2;
                int yCoordinate = (topP + bottomP) / 2 - size / 2;
                Rectangle testRec = new Rectangle(xCoordinate, yCoordinate, size, size);
                //圆心
                Point temp = new Point((rightP + leftP) / 2, (topP + bottomP) / 2);
                lineList.Add(temp);

                //记录圆心和半径，实际是圆的外切正方形
                circle.Add(temp, testRec);
                
                //记录圆心
                //storeList.Add(temp);

                //画圆
                g.DrawEllipse(bPen,testRec);

            }
            #endregion
            //鼠标弹起后，MouseMove事件无效，不再记录移动轨迹
            flag = false;
            //清空列表，继续画圆
            circleList.Clear();
        }

        double distanceTest(Point startP, Point overP)
        {

            double distance = Math.Sqrt(Math.Pow(startP.X - overP.X, 2) + Math.Pow(startP.Y - overP.Y, 2));

            return distance;
        }

        //保存轨迹
        private void button1_Click(object sender, EventArgs e)
        {
            if (picTag == 1)
            {
                //该列表暂时未用到
                FileStream testfs = new FileStream(@"path\points1.txt",FileMode.Create);
                StreamWriter sw = new StreamWriter(testfs);
                //StreamWriter sw = File.AppendText("points1.txt");
                
                foreach (Point p in lineList)
                {
                    sw.WriteLine(p);
                }
                sw.Flush();
                sw.Close();

                //将storeList中的Point对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\arrayList1.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, storeList);
                    //fs.Close();
                }

                //序列化圆心和半径，存入文件
                using (FileStream fs = new FileStream(@"path\dictionary1.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, circle);
                }

                //将存储直线的line中的对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\line1.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, line);
                }
            }
            
            if (picTag == 2)
            {
                StreamWriter sw = File.AppendText(@"path\points2.txt");
                foreach (Point p in lineList)
                {
                    sw.WriteLine(p);
                }
                sw.Flush();
                sw.Close();

                //将storeList中的Point对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\arrayList2.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, storeList);
                    //fs.Close();
                }

                //序列化圆心和半径，存入文件
                using (FileStream fs = new FileStream(@"path\dictionary2.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, circle);
                }

                //将存储直线的line中的对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\line2.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, line);
                }


            }
            if (picTag == 3) 
            {
                StreamWriter sw = File.AppendText(@"path\points3.txt");
                foreach (Point p in lineList)
                {
                    sw.WriteLine(p);
                }
                sw.Flush();
                sw.Close();

                //将storeList中的Point对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\arrayList3.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, storeList);
                    //fs.Close();
                }

                //序列化圆心和半径，存入文件
                using (FileStream fs = new FileStream(@"path\dictionary3.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, circle);
                }

                //将存储直线的line中的对象进行序列化，然后存储
                using (FileStream fs = new FileStream(@"path\line3.txt", FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, line);
                }

            }
            
            /*
            StreamWriter circleFile = File.AppendText("circlePoints.txt");
            foreach (Point p in storeList)
                circleFile.WriteLine(p);
            circleFile.Flush();
            circleFile.Close();
            */

            //清空列表中的内容
            circle.Clear();
            storeList.Clear();
            lineList.Clear();
            circleList.Clear();
            panel1.Refresh();
            line.Clear();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (flag)
            {
                circleList.Add(e.Location);
            }
        }    
        
        //重现轨迹
        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            if (picTag == 1)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary1.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        //Pen bPen = new Pen(Color.Blue, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(45,255,0,0));
                        Rectangle rec = (Rectangle)var.Value;
                        g.FillEllipse(sb,rec);
                        

                    }
                }
                
                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList1.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        ////实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X-5,var.Y),new Point(var.X+5,var.Y)};
                        Point[] vLine = { new Point(var.X,var.Y-5),new Point(var.X,var.Y+5)};
                        g.DrawLines(rPen,hLine);
                        g.DrawLines(rPen,vLine);
                    }
                }
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line1.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        g.DrawLine(rPen, beginP, endP);
                    }
                }
                
            }
            #region
            if (picTag == 2)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary2.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        //Pen bPen = new Pen(Color.Blue, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(45, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        g.FillEllipse(sb, rec);
                    }
                }
                
                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList2.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        //实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                    }
                }
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line2.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        g.DrawLine(rPen, beginP, endP);
                    }
                }
                
            }
            #endregion

            #region
            if (picTag == 3)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary3.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        //Pen bPen = new Pen(Color.Blue, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(45, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        g.FillEllipse(sb, rec);
                    }
                }
                
                
                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList3.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        //实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.FromArgb(60,255,255,255));
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                        
                    }
                }
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line3.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        g.DrawLine(rPen, beginP, endP);
                    }
                }
                
            }

            #endregion
        }
        
        //重置
        private void reset_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            lineList.Clear();
            circleList.Clear();
            storeList.Clear();
            circle.Clear();
            line.Clear();
        }

        //第一幅图
        private void pic1_Click(object sender, EventArgs e)
        {
            //panel1.BackgroundImage = Image.FromFile(@"..\..\resources\pic1.jpg");
            panel1.BackgroundImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory+@"resource\pic1.jpg");
            
            panel1.Visible = true;
            picTag = 1;
        }
        //第二幅图
        private void pic2_Click(object sender, EventArgs e)
        {
            //panel1.BackgroundImage = Image.FromFile(@"..\..\resources\pic2.jpg");
            panel1.BackgroundImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"resource\pic2.jpg");
            panel1.Visible = true;
            picTag = 2;
        }
        //第三幅图
        private void pic3_Click(object sender, EventArgs e)
        {
            //panel1.BackgroundImage = Image.FromFile(@"..\..\resources\pic3.jpg");
            panel1.BackgroundImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"resource\pic3.jpg");
            panel1.Visible = true;
            picTag = 3;
        }
       
        //认证
        private void commit_Click(object sender, EventArgs e)
        {
            //存储点
            ArrayList getList = new ArrayList();
            BinaryFormatter bf = new BinaryFormatter();
            //存储圆心和外切正方形
            Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
            //存储线段
            Dictionary<Point, Point> getLine = new Dictionary<Point, Point>();

            #region
            if (picTag == 1)
            {
                //读取已存储的点
                FileStream fs = new FileStream(@"path\arrayList1.txt", FileMode.Open);
                getList = (ArrayList)bf.Deserialize(fs);
                fs.Close();

                //读取圆心和半径，实际是圆的外切正方形
                using (FileStream fsCircle = new FileStream(@"path\dictionary1.txt", FileMode.Open))
                {
                    BinaryFormatter bfCircle = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bfCircle.Deserialize(fsCircle);
                }
                //读取线段
                using (FileStream fsLine = new FileStream(@"path\line1.txt", FileMode.Open))
                {
                    getLine = (Dictionary<Point, Point>)bf.Deserialize(fsLine);
                }

                //如果认证的数量不对，则密码肯定错误
                if (getList.Count != storeList.Count||temp.Count != circle.Count||getLine.Count != line.Count)
                {
                    MessageBox.Show("密码错误！");
                }
                else
                {
                    #region
                    //对点进行判断
                    Point tmp1 = new Point();
                    Point tmp2 = new Point();
                    bool result = true;
                    for (int i = 0; i < getList.Count; i++)
                    {
                        tmp1 = (Point)getList[i];
                        tmp2 = (Point)storeList[i];
                        if (Math.Abs(tmp1.X - tmp2.X) > error || Math.Abs(tmp1.Y - tmp2.Y) > error)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                       
                    }
                    //对圆心和半径进行判断
                    //首先获取当前认证的圆心列表和外切正方形的信息
                    List<Point> loginPointList = new List<Point>();
                    List<Rectangle> loginCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in circle)
                    {
                        loginPointList.Add(var.Key);
                        loginCircleR.Add(var.Value);
                    }
                    //获取已存储的信息
                    List<Point> registerPointList = new List<Point>();
                    List<Rectangle> registerCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        registerPointList.Add(var.Key);
                        registerCircleR.Add(var.Value);
                    }
                    //分别对存储圆心的list和外切正方形的list进行比较
                    for (int i = 0; i < loginPointList.Count; i++)
                    {
                        if (Math.Abs(loginPointList[i].X - registerPointList[i].X) > circleErr || Math.Abs(loginPointList[i].Y - registerPointList[i].Y) > circleErr)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                        //外切正方形暂时未判断
                    }

                    loginPointList.Clear();
                    loginCircleR.Clear();
                    registerPointList.Clear();
                    registerCircleR.Clear();
                    #endregion
                    //对线进行判断
                    //将起点和重点分别读到列表里
                    List<Point> registerBeginP = new List<Point>();
                    List<Point> registerEndP = new List<Point>();
                    List<Point> loginBeginP = new List<Point>();
                    List<Point> loginEndP = new List<Point>();
                    //登录的信息
                    foreach (KeyValuePair<Point, Point> var in line)
                    {
                        loginBeginP.Add(var.Key);
                        loginEndP.Add(var.Value);
                    }
                    //注册的信息
                    foreach (KeyValuePair<Point, Point> var in getLine)
                    {
                        registerBeginP.Add(var.Key);
                        registerEndP.Add(var.Value);
                    }
                    for (int index = 0; index < registerBeginP.Count; index++)
                    {
                        if (Math.Abs(loginBeginP[index].X - registerBeginP[index].X) > error || Math.Abs(loginBeginP[index].Y - registerBeginP[index].Y) > error || Math.Abs(loginEndP[index].X - registerEndP[index].X) > error || Math.Abs(loginEndP[index].Y - registerEndP[index].Y) > error)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                    }
                    registerBeginP.Clear();
                    registerEndP.Clear();
                    loginBeginP.Clear();
                    loginEndP.Clear();

                        if (result)
                        {
                            MessageBox.Show("密码正确！");
                        }
                }

            }
            #endregion

            #region
            if (picTag == 2)
            {
               
                //读取已存储的点
                FileStream fs = new FileStream(@"path\arrayList2.txt", FileMode.Open);
                getList = (ArrayList)bf.Deserialize(fs);
                fs.Close();
                //读取圆心和半径，实际是圆的外切正方形
                using (FileStream fsCircle = new FileStream(@"path\dictionary2.txt", FileMode.Open))
                {
                    BinaryFormatter bfCircle = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bfCircle.Deserialize(fsCircle);
                }
                //读取线段
                using (FileStream fsLine = new FileStream(@"path\line2.txt", FileMode.Open))
                {
                    getLine = (Dictionary<Point, Point>)bf.Deserialize(fsLine);
                }

                //如果认证的数量不对，则密码肯定错误
                if (getList.Count != storeList.Count || temp.Count != circle.Count||getLine.Count != line.Count)
                {
                    MessageBox.Show("密码错误！");
                }
                else
                {
                    //对点进行判断
                    Point tmp1 = new Point();
                    Point tmp2 = new Point();
                    bool result = true;
                    for (int i = 0; i < getList.Count; i++)
                    {
                        tmp1 = (Point)getList[i];
                        tmp2 = (Point)storeList[i];
                        if (Math.Abs(tmp1.X - tmp2.X) > error || Math.Abs(tmp1.Y - tmp2.Y) > error)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                    }
                    //对圆心和半径进行判断
                    //首先获取当前认证的圆心列表和外切正方形的信息
                    List<Point> loginPointList = new List<Point>();
                    List<Rectangle> loginCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in circle)
                    {
                        loginPointList.Add(var.Key);
                        loginCircleR.Add(var.Value);
                    }
                    //获取已存储的信息
                    List<Point> registerPointList = new List<Point>();
                    List<Rectangle> registerCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        registerPointList.Add(var.Key);
                        registerCircleR.Add(var.Value);
                    }
                    
                    //分别对存储圆心的list和外切正方形的list进行比较
                    for (int i = 0; i < loginPointList.Count; i++)
                    {
                        if (Math.Abs(loginPointList[i].X - registerPointList[i].X) > circleErr || Math.Abs(loginPointList[i].Y - registerPointList[i].Y) > circleErr)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                        //外切正方形暂时未判断
                    }

                    loginPointList.Clear();
                    loginCircleR.Clear();
                    registerPointList.Clear();
                    registerCircleR.Clear();

                    //对线进行判断
                    //将起点和重点分别读到列表里
                    List<Point> registerBeginP = new List<Point>();
                    List<Point> registerEndP = new List<Point>();
                    List<Point> loginBeginP = new List<Point>();
                    List<Point> loginEndP = new List<Point>();
                    //登录的信息
                    foreach (KeyValuePair<Point, Point> var in line)
                    {
                        loginBeginP.Add(var.Key);
                        loginEndP.Add(var.Value);
                    }
                    //注册的信息
                    foreach (KeyValuePair<Point, Point> var in getLine)
                    {
                        registerBeginP.Add(var.Key);
                        registerEndP.Add(var.Value);
                    }
                    for (int index = 0; index < registerBeginP.Count; index++)
                    {
                        if (Math.Abs(loginBeginP[index].X - registerBeginP[index].X) > error || Math.Abs(loginBeginP[index].Y - registerBeginP[index].Y) > error || Math.Abs(loginEndP[index].X - registerEndP[index].X) > error || Math.Abs(loginEndP[index].Y - registerEndP[index].Y) > error)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                    }
                    registerBeginP.Clear();
                    registerEndP.Clear();
                    loginBeginP.Clear();
                    loginEndP.Clear();

                    if (result)
                    {
                        MessageBox.Show("密码正确！");
                    }
                }
            }
            #endregion
            #region
            if (picTag == 3)
            {
               
                //读取已存储的点
                FileStream fs = new FileStream(@"path\arrayList3.txt", FileMode.Open);
                getList = (ArrayList)bf.Deserialize(fs);
                fs.Close();

                //读取圆心和半径，实际是圆的外切正方形
                using (FileStream fsCircle = new FileStream(@"path\dictionary3.txt", FileMode.Open))
                {
                    BinaryFormatter bfCircle = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bfCircle.Deserialize(fsCircle);
                }
                //读取线段
                using (FileStream fsLine = new FileStream(@"path\line3.txt", FileMode.Open))
                {
                    getLine = (Dictionary<Point, Point>)bf.Deserialize(fsLine);
                }
                //如果认证的数量不对，则密码肯定错误
                if (getList.Count != storeList.Count || temp.Count != circle.Count||getLine.Count != line.Count)
                {
                    MessageBox.Show("密码错误！");
                }
                else
                {
                    //对点进行判断
                    Point tmp1 = new Point();
                    Point tmp2 = new Point();
                    bool result = true;
                    for (int i = 0; i < getList.Count; i++)
                    {
                        tmp1 = (Point)getList[i];
                        tmp2 = (Point)storeList[i];
                        if (Math.Abs(tmp1.X - tmp2.X) > 10 || Math.Abs(tmp1.Y - tmp2.Y) > 10)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                    }
                    //对圆心和半径进行判断
                    //首先获取当前认证的圆心列表和外切正方形的信息
                    List<Point> loginPointList = new List<Point>();
                    List<Rectangle> loginCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in circle)
                    {
                        loginPointList.Add(var.Key);
                        loginCircleR.Add(var.Value);
                    }
                    //获取已存储的信息
                    List<Point> registerPointList = new List<Point>();
                    List<Rectangle> registerCircleR = new List<Rectangle>();
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        registerPointList.Add(var.Key);
                        registerCircleR.Add(var.Value);
                    }
                    //分别对存储圆心的list和外切正方形的list进行比较
                    for (int i = 0; i < loginPointList.Count; i++)
                    {
                        if (Math.Abs(loginPointList[i].X - registerPointList[i].X) > circleErr || Math.Abs(loginPointList[i].Y - registerPointList[i].Y) > circleErr)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                        //外切正方形暂时未判断
                    }

                    loginPointList.Clear();
                    loginCircleR.Clear();
                    registerPointList.Clear();
                    registerCircleR.Clear();

                    //对线进行判断
                    //将起点和重点分别读到列表里
                    List<Point> registerBeginP = new List<Point>();
                    List<Point> registerEndP = new List<Point>();
                    List<Point> loginBeginP = new List<Point>();
                    List<Point> loginEndP = new List<Point>();
                    //登录的信息
                    foreach (KeyValuePair<Point, Point> var in line)
                    {
                        loginBeginP.Add(var.Key);
                        loginEndP.Add(var.Value);
                    }
                    //注册的信息
                    foreach (KeyValuePair<Point, Point> var in getLine)
                    {
                        registerBeginP.Add(var.Key);
                        registerEndP.Add(var.Value);
                    }
                    for (int index = 0; index < registerBeginP.Count; index++)
                    {
                        if (Math.Abs(loginBeginP[index].X - registerBeginP[index].X) > error || Math.Abs(loginBeginP[index].Y - registerBeginP[index].Y) > error || Math.Abs(loginEndP[index].X - registerEndP[index].X) > error || Math.Abs(loginEndP[index].Y - registerEndP[index].Y) > error)
                        {
                            MessageBox.Show("密码错误！");
                            result = false;
                            break;
                        }
                    }
                    registerBeginP.Clear();
                    registerEndP.Clear();
                    loginBeginP.Clear();
                    loginEndP.Clear();

                    if (result)
                    {
                        MessageBox.Show("密码正确！");
                    }
                }
            }
            #endregion

            //刷新panel
            panel1.Refresh();
            //清空认证时记录的数据，防止对下次认证造成干扰
            //清空圆心和外切正方形列表
            circle.Clear();
            //清空记录点的列表
            storeList.Clear();
            //清空测试用表
            lineList.Clear();
            //清空圆的轨迹列表
            circleList.Clear();
            //清空线段列表
            line.Clear();
            //每次认证结束清空列表内容
            getList.Clear();
            temp.Clear();
            getLine.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.BackgroundImage = Image.FromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"resource\pic.jpg");
            panel1.Visible = true;
        }

        //数据汇总
        //使用Dictionary不能添加已经存在的键值对，若key已经存在，则无法添加
        private void testList_Click(object sender, EventArgs e)
        {
            //从序列化文件中读取数据，需要先把原来的序列化对象进行反序列化，然后与新的数据合并，再进行序列化存入文件
            //第一幅图
            ArrayList tempArrayList1 = new ArrayList();
            Dictionary<Point, Rectangle> tempDictionary1 = new Dictionary<Point, Rectangle>();
            Dictionary<Point, Point> tempLine1 = new Dictionary<Point, Point>();
            //第二幅图
            ArrayList tempArrayList2 = new ArrayList();
            Dictionary<Point, Rectangle> tempDictionary2 = new Dictionary<Point, Rectangle>();
            Dictionary<Point, Point> tempLine2 = new Dictionary<Point, Point>();
            //第三幅图
            ArrayList tempArrayList3 = new ArrayList();
            Dictionary<Point, Rectangle> tempDictionary3 = new Dictionary<Point, Rectangle>();
            Dictionary<Point, Point> tempLine3 = new Dictionary<Point, Point>();

            BinaryFormatter bf = new BinaryFormatter();

            //暂时存储反序列化出来的数据
            ArrayList tempList = new ArrayList();
            Dictionary<Point, Rectangle> tempCircleList = new Dictionary<Point, Rectangle>();
            Dictionary<Point, Point> tempLine = new Dictionary<Point, Point>();

            //统计点、线段（两个端点）和圆的数量
            //第一幅图
            int pointNumber1 = 0;
            int lineNumber1 = 0;
            int circleNumber1 = 0;
            //第二幅图
            int pointNumber2 = 0;
            int lineNumber2 = 0;
            int circleNumber2 = 0;
            //第三幅图
            int pointNumber3 = 0;
            int lineNumber3 = 0;
            int circleNumber3 = 0;
           
            

            #region
            for (int i = 1; i < 39; i++)
            {
                //第一幅图的路径
                string arrayListPath1 = @"rawdata\" + i + @"\arrayList1.txt";
                string dictionaryPath1 = @"rawdata\" + i + @"\dictionary1.txt";
                string linePath1 = @"rawdata\" + i + @"\line1.txt";
                //第二幅图的路径
                string arrayListPath2 = @"rawdata\" + i + @"\arrayList2.txt";
                string dictionaryPath2 = @"rawdata\" + i + @"\dictionary2.txt";
                string linePath2 = @"rawdata\" + i + @"\line2.txt";
                //第三幅图的路径
                string arrayListPath3 = @"rawdata\" + i + @"\arrayList3.txt";
                string dictionaryPath3 = @"rawdata\" + i + @"\dictionary3.txt";
                string linePath3 = @"rawdata\" + i + @"\line3.txt";
                //MessageBox.Show(arrayListPath1);           

                if (File.Exists(arrayListPath1))
                {
                    //MessageBox.Show(arrayListPath1);
                    //读取存储点的文件            
                    using (FileStream fs = new FileStream(arrayListPath1, FileMode.Open))
                    {
                        tempList = (ArrayList)bf.Deserialize(fs);
                    }
                    foreach (Point p in tempList)
                        tempArrayList1.Add(p);
                 

                    //读取存储圆心和外切正方形的文件
                    using (FileStream fsCircle = new FileStream(dictionaryPath1, FileMode.Open))
                    {
                        tempCircleList = (Dictionary<Point, Rectangle>)bf.Deserialize(fsCircle);
                    }
                    //添加之前需先判断key是否存在
                    foreach (KeyValuePair<Point, Rectangle> pr in tempCircleList)
                    {
                        if (!tempDictionary1.ContainsKey(pr.Key))
                            tempDictionary1.Add(pr.Key, pr.Value);
                    }
                    
                    //读取线
                    using (FileStream fs = new FileStream(linePath1, FileMode.Open))
                    {
                        tempLine = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    }
                    //添加之前需要先判断key是否存在
                    foreach (KeyValuePair<Point, Point> var in tempLine)
                    {
                        if (!tempLine1.ContainsKey(var.Key))
                            tempLine1.Add(var.Key, var.Value);
                    }
                    
                   
                }
                if (File.Exists(arrayListPath2))
                {
                    using (FileStream fs = new FileStream(arrayListPath2, FileMode.Open))
                    {
                        tempList = (ArrayList)bf.Deserialize(fs);
                    }
                    foreach (Point p in tempList)
                        tempArrayList2.Add(p);
                    
                    using (FileStream fsCircle = new FileStream(dictionaryPath2, FileMode.Open))
                    {
                        tempCircleList = (Dictionary<Point, Rectangle>)bf.Deserialize(fsCircle);
                    }
                    foreach (KeyValuePair<Point, Rectangle> pr in tempCircleList)
                    {
                        if (!tempDictionary2.ContainsKey(pr.Key))
                            tempDictionary2.Add(pr.Key, pr.Value);
                    }
                    
                    //读取线
                    using (FileStream fs = new FileStream(linePath2, FileMode.Open))
                    {
                        tempLine = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    }
                    foreach (KeyValuePair<Point, Point> var in tempLine)
                    {
                        if (!tempLine2.ContainsKey(var.Key))
                            tempLine2.Add(var.Key, var.Value);
                    }
                    

                }
                if (File.Exists(arrayListPath3))
                {
                    using (FileStream fs = new FileStream(arrayListPath3, FileMode.Open))
                    {
                        tempList = (ArrayList)bf.Deserialize(fs);
                    }
                    foreach (Point p in tempList)
                        tempArrayList3.Add(p);
                    
                    using (FileStream fsCircle = new FileStream(dictionaryPath3, FileMode.Open))
                    {
                        tempCircleList = (Dictionary<Point, Rectangle>)bf.Deserialize(fsCircle);
                    }
                    foreach (KeyValuePair<Point, Rectangle> pr in tempCircleList)
                    {
                        if (!tempDictionary3.ContainsKey(pr.Key))
                            tempDictionary3.Add(pr.Key, pr.Value);
                    }
                    
                    //读取线
                    using (FileStream fs = new FileStream(linePath3, FileMode.Open))
                    {
                        tempLine = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    }
                    foreach (KeyValuePair<Point, Point> var in tempLine)
                    {
                        if (!tempLine3.ContainsKey(var.Key))
                            tempLine3.Add(var.Key, var.Value);
                    }
                   
                }
            }
            //统计数量
            pointNumber1 = tempArrayList1.Count;
            lineNumber1 = tempLine1.Count;
            circleNumber1 = tempDictionary1.Count;

            pointNumber2 = tempArrayList2.Count;
            lineNumber2 = tempLine2.Count;
            circleNumber2 = tempDictionary2.Count;

            pointNumber3 = tempArrayList3.Count;
            lineNumber3 = tempLine3.Count;
            circleNumber3 = tempDictionary3.Count;

            //线段两个端点，乘2获得端点的个数
            lineNumber1 *= 2;
            lineNumber2 *= 2;
            lineNumber3 *= 2;
            #endregion
            Console.WriteLine("pic1.points" + pointNumber1.ToString());
            Console.WriteLine("pic1.lines" + lineNumber1.ToString());
            Console.WriteLine("pic1.circles" + circleNumber1.ToString());
            /*
            //第一幅图的数据信息
            MessageBox.Show("pic1.points" + pointNumber1.ToString());
            MessageBox.Show("pic1.lines" + lineNumber1.ToString());
            MessageBox.Show("pic1.circles" + circleNumber1.ToString());
            //第二幅图
            MessageBox.Show("pic2.points" + pointNumber2.ToString());
            MessageBox.Show("pic2.lines" + lineNumber2.ToString());
            MessageBox.Show("pic2.circles" + circleNumber2.ToString());
            //第三幅图
            MessageBox.Show("pic3.points" + pointNumber3.ToString());
            MessageBox.Show("pic3.lines" + lineNumber3.ToString());
            MessageBox.Show("pic3.circles" + circleNumber3.ToString());
            */

            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile1 = new FileStream(@"labStudy\arrayList1.txt", FileMode.Append);
            bf.Serialize(writeToFile1, tempArrayList1);
            writeToFile1.Close();
            FileStream circleWrite1 = new FileStream(@"labStudy\circleDictionary1.txt", FileMode.Append);
            bf.Serialize(circleWrite1, tempDictionary1);
            circleWrite1.Close();
            FileStream lineWrite1 = new FileStream(@"labStudy\line1.txt", FileMode.Append);
            bf.Serialize(lineWrite1, tempLine1);
            lineWrite1.Close();
            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile2 = new FileStream(@"labStudy\arrayList2.txt", FileMode.Append);
            bf.Serialize(writeToFile2, tempArrayList2);
            writeToFile2.Close();
            FileStream circleWrite2 = new FileStream(@"labStudy\circleDictionary2.txt", FileMode.Append);
            bf.Serialize(circleWrite2, tempDictionary2);
            circleWrite2.Close();
            FileStream lineWrite2 = new FileStream(@"labStudy\line2.txt", FileMode.Append);
            bf.Serialize(lineWrite2, tempLine2);
            lineWrite2.Close();

            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile3 = new FileStream(@"labStudy\arrayList3.txt", FileMode.Append);
            bf.Serialize(writeToFile3, tempArrayList3);
            writeToFile3.Close();
            FileStream circleWrite3 = new FileStream(@"labStudy\circleDictionary3.txt", FileMode.Append);
            bf.Serialize(circleWrite3, tempDictionary3);
            circleWrite3.Close();
            FileStream lineWrite3 = new FileStream(@"labStudy\line3.txt", FileMode.Append);
            bf.Serialize(lineWrite3, tempLine3);
            lineWrite3.Close();
            /*
            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile1 = new FileStream(@"total\arrayList1.txt", FileMode.Append);
            bf.Serialize(writeToFile1, tempArrayList1);
            writeToFile1.Close();
            FileStream circleWrite1 = new FileStream(@"total\circleDictionary1.txt", FileMode.Append);
            bf.Serialize(circleWrite1, tempDictionary1);
            circleWrite1.Close();
            FileStream lineWrite1 = new FileStream(@"total\line1.txt",FileMode.Append);
            bf.Serialize(lineWrite1,tempLine1);
            lineWrite1.Close();
            
            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile2 = new FileStream(@"total\arrayList2.txt", FileMode.Append);
            bf.Serialize(writeToFile2, tempArrayList2);
            writeToFile2.Close();
            FileStream circleWrite2 = new FileStream(@"total\circleDictionary2.txt", FileMode.Append);
            bf.Serialize(circleWrite2, tempDictionary2);
            circleWrite2.Close();
            FileStream lineWrite2 = new FileStream(@"total\line2.txt", FileMode.Append);
            bf.Serialize(lineWrite2, tempLine2);
            lineWrite2.Close();

            //将读取出来的arraylist写入总的数据文件
            FileStream writeToFile3 = new FileStream(@"total\arrayList3.txt", FileMode.Append);
            bf.Serialize(writeToFile3, tempArrayList3);
            writeToFile3.Close();
            FileStream circleWrite3 = new FileStream(@"total\circleDictionary3.txt", FileMode.Append);
            bf.Serialize(circleWrite3, tempDictionary3);
            circleWrite3.Close();
            FileStream lineWrite3 = new FileStream(@"total\line3.txt", FileMode.Append);
            bf.Serialize(lineWrite3, tempLine3);
            lineWrite3.Close();
            */
        }

        //圆聚簇分析
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            //ArrayList getList = new ArrayList();
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream fs = new FileStream(@"myObject.txt", FileMode.Open);
            //getList = (ArrayList)bf.Deserialize(fs);
            //fs.Close();
           
            //using (StreamWriter circleFile = File.AppendText("deserialize.txt"))
            //{
            //    foreach (var p in getList)
            //        circleFile.WriteLine(p);
            //    //circleFile.Flush();
            //    //circleFile.Close();
            //}
            if (picTag == 1)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary1.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100,255,255,255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb,rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);

                    }
                }
                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"thesecond\circle.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 20, var.Y - 20, 40, 40);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }

            }
            #region
            if (picTag == 2)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary2.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb, rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);
                    }
                }
            }
            #endregion

            #region
            if (picTag == 3)
            {
                
                //重现圆
                using (FileStream fs = new FileStream(@"total\circledictionary3.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb, rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);
                    }
                }

            }

            #endregion

        }
        //点聚簇分析
        private void add1_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            //ArrayList my1 = new ArrayList();
            //my1.Add(5);
            //my1.Add(6);
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream fs = new FileStream(@"myObject.txt",FileMode.CreateNew);
            //bf.Serialize(fs,my1);
            //fs.Close();
            if (picTag == 1)
            {
                
                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList1.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        ////实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X-5,var.Y),new Point(var.X+5,var.Y)};
                        Point[] vLine = { new Point(var.X,var.Y-5),new Point(var.X,var.Y+5)};
                        g.DrawLines(rPen,hLine);
                        g.DrawLines(rPen,vLine);
                    }
                }
                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"thesecond\points.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 15, var.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            #region
            if (picTag == 2)
            {
                
                
                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList2.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        //实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                    }
                }
               
                

            }
            #endregion

            #region
            if (picTag == 3)
            {

                //重现点
                using (FileStream fs = new FileStream(@"total\arrayList3.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {
                        
                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        //实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.FromArgb(60,255,255,255));
                        //g.FillEllipse(bsh, rec);
                        
                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                        
                    }
                }
                
 

            }

            #endregion
        }
        //线聚簇分析
        private void add2_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            //ArrayList my2 = new ArrayList();
            //my2.Add(3);
            //BinaryFormatter bf = new BinaryFormatter();
            //ArrayList temp = new ArrayList();
            
            
            //using (FileStream tempFileStream = new FileStream(@"myObject.txt", FileMode.Open))
            //{
            //    temp = (ArrayList)bf.Deserialize(tempFileStream);
            //    foreach (var content in temp)
            //        my2.Add(content);            
            //}
            
            //foreach (var ddd in my2)
            //    MessageBox.Show(ddd.ToString());
            
            //FileStream fs = new FileStream(@"myObject.txt", FileMode.Create);
            //bf.Serialize(fs, my2);
            //fs.Close();
            if (picTag == 1)
            {
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line1.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        //以十字的形式显示端点
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }
                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"thesecond\lineSegments.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 15, var.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }

            }
            #region
            if (picTag == 2)
            {
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line2.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }

            }
            #endregion

            #region
            if (picTag == 3)
            {
                
                //重现线
                using (FileStream fs = new FileStream(@"total\line3.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }

            }

            #endregion

        }

        //根据需求，将聚簇分类，得到热点区域
        private void pointsProcess_Click(object sender, EventArgs e)
        {
            //记录聚簇中心点和该聚簇范围内点的数量
            Dictionary<Point, int> countPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> countLineSegments = new Dictionary<Point, int>();
            Dictionary<Point, int> countCircle = new Dictionary<Point, int>();
            #region
            //store the final cluster of points
            ArrayList theFinalResult = new ArrayList();
            //analysis the points
            using(FileStream fs = new FileStream(@"total\arrayList1.txt",FileMode.Open))
            {
                //read data from files
                ArrayList temp = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (ArrayList)bf.Deserialize(fs);
                //the midResult
                ArrayList midResult = new ArrayList(); 
                while (temp.Count != 0)
                {
                    Point p = (Point)temp[0];
                    foreach (Point var in temp)
                    {
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                            //temp.Remove(var);
                        }
                    }
                    foreach (Point var in midResult)
                        temp.Remove(var);
                    //calculate the cluster
                    //if the size of cluster is small, then ignore. the size can change
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        theFinalResult.Add(clusterPoint);
                        //记录聚簇和聚簇范围内的点数量
                        countPoint.Add(clusterPoint,midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }
                
                Graphics g = panel1.CreateGraphics();
                foreach (Point var in theFinalResult)
                {
                    Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                    //实心填充圆
                    SolidBrush bsh = new SolidBrush(Color.Red);
                    g.FillEllipse(bsh, rec);
                }
                //MessageBox.Show(theFinalResult.Count.ToString());
                
            }
            //将分析后的点聚簇进行序列化存储
            using (FileStream fsPoints = new FileStream(@"cluster\points.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fsPoints,theFinalResult);
            }
            //独立存储部分热点聚簇
            using (FileStream fs = new FileStream(@"part\points.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countPoint);
            }
            //MessageBox.Show("点聚簇数量："+theFinalResult.Count.ToString());
            Console.WriteLine("点聚簇数量：" + theFinalResult.Count.ToString());
            #endregion
            //线段聚簇分析
            ArrayList lineSegmentsResult = new ArrayList();
            using (FileStream fsLineSegment = new FileStream(@"total\line1.txt", FileMode.Open))
            {
                //读取文件中存储的线段数据
                Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                ArrayList pointsOfLine = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point,Point>)bf.Deserialize(fsLineSegment);
                //将起点和终点数据进行聚簇分析，分析成各个独立的点聚簇
                foreach (KeyValuePair<Point, Point> var in temp)
                {
                    pointsOfLine.Add((Point)var.Key);
                    pointsOfLine.Add((Point)var.Value);
                }
                //临时结果集
                ArrayList midResult = new ArrayList();
                //转换为点集合后进行聚簇分类
                while (pointsOfLine.Count != 0)
                { 
                    Point p = (Point)pointsOfLine[0];
                    foreach(Point var in pointsOfLine)
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                        }
                    //从点集合中移除已经分析过的点
                    foreach (Point var in midResult)
                        pointsOfLine.Remove(var);
                    //当中间结果集中的点数量超过一定值时，作为聚簇分析
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        lineSegmentsResult.Add(clusterPoint);
                        countLineSegments.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }
                
                //MessageBox.Show("线段聚簇数量："+lineSegmentsResult.Count.ToString());
                Console.WriteLine("线段聚簇数量：" + lineSegmentsResult.Count.ToString());
            }
            //存储分析的结果
            using (FileStream fsLine = new FileStream(@"cluster\lineSegments.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fsLine,lineSegmentsResult);
            }
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"part\lineSegments.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs,countLineSegments);
            }
            //圆的聚簇分析
            //先对圆心进行聚簇分类，然后根据半径，对圆进一步分析
            //序列化文件中存储的是圆心和外切正方形
            ArrayList circleClusterResult = new ArrayList();
            using (FileStream fs = new FileStream(@"total\circleDictionary1.txt", FileMode.Open))
            {
                Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                //store the center of circle
                ArrayList centerPoints = new ArrayList();
                foreach (KeyValuePair<Point, Rectangle> var in temp)
                    centerPoints.Add((Point)var.Key);
                ArrayList midResult = new ArrayList();
                while (centerPoints.Count != 0)
                {
                    Point p = (Point)centerPoints[0];
                    foreach(Point var in centerPoints)
                        if ((Math.Abs(var.X - p.X) < 25) && (Math.Abs(var.Y - p.Y) < 25))
                        {
                            midResult.Add(var);
                        }
                    foreach (Point var in midResult)
                        centerPoints.Remove(var);
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point currentPoint = new Point(xPoint, yPoint);
                        circleClusterResult.Add(currentPoint);
                        countCircle.Add(currentPoint, midResult.Count);
                    }
                    midResult.Clear();
                }  
            }
            using (FileStream fs = new FileStream(@"cluster\circle.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs,circleClusterResult);
            }
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"part\circle.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countCircle);
            }
            //MessageBox.Show("圆聚簇数量："+circleClusterResult.Count.ToString());
            Console.WriteLine("圆聚簇数量：" + circleClusterResult.Count.ToString());
        }

        //测试组合种类
        private void testCombine_Click(object sender, EventArgs e)
        {
            //点序列
            ArrayList myCluster = new ArrayList();
            using(FileStream fs = new FileStream(@"cluster\points.txt",FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                myCluster = (ArrayList)bf.Deserialize(fs);
            }
            Console.WriteLine("聚簇的数量："+myCluster.Count);
            //存储结果
            ArrayList combineResult = new ArrayList();       
            ArrayList flagArray = new ArrayList();
            //初始化标志数组
            for (int ix = 0; ix < myCluster.Count; ++ix)
            {
                if (ix < 2)
                    flagArray.Add(1);
                else
                    flagArray.Add(0);
            }
            //输出最初的序列
            for (int i = 0; i < myCluster.Count; ++i)
            {
                if ((int)flagArray[i] == 1)
                    //Console.Write(myCluster[i]);
                    combineResult.Add((Point)myCluster[i]);
            }
            //结束标志
            bool isEnd = false;
            while (!isEnd)
            {
                
                //记录10序列的位置，即1的位置
                int pose = 0;
                //是否查找到10序列
                bool isFind = false;
                //查找10序列，然后交换位置
                for (int i = 0; i < (flagArray.Count-1); ++i)
                {
                    if (((int)flagArray[i] == 1) && ((int)flagArray[i + 1] == 0))
                    {
                        flagArray[i] = 0;
                        flagArray[i + 1] = 1;
                        isFind = true;
                        pose = i;
                    }
                    if (isFind)
                        break;               
                }
                if (isFind)
                {
                    //将pose之前的1全部移到左边
                    //sum统计pose之前1的个数
                    int sum = 0;
                    for (int i = 0; i < pose; ++i)
                    {
                        if ((int)flagArray[i] == 1)
                            sum++;
                    }
                    //判断左边的1是否需要移动
                    bool isMove = false;
                    for (int i = 0; i < sum; ++i)
                    {
                        //如果最左边的sum个元素中有0存在，则需要移动
                        if ((int)flagArray[i] == 0)
                            isMove = true;
                    }
                    //移动元素
                    if (isMove)
                    {
                        for (int i = 0; i < pose; ++i)
                        {
                            if (i < sum)
                                flagArray[i] = 1;
                            else
                                flagArray[i] = 0;
                        }
                    }
                    //根据交换的结果，打印结果
                    for (int i = 0; i < flagArray.Count; ++i)
                    {
                        if ((int)flagArray[i] == 1)
                            //Console.Write(myCluster[i]);
                            combineResult.Add((Point)myCluster[i]);
                    }                  
                }
                else
                    //如果没有找到10序列，查找结束
                    isEnd = true;
            }
            Console.WriteLine("组合的数量：" + combineResult.Count / 2);
            int rowFlag = 0;
            foreach (Point var in combineResult)
            {
                rowFlag++;
                Console.Write(var);
                if (rowFlag % 2 == 0)
                    Console.WriteLine();
            }       
        }

        //破解
        private void hack_Click(object sender, EventArgs e)
        {
            //点的聚簇
            ArrayList pointList = new ArrayList();
            //线的端点聚簇
            ArrayList lineSegmentList = new ArrayList();
            //圆心聚簇
            ArrayList circleList = new ArrayList();
            //读取圆心聚簇
            using (FileStream fs = new FileStream(@"cluster\points.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                pointList = (ArrayList)bf.Deserialize(fs);
            }
            //读取线段的聚簇
            using (FileStream fs = new FileStream(@"cluster\lineSegments.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                lineSegmentList = (ArrayList)bf.Deserialize(fs);
            }
            //读取圆心的聚簇
            using (FileStream fs = new FileStream(@"cluster\circle.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                circleList = (ArrayList)bf.Deserialize(fs);
            }
            bool isDown = false;
            MessageBox.Show(storeList.Count.ToString()+":"+line.Count.ToString()+":"+circle.Count.ToString());
            if (storeList.Count != 0 && !isDown)
            {
                //判断是否在聚簇中，如果不在聚簇中，则破解失败
                for (int i = 0; i < storeList.Count; ++i)
                {
                    isDown = false;
                    foreach (Point var in pointList)
                    {
                        if (Math.Abs(((Point)storeList[i]).X - var.X) < 15 && Math.Abs(((Point)storeList[i]).Y - var.Y) < 15)
                            isDown = true;
                    }
                    if (!isDown)
                        break;
                }
            }
            if (line.Count != 0 && !isDown)
            {   
                ArrayList lineTrans = new ArrayList();
                foreach (KeyValuePair<Point, Point> var in line)
                {
                    lineTrans.Add((Point)var.Key);
                    lineTrans.Add((Point)var.Value);
                }
                for (int i = 0; i < lineTrans.Count; ++i)
                {
                    isDown = false;
                    foreach (Point var in lineSegmentList)
                    {
                        if (Math.Abs(((Point)lineTrans[i]).X - var.X) > 15 && Math.Abs(((Point)lineTrans[i]).Y - var.Y) > 15)
                            isDown = true;
                    }
                    if (!isDown)
                        break;
                }
            }
            if (circle.Count != 0 && !isDown)
            {
                ArrayList circleTrans = new ArrayList();
                //只对圆心处理
                foreach (KeyValuePair<Point, Rectangle> var in circle)
                    circleTrans.Add((Point)var.Key);
                for (int i = 0; i < circleTrans.Count; ++i)
                {
                    isDown = false;
                    foreach (Point var in circleList)
                    {
                        if (Math.Abs(((Point)circleTrans[i]).X - var.X) > 30 && Math.Abs(((Point)circleTrans[i]).Y - var.Y) > 30)
                            isDown = false;
                    }
                    if (!isDown)
                        break;
                }
            }
            if (isDown)
            {
                MessageBox.Show("破解成功");
                using (FileStream fs = new FileStream(@"hackResult\result.txt", FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("1");
                    sw.Close();
                }
            }
            else
            {
                MessageBox.Show("破解失败");
                using (FileStream fs = new FileStream(@"hackResult\result.txt", FileMode.Append))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.WriteLine("0");
                    sw.Close();
                }
            }
            //清空列表中的内容
            circle.Clear();
            storeList.Clear();
            lineList.Clear();
            circleList.Clear();
            panel1.Refresh();
            line.Clear();
        }
      
        //点聚簇显示
        private void pointCluster_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            ArrayList pointArray = new ArrayList();
            using (FileStream fs = new FileStream(@"cluster\points.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                pointArray = (ArrayList)bf.Deserialize(fs);
            }
            //聚簇结果显示
            Graphics g = panel1.CreateGraphics();
            foreach (Point var in pointArray)
            {
                Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                //实心填充圆
                SolidBrush rsh = new SolidBrush(Color.Red);
                g.FillEllipse(rsh, rec);
            }
            
        }
        //线段聚簇显示
        private void lineCluster_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            ArrayList lineSegment = new ArrayList();
            using (FileStream fs = new FileStream(@"cluster\lineSegments.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                lineSegment = (ArrayList)bf.Deserialize(fs);
            }
            //聚簇结果显示
            Graphics g = panel1.CreateGraphics();
            foreach (Point var in lineSegment)
            {
                Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                //实心填充圆
                SolidBrush rsh = new SolidBrush(Color.Red);
                g.FillEllipse(rsh, rec);
            }
            
        }
        //圆聚簇显示
        private void circleCluster_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            ArrayList circleArray = new ArrayList();
            using (FileStream fs = new FileStream(@"cluster\circle.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                circleArray = (ArrayList)bf.Deserialize(fs);
            }
            //聚簇结果显示
            Graphics g = panel1.CreateGraphics();
            foreach (Point var in circleArray)
            {
                Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                //实心填充圆
                SolidBrush rsh = new SolidBrush(Color.Red);
                g.FillEllipse(rsh, rec);
            }    
        }
        //部分聚簇
        private void partCluster_Click(object sender, EventArgs e)
        {
            //读取部分聚簇
            Dictionary<Point, int> partPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> partLineSegment = new Dictionary<Point, int>();
            Dictionary<Point, int> partCircle = new Dictionary<Point, int>();
            //读取点聚簇
            using (FileStream fs = new FileStream(@"part\points.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partPoint = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            using (FileStream fs = new FileStream(@"part\lineSegments.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partLineSegment = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            using (FileStream fs = new FileStream(@"part\circle.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partCircle = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            //对dictionary中的键值对按照值进行排序，利用了linq的知识
            partPoint = (from entry in partPoint
                         orderby entry.Value descending
                         select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            partLineSegment = (from entry in partLineSegment
                               orderby entry.Value descending
                               select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            partCircle = (from entry in partCircle
                          orderby entry.Value descending
                          select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            //暂时存储聚簇中的前15个点
            ArrayList tempPoint = new ArrayList();
            ArrayList tempLineSegment = new ArrayList();
            ArrayList tempCircle = new ArrayList();
            int i = 0;
            foreach (KeyValuePair<Point, int> var in partPoint)
            {
                if (i < 15)
                    tempPoint.Add((Point)var.Key);
                ++i;
            }
            i = 0;
            foreach (KeyValuePair<Point, int> var in partLineSegment)
            {
                if (i < 15)
                    tempLineSegment.Add((Point)var.Key);
                ++i;
            }
            i = 0;
            foreach (KeyValuePair<Point, int> var in partCircle)
            {
                if (i < 15)
                    tempCircle.Add((Point)var.Key);
                ++i;
            }
            //存储部分热点聚簇
            using (FileStream fs = new FileStream(@"thesecond\points.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempPoint);
            }
            using (FileStream fs = new FileStream(@"thesecond\lineSegments.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempLineSegment);
            }
            using (FileStream fs = new FileStream(@"thesecond\circle.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempCircle);
            }
            Console.WriteLine("点聚簇的全部数量：" + partPoint.Count);
            Console.WriteLine("线聚簇的全部数量：" + partLineSegment.Count);
            Console.WriteLine("圆聚簇的全部数量：" + partCircle.Count);
            
            foreach (KeyValuePair<Point, int> var in partPoint)
                Console.WriteLine(var.Key + ":" + var.Value);
            Console.WriteLine("-----------------------------");
            foreach (KeyValuePair<Point, int> var in partLineSegment)
                Console.WriteLine(var.Key + ":" + var.Value);
            Console.WriteLine("-----------------------------");
            foreach (KeyValuePair<Point, int> var in partCircle)
                Console.WriteLine(var.Key + ":" + var.Value);
            
        }

        //数据分析
        private void processResult_Click(object sender, EventArgs e)
        {
            String resultStr = string.Empty;
            using (FileStream fs = new FileStream(@"hackResult\result.txt", FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(fs,Encoding.UTF8))
                { 
                    while (!sr.EndOfStream)
                    {
                        resultStr += sr.ReadLine();
                    }
                    Console.WriteLine("结果序列：" + resultStr);
                }
            }
            int success = 0;
            for (int i = 0; i < resultStr.Count(); ++i)
            {
                if (resultStr[i].Equals('1'))
                    ++success;
            }
            double rate = (double)success / (double)(resultStr.Length);
            Console.WriteLine("破解总数：" + resultStr.Count());
            Console.WriteLine("破解数量：" + success);
            Console.WriteLine("成功率：" + rate);
        }

        private void labStudy_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            if (picTag == 1)
            {
                //重现点
                using (FileStream fs = new FileStream(@"labStudy\arrayList1.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {

                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        ////实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);

                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                    }
                }

                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"labOrder\points.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 15, var.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 2)
            {
                using (FileStream fs = new FileStream(@"labStudy\arrayList2.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {

                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        ////实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);

                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                    }
                }
             
                //将聚簇包含在区域中
                //ArrayList pointArray = new ArrayList();
                Dictionary<Point, int> partPoint = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\points2.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //pointArray = (ArrayList)bf.Deserialize(fs);
                    partPoint = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partPoint = (from entry in partPoint
                             orderby entry.Value descending
                             select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point,int> var in partPoint)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 15, var.Key.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 3)
            {
                using (FileStream fs = new FileStream(@"labStudy\arrayList3.txt", FileMode.Open))
                {
                    Graphics g = panel1.CreateGraphics();
                    Pen rPen = new Pen(Color.Red, 1);
                    ArrayList temp = new ArrayList();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (ArrayList)bf.Deserialize(fs);
                    foreach (Point var in temp)
                    {

                        //Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                        ////实心填充圆
                        //SolidBrush bsh = new SolidBrush(Color.Red);
                        //g.FillEllipse(bsh, rec);

                        Point[] hLine = { new Point(var.X - 5, var.Y), new Point(var.X + 5, var.Y) };
                        Point[] vLine = { new Point(var.X, var.Y - 5), new Point(var.X, var.Y + 5) };
                        g.DrawLines(rPen, hLine);
                        g.DrawLines(rPen, vLine);
                    }
                }
                
                //将聚簇包含在区域中
                Dictionary<Point, int> partPoint = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\points3.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    partPoint = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partPoint = (from entry in partPoint
                             orderby entry.Value descending
                             select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point, int> var in partPoint)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 15, var.Key.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
        }

        private void labCluster_Click(object sender, EventArgs e)
        {
            //读取部分聚簇
            Dictionary<Point, int> partPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> partLineSegment = new Dictionary<Point, int>();
            Dictionary<Point, int> partCircle = new Dictionary<Point, int>();
            //读取点聚簇
            using (FileStream fs = new FileStream(@"labCluster\points.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partPoint = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            using (FileStream fs = new FileStream(@"labCluster\lineSegments.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partLineSegment = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            using (FileStream fs = new FileStream(@"labCluster\circle.txt", FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                partCircle = (Dictionary<Point, int>)bf.Deserialize(fs);
            }
            //对dictionary中的键值对按照值进行排序，利用了linq的知识
            partPoint = (from entry in partPoint
                         orderby entry.Value descending
                         select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            partLineSegment = (from entry in partLineSegment
                               orderby entry.Value descending
                               select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            partCircle = (from entry in partCircle
                          orderby entry.Value descending
                          select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
            //暂时存储聚簇中的前15个点
            ArrayList tempPoint = new ArrayList();
            ArrayList tempLineSegment = new ArrayList();
            ArrayList tempCircle = new ArrayList();
            int i = 0;
            foreach (KeyValuePair<Point, int> var in partPoint)
            {
                if (i < 15)
                    tempPoint.Add((Point)var.Key);
                ++i;
            }
            i = 0;
            foreach (KeyValuePair<Point, int> var in partLineSegment)
            {
                if (i < 15)
                    tempLineSegment.Add((Point)var.Key);
                ++i;
            }
            i = 0;
            foreach (KeyValuePair<Point, int> var in partCircle)
            {
                if (i < 15)
                    tempCircle.Add((Point)var.Key);
                ++i;
            }
            //存储部分热点聚簇
            using (FileStream fs = new FileStream(@"labOrder\points.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempPoint);
            }
            using (FileStream fs = new FileStream(@"labOrder\lineSegments.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempLineSegment);
            }
            using (FileStream fs = new FileStream(@"labOrder\circle.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, tempCircle);
            }
            Console.WriteLine("点聚簇的全部数量：" + partPoint.Count);
            Console.WriteLine("线聚簇的全部数量：" + partLineSegment.Count);
            Console.WriteLine("圆聚簇的全部数量：" + partCircle.Count);

            foreach (KeyValuePair<Point, int> var in partPoint)
                Console.WriteLine(var.Key + ":" + var.Value);
            Console.WriteLine("-----------------------------");
            foreach (KeyValuePair<Point, int> var in partLineSegment)
                Console.WriteLine(var.Key + ":" + var.Value);
            Console.WriteLine("-----------------------------");
            foreach (KeyValuePair<Point, int> var in partCircle)
                Console.WriteLine(var.Key + ":" + var.Value);
        }

        private void labAllStudy_Click(object sender, EventArgs e)
        {
            //记录聚簇中心点和该聚簇范围内点的数量
            Dictionary<Point, int> countPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> countLineSegments = new Dictionary<Point, int>();
            Dictionary<Point, int> countCircle = new Dictionary<Point, int>();
            #region
            //store the final cluster of points
            ArrayList theFinalResult = new ArrayList();
            //analysis the points
            using (FileStream fs = new FileStream(@"labStudy\arrayList1.txt", FileMode.Open))
            {
                //read data from files
                ArrayList temp = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (ArrayList)bf.Deserialize(fs);
                //the midResult
                ArrayList midResult = new ArrayList();
                while (temp.Count != 0)
                {
                    Point p = (Point)temp[0];
                    foreach (Point var in temp)
                    {
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                            //temp.Remove(var);
                        }
                    }
                    foreach (Point var in midResult)
                        temp.Remove(var);
                    //calculate the cluster
                    //if the size of cluster is small, then ignore. the size can change
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        theFinalResult.Add(clusterPoint);
                        //记录聚簇和聚簇范围内的点数量
                        countPoint.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                Graphics g = panel1.CreateGraphics();
                foreach (Point var in theFinalResult)
                {
                    Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                    //实心填充圆
                    SolidBrush bsh = new SolidBrush(Color.Red);
                    g.FillEllipse(bsh, rec);
                }
                //MessageBox.Show(theFinalResult.Count.ToString());

            }
            
            //独立存储部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\points.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countPoint);
            }
            //MessageBox.Show("点聚簇数量："+theFinalResult.Count.ToString());
            Console.WriteLine("点聚簇数量：" + theFinalResult.Count.ToString());
            #endregion
            //线段聚簇分析
            ArrayList lineSegmentsResult = new ArrayList();
            using (FileStream fsLineSegment = new FileStream(@"labStudy\line1.txt", FileMode.Open))
            {
                //读取文件中存储的线段数据
                Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                ArrayList pointsOfLine = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Point>)bf.Deserialize(fsLineSegment);
                //将起点和终点数据进行聚簇分析，分析成各个独立的点聚簇
                foreach (KeyValuePair<Point, Point> var in temp)
                {
                    pointsOfLine.Add((Point)var.Key);
                    pointsOfLine.Add((Point)var.Value);
                }
                //临时结果集
                ArrayList midResult = new ArrayList();
                //转换为点集合后进行聚簇分类
                while (pointsOfLine.Count != 0)
                {
                    Point p = (Point)pointsOfLine[0];
                    foreach (Point var in pointsOfLine)
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                        }
                    //从点集合中移除已经分析过的点
                    foreach (Point var in midResult)
                        pointsOfLine.Remove(var);
                    //当中间结果集中的点数量超过一定值时，作为聚簇分析
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        lineSegmentsResult.Add(clusterPoint);
                        countLineSegments.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                //MessageBox.Show("线段聚簇数量："+lineSegmentsResult.Count.ToString());
                Console.WriteLine("线段聚簇数量：" + lineSegmentsResult.Count.ToString());
            }
            
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\lineSegments.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countLineSegments);
            }
            //圆的聚簇分析
            //先对圆心进行聚簇分类，然后根据半径，对圆进一步分析
            //序列化文件中存储的是圆心和外切正方形
            ArrayList circleClusterResult = new ArrayList();
            using (FileStream fs = new FileStream(@"labStudy\circleDictionary1.txt", FileMode.Open))
            {
                Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                //store the center of circle
                ArrayList centerPoints = new ArrayList();
                foreach (KeyValuePair<Point, Rectangle> var in temp)
                    centerPoints.Add((Point)var.Key);
                ArrayList midResult = new ArrayList();
                while (centerPoints.Count != 0)
                {
                    Point p = (Point)centerPoints[0];
                    foreach (Point var in centerPoints)
                        if ((Math.Abs(var.X - p.X) < 35) && (Math.Abs(var.Y - p.Y) < 35))
                        {
                            midResult.Add(var);
                        }
                    foreach (Point var in midResult)
                        centerPoints.Remove(var);
                    if (midResult.Count > 1)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point currentPoint = new Point(xPoint, yPoint);
                        circleClusterResult.Add(currentPoint);
                        countCircle.Add(currentPoint, midResult.Count);
                    }
                    midResult.Clear();
                }
            }
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\circle.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countCircle);
            }
            //MessageBox.Show("圆聚簇数量："+circleClusterResult.Count.ToString());
            Console.WriteLine("圆聚簇数量：" + circleClusterResult.Count.ToString());
        }

        private void labLineSegments_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            if (picTag == 1)
            {
                //重现线
                using (FileStream fs = new FileStream(@"labStudy\line1.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        //以十字的形式显示端点
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }
                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"labOrder\lineSegments.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 15, var.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 2)
            {
                //重现线
                using (FileStream fs = new FileStream(@"labStudy\line2.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        //以十字的形式显示端点
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }

                //将聚簇包含在区域中
                //ArrayList pointArray = new ArrayList();
                Dictionary<Point, int> partLine = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\lineSegments2.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //pointArray = (ArrayList)bf.Deserialize(fs);
                    partLine = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partLine = (from entry in partLine
                                   orderby entry.Value descending
                                   select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point,int> var in partLine)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 15, var.Key.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 3)
            {
                //重现线
                using (FileStream fs = new FileStream(@"labStudy\line3.txt", FileMode.Open))
                {
                    Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Point>)bf.Deserialize(fs);
                    Point beginP = new Point();
                    Point endP = new Point();
                    Graphics g = panel1.CreateGraphics();

                    Pen rPen = new Pen(Color.Red, 1);
                    foreach (KeyValuePair<Point, Point> var in temp)
                    {

                        beginP = (Point)var.Key;
                        endP = (Point)var.Value;
                        //起点
                        Rectangle bRec = new Rectangle(beginP.X - 5, beginP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, bRec);
                        //终点
                        Rectangle eRec = new Rectangle(endP.X - 5, endP.Y - 5, 10, 10);
                        //g.DrawEllipse(rPen, eRec);

                        //填充起点和终点
                        SolidBrush sb = new SolidBrush(Color.Red);
                        //g.FillEllipse(sb, bRec);
                        //g.FillEllipse(sb, eRec);
                        //g.DrawLine(rPen, beginP, endP);
                        //以十字的形式显示端点
                        Point[] hBeginPoint = { new Point(beginP.X - 5, beginP.Y), new Point(beginP.X + 5, beginP.Y) };
                        Point[] vBeginPoint = { new Point(beginP.X, beginP.Y - 5), new Point(beginP.X, beginP.Y + 5) };
                        g.DrawLines(rPen, hBeginPoint);
                        g.DrawLines(rPen, vBeginPoint);
                        Point[] hEndPoint = { new Point(endP.X - 5, endP.Y), new Point(endP.X + 5, endP.Y) };
                        Point[] vEndPoint = { new Point(endP.X, endP.Y - 5), new Point(endP.X, endP.Y + 5) };
                        g.DrawLines(rPen, hEndPoint);
                        g.DrawLines(rPen, vEndPoint);
                    }
                }
                //将聚簇包含在区域中
                
                Dictionary<Point, int> partLine = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\lineSegments3.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    partLine = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partLine = (from entry in partLine
                            orderby entry.Value descending
                            select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point,int> var in partLine)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 15, var.Key.Y - 15, 30, 30);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
        }

        private void labCircle_Click(object sender, EventArgs e)
        {
            panel1.Refresh();
            if (picTag == 1)
            {
                //重现圆
                using (FileStream fs = new FileStream(@"labStudy\circledictionary1.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb,rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);

                    }
                }
                //将聚簇包含在区域中
                ArrayList pointArray = new ArrayList();
                using (FileStream fs = new FileStream(@"labOrder\circle.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    pointArray = (ArrayList)bf.Deserialize(fs);
                }
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (Point var in pointArray)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.X - 20, var.Y - 20, 40, 40);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 2)
            {
                //重现圆
                using (FileStream fs = new FileStream(@"labStudy\circledictionary2.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb,rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);

                    }
                }
                //将聚簇包含在区域中
                //ArrayList pointArray = new ArrayList();
                Dictionary<Point, int> partCircle = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\circle2.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    partCircle = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partCircle = (from entry in partCircle
                              orderby entry.Value descending
                              select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point,int> var in partCircle)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 20, var.Key.Y - 20, 40, 40);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
            if (picTag == 3)
            {
                //重现圆
                using (FileStream fs = new FileStream(@"labStudy\circledictionary3.txt", FileMode.Open))
                {
                    Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                    BinaryFormatter bf = new BinaryFormatter();
                    temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                    foreach (KeyValuePair<Point, Rectangle> var in temp)
                    {
                        Graphics g = panel1.CreateGraphics();
                        Pen bPen = new Pen(Color.Red, 1);
                        //g.DrawEllipse(bPen, var.Value);

                        SolidBrush sb = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
                        Rectangle rec = (Rectangle)var.Value;
                        //g.FillEllipse(sb,rec);
                        Point tempPoint = (Point)var.Key;
                        Point[] hPoint = { new Point(tempPoint.X - 5, tempPoint.Y), new Point(tempPoint.X + 5, tempPoint.Y) };
                        Point[] vPoint = { new Point(tempPoint.X, tempPoint.Y - 5), new Point(tempPoint.X, tempPoint.Y + 5) };
                        g.DrawLines(bPen, hPoint);
                        g.DrawLines(bPen, vPoint);

                    }
                }
                //将聚簇包含在区域中
                //ArrayList pointArray = new ArrayList();
                Dictionary<Point, int> partCircle = new Dictionary<Point, int>();
                using (FileStream fs = new FileStream(@"labCluster\circle3.txt", FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    partCircle = (Dictionary<Point, int>)bf.Deserialize(fs);
                }
                partCircle = (from entry in partCircle
                              orderby entry.Value descending
                              select entry).ToDictionary(pair => pair.Key, pair => pair.Value);
                //聚簇结果显示
                Graphics gPoint = panel1.CreateGraphics();
                int i = 0;
                foreach (KeyValuePair<Point,int> var in partCircle)
                {
                    if (i < 5)
                    {
                        Rectangle rec = new Rectangle(var.Key.X - 20, var.Key.Y - 20, 40, 40);
                        //实心填充圆
                        SolidBrush rsh = new SolidBrush(Color.FromArgb(100, 255, 0, 0));
                        gPoint.FillEllipse(rsh, rec);
                    }
                    ++i;
                }
            }
        }

        private void pic2Cluster_Click(object sender, EventArgs e)
        {
            //记录聚簇中心点和该聚簇范围内点的数量
            Dictionary<Point, int> countPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> countLineSegments = new Dictionary<Point, int>();
            Dictionary<Point, int> countCircle = new Dictionary<Point, int>();
            #region
            //store the final cluster of points
            ArrayList theFinalResult = new ArrayList();
            //analysis the points
            using (FileStream fs = new FileStream(@"labStudy\arrayList2.txt", FileMode.Open))
            {
                //read data from files
                ArrayList temp = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (ArrayList)bf.Deserialize(fs);
                //the midResult
                ArrayList midResult = new ArrayList();
                while (temp.Count != 0)
                {
                    Point p = (Point)temp[0];
                    foreach (Point var in temp)
                    {
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                            //temp.Remove(var);
                        }
                    }
                    foreach (Point var in midResult)
                        temp.Remove(var);
                    //calculate the cluster
                    //if the size of cluster is small, then ignore. the size can change
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        theFinalResult.Add(clusterPoint);
                        //记录聚簇和聚簇范围内的点数量
                        countPoint.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                Graphics g = panel1.CreateGraphics();
                foreach (Point var in theFinalResult)
                {
                    Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                    //实心填充圆
                    SolidBrush bsh = new SolidBrush(Color.Red);
                    g.FillEllipse(bsh, rec);
                }
                //MessageBox.Show(theFinalResult.Count.ToString());

            }

            //独立存储部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\points2.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countPoint);
            }
            //MessageBox.Show("点聚簇数量："+theFinalResult.Count.ToString());
            Console.WriteLine("点聚簇数量：" + theFinalResult.Count.ToString());
            #endregion
            //线段聚簇分析
            ArrayList lineSegmentsResult = new ArrayList();
            using (FileStream fsLineSegment = new FileStream(@"labStudy\line2.txt", FileMode.Open))
            {
                //读取文件中存储的线段数据
                Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                ArrayList pointsOfLine = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Point>)bf.Deserialize(fsLineSegment);
                //将起点和终点数据进行聚簇分析，分析成各个独立的点聚簇
                foreach (KeyValuePair<Point, Point> var in temp)
                {
                    pointsOfLine.Add((Point)var.Key);
                    pointsOfLine.Add((Point)var.Value);
                }
                //临时结果集
                ArrayList midResult = new ArrayList();
                //转换为点集合后进行聚簇分类
                while (pointsOfLine.Count != 0)
                {
                    Point p = (Point)pointsOfLine[0];
                    foreach (Point var in pointsOfLine)
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                        }
                    //从点集合中移除已经分析过的点
                    foreach (Point var in midResult)
                        pointsOfLine.Remove(var);
                    //当中间结果集中的点数量超过一定值时，作为聚簇分析
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        lineSegmentsResult.Add(clusterPoint);
                        countLineSegments.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                //MessageBox.Show("线段聚簇数量："+lineSegmentsResult.Count.ToString());
                Console.WriteLine("线段聚簇数量：" + lineSegmentsResult.Count.ToString());
            }

            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\lineSegments2.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countLineSegments);
            }
            //圆的聚簇分析
            //先对圆心进行聚簇分类，然后根据半径，对圆进一步分析
            //序列化文件中存储的是圆心和外切正方形
            ArrayList circleClusterResult = new ArrayList();
            using (FileStream fs = new FileStream(@"labStudy\circleDictionary2.txt", FileMode.Open))
            {
                Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                //store the center of circle
                ArrayList centerPoints = new ArrayList();
                foreach (KeyValuePair<Point, Rectangle> var in temp)
                    centerPoints.Add((Point)var.Key);
                ArrayList midResult = new ArrayList();
                while (centerPoints.Count != 0)
                {
                    Point p = (Point)centerPoints[0];
                    foreach (Point var in centerPoints)
                        if ((Math.Abs(var.X - p.X) < 35) && (Math.Abs(var.Y - p.Y) < 35))
                        {
                            midResult.Add(var);
                        }
                    foreach (Point var in midResult)
                        centerPoints.Remove(var);
                    if (midResult.Count > 1)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point currentPoint = new Point(xPoint, yPoint);
                        circleClusterResult.Add(currentPoint);
                        countCircle.Add(currentPoint, midResult.Count);
                    }
                    midResult.Clear();
                }
            }
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\circle2.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countCircle);
            }
            //MessageBox.Show("圆聚簇数量："+circleClusterResult.Count.ToString());
            Console.WriteLine("圆聚簇数量：" + circleClusterResult.Count.ToString());
        }

        private void pic3Cluster_Click(object sender, EventArgs e)
        {
            //记录聚簇中心点和该聚簇范围内点的数量
            Dictionary<Point, int> countPoint = new Dictionary<Point, int>();
            Dictionary<Point, int> countLineSegments = new Dictionary<Point, int>();
            Dictionary<Point, int> countCircle = new Dictionary<Point, int>();
            #region
            //store the final cluster of points
            ArrayList theFinalResult = new ArrayList();
            //analysis the points
            using (FileStream fs = new FileStream(@"labStudy\arrayList3.txt", FileMode.Open))
            {
                //read data from files
                ArrayList temp = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (ArrayList)bf.Deserialize(fs);
                //the midResult
                ArrayList midResult = new ArrayList();
                while (temp.Count != 0)
                {
                    Point p = (Point)temp[0];
                    foreach (Point var in temp)
                    {
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                            //temp.Remove(var);
                        }
                    }
                    foreach (Point var in midResult)
                        temp.Remove(var);
                    //calculate the cluster
                    //if the size of cluster is small, then ignore. the size can change
                    if (midResult.Count > 3)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        theFinalResult.Add(clusterPoint);
                        //记录聚簇和聚簇范围内的点数量
                        countPoint.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                Graphics g = panel1.CreateGraphics();
                foreach (Point var in theFinalResult)
                {
                    Rectangle rec = new Rectangle(var.X - 10, var.Y - 10, 20, 20);
                    //实心填充圆
                    SolidBrush bsh = new SolidBrush(Color.Red);
                    g.FillEllipse(bsh, rec);
                }
                //MessageBox.Show(theFinalResult.Count.ToString());

            }

            //独立存储部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\points3.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countPoint);
            }
            //MessageBox.Show("点聚簇数量："+theFinalResult.Count.ToString());
            Console.WriteLine("点聚簇数量：" + theFinalResult.Count.ToString());
            #endregion
            //线段聚簇分析
            ArrayList lineSegmentsResult = new ArrayList();
            using (FileStream fsLineSegment = new FileStream(@"labStudy\line3.txt", FileMode.Open))
            {
                //读取文件中存储的线段数据
                Dictionary<Point, Point> temp = new Dictionary<Point, Point>();
                ArrayList pointsOfLine = new ArrayList();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Point>)bf.Deserialize(fsLineSegment);
                //将起点和终点数据进行聚簇分析，分析成各个独立的点聚簇
                foreach (KeyValuePair<Point, Point> var in temp)
                {
                    pointsOfLine.Add((Point)var.Key);
                    pointsOfLine.Add((Point)var.Value);
                }
                //临时结果集
                ArrayList midResult = new ArrayList();
                //转换为点集合后进行聚簇分类
                while (pointsOfLine.Count != 0)
                {
                    Point p = (Point)pointsOfLine[0];
                    foreach (Point var in pointsOfLine)
                        if ((Math.Abs(var.X - p.X) < 10) && (Math.Abs(var.Y - p.Y) < 10))
                        {
                            midResult.Add(var);
                        }
                    //从点集合中移除已经分析过的点
                    foreach (Point var in midResult)
                        pointsOfLine.Remove(var);
                    //当中间结果集中的点数量超过一定值时，作为聚簇分析
                    if (midResult.Count > 2)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point clusterPoint = new Point(xPoint, yPoint);
                        //add the average value to theFinalResult
                        lineSegmentsResult.Add(clusterPoint);
                        countLineSegments.Add(clusterPoint, midResult.Count);
                    }
                    //clear the midResult list
                    midResult.Clear();
                }

                //MessageBox.Show("线段聚簇数量："+lineSegmentsResult.Count.ToString());
                Console.WriteLine("线段聚簇数量：" + lineSegmentsResult.Count.ToString());
            }

            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\lineSegments3.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countLineSegments);
            }
            //圆的聚簇分析
            //先对圆心进行聚簇分类，然后根据半径，对圆进一步分析
            //序列化文件中存储的是圆心和外切正方形
            ArrayList circleClusterResult = new ArrayList();
            using (FileStream fs = new FileStream(@"labStudy\circleDictionary3.txt", FileMode.Open))
            {
                Dictionary<Point, Rectangle> temp = new Dictionary<Point, Rectangle>();
                BinaryFormatter bf = new BinaryFormatter();
                temp = (Dictionary<Point, Rectangle>)bf.Deserialize(fs);
                //store the center of circle
                ArrayList centerPoints = new ArrayList();
                foreach (KeyValuePair<Point, Rectangle> var in temp)
                    centerPoints.Add((Point)var.Key);
                ArrayList midResult = new ArrayList();
                while (centerPoints.Count != 0)
                {
                    Point p = (Point)centerPoints[0];
                    foreach (Point var in centerPoints)
                        if ((Math.Abs(var.X - p.X) < 35) && (Math.Abs(var.Y - p.Y) < 35))
                        {
                            midResult.Add(var);
                        }
                    foreach (Point var in midResult)
                        centerPoints.Remove(var);
                    if (midResult.Count > 1)
                    {
                        int xPoint = 0;
                        int yPoint = 0;
                        foreach (Point var in midResult)
                        {
                            xPoint += var.X;
                            yPoint += var.Y;
                        }
                        //calculate the average value
                        xPoint = xPoint / midResult.Count;
                        yPoint = yPoint / midResult.Count;
                        Point currentPoint = new Point(xPoint, yPoint);
                        circleClusterResult.Add(currentPoint);
                        countCircle.Add(currentPoint, midResult.Count);
                    }
                    midResult.Clear();
                }
            }
            //部分热点聚簇
            using (FileStream fs = new FileStream(@"labCluster\circle3.txt", FileMode.Create))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, countCircle);
            }
            //MessageBox.Show("圆聚簇数量："+circleClusterResult.Count.ToString());
            Console.WriteLine("圆聚簇数量：" + circleClusterResult.Count.ToString());
        }
    }
}
