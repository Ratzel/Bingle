using UnityEngine;

namespace Dafhne.Util
{
    public enum Swipe
    {
        NONE = -1,
        RIGHT,
        UP,
        LEFT,
        DOWN
    }
    public static class TouchEvaluator
    {
        //두 지점을 활용하여 Swipe 방향을 구한다. 
        //UP : 45 ~ 135
        //LEFT : 135 ~ 255
        //DOWN : 225 ~ 315
        //RIGHT : 0 ~ 45, 0 ~ 315

        public static Swipe EvalSwipeDir(Vector2 vectStart, Vector2 vecEnd)
        {
            float angle = EvalDragAngle(vectStart, vecEnd);
            if(angle < 0)
            {
                return Swipe.NONE;
            }
            
            int swipe = (((int)angle +45) % 360) / 90;

            switch (swipe)
            {
                case 0 : return Swipe.RIGHT;
                case 1 : return Swipe.UP;
                case 2 : return Swipe.LEFT;
                case 3 : return Swipe.DOWN;
            }

            return Swipe.NONE;
        }

        //두 포인트 사이의 각도를 구한다. 
        //Input(마우스, 터치) 장치 드래그시에 드래그한 각도를 구하는데 활용한다. 
        //@return 두 포인트 사이의 각도
        static float EvalDragAngle(Vector2 vecStart, Vector2 vecEnd)
        {
            Vector2 dragDirection = vecEnd - vecStart;
            if(dragDirection.magnitude <= 0.2f) //최소 움직임값
            {
                return -1;
            }

            float aimAngle = Mathf.Atan2(dragDirection.y, dragDirection.x);
            if(aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }

            return aimAngle * Mathf.Rad2Deg;
        }

        public static int GetTargetRow(this Swipe swipeDir)
        {
            switch(swipeDir)
            {
                case Swipe.DOWN : return -1;
                case Swipe.UP : return 1;
                default : return 0;
            }
        }

        public static int GetTargetCol(this Swipe swipeDir)
        {
            switch(swipeDir)
            {
                case Swipe.LEFT : return -1;
                case Swipe.RIGHT : return 1;
                default : return 0;
            }
        }
    }
}


