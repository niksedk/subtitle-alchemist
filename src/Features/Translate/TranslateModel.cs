using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SubtitleAlchemist.Features.Main;
using Nikse.SubtitleEdit.Core.AutoTranslate;

namespace SubtitleAlchemist.Features.Translate;

public partial class TranslateModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private ObservableCollection<ExcelRow> _lines;

    [ObservableProperty]
    private ObservableCollection<DisplayParagraph> _paragraphs;

    [ObservableProperty]
    private ObservableCollection<IAutoTranslator> _autoTranslators;

    [ObservableProperty]
    private IAutoTranslator _selectedAutoTranslator;

    public TranslateModel()
    {
        Paragraphs = new ObservableCollection<DisplayParagraph>();

        Lines = new ObservableCollection<ExcelRow>();

        AutoTranslators = new ObservableCollection<IAutoTranslator>
        {
            new GoogleTranslateV1(),
            new DeepLTranslate(),
            new LibreTranslate(),
            new ChatGptTranslate(),
            new OllamaTranslate(),
            new AnthropicTranslate(),
            new GoogleTranslateV2(),
        };
        SelectedAutoTranslator = AutoTranslators[0];
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query["Paragraphs"] is ObservableCollection<DisplayParagraph> paragraphs)
        {
            Paragraphs = paragraphs;

            Lines.Clear();
            foreach (var p in Paragraphs)
            {
                Lines.Add(new ExcelRow
                {
                    Number = p.Number,
                    StartTime = p.Start,
                    OriginalText = p.Text,
                    TranslatedText = string.Empty,
                });
            }
        }
    }
}