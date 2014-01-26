using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Xml;
using System.Windows.Media.Animation;

namespace God_C_Creator
{
    class Colorizer
    {
        static public void Colorize(RichTextBox textBox, MainWindow mainWindow)
        {
            if (textBox.Document == null)
                return;

            TextRange documentRange = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            documentRange.ClearAllProperties();

            TextPointer navigator = textBox.Document.ContentStart;
            while (navigator.CompareTo(textBox.Document.ContentEnd) < 0)
            {
                TextPointerContext context = navigator.GetPointerContext(LogicalDirection.Backward);
                if (context == TextPointerContext.ElementStart && navigator.Parent is Run)
                {
                    CheckWordsInRun((Run)navigator.Parent);
                }
                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            Format(textBox, mainWindow);
        }
        public new struct Tag
        {
            public TextPointer StartPosition;
            public TextPointer EndPosition;
            public string Word;

        }
        static public List<Tag> m_tags = new List<Tag>();
        static public void Format(RichTextBox textBox, MainWindow mainWindow)
        {
            textBox.TextChanged -= mainWindow.onRichTextBoxTextChanged;

            for (int i = 0; i < m_tags.Count; i++)
            {
                TextRange range = new TextRange(m_tags[i].StartPosition, m_tags[i].EndPosition);
                range.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                range.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
            }
            m_tags.Clear();

            textBox.TextChanged += mainWindow.onRichTextBoxTextChanged;
        }

        static public void CheckWordsInRun(Run run)
        {
            string text = run.Text;

            int sIndex = 0;
            int eIndex = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (Char.IsWhiteSpace(text[i]) | GOD_C_SyntaxProvider.GetSpecials.Contains(text[i]))
                {
                    if (i > 0 && !(Char.IsWhiteSpace(text[i - 1]) | GOD_C_SyntaxProvider.GetSpecials.Contains(text[i - 1])))
                    {
                        eIndex = i - 1;
                        string word = text.Substring(sIndex, eIndex - sIndex + 1);

                        if (GOD_C_SyntaxProvider.IsKnownTag(word))
                        {
                            Tag t = new Tag();
                            t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                            t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                            t.Word = word;
                            m_tags.Add(t);
                        }
                    }
                    sIndex = i + 1;
                }
            }

            string lastWord = text.Substring(sIndex, text.Length - sIndex);
            if (GOD_C_SyntaxProvider.IsKnownTag(lastWord))
            {
                Tag t = new Tag();
                t.StartPosition = run.ContentStart.GetPositionAtOffset(sIndex, LogicalDirection.Forward);
                t.EndPosition = run.ContentStart.GetPositionAtOffset(eIndex + 1, LogicalDirection.Backward);
                t.Word = lastWord;
                m_tags.Add(t);
            }
        }
    }
}
