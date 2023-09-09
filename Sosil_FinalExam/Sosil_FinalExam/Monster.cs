namespace Sosil_FinalExam
{
    public class Monster
    {
        public int stage;
        public int xPos;
        public int yPos;

        public Monster(int stage, int xPos, int yPos)
        {
            this.stage = stage;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public void MoveTo(int x, int y)
        {
            this.xPos = x;
            this.yPos = y;
        }
    }
}
