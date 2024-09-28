﻿namespace SubtitleAlchemist.Logic.Dictionaries;

public interface INamesList
{
    void Load(string dictionaryFolder, string languageCode);
    bool IsName(string candidate);
    HashSet<string> GetAbbreviations();
}