using System.Collections;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Tutorial Breaking News")]
public class TutorialBreakingNews : Editor, IEnumerable<LocalizableString>
{
    [SerializeInEditor(name: "Headline 1")]
    public LocalizableString Headline1 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 2")]
    public LocalizableString Headline2 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 3")]
    public LocalizableString Headline3 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 4")]
    public LocalizableString Headline4 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 5")]
    public LocalizableString Headline5 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 6")]
    public LocalizableString Headline6 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 7")]
    public LocalizableString Headline7 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 8")]
    public LocalizableString Headline8 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 9")]
    public LocalizableString Headline9 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline X")]
    public LocalizableString HeadlineX = new() { Localize = false, Category = "Headlines" };

    public IEnumerator<LocalizableString> GetEnumerator()
    {
        if (Headline1.Content is not (null or "")) yield return Headline1;
        if (Headline2.Content is not (null or "")) yield return Headline2;
        if (Headline3.Content is not (null or "")) yield return Headline3;
        if (Headline4.Content is not (null or "")) yield return Headline4;
        if (Headline5.Content is not (null or "")) yield return Headline5;
        if (Headline6.Content is not (null or "")) yield return Headline6;
        if (Headline7.Content is not (null or "")) yield return Headline7;
        if (Headline8.Content is not (null or "")) yield return Headline8;
        if (Headline9.Content is not (null or "")) yield return Headline9;
        if (HeadlineX.Content is not (null or "")) yield return HeadlineX;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}