﻿using System;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        //It also creates an extension for ArgumentNullException and ArgumentOutOfRangeException
        public static string GetMessageWithoutParamName(this ArgumentException argumentException)
        {
            string[] messageLines = argumentException.Message.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            return messageLines[0];
        }

        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }

        ///<summary>Truncates DateTime to specified TimeSpan</summary>
        ///<remarks>dateTime = dateTime.Truncate(TimeSpan.FromSeconds(1)); // Truncate to whole second</remarks>
        ///<param name="timeSpan">TimeSpan to truncate to</param>
        //Extension method provided by 'Joe' http://stackoverflow.com/users/13087/joe
        public static DateTime Truncate(this DateTime dateTime, TimeSpan timeSpan)
        {
            return dateTime.AddTicks(-(dateTime.Ticks % timeSpan.Ticks));
        }
    }
}
