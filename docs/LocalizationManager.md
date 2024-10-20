# 如何在 BepInEx 插件中为新添加的 GUI 组件添加对应翻译

## I2.Loc

- LocalizationManager 本地化数据管理器
  * Sources `List<LanguageSourceData>`

```csharp
public static void UpdateSources()
{
    if (I2.Loc.LocalizationManager.Sources
        .SelectMany(loaded => loaded.GetCategories())
        .Any(category => category == "Mod"))
    {
        // 避免重复添加
        return;
    }
    
    var source = new I2.Loc.LanguageSourceData();
    try
    {
        source.Import_CSV(Category: "Mod", CSVstring: Resource("MOD.csv"));
    }
    catch (FileNotFoundException e)
    {
        Logger.LogError(e);
        return;
    }
    
    I2.Loc.LocalizationManager.Sources.Add(source);
    Logger.LogInfo("Mod LanguageSource Loaded.");
}
```

MOD.csv:
```csv
Key,Type,Desc,English,French,German,Russian,Polish,Portuguese,Spanish,Korean,Japanese,Chinese (Simplified),Chinese (Traditional)
Tab_Mod,Text,,MOD,,,,,,,,,模组,
```

- Localize
  * Term

```csharp
public static void UpdateTerms(this GameObject body)
{
    var category = "Mod"; // 可自行修改
    var key = body.name; // 可自行修改
    body.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
        .ForEach(localize => localize.Term = $"{category}/{key}");
}
```
