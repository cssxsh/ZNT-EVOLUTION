# 如何在 BepInEx 插件中为新添加的 GUI 组件添加对应翻译

## I2.Loc

- LocalizationManager 本地化数据管理器
  * Sources `List<LanguageSourceData>`

```csharp
    if (I2.Loc.LocalizationManager.Sources
        .SelectMany(loaded => loaded.GetCategories())
        .Any(category => category == "Mod"))
    {
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
    }
    
    I2.Loc.LocalizationManager.Sources.Add(source);
```

```csv
Key,Type,Desc,English,French,German,Russian,Polish,Portuguese,Spanish,Korean,Japanese,Chinese (Simplified),Chinese (Traditional)
Tab_Mod,Text,,MOD,,,,,,,,,模组,
```

- Localize
  * Term

```csharp
body.GetComponentsInChildren<I2.Loc.Localize>(includeInactive: true)
    .ForEach(localize => localize.Term = $"{Category}/{Key}");
```
