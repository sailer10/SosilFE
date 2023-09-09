using System.Collections.Generic;

namespace Sosil_FinalExam
{
    public class Stage
    {
        public Form1.PlateInfo[,] Location;
        public List<Portal> portals; // key: 어디로 넘어가는지, Portal: 현재 스테이지의 포탈
        public List<Monster> monsters;

        public Stage(int height, int width)
        {
            Location = new Form1.PlateInfo[height, width];
            portals = new List<Portal>();
            monsters = new List<Monster>();
        }

        public bool ContainsWayTo(int stage)
        {
            for(int i=0; i<portals.Count; i++)
            {
                if (portals[i].WayTo.stage == stage)
                    return true;
            }
            return false;
        }

        public Portal WhereToGo(int xPos, int yPos) // 해당 좌표의 포탈을 찾아서 그 포탈이 어디로 향하는지 리턴
        {
            foreach(Portal p in portals)
            {
                if(p.xPos == xPos && p.yPos == yPos)
                {
                    return p.WayTo;
                }
            }
            return null;
        }
    }
}
