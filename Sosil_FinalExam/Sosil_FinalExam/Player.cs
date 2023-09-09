namespace Sosil_FinalExam
{
    class Player
    {
        public int stage;
        public int lifePoint;
        public int xPos;
        public int yPos;

        public Player(int lifePoint)
        {
            this.lifePoint = lifePoint;
        }

        //이동시 스테이지 번호와 위치 설정
        public void MoveStage(int stage, int x, int y)
        {
            this.stage = stage;
            xPos = x;
            yPos = y;
        }

        public void MoveTo(int x, int y)
        {
            this.xPos = x;
            this.yPos = y;
        }

        // 몬스터에게 공격받았을때
        public void Attacked()
        {
            lifePoint--;
        }
    }
}
