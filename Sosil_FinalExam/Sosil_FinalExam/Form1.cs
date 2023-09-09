using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Sosil_FinalExam
{
    public partial class Form1 : Form
    {
        public enum PlateInfo
        {
            empty,      // ▩
            player,     // ■
            monster,    // Σ
            portal,     // ●
            exit,       // ◎
            notExist
        }
        private const int LIFEPOINT = 5;

        private Random r;
        private List<Stage> stages;
        private Label[,] labels;        //화면에 표시할 라벨들
        private Player player;
        private Exit exit; 

        public Form1()
        {
            InitializeComponent();
            SetVar();
        }

        public void SetVar()
        {
            r = new Random();
            stages = new List<Stage>();

            //2~10개의 스테이지 생성
            int stageCount = r.Next(2, 10);
            for (int i=0; i<stageCount; i++)
                CreateStage(i);

            //오브젝트들 생성
            MakeObjects();

            // 시작하는 지도정보, 오브젝트 정보를 화면에 표시
            ShowStage(player.stage);
        }

        public void CreateStage(int stageNum)   //스테이지 생성& 초기화
        {
            int width = r.Next(3, 21);  // 3이상 21 미만
            int height = r.Next(3, 21);
            stages.Add(new Stage(height, width));

            for (int i = 0; i < height; i++)
                for(int j=0; j < width; j++)
                    stages[stageNum].Location[i, j] = 0;    //PlateInfo.empty;

            ListViewItem item = new ListViewItem(stageNum.ToString());
            item.SubItems.Add(height.ToString());
            item.SubItems.Add(width.ToString());
            listView1.Items.Add(item);
        }

        public void MakeObjects()
        {
            int[] pos;

            //포탈 생성
            MakePortals();
            //출구 생성
            exit = new Exit(r.Next(0, stages.Count));
            pos = FindEmpty(exit.stage);
            exit.xPos = pos[0];
            exit.yPos = pos[1];
            stages[exit.stage].Location[exit.xPos, exit.yPos] = PlateInfo.exit;
            //플레이어 생성
            player = new Player(LIFEPOINT);
            player.stage = r.Next(0, stages.Count);

            pos = FindEmpty(player.stage);
            player.MoveTo(pos[0], pos[1]);
            stages[player.stage].Location[player.xPos, player.yPos] = PlateInfo.player;
            
            //몬스터 생성
            MakeMonsters();
        }

        public void MakePortals()
        {
            int depart;     // 출발
            int arrive;     // 도착
            int stageCount = stages.Count;
            for(int i=0; i<stageCount; i++)
            {
                //이미 4개이상의 포탈이 만들어 졌으면 넘어감
                if (stages[i].portals.Count >= 4)
                    continue;
                depart = i;
                arrive = r.Next(0, stageCount);
                // 자기자신으로 가거나 도착하는 곳의 포탈의 수가 4개넘거나 이미 통하는 곳이면 continue
                if(arrive == i  || stages[arrive].portals.Count >= 4 
                    || stages[depart].ContainsWayTo(arrive)) 
                {
                    i--;
                    continue;
                }

                Portal dPortal = new Portal();
                Portal aPortal = new Portal();

                stages[depart].portals.Add(dPortal);    dPortal.stage = depart;
                stages[arrive].portals.Add(aPortal);    aPortal.stage = arrive;
                //포탈 서로 연결
                dPortal.Connect(aPortal);
                aPortal.Connect(dPortal);
                //두포탈의 좌표값 할당
                int[] pos = FindEmpty(depart);
                stages[depart].Location[pos[0], pos[1]] = PlateInfo.portal;
                dPortal.xPos = pos[0];  dPortal.yPos = pos[1];

                pos = FindEmpty(arrive);
                stages[arrive].Location[pos[0], pos[1]] = PlateInfo.portal;
                aPortal.xPos = pos[0]; aPortal.yPos = pos[1];

            }
        }

        public void MakeMonsters()
        {
            for(int i=0; i<stages.Count; i++)
            {
                int maximum = (int)(stages[i].Location.Length * 0.2);
                int monsterCount = r.Next(0, maximum);
                // 정해진 숫자만큼 해당 스테이지에 몬스터 생성
                for(int j=0; j<monsterCount; j++)
                {
                    int[] pos = FindEmpty(i);
                    stages[i].monsters.Add(new Monster(i, pos[0], pos[1]));
                    stages[i].Location[pos[0], pos[1]] = PlateInfo.monster;
                }

            }
        }

        public int[] FindEmpty(int stage)   // 해당 스테이지의 빈좌표를 찾아서 리턴
        {
            while(true)
            {
                int xPos = r.Next(0, stages[stage].Location.GetLength(0));
                int yPos = r.Next(0, stages[stage].Location.GetLength(1));

                if(stages[stage].Location[xPos, yPos] == PlateInfo.empty)
                {
                    return new int[2] { xPos, yPos };
                }
            }
        }

        

        public void ShowStage(int stageNum)
        {
            int width = stages[stageNum].Location.GetLength(1);   //GetLength() 메소드를 이용해 행의 길이, 열의 길이 모두 구할수 있다.
            int height = stages[stageNum].Location.GetLength(0);
            //패널을 지우고 새로운 라벨 할당
            panel1.Controls.Clear();
            labels = new Label[height, width];

            for (int i = 0; i < height; i++)
            {
                for(int j=0; j< width; j++)
                {
                    labels[i, j] = new Label();
                    labels[i, j].AutoSize = true;
                    labels[i, j].Location = new Point(j * 25 + 10, i * 25 + 10);
                    labels[i, j].Name = "lbl_" + i.ToString() + "," + j.ToString();
                    labels[i, j].Text = GetPlateString(stages[stageNum].Location[i, j]);
                    labels[i, j].Font = new Font("굴림", 10);
                    panel1.Controls.Add(labels[i, j]);              //게임 패널에 컨트롤 추가
                }
            }

            panel1.Size = new Size(width * 26, height * 26 + 10);
            this.Size = new Size(width * 26 + panel2.Width + 50, height * 26 + 140);

            panel1.Controls.Add(panel3);
            panel3.Dock = DockStyle.Bottom;

            ShowLifeAndStage();
        }


        public void ShowLifeAndStage()
        {
            lblStage.Text = "Stage : " + player.stage.ToString();
            lblLife.Text = "Life : " + player.lifePoint.ToString();

            if(player.lifePoint <= 0)   // 패배 처리
            {
                Result result = new Result(false);
                result.ShowDialog();
                this.Close();
            }
        }

        public string GetPlateString(PlateInfo info)
        {
            string s = "";
            switch (info)
            {
                case PlateInfo.empty:
                    s = "▩";    break;
                case PlateInfo.player:
                    s = "■";    break;
                case PlateInfo.monster:
                    s = "Σ";    break;
                case PlateInfo.portal:
                    s = "●";    break;
                case PlateInfo.exit:
                    s = "◎";    break;
            }
            return s;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int xPos = player.xPos;
            int yPos = player.yPos;
            switch(e.KeyCode)
            {
                case Keys.Right:
                    yPos++;
                    break;
                case Keys.Left:
                    yPos--;
                    break;
                case Keys.Up:
                    xPos--;
                    break;
                case Keys.Down:
                    xPos++;
                    break;
                default:
                    return;
            }

            PlateInfo info = WhatExists(player.stage, xPos, yPos);
            switch(info)
            {
                case PlateInfo.empty:
                    MoveTo(xPos, yPos);
                    break;
                case PlateInfo.portal:
                    Portal portal = stages[player.stage].WhereToGo(xPos, yPos); // 이동할 스테이지의 포탈
                    int[] pos = CalcLocation(portal);   // 이동할 위치 계산
                    if (pos[0] == -1 && pos[1] == -1)
                    {
                        MessageBox.Show("이동가능한 위치가 없습니다!");
                        break;
                    }
                    MoveStage(portal.stage, pos[0], pos[1]);
                    ShowStage(portal.stage);
                    labels[player.xPos, player.yPos].Text = GetPlateString(PlateInfo.player);
                    break;
                case PlateInfo.exit:
                    Result result = new Result(true);
                    result.ShowDialog();
                    this.Close();
                    break;
                case PlateInfo.monster: // 일단 몬스터와 맵 밖으로는 이동할 수 없게 구현
                    return;
                case PlateInfo.notExist:
                    return;
            }

            //플레이어 이동한 후 해당 스테이지의 몬스터 이동
            for(int i=0; i<stages[player.stage].monsters.Count; i++)
                MoveMonster(stages[player.stage].monsters[i]);
            //결과 반영
            ShowLifeAndStage();
        }

        public void MoveStage(int stage, int xPos, int yPos)
        {
            stages[player.stage].Location[player.xPos, player.yPos] = PlateInfo.empty;

            player.MoveStage(stage, xPos, yPos);

            stages[stage].Location[player.xPos, player.yPos] = PlateInfo.player;
        }

        public void MoveTo(int xPos, int yPos)  // 현재 스테이지 내에서 이동할 때 사용
        {
            stages[player.stage].Location[player.xPos, player.yPos] = PlateInfo.empty;
            labels[player.xPos, player.yPos].Text = GetPlateString(PlateInfo.empty);

            player.MoveTo(xPos, yPos);

            stages[player.stage].Location[player.xPos, player.yPos] = PlateInfo.player;
            labels[player.xPos, player.yPos].Text = GetPlateString(PlateInfo.player);
        }

        public void MoveMonster(Monster monster) // 플레이어가 존재하는 스테이지의 몬스터만 이동
        {
            List<Point> points = new List<Point>();
            int stage = monster.stage;
            int xPos = monster.xPos - 1;
            int yPos = monster.yPos - 1;
            //이동 가능 위치 계산
            for (int i=0; i<3; i++)
            {
                for(int j=0; j<3; j++)
                {
                    PlateInfo plate = WhatExists(stage, xPos + i, yPos + j);
                    if (plate == PlateInfo.empty || plate == PlateInfo.player)
                        points.Add(new Point(xPos + i, yPos + j));
                }
            }
            xPos++; yPos++;

            //이동가능한 위치가 없을경우 그냥 리턴
            if (points.Count == 0)
                return;

            int index = r.Next(0, points.Count);
            //이동할 위치가 빈공간이면 이동 / 플레이어면 lifepoint-- 하고 제자리에 남기
            if (WhatExists(stage, points[index].X, points[index].Y) == PlateInfo.empty)
            {
                stages[stage].Location[xPos, yPos] = PlateInfo.empty;
                labels[xPos, yPos].Text = GetPlateString(PlateInfo.empty);

                monster.MoveTo(points[index].X, points[index].Y);

                stages[stage].Location[monster.xPos, monster.yPos] = PlateInfo.monster;
                labels[monster.xPos, monster.yPos].Text = GetPlateString(PlateInfo.monster);
            }
            else if (WhatExists(stage, points[index].X, points[index].Y) == PlateInfo.player)
            {
                MessageBox.Show("몬스터로부터 공격받았습니다!\n 체력이 1 감소합니다");
                player.Attacked();
            }
        }

        private int[] CalcLocation(Portal p) // 플레이어가 이동가능한 좌표계산
        {
            int[] location = new int[2];
            int xPos = p.xPos;
            int yPos = p.yPos;
            int stage = p.stage;    // 포탈이 존재하는 스테이지

            if(WhatExists(stage, xPos - 1, yPos) == PlateInfo.empty)
            {
                location[0] = xPos - 1;
                location[1] = yPos;
            }
            else if(WhatExists(stage, xPos + 1, yPos) == PlateInfo.empty)
            {
                location[0] = xPos + 1;
                location[1] = yPos;
            }
            else if (WhatExists(stage, xPos, yPos + 1) == PlateInfo.empty)
            {
                location[0] = xPos;
                location[1] = yPos + 1;
            }
            else if (WhatExists(stage, xPos, yPos - 1) == PlateInfo.empty)
            {
                location[0] = xPos;
                location[1] = yPos - 1;
            }
            else
            {
                location[0] = -1;
                location[1] = -1;
            }
            return location;
        }

        // 현재 스테이지에서 해당 좌표 로 이동할 수 있는지 확인
        public PlateInfo WhatExists(int stage, int xPos, int yPos)
        {
            try
            {
                return stages[stage].Location[xPos, yPos];
            }
            catch   //지도범위 벗어날경우
            {
                return PlateInfo.notExist;
            }
        }
    }
}
