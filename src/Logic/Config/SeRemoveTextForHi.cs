﻿using CommunityToolkit.Mvvm.ComponentModel;

namespace SubtitleAlchemist.Logic.Config;

public class SeRemoveTextForHi
{
    public bool IsRemoveBracketsOn { get; set; }
    public bool IsRemoveCurlyBracketsOn { get; set; }
    public bool IsRemoveParenthesesOn { get; set; }
    public bool IsRemoveCustomOn { get; set; }
    public string CustomStart { get; set; }
    public string CustomEnd { get; set; }
    public bool IsOnlySeparateLine { get; set; }
    public bool IsOnlySingleLine { get; set; }

    public bool IsRemoveTextBeforeColonOn { get; set; }
    public bool IsRemoveTextBeforeColonUppercaseOn { get; set; }
    public bool IsRemoveTextBeforeColonSeparateLineOn { get; set; }

    public bool IsRemoveTextUppercaseLineOn { get; set; }

    public bool IsRemoveTextContainsOn { get; set; }
    public string TextContains { get; set; }

    public bool IsRemoveOnlyMusicSymbolsOn { get; set; }

    public bool IsRemoveInterjectionsOn { get; set; }
    public bool IsInterjectionsSeparateLineOn { get; set; }

    public SeRemoveTextForHi()
    {
        IsRemoveBracketsOn = true;
        IsRemoveCurlyBracketsOn = true;
        IsRemoveParenthesesOn = true;
        IsRemoveTextBeforeColonOn = true;

        CustomStart = "?";
        CustomEnd = "?";
        TextContains = string.Empty;
    }
}