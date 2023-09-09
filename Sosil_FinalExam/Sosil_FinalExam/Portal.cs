namespace Sosil_FinalExam
{
    public class Portal
    {
        public int stage;           //존재하는 스테이지
        public int xPos;
        public int yPos;
        public Portal WayTo;        //이동되는 위치

        
        public Portal()
        {

        }

        // 포탈끼리 연결. 서로 연결시켜줘야함
        public void Connect(Portal portal)
        {
            WayTo = portal;
        }
    }
}
