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
    public LocalizableString Headline7 =  new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 8")]
    public LocalizableString Headline8 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline 9")]
    public LocalizableString Headline9 = new() { Localize = false, Category = "Headlines" };

    [SerializeInEditor(name: "Headline X")]
    public LocalizableString HeadlineX = new() { Localize = false, Category = "Headlines" };

    public IEnumerator<LocalizableString> GetEnumerator()
    {
        if (!string.IsNullOrEmpty(Headline1.Content)) yield return Headline1;
        if (!string.IsNullOrEmpty(Headline2.Content)) yield return Headline2;
        if (!string.IsNullOrEmpty(Headline3.Content)) yield return Headline3;
        if (!string.IsNullOrEmpty(Headline4.Content)) yield return Headline4;
        if (!string.IsNullOrEmpty(Headline5.Content)) yield return Headline5;
        if (!string.IsNullOrEmpty(Headline6.Content)) yield return Headline6;
        if (!string.IsNullOrEmpty(Headline7.Content)) yield return Headline7;
        if (!string.IsNullOrEmpty(Headline8.Content)) yield return Headline8;
        if (!string.IsNullOrEmpty(Headline9.Content)) yield return Headline9;
        if (!string.IsNullOrEmpty(HeadlineX.Content)) yield return HeadlineX;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}