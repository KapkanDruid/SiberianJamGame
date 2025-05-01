using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Runtime.Utils.Helpers
{
    public static partial class Helpers
    {
        public class TextHelper
        {
            public static string CharSmart(string input, int charAt)
            {
                if (string.IsNullOrEmpty(input))
                    return "";

                return RemoveTags(input).Substring(charAt - 1, 1);
            }

            public static int GetVisibleLength(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return 0;

                return RemoveTags(input).Length;
            }

            public static string RemoveTags(string input, string tagPattern = "<[^>]*>")
            {
                return Regex.Replace(input, tagPattern, "");
            }
            
            public static string FormatSeconds(int seconds)
            {
                int minutes = seconds / 60;
                int remainingSeconds = seconds % 60;

                return $"{minutes:00}:{remainingSeconds:00}";
            }

            public static string CutSmart(string input, int maxLength)
            {
                if (string.IsNullOrEmpty(input) || maxLength <= 0)
                    return "";

                StringBuilder output = new StringBuilder();
                int visibleLength = 0; // Only counts visible characters
                Stack<string> openTags = new Stack<string>();

                int i = 0;
                while (i < input.Length && visibleLength < maxLength)
                {
                    if (input[i] == '<')
                    {
                        int closeTagPos = input.IndexOf('>', i);
                        if (closeTagPos != -1)
                        {
                            // Extract the tag content
                            string tagContent = input.Substring(i, closeTagPos - i + 1);
                            bool isClosingTag = tagContent.StartsWith("</");

                            if (isClosingTag)
                            {
                                // Closing tag: pop from the stack
                                if (openTags.Count > 0)
                                    openTags.Pop();
                            }
                            else
                            {
                                // Opening tag: push to the stack
                                int spaceIndex = tagContent.IndexOf(' ');
                                spaceIndex = spaceIndex == -1 ? tagContent.Length - 1 : spaceIndex;
                                string tagName = tagContent.Substring(1, spaceIndex - 1).TrimEnd('/');

                                if (!tagName.EndsWith("/")) // Self-closing tag check
                                    openTags.Push(tagName);
                            }

                            output.Append(tagContent);
                            i = closeTagPos;
                        }
                    }
                    else if (char.IsHighSurrogate(input[i]) && i + 1 < input.Length &&
                             char.IsLowSurrogate(input[i + 1]))
                    {
                        // Handle surrogate pairs (for UTF-16 encoding of certain characters)
                        if (visibleLength < maxLength - 1)
                        {
                            output.Append(input[i]);
                            output.Append(input[i + 1]);
                            visibleLength++;
                            i++; // Skip the next character as it's part of the surrogate pair
                        }
                    }
                    else
                    {
                        output.Append(input[i]);
                        visibleLength++;
                    }

                    i++;
                }

                // Close any unclosed tags
                while (openTags.Count > 0)
                {
                    string tag = openTags.Pop();
                    output.Append($"</{tag}>");
                }

                return output.ToString();
            }
        }
    }
}