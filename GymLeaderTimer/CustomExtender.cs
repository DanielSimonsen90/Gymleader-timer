using System.Collections.Generic;
using System.Windows.Controls;

namespace GymLeaderTimer
{
    internal static class CustomExtender
    {
        /// <summary>
        /// Returns true if <paramref name="btns"/>'s buttons'.Content matches <paramref name="value"/>, else return false
        /// </summary>
        /// <param name="btns">Button array | caller</param>
        /// <param name="value">.Content value</param>
        /// <returns></returns>
        public static bool Contains(this Button[] btns, string value)
        {
            foreach (Button btn in btns)
                if (btn.Content.ToString() == value)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns <paramref name="list"/> with <paramref name="buttons"/> added to its own value
        /// </summary>
        /// <param name="list">caller</param>
        /// <param name="buttons">buttons to add into <paramref name="list"/></param>
        /// <returns></returns>
        public static List<Button> AddInto(this List<Button> list, Button[] buttons)
        {
            list.AddRange(buttons);
            return list;
        }
    }
}
