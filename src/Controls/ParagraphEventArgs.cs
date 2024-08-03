﻿using Nikse.SubtitleEdit.Core.Common;

namespace SubtitleAlchemist.Controls
{
    public class ParagraphEventArgs
    {
        public Paragraph Paragraph { get; }
        public double Seconds { get; }
        public Paragraph BeforeParagraph { get; set; }
        public MouseDownParagraphType MouseDownParagraphType { get; set; }
        public bool MovePreviousOrNext { get; set; }
        public double AdjustMs { get; set; }
        public ParagraphEventArgs(Paragraph p)
        {
            Paragraph = p;
        }
        public ParagraphEventArgs(double seconds, Paragraph p)
        {
            Seconds = seconds;
            Paragraph = p;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
            MouseDownParagraphType = mouseDownParagraphType;
        }
        public ParagraphEventArgs(double seconds, Paragraph p, Paragraph b, MouseDownParagraphType mouseDownParagraphType, bool movePreviousOrNext)
        {
            Seconds = seconds;
            Paragraph = p;
            BeforeParagraph = b;
            MouseDownParagraphType = mouseDownParagraphType;
            MovePreviousOrNext = movePreviousOrNext;
        }
    }
}
