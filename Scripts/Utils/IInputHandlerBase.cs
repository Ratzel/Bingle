using UnityEngine;

namespace Dafhne
{
    public interface IInputHandlerBase
    {
        bool isInputDown{ get; }
        bool isInputUp{ get; }
        Vector2 inputPosition{ get; }
    }
}