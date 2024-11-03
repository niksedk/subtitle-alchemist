namespace SubtitleAlchemist.Features.Options.Settings;

public enum ShortcutAction
{
    GeneralMergeSelectedLines,
    GeneralMergeWithPrevious,
    GeneralMergeWithNext,
    GeneralMergeWithPreviousAndUnbreak,
    GeneralMergeWithNextAndUnbreak,
    GeneralMergeWithPreviousAndAutoBreak,
    GeneralMergeWithNextAndAutoBreak,
    GeneralMergeSelectedLinesAndAutoBreak,
    GeneralMergeSelectedLinesAndUnbreak,
    GeneralMergeSelectedLinesAndUnbreakCjk,
    GeneralMergeSelectedLinesOnlyFirstText,
    GeneralMergeSelectedLinesBilingual,
    GeneralMergeWithPreviousBilingual,
    GeneralMergeWithNextBilingual,
    GeneralMergeOriginalAndTranslation,
    GeneralToggleTranslationMode,
    GeneralSwitchOriginalAndTranslation,
    GeneralSwitchOriginalAndTranslationTextBoxes,
    GeneralChooseLayout,
    GeneralLayoutChoose1,
    GeneralLayoutChoose2,
    GeneralLayoutChoose3,
    GeneralLayoutChoose4,
    GeneralLayoutChoose5,
    GeneralLayoutChoose6,
    GeneralLayoutChoose7,
    GeneralLayoutChoose8,
    GeneralLayoutChoose9,
    GeneralLayoutChoose10,
    GeneralLayoutChoose11,
    GeneralLayoutChoose12,
    GeneralPlayFirstSelected,
    GeneralGoToFirstSelectedLine,
    GeneralGoToNextEmptyLine,
    GeneralGoToNextSubtitle,
    GeneralGoToNextSubtitlePlayTranslate,
    GeneralGoToNextSubtitleCursorAtEnd,
    GeneralGoToPrevSubtitle,
    GeneralGoToPrevSubtitlePlayTranslate,
    GeneralGoToStartOfCurrentSubtitle,
    GeneralGoToEndOfCurrentSubtitle,
    GeneralGoToPreviousSubtitleAndFocusVideo,
    GeneralGoToNextSubtitleAndFocusVideo,
    GeneralGoToPrevSubtitleAndPlay,
    GeneralGoToNextSubtitleAndPlay,
    GeneralGoToPreviousSubtitleAndFocusWaveform,
    GeneralGoToNextSubtitleAndFocusWaveform,
    GeneralGoToLineNumber,
    GeneralToggleBookmarks,
    GeneralFocusTextBox,
    GeneralToggleBookmarksWithText,
    GeneralEditBookmarks,
    GeneralToggleFocus,
    GeneralListErrors,


    //AddNode(generalNode, language.ClearBookmarks, nameof(Configuration.Settings.Shortcuts.GeneralClearBookmarks));
    //AddNode(generalNode, language.GoToBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToBookmark));
    //AddNode(generalNode, language.GoToPreviousBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToPreviousBookmark));
    //AddNode(generalNode, language.GoToNextBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextBookmark));
    //AddNode(generalNode, language.ChooseProfile, nameof(Configuration.Settings.Shortcuts.GeneralChooseProfile));
    GeneralDuplicateLine,
    GeneralOpenDataFolder,
    //AddNode(generalNode, LanguageSettings.Current.Main.Menu.File.OpenContainingFolder, nameof(Configuration.Settings.Shortcuts.OpenContainingFolder), true);
    //AddNode(generalNode, language.ToggleView, nameof(Configuration.Settings.Shortcuts.GeneralToggleView), true);
    //AddNode(generalNode, language.ToggleMode, nameof(Configuration.Settings.Shortcuts.GeneralToggleMode));
    //AddNode(generalNode, language.TogglePreviewOnVideo, nameof(Configuration.Settings.Shortcuts.GeneralTogglePreviewOnVideo));
    //AddNode(generalNode, language.RemoveBlankLines, nameof(Configuration.Settings.Shortcuts.GeneralRemoveBlankLines));
    //AddNode(generalNode, language.ApplyAssaOverrideTags, nameof(Configuration.Settings.Shortcuts.GeneralApplyAssaOverrideTags), true);
    //AddNode(generalNode, language.SetAssaPosition, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaPosition), true);
    //AddNode(generalNode, language.SetAssaResolution, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaResolution));
    //AddNode(generalNode, language.SetAssaBgBox, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaBgBox), true);
    //AddNode(generalNode, LanguageSettings.Current.ImageColorPicker.Title, nameof(Configuration.Settings.Shortcuts.GeneralColorPicker));
    //AddNode(generalNode, language.TakeAutoBackup, nameof(Configuration.Settings.Shortcuts.GeneralTakeAutoBackup));
    GeneralHelp,






    FileNew,
    FileOpen,
    FileOpenKeepVideo,
    FileSave,
    FileSaveAs,
    FileSaveAll,
    FileSaveOriginal,
    FileSaveOriginalAs,
    FileOpenOriginalSubtitle,
    FileCloseOriginalSubtitle,
    FileTranslatedSubtitle,
    FileCompare,
    FileImportPlainText,
    FileImportBluRaySupForOcr,
    FileImportBluRaySupForEdit,
    FileImportTimeCodes,
    FileExportEbuStl,
    FileExportPac,
    FileExportEdlClipName,
    FileExportPlainText,
    FileExportCustomTextFormat1,
    FileExportCustomTextFormat2,
    FileExportCustomTextFormat3,
    FileExit,

    EditFind,
    EditFindNext,
    EditReplace,
    EditMultipleReplace,
    EditModifySelection,
    Tools,
    SpellCheck,


    VideoOpen,
    VideoClose,
    VideoPlayPauseToggle,
    VideoPause,
    VideoStop,
    VideoPlayFromJustBefore,
    VideoPlayFromBeginning,
    VideoFocusSetVideoPosition,
    VideoToggleVideoControls,
    Video1FrameLeft,
    Video1FrameRight,
    Video1FrameLeftWithPlay,
    Video1FrameRightWithPlay,
    Video100MsLeft,
    Video100MsRight,
    Video500MsLeft,
    Video500MsRight,
    Video1000MsLeft,
    Video1000MsRight,
    Video3000MsLeft,
    Video3000MsRight,
    Video5000MsLeft,
    Video5000MsRight,
    VideoXSMsLeft,
    VideoXSMsRight,
    VideoXLMsLeft,
    VideoXLMsRight,
    VideoGoToStartCurrent,
    VideoToggleStartEndCurrent,
    VideoPlaySelectedLines,
    VideoLoopSelectedLines,
    VideoGoToPrevSubtitle,
    VideoGoToNextSubtitle,
    VideoGoToPrevTimeCode,
    VideoGoToNextTimeCode,
    VideoGoToPrevChapter,
    VideoGoToNextChapter,
    VideoSelectNextSubtitle,
    VideoFullscreen,
    VideoPlay150Speed,
    VideoPlay200Speed,
    VideoSlower,
    VideoFaster,
    VideoSpeedToggle,
    VideoReset,
    VideoToggleControls,
    VideoAudioToTextVosk,
    VideoAudioToTextWhisper,
    VideoTextToSpeech,
    VideoAudioExtractAudioSelectedLines,
    VideoToggleContrast,
    VideoToggleBrightness,

    Synchronization,
    Options,
    Translate,
    SubtitleListViewAndTextBox,
    
    
    SubtitleListView,

    ListMergeDialog,
    ListMergeDialogWithNext,
    ListMergeDialogWithPrevious,
    ListAutoBalanceSelectedLines,
    ListEvenlyDistributeSelectedLines,
    ListToggleFocusWaveform,
    ListToggleFocusWaveformTextBox,
    ListToggleDashes,
    ListAlignment,
    ListCopyText,
    ListCopyPlainText,
    ListCopyTextFromOriginalToCurrent,
    ListAutoDuration,
    ListColumnDeleteText,
    ListColumnDeleteTextAndShiftUp,
    ListColumnInsertText,
    ListColumnPaste,
    ListColumnTextUp,
    ListColumnTextDown,
    ListGoToNextError,
    ListSortByNumber,
    ListSortByStartTime,
    ListSortByEndTime,
    ListSortByDuration,
    ListSortByGap,
    ListSortByText,
    ListSortBySingleLineMaxLen,
    ListSortByTextTotalLength,
    ListSortByCps,
    ListSortByWpm,
    ListSortByNumberOfLines,
    ListSortByStyle,

    TextBoxSplitAtCursor,
    TextBoxSplitAtCursorAndAutoBr,
    TextBoxSplitAtCursorAndVideoPos,
    TextBoxSplitSelectedLineBilingual,
    TextBoxMoveLastWordDown,
    TextBoxMoveFirstWordFromNextUp,
    TextBoxMoveLastWordDownCurrent,
    TextBoxMoveFirstWordUpCurrent,
    TextBoxMoveFromCursorToNextAndGoToNext,
    TextBoxSelectionToLower,
    TextBoxSelectionToUpper,
    TextBoxSelectionToRuby,
    TextBoxToggleAutoDuration,
    TextBoxAutoBreak,
    TextBoxBreakAtPosition,
    TextBoxBreakAtPositionAndGoToNext,
    TextBoxRecord,
    TextBoxAssaIntellisense,
    TextBoxAssaRemoveTag,

    WaveformAndSpectrogram,
    WaveformAdd,
    WaveformZoomIn,
    WaveformZoomOut,
    WaveformVerticalZoom,
    WaveformVerticalZoomOut,
    WaveformSplit,
    WaveformSearchSilenceForward,
    WaveformSearchSilenceBack,
    WaveformAddTextHere,
    WaveformAddTextHereFromClipboard,
    WaveformSetParagraphAsSelection,
    WaveformPlaySelection,
    WaveformPlaySelectionEnd,
    WaveformInsertAtCurrentPosition,
    WaveformGoToPreviousShotChange,
    WaveformGoToNextShotChange,
    WaveformAllShotChangesOneFrameBack,
    WaveformAllShotChangesOneFrameForward,
    WaveformToggleShotChange,
    WaveformListShotChanges,
    WaveformGuessStart,
    Waveform100MsLeft,
    Waveform100MsRight,
    Waveform1000MsLeft,
    Waveform1000MsRight,
    WaveformAudioToTextVosk,
    WaveformAudioToTextWhisper,
}


//var generalNode = new ShortcutNode(LanguageSettings.Current.General.GeneralText);
//AddNode(generalNode, language.MergeSelectedLines, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLines));
//AddNode(generalNode, language.MergeWithPrevious, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithPrevious));
//AddNode(generalNode, language.MergeWithNext, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithNext));
//AddNode(generalNode, language.MergeWithPreviousAndUnbreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithPreviousAndUnbreak));
//AddNode(generalNode, language.MergeWithNextAndUnbreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithNextAndUnbreak));
//AddNode(generalNode, language.MergeWithPreviousAndBreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithPreviousAndBreak));
//AddNode(generalNode, language.MergeWithNextAndBreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithNextAndBreak));
//AddNode(generalNode, language.MergeSelectedLinesAndAutoBreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesAndAutoBreak));
//AddNode(generalNode, language.MergeSelectedLinesAndUnbreak, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesAndUnbreak));
//AddNode(generalNode, language.MergeSelectedLinesAndUnbreakCjk, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesAndUnbreakCjk));
//AddNode(generalNode, language.MergeSelectedLinesOnlyFirstText, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesOnlyFirstText));
//AddNode(generalNode, language.MergeSelectedLinesBilingual, nameof(Configuration.Settings.Shortcuts.GeneralMergeSelectedLinesBilingual));
//AddNode(generalNode, language.MergeWithPreviousBilingual, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithPreviousBilingual));
//AddNode(generalNode, language.MergeWithNextBilingual, nameof(Configuration.Settings.Shortcuts.GeneralMergeWithNextBilingual));
//AddNode(generalNode, language.MergeOriginalAndTranslation, nameof(Configuration.Settings.Shortcuts.GeneralMergeOriginalAndTranslation));
//AddNode(generalNode, language.ToggleTranslationMode, nameof(Configuration.Settings.Shortcuts.GeneralToggleTranslationMode));
//AddNode(generalNode, language.SwitchOriginalAndTranslation, nameof(Configuration.Settings.Shortcuts.GeneralSwitchOriginalAndTranslation));
//AddNode(generalNode, language.SwitchOriginalAndTranslationTextBoxes, nameof(Configuration.Settings.Shortcuts.GeneralSwitchOriginalAndTranslationTextBoxes));
//AddNode(generalNode, LanguageSettings.Current.Main.ChooseLayout, nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose));
//AddNode(generalNode, string.Format(language.ChooseLayoutX, 1), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose1));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 2), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose2));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 3), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose3));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 4), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose4));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 5), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose5));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 6), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose6));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 7), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose7));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 8), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose8));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 9), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose9));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 10), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose10));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 11), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose11));
//            AddNode(generalNode, string.Format(language.ChooseLayoutX, 12), nameof(Configuration.Settings.Shortcuts.GeneralLayoutChoose12));
//            AddNode(generalNode, language.WaveformPlayFirstSelectedSubtitle, nameof(Configuration.Settings.Shortcuts.GeneralPlayFirstSelected));
//AddNode(generalNode, language.GoToFirstSelectedLine, nameof(Configuration.Settings.Shortcuts.GeneralGoToFirstSelectedLine));
//AddNode(generalNode, language.GoToNextEmptyLine, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextEmptyLine));
//AddNode(generalNode, language.GoToNext, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitle));
//AddNode(generalNode, language.GoToNextPlayTranslate, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitlePlayTranslate));
//AddNode(generalNode, language.GoToNextCursorAtEnd, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitleCursorAtEnd));
//AddNode(generalNode, language.GoToPrevious, nameof(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitle));
//AddNode(generalNode, language.GoToPreviousPlayTranslate, nameof(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitlePlayTranslate));
//AddNode(generalNode, language.GoToCurrentSubtitleStart, nameof(Configuration.Settings.Shortcuts.GeneralGoToStartOfCurrentSubtitle));
//AddNode(generalNode, language.GoToCurrentSubtitleEnd, nameof(Configuration.Settings.Shortcuts.GeneralGoToEndOfCurrentSubtitle));
//AddNode(generalNode, language.GoToPreviousSubtitleAndFocusVideo, nameof(Configuration.Settings.Shortcuts.GeneralGoToPreviousSubtitleAndFocusVideo));
//AddNode(generalNode, language.GoToNextSubtitleAndFocusVideo, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitleAndFocusVideo));
//AddNode(generalNode, language.GoToPrevSubtitleAndPlay, nameof(Configuration.Settings.Shortcuts.GeneralGoToPrevSubtitleAndPlay));
//AddNode(generalNode, language.GoToNextSubtitleAndPlay, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitleAndPlay));
//AddNode(generalNode, language.GoToPreviousSubtitleAndFocusWaveform, nameof(Configuration.Settings.Shortcuts.GeneralGoToPreviousSubtitleAndFocusWaveform));
//AddNode(generalNode, language.GoToNextSubtitleAndFocusWaveform, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextSubtitleAndFocusWaveform));
//AddNode(generalNode, language.ToggleBookmarks, nameof(Configuration.Settings.Shortcuts.GeneralToggleBookmarks));
//AddNode(generalNode, language.FocusTextBox, nameof(Configuration.Settings.Shortcuts.GeneralFocusTextBox));
//AddNode(generalNode, language.ToggleBookmarksWithComment, nameof(Configuration.Settings.Shortcuts.GeneralToggleBookmarksWithText), true);
//AddNode(generalNode, LanguageSettings.Current.Bookmarks.EditBookmark, nameof(Configuration.Settings.Shortcuts.GeneralEditBookmarks), true);
//AddNode(generalNode, language.ClearBookmarks, nameof(Configuration.Settings.Shortcuts.GeneralClearBookmarks));
//AddNode(generalNode, language.GoToBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToBookmark));
//AddNode(generalNode, language.GoToPreviousBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToPreviousBookmark));
//AddNode(generalNode, language.GoToNextBookmark, nameof(Configuration.Settings.Shortcuts.GeneralGoToNextBookmark));
//AddNode(generalNode, language.ChooseProfile, nameof(Configuration.Settings.Shortcuts.GeneralChooseProfile));
//AddNode(generalNode, language.DuplicateLine, nameof(Configuration.Settings.Shortcuts.GeneralDuplicateLine));
//AddNode(generalNode, language.OpenDataFolder, nameof(Configuration.Settings.Shortcuts.OpenDataFolder));
//AddNode(generalNode, LanguageSettings.Current.Main.Menu.File.OpenContainingFolder, nameof(Configuration.Settings.Shortcuts.OpenContainingFolder), true);
//AddNode(generalNode, language.ToggleView, nameof(Configuration.Settings.Shortcuts.GeneralToggleView), true);
//AddNode(generalNode, language.ToggleMode, nameof(Configuration.Settings.Shortcuts.GeneralToggleMode));
//AddNode(generalNode, language.TogglePreviewOnVideo, nameof(Configuration.Settings.Shortcuts.GeneralTogglePreviewOnVideo));
//AddNode(generalNode, language.RemoveBlankLines, nameof(Configuration.Settings.Shortcuts.GeneralRemoveBlankLines));
//AddNode(generalNode, language.ApplyAssaOverrideTags, nameof(Configuration.Settings.Shortcuts.GeneralApplyAssaOverrideTags), true);
//AddNode(generalNode, language.SetAssaPosition, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaPosition), true);
//AddNode(generalNode, language.SetAssaResolution, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaResolution));
//AddNode(generalNode, language.SetAssaBgBox, nameof(Configuration.Settings.Shortcuts.GeneralSetAssaBgBox), true);
//AddNode(generalNode, LanguageSettings.Current.ImageColorPicker.Title, nameof(Configuration.Settings.Shortcuts.GeneralColorPicker));
//AddNode(generalNode, language.TakeAutoBackup, nameof(Configuration.Settings.Shortcuts.GeneralTakeAutoBackup));
//AddNode(generalNode, language.Help, nameof(Configuration.Settings.Shortcuts.GeneralHelp), true);
//_shortcuts.Nodes.Add(generalNode);

//            var fileNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.File.Title);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.New, nameof(Configuration.Settings.Shortcuts.MainFileNew), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Open, nameof(Configuration.Settings.Shortcuts.MainFileOpen), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.OpenKeepVideo, nameof(Configuration.Settings.Shortcuts.MainFileOpenKeepVideo), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Save, nameof(Configuration.Settings.Shortcuts.MainFileSave), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.SaveAs, nameof(Configuration.Settings.Shortcuts.MainFileSaveAs), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.SaveOriginal, nameof(Configuration.Settings.Shortcuts.MainFileSaveOriginal), true);
//AddNode(fileNode, LanguageSettings.Current.Main.SaveOriginalSubtitleAs, nameof(Configuration.Settings.Shortcuts.MainFileSaveOriginalAs), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.OpenOriginal, nameof(Configuration.Settings.Shortcuts.MainFileOpenOriginal), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.CloseOriginal, nameof(Configuration.Settings.Shortcuts.MainFileCloseOriginal), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.CloseTranslation, nameof(Configuration.Settings.Shortcuts.MainFileCloseTranslation), true);
//AddNode(fileNode, language.MainFileSaveAll, nameof(Configuration.Settings.Shortcuts.MainFileSaveAll));
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Compare, nameof(Configuration.Settings.Shortcuts.MainFileCompare), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.VerifyCompleteness, nameof(Configuration.Settings.Shortcuts.MainFileVerifyCompleteness), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Import + " -> " + LanguageSettings.Current.Main.Menu.File.ImportText, nameof(Configuration.Settings.Shortcuts.MainFileImportPlainText), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Import + " -> " + LanguageSettings.Current.Main.Menu.File.ImportBluRaySupFileEdit, nameof(Configuration.Settings.Shortcuts.MainFileImportBdSupForEdit), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Import + " -> " + LanguageSettings.Current.Main.Menu.File.ImportTimecodes, nameof(Configuration.Settings.Shortcuts.MainFileImportTimeCodes), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportEbu, nameof(Configuration.Settings.Shortcuts.MainFileExportEbu), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportPac, nameof(Configuration.Settings.Shortcuts.MainFileExportPac), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportBluRaySup, nameof(Configuration.Settings.Shortcuts.MainFileExportBdSup), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> EDL/CLIPNAME", nameof(Configuration.Settings.Shortcuts.MainFileExportEdlClip), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportPlainText, nameof(Configuration.Settings.Shortcuts.MainFileExportPlainText), true);
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportCustomTextFormat + " 1", nameof(Configuration.Settings.Shortcuts.MainFileExportCustomText1));
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportCustomTextFormat + " 2", nameof(Configuration.Settings.Shortcuts.MainFileExportCustomText2));
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Export + " -> " + LanguageSettings.Current.Main.Menu.File.ExportCustomTextFormat + " 3", nameof(Configuration.Settings.Shortcuts.MainFileExportCustomText3));
//AddNode(fileNode, LanguageSettings.Current.Main.Menu.File.Exit, nameof(Configuration.Settings.Shortcuts.MainFileExit), true);
//_shortcuts.Nodes.Add(fileNode);

//            var editNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.Edit.Title);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.Undo, nameof(Configuration.Settings.Shortcuts.MainEditUndo), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.Redo, nameof(Configuration.Settings.Shortcuts.MainEditRedo), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.Find, nameof(Configuration.Settings.Shortcuts.MainEditFind), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.FindNext, nameof(Configuration.Settings.Shortcuts.MainEditFindNext), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.Replace, nameof(Configuration.Settings.Shortcuts.MainEditReplace), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.MultipleReplace, nameof(Configuration.Settings.Shortcuts.MainEditMultipleReplace), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.ModifySelection, nameof(Configuration.Settings.Shortcuts.MainEditModifySelection), true);
//AddNode(editNode, LanguageSettings.Current.Main.Menu.Edit.GoToSubtitleNumber, nameof(Configuration.Settings.Shortcuts.MainEditGoToLineNumber), true);
//AddNode(editNode, LanguageSettings.Current.VobSubOcr.RightToLeft, nameof(Configuration.Settings.Shortcuts.MainEditRightToLeft), true);
//AddNode(editNode, language.FixRTLViaUnicodeChars, nameof(Configuration.Settings.Shortcuts.MainEditFixRTLViaUnicodeChars), true);
//AddNode(editNode, language.RemoveRTLUnicodeChars, nameof(Configuration.Settings.Shortcuts.MainEditRemoveRTLUnicodeChars), true);
//AddNode(editNode, language.ReverseStartAndEndingForRtl, nameof(Configuration.Settings.Shortcuts.MainEditReverseStartAndEndingForRTL), true);
//AddNode(editNode, language.ToggleTranslationAndOriginalInPreviews, nameof(Configuration.Settings.Shortcuts.MainEditToggleTranslationOriginalInPreviews), true);
//_shortcuts.Nodes.Add(editNode);

//            var toolsNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.Tools.Title);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.AdjustDisplayDuration, nameof(Configuration.Settings.Shortcuts.MainToolsAdjustDuration), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.ApplyDurationLimits.Trim('.'), nameof(Configuration.Settings.Shortcuts.MainToolsAdjustDurationLimits), true);
//            AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.SubtitlesBridgeGaps, nameof(Configuration.Settings.Shortcuts.MainToolsDurationsBridgeGap), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MinimumDisplayTimeBetweenParagraphs, nameof(Configuration.Settings.Shortcuts.MainToolsMinimumDisplayTimeBetweenParagraphs), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.FixCommonErrors, nameof(Configuration.Settings.Shortcuts.MainToolsFixCommonErrors), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.StartNumberingFrom, nameof(Configuration.Settings.Shortcuts.MainToolsRenumber), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.RemoveTextForHearingImpaired, nameof(Configuration.Settings.Shortcuts.MainToolsRemoveTextForHI), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.ConvertColorsToDialog, nameof(Configuration.Settings.Shortcuts.MainToolsConvertColorsToDialog), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.ChangeCasing, nameof(Configuration.Settings.Shortcuts.MainToolsChangeCasing), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MakeNewEmptyTranslationFromCurrentSubtitle, nameof(Configuration.Settings.Shortcuts.MainToolsMakeEmptyFromCurrent), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MergeShortLines, nameof(Configuration.Settings.Shortcuts.MainToolsMergeShortLines), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MergeDuplicateText, nameof(Configuration.Settings.Shortcuts.MainToolsMergeDuplicateText), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MergeSameTimeCodes, nameof(Configuration.Settings.Shortcuts.MainToolsMergeSameTimeCodes), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.SplitLongLines, nameof(Configuration.Settings.Shortcuts.MainToolsSplitLongLines), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.BatchConvert, nameof(Configuration.Settings.Shortcuts.MainToolsBatchConvert), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.MeasurementConverter, nameof(Configuration.Settings.Shortcuts.MainToolsMeasurementConverter), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.SplitSubtitle, nameof(Configuration.Settings.Shortcuts.MainToolsSplit), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.AppendSubtitle, nameof(Configuration.Settings.Shortcuts.MainToolsAppend), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.Tools.JoinSubtitles, nameof(Configuration.Settings.Shortcuts.MainToolsJoin), true);
//AddNode(toolsNode, LanguageSettings.Current.Main.Menu.ContextMenu.AutoDurationCurrentLine, nameof(Configuration.Settings.Shortcuts.MainToolsAutoDuration));
//AddNode(toolsNode, language.ShowStyleManager, nameof(Configuration.Settings.Shortcuts.MainToolsStyleManager));
//_shortcuts.Nodes.Add(toolsNode);

//            var videoNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.Video.Title);
//AddNode(videoNode, LanguageSettings.Current.Main.Menu.Video.OpenVideo, nameof(Configuration.Settings.Shortcuts.MainVideoOpen), true);
//AddNode(videoNode, LanguageSettings.Current.Main.Menu.Video.CloseVideo, nameof(Configuration.Settings.Shortcuts.MainVideoClose), true);
//AddNode(videoNode, language.TogglePlayPause, nameof(Configuration.Settings.Shortcuts.MainVideoPlayPauseToggle));
//AddNode(videoNode, language.Pause, nameof(Configuration.Settings.Shortcuts.MainVideoPause));
//AddNode(videoNode, LanguageSettings.Current.Main.VideoControls.Stop, nameof(Configuration.Settings.Shortcuts.MainVideoStop));
//AddNode(videoNode, LanguageSettings.Current.Main.VideoControls.PlayFromJustBeforeText, nameof(Configuration.Settings.Shortcuts.MainVideoPlayFromJustBefore));
//AddNode(videoNode, LanguageSettings.Current.Main.VideoControls.PlayFromBeginning, nameof(Configuration.Settings.Shortcuts.MainVideoPlayFromBeginning));
//AddNode(videoNode, language.FocusSetVideoPosition, nameof(Configuration.Settings.Shortcuts.MainVideoFocusSetVideoPosition));
//AddNode(videoNode, language.ToggleDockUndockOfVideoControls, nameof(Configuration.Settings.Shortcuts.MainVideoToggleVideoControls), true);
//AddNode(videoNode, language.GoBack1Frame, nameof(Configuration.Settings.Shortcuts.MainVideo1FrameLeft));
//AddNode(videoNode, language.GoForward1Frame, nameof(Configuration.Settings.Shortcuts.MainVideo1FrameRight));
//AddNode(videoNode, language.GoBack1FrameWithPlay, nameof(Configuration.Settings.Shortcuts.MainVideo1FrameLeftWithPlay));
//AddNode(videoNode, language.GoForward1FrameWithPlay, nameof(Configuration.Settings.Shortcuts.MainVideo1FrameRightWithPlay));
//AddNode(videoNode, language.GoBack100Milliseconds, nameof(Configuration.Settings.Shortcuts.MainVideo100MsLeft));
//AddNode(videoNode, language.GoForward100Milliseconds, nameof(Configuration.Settings.Shortcuts.MainVideo100MsRight));
//AddNode(videoNode, language.GoBack500Milliseconds, nameof(Configuration.Settings.Shortcuts.MainVideo500MsLeft));
//AddNode(videoNode, language.GoForward500Milliseconds, nameof(Configuration.Settings.Shortcuts.MainVideo500MsRight));
//AddNode(videoNode, language.GoBack1Second, nameof(Configuration.Settings.Shortcuts.MainVideo1000MsLeft));
//AddNode(videoNode, language.GoForward1Second, nameof(Configuration.Settings.Shortcuts.MainVideo1000MsRight));
//AddNode(videoNode, language.GoBack3Seconds, nameof(Configuration.Settings.Shortcuts.MainVideo3000MsLeft));
//AddNode(videoNode, language.GoForward3Seconds, nameof(Configuration.Settings.Shortcuts.MainVideo3000MsRight));
//AddNode(videoNode, language.GoBack5Seconds, nameof(Configuration.Settings.Shortcuts.MainVideo5000MsLeft));
//AddNode(videoNode, language.GoForward5Seconds, nameof(Configuration.Settings.Shortcuts.MainVideo5000MsRight));
//AddNode(videoNode, language.GoBackXSSeconds, nameof(Configuration.Settings.Shortcuts.MainVideoXSMsLeft));
//AddNode(videoNode, language.GoForwardXSSeconds, nameof(Configuration.Settings.Shortcuts.MainVideoXSMsRight));
//AddNode(videoNode, language.GoBackXLSeconds, nameof(Configuration.Settings.Shortcuts.MainVideoXLMsLeft));
//AddNode(videoNode, language.GoForwardXLSeconds, nameof(Configuration.Settings.Shortcuts.MainVideoXLMsRight));
//AddNode(videoNode, language.GoToStartCurrent, nameof(Configuration.Settings.Shortcuts.MainVideoGoToStartCurrent));
//AddNode(videoNode, language.ToggleStartEndCurrent, nameof(Configuration.Settings.Shortcuts.MainVideoToggleStartEndCurrent));
//AddNode(videoNode, language.PlaySelectedLines, nameof(Configuration.Settings.Shortcuts.MainVideoPlaySelectedLines));
//AddNode(videoNode, language.LoopSelectedLines, nameof(Configuration.Settings.Shortcuts.MainVideoLoopSelectedLines));
//AddNode(videoNode, language.WaveformGoToPrevSubtitle, nameof(Configuration.Settings.Shortcuts.MainVideoGoToPrevSubtitle));
//AddNode(videoNode, language.WaveformGoToNextSubtitle, nameof(Configuration.Settings.Shortcuts.MainVideoGoToNextSubtitle));
//AddNode(videoNode, language.WaveformGoToPrevTimeCode, nameof(Configuration.Settings.Shortcuts.MainVideoGoToPrevTimeCode));
//AddNode(videoNode, language.WaveformGoToNextTimeCode, nameof(Configuration.Settings.Shortcuts.MainVideoGoToNextTimeCode));
//AddNode(videoNode, language.WaveformGoToPrevChapter, nameof(Configuration.Settings.Shortcuts.MainVideoGoToPrevChapter));
//AddNode(videoNode, language.WaveformGoToNextChapter, nameof(Configuration.Settings.Shortcuts.MainVideoGoToNextChapter));
//AddNode(videoNode, language.WaveformSelectNextSubtitle, nameof(Configuration.Settings.Shortcuts.MainVideoSelectNextSubtitle));
//AddNode(videoNode, language.Fullscreen, nameof(Configuration.Settings.Shortcuts.MainVideoFullscreen));
//AddNode(videoNode, language.Play150Speed, nameof(Configuration.Settings.Shortcuts.MainVideoPlay150Speed));
//AddNode(videoNode, language.Play200Speed, nameof(Configuration.Settings.Shortcuts.MainVideoPlay200Speed));
//AddNode(videoNode, language.PlayRateSlower, nameof(Configuration.Settings.Shortcuts.MainVideoSlower));
//AddNode(videoNode, language.PlayRateFaster, nameof(Configuration.Settings.Shortcuts.MainVideoFaster));
//AddNode(videoNode, language.PlayRateToggle, nameof(Configuration.Settings.Shortcuts.MainVideoSpeedToggle));
//AddNode(videoNode, language.VideoResetSpeedAndZoom, nameof(Configuration.Settings.Shortcuts.MainVideoReset));
//AddNode(videoNode, language.MainToggleVideoControls, nameof(Configuration.Settings.Shortcuts.MainVideoToggleControls));
//AddNode(videoNode, string.Format(language.AudioToTextX, "Vosk"), nameof(Configuration.Settings.Shortcuts.MainVideoAudioToTextVosk));
//            AddNode(videoNode, string.Format(language.AudioToTextX, "Whisper"), nameof(Configuration.Settings.Shortcuts.MainVideoAudioToTextWhisper));
//            AddNode(videoNode, LanguageSettings.Current.TextToSpeech.Title, nameof(Configuration.Settings.Shortcuts.MainVideoTextToSpeech));
//AddNode(videoNode, language.AudioExtractSelectedLines, nameof(Configuration.Settings.Shortcuts.MainVideoAudioExtractAudioSelectedLines));
//AddNode(videoNode, language.VideoToggleContrast, nameof(Configuration.Settings.Shortcuts.MainVideoToggleContrast));
//AddNode(videoNode, language.VideoToggleBrightness, nameof(Configuration.Settings.Shortcuts.MainVideoToggleBrightness));
//_shortcuts.Nodes.Add(videoNode);

//            var spellCheckNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.SpellCheck.Title);
//AddNode(spellCheckNode, LanguageSettings.Current.Main.Menu.SpellCheck.Title, nameof(Configuration.Settings.Shortcuts.MainSpellCheck), true);
//AddNode(spellCheckNode, LanguageSettings.Current.Main.Menu.SpellCheck.FindDoubleWords, nameof(Configuration.Settings.Shortcuts.MainSpellCheckFindDoubleWords), true);
//AddNode(spellCheckNode, LanguageSettings.Current.Main.Menu.SpellCheck.AddToNameList, nameof(Configuration.Settings.Shortcuts.MainSpellCheckAddWordToNames), true);
//_shortcuts.Nodes.Add(spellCheckNode);

//            var syncNode = new ShortcutNode(LanguageSettings.Current.Main.Menu.Synchronization.Title);
//AddNode(syncNode, LanguageSettings.Current.Main.Menu.Synchronization.AdjustAllTimes, nameof(Configuration.Settings.Shortcuts.MainSynchronizationAdjustTimes), true);
//AddNode(syncNode, LanguageSettings.Current.Main.Menu.Synchronization.VisualSync, nameof(Configuration.Settings.Shortcuts.MainSynchronizationVisualSync), true);
//AddNode(syncNode, LanguageSettings.Current.Main.Menu.Synchronization.PointSync, nameof(Configuration.Settings.Shortcuts.MainSynchronizationPointSync), true);
//AddNode(syncNode, LanguageSettings.Current.Main.Menu.Synchronization.PointSyncViaOtherSubtitle, nameof(Configuration.Settings.Shortcuts.MainSynchronizationPointSyncViaFile), true);
//AddNode(syncNode, LanguageSettings.Current.Main.Menu.Tools.ChangeFrameRate, nameof(Configuration.Settings.Shortcuts.MainSynchronizationChangeFrameRate), true);
//_shortcuts.Nodes.Add(syncNode);

//            var listViewAndTextBoxNode = new ShortcutNode(language.ListViewAndTextBox);
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.InsertAfter, nameof(Configuration.Settings.Shortcuts.MainInsertAfter));
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.InsertBefore, nameof(Configuration.Settings.Shortcuts.MainInsertBefore));
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.General.Italic, nameof(Configuration.Settings.Shortcuts.MainListViewItalic), true);
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.General.Bold, nameof(Configuration.Settings.Shortcuts.MainListViewBold), true);
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.General.Underline, nameof(Configuration.Settings.Shortcuts.MainListViewUnderline), true);
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.Box, nameof(Configuration.Settings.Shortcuts.MainListViewBox), true);
//AddNode(listViewAndTextBoxNode, language.ToggleQuotes, nameof(Configuration.Settings.Shortcuts.MainListViewToggleQuotes), true);
//AddNode(listViewAndTextBoxNode, language.ToggleHiTags, nameof(Configuration.Settings.Shortcuts.MainListViewToggleHiTags), true);
//AddNode(listViewAndTextBoxNode, language.ToggleCustomTags, nameof(Configuration.Settings.Shortcuts.MainListViewToggleCustomTags), false);
//AddNode(listViewAndTextBoxNode, language.MainTextBoxSelectionToggleCasing, nameof(Configuration.Settings.Shortcuts.MainTextBoxSelectionToggleCasing));
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.General.SplitLine.Replace("!", string.Empty), nameof(Configuration.Settings.Shortcuts.MainListViewSplit), true);
//            AddNode(listViewAndTextBoxNode, language.ToggleMusicSymbols, nameof(Configuration.Settings.Shortcuts.MainListViewToggleMusicSymbols), true);
//AddNode(listViewAndTextBoxNode, language.AlignmentN1, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN1));
//AddNode(listViewAndTextBoxNode, language.AlignmentN2, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN2));
//AddNode(listViewAndTextBoxNode, language.AlignmentN3, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN3));
//AddNode(listViewAndTextBoxNode, language.AlignmentN4, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN4));
//AddNode(listViewAndTextBoxNode, language.AlignmentN5, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN5));
//AddNode(listViewAndTextBoxNode, language.AlignmentN6, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN6));
//AddNode(listViewAndTextBoxNode, language.AlignmentN7, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN7));
//AddNode(listViewAndTextBoxNode, language.AlignmentN8, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN8));
//AddNode(listViewAndTextBoxNode, language.AlignmentN9, nameof(Configuration.Settings.Shortcuts.MainListViewAlignmentN9));
//AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "1", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color1)), nameof(Configuration.Settings.Shortcuts.MainListViewColor1));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "2", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color2)), nameof(Configuration.Settings.Shortcuts.MainListViewColor2));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "3", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color3)), nameof(Configuration.Settings.Shortcuts.MainListViewColor3));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "4", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color4)), nameof(Configuration.Settings.Shortcuts.MainListViewColor4));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "5", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color5)), nameof(Configuration.Settings.Shortcuts.MainListViewColor5));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "6", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color6)), nameof(Configuration.Settings.Shortcuts.MainListViewColor6));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "7", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color7)), nameof(Configuration.Settings.Shortcuts.MainListViewColor7));
//            AddNode(listViewAndTextBoxNode, string.Format(language.ColorX, "8", ColorTranslator.ToHtml(Configuration.Settings.Tools.Color8)), nameof(Configuration.Settings.Shortcuts.MainListViewColor8));
//            AddNode(listViewAndTextBoxNode, LanguageSettings.Current.DCinemaProperties.FontColor, nameof(Configuration.Settings.Shortcuts.MainListViewColorChoose), true);
//AddNode(listViewAndTextBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.RemoveFormattingAll, nameof(Configuration.Settings.Shortcuts.MainRemoveFormatting), true);
//AddNode(listViewAndTextBoxNode, language.RemoveTimeCodes, nameof(Configuration.Settings.Shortcuts.MainListViewRemoveTimeCodes));
//AddNode(listViewAndTextBoxNode, language.MainTextBoxUnbreak, nameof(Configuration.Settings.Shortcuts.MainTextBoxUnbreak));
//AddNode(listViewAndTextBoxNode, language.MainTextBoxUnbreakNoSpace, nameof(Configuration.Settings.Shortcuts.MainTextBoxUnbreakNoSpace));
//AddNode(listViewAndTextBoxNode, language.SetNewActor, nameof(Configuration.Settings.Shortcuts.MainListViewSetNewActor));
//AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "1"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor1), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "2"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor2), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "3"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor3), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "4"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor4), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "5"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor5), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "6"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor6), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "7"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor7), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "8"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor8), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "9"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor9), true);
//            AddNode(listViewAndTextBoxNode, string.Format(language.SetActorX, "10"), nameof(Configuration.Settings.Shortcuts.MainListViewSetActor10), true);
//            _shortcuts.Nodes.Add(listViewAndTextBoxNode);

//            var listViewNode = new ShortcutNode(language.ListView);
//AddNode(listViewNode, language.MergeDialog, nameof(Configuration.Settings.Shortcuts.MainMergeDialog));
//AddNode(listViewNode, language.MergeDialogWithNext, nameof(Configuration.Settings.Shortcuts.MainMergeDialogWithNext));
//AddNode(listViewNode, language.MergeDialogWithPrevious, nameof(Configuration.Settings.Shortcuts.MainMergeDialogWithPrevious));
//AddNode(listViewNode, language.AutoBalanceSelectedLines, nameof(Configuration.Settings.Shortcuts.MainAutoBalanceSelectedLines), true);
//AddNode(listViewNode, language.EvenlyDistributeSelectedLines, nameof(Configuration.Settings.Shortcuts.MainEvenlyDistributeSelectedLines), true);
//AddNode(listViewNode, language.ToggleFocus, nameof(Configuration.Settings.Shortcuts.MainToggleFocus));
//AddNode(listViewNode, language.ToggleFocusWaveform, nameof(Configuration.Settings.Shortcuts.MainToggleFocusWaveform));
//AddNode(listViewNode, language.ToggleFocusWaveformTextBox, nameof(Configuration.Settings.Shortcuts.MainToggleFocusWaveformTextBox));
//AddNode(listViewNode, language.ToggleDialogDashes, nameof(Configuration.Settings.Shortcuts.MainListViewToggleDashes));
//AddNode(listViewNode, language.Alignment, nameof(Configuration.Settings.Shortcuts.MainListViewAlignment), true);
//AddNode(listViewNode, language.CopyTextOnly, nameof(Configuration.Settings.Shortcuts.MainListViewCopyText));
//AddNode(listViewNode, language.CopyPlainText, nameof(Configuration.Settings.Shortcuts.MainListViewCopyPlainText));
//AddNode(listViewNode, language.CopyTextOnlyFromOriginalToCurrent, nameof(Configuration.Settings.Shortcuts.MainListViewCopyTextFromOriginalToCurrent), true);
//AddNode(listViewNode, language.AutoDurationSelectedLines, nameof(Configuration.Settings.Shortcuts.MainListViewAutoDuration));
//AddNode(listViewNode, language.ListViewColumnDelete, nameof(Configuration.Settings.Shortcuts.MainListViewColumnDeleteText), true);
//AddNode(listViewNode, language.ListViewColumnDeleteAndShiftUp, nameof(Configuration.Settings.Shortcuts.MainListViewColumnDeleteTextAndShiftUp), true);
//AddNode(listViewNode, language.ListViewColumnInsert, nameof(Configuration.Settings.Shortcuts.MainListViewColumnInsertText), true);
//AddNode(listViewNode, language.ListViewColumnPaste, nameof(Configuration.Settings.Shortcuts.MainListViewColumnPaste), true);
//AddNode(listViewNode, language.ListViewColumnTextUp, nameof(Configuration.Settings.Shortcuts.MainListViewColumnTextUp), true);
//AddNode(listViewNode, language.ListViewColumnTextDown, nameof(Configuration.Settings.Shortcuts.MainListViewColumnTextDown), true);
//AddNode(listViewNode, language.ListViewGoToNextError, nameof(Configuration.Settings.Shortcuts.MainListViewGoToNextError));
//AddNode(listViewNode, language.ListViewListErrors, nameof(Configuration.Settings.Shortcuts.MainListViewListErrors), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.Number), nameof(Configuration.Settings.Shortcuts.MainListViewSortByNumber), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.StartTime), nameof(Configuration.Settings.Shortcuts.MainListViewSortByStartTime), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.EndTime), nameof(Configuration.Settings.Shortcuts.MainListViewSortByEndTime), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.Duration), nameof(Configuration.Settings.Shortcuts.MainListViewSortByDuration), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.Gap), nameof(Configuration.Settings.Shortcuts.MainListViewSortByGap), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.Main.Menu.Tools.TextAlphabetically), nameof(Configuration.Settings.Shortcuts.MainListViewSortByText), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.Main.Menu.Tools.TextSingleLineMaximumLength), nameof(Configuration.Settings.Shortcuts.MainListViewSortBySingleLineMaxLen), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.Main.Menu.Tools.TextTotalLength), nameof(Configuration.Settings.Shortcuts.MainListViewSortByTextTotalLength), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.CharsPerSec), nameof(Configuration.Settings.Shortcuts.MainListViewSortByCps), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.WordsPerMin), nameof(Configuration.Settings.Shortcuts.MainListViewSortByWpm), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.Main.Menu.Tools.TextNumberOfLines), nameof(Configuration.Settings.Shortcuts.MainListViewSortByNumberOfLines), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.Style), nameof(Configuration.Settings.Shortcuts.MainListViewSortByStyle), true);
//AddNode(listViewNode, string.Format(language.ListViewListSortByX, LanguageSettings.Current.General.Actor), nameof(Configuration.Settings.Shortcuts.MainListViewSortByActor), true);
//_shortcuts.Nodes.Add(listViewNode);

//            var textBoxNode = new ShortcutNode(language.TextBox);
//AddNode(textBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.SplitLineAtCursorPosition, nameof(Configuration.Settings.Shortcuts.MainTextBoxSplitAtCursor));
//AddNode(textBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.SplitLineAtCursorPositionAndAutoBr, nameof(Configuration.Settings.Shortcuts.MainTextBoxSplitAtCursorAndAutoBr));
//AddNode(textBoxNode, LanguageSettings.Current.Main.Menu.ContextMenu.SplitLineAtCursorAndWaveformPosition, nameof(Configuration.Settings.Shortcuts.MainTextBoxSplitAtCursorAndVideoPos));
//AddNode(textBoxNode, language.SplitSelectedLineBilingual, nameof(Configuration.Settings.Shortcuts.MainTextBoxSplitSelectedLineBilingual));
//AddNode(textBoxNode, language.MainTextBoxMoveLastWordDown, nameof(Configuration.Settings.Shortcuts.MainTextBoxMoveLastWordDown));
//AddNode(textBoxNode, language.MainTextBoxMoveFirstWordFromNextUp, nameof(Configuration.Settings.Shortcuts.MainTextBoxMoveFirstWordFromNextUp));
//AddNode(textBoxNode, language.MainTextBoxMoveLastWordDownCurrent, nameof(Configuration.Settings.Shortcuts.MainTextBoxMoveLastWordDownCurrent));
//AddNode(textBoxNode, language.MainTextBoxMoveFirstWordUpCurrent, nameof(Configuration.Settings.Shortcuts.MainTextBoxMoveFirstWordUpCurrent));
//AddNode(textBoxNode, language.MainTextBoxMoveFromCursorToNext, nameof(Configuration.Settings.Shortcuts.MainTextBoxMoveFromCursorToNextAndGoToNext));
//AddNode(textBoxNode, language.MainTextBoxSelectionToLower, nameof(Configuration.Settings.Shortcuts.MainTextBoxSelectionToLower));
//AddNode(textBoxNode, language.MainTextBoxSelectionToUpper, nameof(Configuration.Settings.Shortcuts.MainTextBoxSelectionToUpper));
//AddNode(textBoxNode, language.MainTextBoxSelectionToRuby, nameof(Configuration.Settings.Shortcuts.MainTextBoxSelectionToRuby), true);
//AddNode(textBoxNode, language.MainTextBoxToggleAutoDuration, nameof(Configuration.Settings.Shortcuts.MainTextBoxToggleAutoDuration));
//AddNode(textBoxNode, language.MainTextBoxAutoBreak, nameof(Configuration.Settings.Shortcuts.MainTextBoxAutoBreak));
//AddNode(textBoxNode, language.MainTextBoxAutoBreakFromPos, nameof(Configuration.Settings.Shortcuts.MainTextBoxBreakAtPosition));
//AddNode(textBoxNode, language.MainTextBoxAutoBreakFromPosAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainTextBoxBreakAtPositionAndGoToNext));
//AddNode(textBoxNode, language.MainTextBoxDictate, nameof(Configuration.Settings.Shortcuts.MainTextBoxRecord));
//AddNode(textBoxNode, language.MainTextBoxAssaIntellisense, nameof(Configuration.Settings.Shortcuts.MainTextBoxAssaIntellisense));
//AddNode(textBoxNode, language.MainTextBoxAssaRemoveTag, nameof(Configuration.Settings.Shortcuts.MainTextBoxAssaRemoveTag));
//_shortcuts.Nodes.Add(textBoxNode);

//            var translateNode = new ShortcutNode(LanguageSettings.Current.Main.VideoControls.Translate);
//AddNode(translateNode, LanguageSettings.Current.Main.VideoControls.GoogleIt, nameof(Configuration.Settings.Shortcuts.MainTranslateGoogleIt));
//AddNode(translateNode, LanguageSettings.Current.Main.VideoControls.GoogleTranslate, nameof(Configuration.Settings.Shortcuts.MainTranslateGoogleTranslateIt));
//AddNode(translateNode, LanguageSettings.Current.Main.VideoControls.AutoTranslate, nameof(Configuration.Settings.Shortcuts.MainTranslateAuto), true);
//AddNode(translateNode, language.AutoTranslateSelectedLines, nameof(Configuration.Settings.Shortcuts.MainTranslateAutoSelectedLines), true);
//AddNode(translateNode, language.CustomSearch1, nameof(Configuration.Settings.Shortcuts.MainTranslateCustomSearch1));
//AddNode(translateNode, language.CustomSearch2, nameof(Configuration.Settings.Shortcuts.MainTranslateCustomSearch2));
//AddNode(translateNode, language.CustomSearch3, nameof(Configuration.Settings.Shortcuts.MainTranslateCustomSearch3));
//AddNode(translateNode, language.CustomSearch4, nameof(Configuration.Settings.Shortcuts.MainTranslateCustomSearch4));
//AddNode(translateNode, language.CustomSearch5, nameof(Configuration.Settings.Shortcuts.MainTranslateCustomSearch5));
//_shortcuts.Nodes.Add(translateNode);

//            var createAndAdjustNode = new ShortcutNode(LanguageSettings.Current.Main.VideoControls.CreateAndAdjust);
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.InsertNewSubtitleAtVideoPosition, nameof(Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPos));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.InsertNewSubtitleAtVideoPositionNoTextBoxFocus, nameof(Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPosNoTextBoxFocus));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.InsertNewSubtitleAtVideoPositionMax, nameof(Configuration.Settings.Shortcuts.MainCreateInsertSubAtVideoPosMax));
//AddNode(createAndAdjustNode, language.MainCreateStartDownEndUp, nameof(Configuration.Settings.Shortcuts.MainCreateStartDownEndUp));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.SetStartTime, nameof(Configuration.Settings.Shortcuts.MainCreateSetStart));
//AddNode(createAndAdjustNode, language.AdjustSetStartTimeKeepDuration, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartKeepDuration));
//AddNode(createAndAdjustNode, language.AdjustVideoSetStartForAppropriateLine, nameof(Configuration.Settings.Shortcuts.MainAdjustVideoSetStartForAppropriateLine));
//AddNode(createAndAdjustNode, language.AdjustVideoSetEndForAppropriateLine, nameof(Configuration.Settings.Shortcuts.MainAdjustVideoSetEndForAppropriateLine));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.SetStartTimeAndOffsetTheRest, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.SetStartTimeAndOffsetTheRest, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheRest2));
//AddNode(createAndAdjustNode, language.AdjustSetStartAndOffsetTheWholeSubtitle, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndOffsetTheWholeSubtitle));
//AddNode(createAndAdjustNode, language.AdjustSetStartAutoDurationAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAutoDurationAndGoToNext));
//AddNode(createAndAdjustNode, language.AdjustStartDownEndUpAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustStartDownEndUpAndGoToNext));
//AddNode(createAndAdjustNode, language.AdjustSetStartAndEndOfPrevious, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndEndOfPrevious));
//AddNode(createAndAdjustNode, language.AdjustSetStartAndEndOfPreviousAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndEndOfPreviousAndGoToNext));
//AddNode(createAndAdjustNode, LanguageSettings.Current.Main.VideoControls.SetEndTime, nameof(Configuration.Settings.Shortcuts.MainCreateSetEnd));
//AddNode(createAndAdjustNode, language.AdjustSetEndTimeAndPause, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndAndPause));
//AddNode(createAndAdjustNode, language.CreateSetEndAddNewAndGoToNew, nameof(Configuration.Settings.Shortcuts.MainCreateSetEndAddNewAndGoToNew));
//AddNode(createAndAdjustNode, language.AdjustSetStartTimeAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetStartAndGotoNext));
//AddNode(createAndAdjustNode, language.AdjustSetEndTimeAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndAndGotoNext));
//AddNode(createAndAdjustNode, language.AdjustSetEndAndOffsetTheRest, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRest));
//AddNode(createAndAdjustNode, language.AdjustSetEndAndOffsetTheRestAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndAndOffsetTheRestAndGoToNext));
//AddNode(createAndAdjustNode, language.AdjustSetEndNextStartAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndNextStartAndGoToNext));
//AddNode(createAndAdjustNode, language.AdjustSetEndMinusGapAndStartNextHere, nameof(Configuration.Settings.Shortcuts.MainAdjustSetEndMinusGapAndStartNextHere));
//AddNode(createAndAdjustNode, language.AdjustSetEndAndStartNextAfterGap, nameof(Configuration.Settings.Shortcuts.MainSetEndAndStartNextAfterGap));
//AddNode(createAndAdjustNode, language.AdjustViaEndAutoStart, nameof(Configuration.Settings.Shortcuts.MainAdjustViaEndAutoStart));
//AddNode(createAndAdjustNode, language.AdjustViaEndAutoStartAndGoToNext, nameof(Configuration.Settings.Shortcuts.MainAdjustViaEndAutoStartAndGoToNext));
//AddNode(createAndAdjustNode, language.AdjustSelected100MsBack, nameof(Configuration.Settings.Shortcuts.MainAdjustSelected100MsBack));
//AddNode(createAndAdjustNode, language.AdjustSelected100MsForward, nameof(Configuration.Settings.Shortcuts.MainAdjustSelected100MsForward));
//AddNode(createAndAdjustNode, string.Format(language.AdjustStartXMsBack, Configuration.Settings.Tools.MoveStartEndMs), nameof(Configuration.Settings.Shortcuts.MainAdjustStartXMsBack));
//AddNode(createAndAdjustNode, string.Format(language.AdjustStartXMsForward, Configuration.Settings.Tools.MoveStartEndMs), nameof(Configuration.Settings.Shortcuts.MainAdjustStartXMsForward));
//AddNode(createAndAdjustNode, string.Format(language.AdjustEndXMsBack, Configuration.Settings.Tools.MoveStartEndMs), nameof(Configuration.Settings.Shortcuts.MainAdjustEndXMsBack));
//AddNode(createAndAdjustNode, string.Format(language.AdjustEndXMsForward, Configuration.Settings.Tools.MoveStartEndMs), nameof(Configuration.Settings.Shortcuts.MainAdjustEndXMsForward));
//AddNode(createAndAdjustNode, language.AdjustStartOneFrameBack, nameof(Configuration.Settings.Shortcuts.MoveStartOneFrameBack));
//AddNode(createAndAdjustNode, language.AdjustStartOneFrameForward, nameof(Configuration.Settings.Shortcuts.MoveStartOneFrameForward));
//AddNode(createAndAdjustNode, language.AdjustEndOneFrameBack, nameof(Configuration.Settings.Shortcuts.MoveEndOneFrameBack));
//AddNode(createAndAdjustNode, language.AdjustEndOneFrameForward, nameof(Configuration.Settings.Shortcuts.MoveEndOneFrameForward));
//AddNode(createAndAdjustNode, language.AdjustStartOneFrameBackKeepGapPrev, nameof(Configuration.Settings.Shortcuts.MoveStartOneFrameBackKeepGapPrev));
//AddNode(createAndAdjustNode, language.AdjustStartOneFrameForwardKeepGapPrev, nameof(Configuration.Settings.Shortcuts.MoveStartOneFrameForwardKeepGapPrev));
//AddNode(createAndAdjustNode, language.AdjustEndOneFrameBackKeepGapNext, nameof(Configuration.Settings.Shortcuts.MoveEndOneFrameBackKeepGapNext));
//AddNode(createAndAdjustNode, language.AdjustEndOneFrameForwardKeepGapNext, nameof(Configuration.Settings.Shortcuts.MoveEndOneFrameForwardKeepGapNext));
//AddNode(createAndAdjustNode, language.RecalculateDurationOfCurrentSubtitle, nameof(Configuration.Settings.Shortcuts.GeneralAutoCalcCurrentDuration));
//AddNode(createAndAdjustNode, language.RecalculateDurationOfCurrentSubtitleByOptimalReadingSpeed, nameof(Configuration.Settings.Shortcuts.GeneralAutoCalcCurrentDurationByOptimalReadingSpeed));
//AddNode(createAndAdjustNode, language.RecalculateDurationOfCurrentSubtitleByMinReadingSpeed, nameof(Configuration.Settings.Shortcuts.GeneralAutoCalcCurrentDurationByMinReadingSpeed));
//AddNode(createAndAdjustNode, language.AdjustSnapStartToNextShotChange, nameof(Configuration.Settings.Shortcuts.MainAdjustSnapStartToNextShotChange));
//AddNode(createAndAdjustNode, language.AdjustSnapEndToPreviousShotChange, nameof(Configuration.Settings.Shortcuts.MainAdjustSnapEndToPreviousShotChange));
//AddNode(createAndAdjustNode, language.AdjustExtendToNextShotChange, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToNextShotChange));
//AddNode(createAndAdjustNode, language.AdjustExtendToPreviousShotChange, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToPreviousShotChange));
//AddNode(createAndAdjustNode, language.AdjustExtendToNextSubtitle, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToNextSubtitle));
//AddNode(createAndAdjustNode, language.AdjustExtendToPreviousSubtitle, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToPreviousSubtitle));
//AddNode(createAndAdjustNode, language.AdjustExtendToNextSubtitleMinusChainingGap, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToNextSubtitleMinusChainingGap));
//AddNode(createAndAdjustNode, language.AdjustExtendToPreviousSubtitleMinusChainingGap, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendToPreviousSubtitleMinusChainingGap));
//AddNode(createAndAdjustNode, language.AdjustExtendCurrentSubtitle, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendCurrentSubtitle));
//AddNode(createAndAdjustNode, language.AdjustExtendPreviousLineEndToCurrentStart, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendPreviousLineEndToCurrentStart));
//AddNode(createAndAdjustNode, language.AdjustExtendNextLineStartToCurrentEnd, nameof(Configuration.Settings.Shortcuts.MainAdjustExtendNextLineStartToCurrentEnd));
//AddNode(createAndAdjustNode, language.SetInCueToClosestShotChangeLeftGreenZone, nameof(Configuration.Settings.Shortcuts.MainSetInCueToClosestShotChangeLeftGreenZone));
//AddNode(createAndAdjustNode, language.SetInCueToClosestShotChangeRightGreenZone, nameof(Configuration.Settings.Shortcuts.MainSetInCueToClosestShotChangeRightGreenZone));
//AddNode(createAndAdjustNode, language.SetOutCueToClosestShotChangeLeftGreenZone, nameof(Configuration.Settings.Shortcuts.MainSetOutCueToClosestShotChangeLeftGreenZone));
//AddNode(createAndAdjustNode, language.SetOutCueToClosestShotChangeRightGreenZone, nameof(Configuration.Settings.Shortcuts.MainSetOutCueToClosestShotChangeRightGreenZone));
//_shortcuts.Nodes.Add(createAndAdjustNode);

//            var audioVisualizerNode = new ShortcutNode(language.WaveformAndSpectrogram);
//AddNode(audioVisualizerNode, LanguageSettings.Current.Waveform.AddWaveformAndSpectrogram, nameof(Configuration.Settings.Shortcuts.WaveformAdd));
//AddNode(audioVisualizerNode, LanguageSettings.Current.Waveform.ZoomIn, nameof(Configuration.Settings.Shortcuts.WaveformZoomIn));
//AddNode(audioVisualizerNode, LanguageSettings.Current.Waveform.ZoomOut, nameof(Configuration.Settings.Shortcuts.WaveformZoomOut));
//AddNode(audioVisualizerNode, language.VerticalZoom, nameof(Configuration.Settings.Shortcuts.WaveformVerticalZoom));
//AddNode(audioVisualizerNode, language.VerticalZoomOut, nameof(Configuration.Settings.Shortcuts.WaveformVerticalZoomOut));
//AddNode(audioVisualizerNode, LanguageSettings.Current.Main.Menu.ContextMenu.Split, nameof(Configuration.Settings.Shortcuts.WaveformSplit));
//AddNode(audioVisualizerNode, language.WaveformSeekSilenceForward, nameof(Configuration.Settings.Shortcuts.WaveformSearchSilenceForward));
//AddNode(audioVisualizerNode, language.WaveformSeekSilenceBack, nameof(Configuration.Settings.Shortcuts.WaveformSearchSilenceBack));
//AddNode(audioVisualizerNode, language.WaveformAddTextHere, nameof(Configuration.Settings.Shortcuts.WaveformAddTextHere));
//AddNode(audioVisualizerNode, language.WaveformAddTextHereFromClipboard, nameof(Configuration.Settings.Shortcuts.WaveformAddTextHereFromClipboard));
//AddNode(audioVisualizerNode, language.SetParagraphAsSelection, nameof(Configuration.Settings.Shortcuts.WaveformSetParagraphAsSelection));
//AddNode(audioVisualizerNode, language.WaveformPlayNewSelection, nameof(Configuration.Settings.Shortcuts.WaveformPlaySelection));
//AddNode(audioVisualizerNode, language.WaveformPlayNewSelectionEnd, nameof(Configuration.Settings.Shortcuts.WaveformPlaySelectionEnd));
//AddNode(audioVisualizerNode, LanguageSettings.Current.Main.VideoControls.InsertNewSubtitleAtVideoPosition, nameof(Configuration.Settings.Shortcuts.MainWaveformInsertAtCurrentPosition));
//AddNode(audioVisualizerNode, language.WaveformGoToPreviousShotChange, nameof(Configuration.Settings.Shortcuts.WaveformGoToPreviousShotChange));
//AddNode(audioVisualizerNode, language.WaveformGoToNextShotChange, nameof(Configuration.Settings.Shortcuts.WaveformGoToNextShotChange));
//AddNode(audioVisualizerNode, language.WaveformAllShotChangesOneFrameBack, nameof(Configuration.Settings.Shortcuts.WaveformAllShotChangesOneFrameBack));
//AddNode(audioVisualizerNode, language.WaveformAllShotChangesOneFrameForward, nameof(Configuration.Settings.Shortcuts.WaveformAllShotChangesOneFrameForward));
//AddNode(audioVisualizerNode, language.WaveformToggleShotChange, nameof(Configuration.Settings.Shortcuts.WaveformToggleShotChange));
//AddNode(audioVisualizerNode, language.WaveformRemoveOrExportShotChanges, nameof(Configuration.Settings.Shortcuts.WaveformListShotChanges), true);
//AddNode(audioVisualizerNode, language.WaveformGuessStart, nameof(Configuration.Settings.Shortcuts.WaveformGuessStart));
//AddNode(audioVisualizerNode, language.GoBack100Milliseconds, nameof(Configuration.Settings.Shortcuts.Waveform100MsLeft));
//AddNode(audioVisualizerNode, language.GoForward100Milliseconds, nameof(Configuration.Settings.Shortcuts.Waveform100MsRight));
//AddNode(audioVisualizerNode, language.GoBack1Second, nameof(Configuration.Settings.Shortcuts.Waveform1000MsLeft));
//AddNode(audioVisualizerNode, language.GoForward1Second, nameof(Configuration.Settings.Shortcuts.Waveform1000MsRight));
//AddNode(audioVisualizerNode, string.Format(language.AudioToTextSelectedLinesX, "Vosk"), nameof(Configuration.Settings.Shortcuts.WaveformAudioToTextVosk));
//            AddNode(audioVisualizerNode, string.Format(language.AudioToTextSelectedLinesX, "Whisper"), nameof(Configuration.Settings.Shortcuts.WaveformAudioToTextWhisper));
//            _shortcuts.Nodes.Add(audioVisualizerNode);
