using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace 扫雷
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<List<int>> inIndex = new List<List<int>>
        {
            new List<int>{0,1},
            new List<int>{1,0},
            new List<int>{-1,0},
            new List<int>{0,-1},
        };
        private List<List<int>> ints = new List<List<int>>
        {
            new List<int>{0,1},
            new List<int>{1,0},
            new List<int>{-1,0},
            new List<int>{0,-1},
            new List<int>{-1,-1},
            new List<int>{1,1},
            new List<int>{-1,1},
            new List<int>{1,-1},
        };

        private int OnClickNum = 0;
        private int Win = 0;
        private int InitNum = 0;
        private int[,] map = new int[16,30];
        private int[,] maped = new int[16, 30];
        private bool[,] ToWin = new bool[16, 30];
        private List<List<int>> DfsNum = new List<List<int>>();
        private List<bool> bools = new List<bool>();
        private HashSet<int> set = new HashSet<int>();
        private int SumTime = 0;
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitLey();
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // 设置间隔时间为1秒
            timer.Tick += Timer_Tick; // 注册Tick事件
            timer.Start(); // 启动定时器
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (OnClickNum == 1)SumTime++;
            ContSJ.Content = "时间:"+SumTime;
        }

        private void InitMap()
        {
            int a = 0;
            for(int i = 0; i < 16; ++i)
            {
                for(int j = 0; j < 30; ++j)
                {
                    map[i,j] = bools[a]?0:-1;
                    a++;
                }
            }
            for (int i = 0; i < 16; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    int b = 0;
                    if (map[i,j] == 0)
                    {
                        for (int k = 0; k < 8; ++k)
                        {
                            if (InSetIt(i + ints[k][0],j+ ints[k][1]) &&
                                map[i + ints[k][0], j + ints[k][1]]==-1) 
                                b++;
                        }
                        map[i, j] = b;
                    }
                }
            }
            for (int i = 0; i < 16; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    maped[i, j] = map[i,j] ;
                    ToWin[i, j] = false;
                }
            }

        }

        private void InitLey()
        {
            for (int i = 0; i < 16; ++i)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                LeiQu.RowDefinitions.Add(row);
            }

            for (int i = 0; i < 30; ++i)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                LeiQu.ColumnDefinitions.Add(column);
            }
        }

        private void InitLeiQu()
        {
            
            OnClickNum = 0;
            SumTime = 0;
            ContFS.Content = "分数:" + 0;
            Win = 0;
            ToWin = new bool[16, 30];
            bools = new List<bool>();
            set = new HashSet<int>();
            map = new int[16, 30];
            maped = new int[16, 30];
            DfsNum = new List<List<int>>();
            for (int i = 0; i < 16; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    Button button = new Button();
                    button.Background = Brushes.Gray;
                    button.Margin  = new Thickness (1);
                    button.PreviewMouseRightButtonDown +=(sender,e)=> OnClickRightButton((Button)sender);
                    button.Click += (sender, e) => ShowButton((Button)sender);
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    LeiQu.Children.Add(button);
                }
            }
            long seed = (long)(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond) ^ (long)Thread.CurrentThread.ManagedThreadId;
            Random random = new Random((int)(seed));
            while (set.Count != 100)
            {
                int randomNumber = random.Next(0, 480);
                set.Add(randomNumber);
            }
            for (int i = 0; i < 480; ++i) bools.Add(true);
            foreach (var i in set) bools[i] = false;
            InitMap();
        }

        private void OnEndClick()
        {
           
                foreach (var elment in LeiQu.Children)
                {
                    if (elment is Button button)
                    {
                        int row = Grid.GetRow(button);
                        int col = Grid.GetColumn(button);
                        if (map[row, col] == -1)
                        {
                            button.Background = Brushes.Red;

                        }
                        else if (map[row, col] == 0)
                        {
                            button.Background = Brushes.White;
                        }
                        else
                        {
                            button.Background = Brushes.White;
                            button.Content = map[row, col];
                        }
                    }
                }
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           InitLeiQu();
        }

        private void OnClickRightButton(Button button)
        {
            if(button.Background.Equals(Brushes.Yellow))
                button.Background = Brushes.Gray;
            else 
                button.Background = Brushes.Yellow;
        }

        private void ShowButton(Button button)
        {
            OnClickNum = 1;
            int row = Grid.GetRow(button); 
            int col = Grid.GetColumn(button);
            if (map[row,col] == -1)
            {
                button.Background = Brushes.Red;
                MessageBox.Show("游戏结束!");
                OnEndClick();
                return;
            }
            else if(map[row,col] == 0)
            {
                dfs(maped,row,col);
            }
            else
            {
                button.Background= Brushes.White;
                button.Content = map[row,col];
                ToWin[row,col] = true;
            }
            OnTheWin();
        }



        private void ShowAll(object sender, RoutedEventArgs e)
        {
            OnEndClick();
        }

        private void dfs(int[,]DfsSet,int x,int y)
        {
            if ((x < 0 || x > 15 || y < 0 || y > 29) || DfsSet[x,y]!=0) return;
            foreach (var elment in LeiQu.Children)
            {
                if (elment is Button buttonF)
                {
                    int a1 = Grid.GetRow(buttonF);
                    int b1 = Grid.GetColumn(buttonF);
                    if (a1 == x && b1 == y)
                    {
                        buttonF.Background = Brushes.White;
                        ToWin[x,y] = true;
                    }
                    for(int i = 0; i < 8; ++i)
                    {
                        int row = x + ints[i][0];
                        int col = y + ints[i][1];
                        if(InSetIt(row,col) && a1==row && b1 == col)
                        {
                            if (map[row,col] != 0)
                            {
                                buttonF.Background= Brushes.White;
                                buttonF.Content = map[row,col];
                                ToWin[row,col] = true;
                            }
                        }
                    }
                }
            }
            DfsSet[x,y] = -1;
            for(int i = 0; i < 4; ++i)
            {
                dfs(DfsSet, x + inIndex[i][0], y + inIndex[i][1]);
            }
        }

        private void OnTheWin()
        {
            int k = 0;
            for (int i = 0; i < 16; ++i)
            {
                for (int j = 0; j < 30; ++j)
                {
                    if (ToWin[i, j])
                    {
                        k++;
                    }
                }
            }
            if(k==380)
            {
                MessageBox.Show("你胜利了!");
                return;
            }
            ContFS.Content ="分数:" +k;
        }

        private bool InSetIt(int x, int y)
        {
            return x >= 0 && x <= 15 && y >= 0 && y <= 29;
        }
    }
}
