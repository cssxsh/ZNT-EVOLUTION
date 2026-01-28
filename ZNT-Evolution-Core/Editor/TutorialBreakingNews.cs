using System.Collections;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming
namespace ZNT.Evolution.Core.Editor;

[SerializeInEditor(name: "Tutorial Breaking News")]
public class TutorialBreakingNews : Editor, IEnumerable<string>
{
    [SerializeInEditor(name: "Headline 1")]
    public string Headline1 = "";

    [SerializeInEditor(name: "Headline 2")]
    public string Headline2 = "";

    [SerializeInEditor(name: "Headline 3")]
    public string Headline3 = "";

    [SerializeInEditor(name: "Headline 4")]
    public string Headline4 = "";

    [SerializeInEditor(name: "Headline 5")]
    public string Headline5 = "";

    [SerializeInEditor(name: "Headline 6")]
    public string Headline6 = "";

    [SerializeInEditor(name: "Headline 7")]
    public string Headline7 = "";

    [SerializeInEditor(name: "Headline 8")]
    public string Headline8 = "";

    [SerializeInEditor(name: "Headline 9")]
    public string Headline9 = "";

    [SerializeInEditor(name: "Headline X")]
    public string HeadlineX = "";

    private void OnDespawned()
    {
        Headline1 = "";
        Headline2 = "";
        Headline3 = "";
        Headline4 = "";
        Headline5 = "";
        Headline6 = "";
        Headline7 = "";
        Headline8 = "";
        Headline9 = "";
        HeadlineX = "";
    }

    public IEnumerator<string> GetEnumerator()
    {
        if (!string.IsNullOrEmpty(Headline1)) yield return Headline1;
        if (!string.IsNullOrEmpty(Headline2)) yield return Headline2;
        if (!string.IsNullOrEmpty(Headline3)) yield return Headline3;
        if (!string.IsNullOrEmpty(Headline4)) yield return Headline4;
        if (!string.IsNullOrEmpty(Headline5)) yield return Headline5;
        if (!string.IsNullOrEmpty(Headline6)) yield return Headline6;
        if (!string.IsNullOrEmpty(Headline7)) yield return Headline7;
        if (!string.IsNullOrEmpty(Headline8)) yield return Headline8;
        if (!string.IsNullOrEmpty(Headline9)) yield return Headline9;
        if (!string.IsNullOrEmpty(HeadlineX)) yield return HeadlineX;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}