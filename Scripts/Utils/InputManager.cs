using UnityEngine;

namespace Dafhne.Util
{
    public class InputManager
    {
        Transform _container;

#if !UNITY_EDITOR
        IInputHandlerBase _inputHandler = new TouchHandler();
#else
        IInputHandlerBase _inputHandler = new MouseHandler();
#endif

        public InputManager(Transform container){

            _container = container;
        }

        public bool isTouchDown => _inputHandler.isInputDown;
        public bool isTouchUp => _inputHandler.isInputUp;
        public Vector2 touchPosition => _inputHandler.inputPosition;
        public Vector2 touch2BoardPosition => TouchToPosition(_inputHandler.inputPosition);

        //터치 좌표(Screen 좌표)를 보드의 루트인 컨테이너 기준으로 변경된 2차원 좌표를 리턴한다.
        //@parm vectInput Screen좌표 즉, 픽셀 좌표 (좌하(0,0) -> 우상(Screen.Widht, Screen.Height))

        Vector2 TouchToPosition(Vector3 vecInput)
        {
            //1.스크린 좌표 -> 월드좌표로
            Vector3 vecMousePosWorld = Camera.main.ScreenToWorldPoint(vecInput);

            //2. 컨ㅔ이너 Local 좌표계로 변환(컨테이너 위치 이동시에도 컨테이너 기준의 로컬 좌표계이므로 화면 구성이 유연하다.)
            Vector3 vecContainerLocal = _container.transform.InverseTransformPoint(vecMousePosWorld);

            return vecContainerLocal;
        }

        public Swipe EvalSwipeDir(Vector2 vecStart, Vector2 vecEnd)
        {
            return TouchEvaluator.EvalSwipeDir(vecStart, vecEnd);
        }
        
    }
}