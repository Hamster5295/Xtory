using System;

namespace Xtory
{
    public interface IInterfaceProvider
    {
        void ShowCharacter(string character, Action<string> callback = null);
        void ShowText(string content, Action<string> callback = null);
        void ShowMenu(string menu, Action<string> callback = null);
        void ClearMenu(Action<string> callback = null);
    }
}