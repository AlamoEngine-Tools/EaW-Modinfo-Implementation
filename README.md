# EaW-Modinfo-Implementation

[![Nuget](https://img.shields.io/nuget/v/AlamoEngineTools.Modinfo)](https://www.nuget.org/packages/AlamoEngineTools.Modinfo/) 
[![CI/CD Master](https://github.com/AlamoEngine-Tools/EaW-Modinfo-Implementation/actions/workflows/release.yml/badge.svg)](https://github.com/AlamoEngine-Tools/EaW-Modinfo-Implementation/actions/workflows/release.yml)

C# implementation for the [EaW Modinfo Specification](https://github.com/AlamoEngine-Tools/eaw.modinfo)

## Features

### Deserialize modinfo files into immutable Objects 

```cs
var file = new FileInfo("modinfo.json");
IModinfoFile modinfoFile = new MainModinfoFile(file);

IModinfo modinfo = modinfoFile.GetModinfo();
```

### Searching for modinfo files

```cs
var dir = new DirectoryInfo("YourModPath");
ModinfoFinderCollection result = ModinfoFileFinder.FindModinfoFiles(dir);

IModinfoFile mainFile = result.MainModinfo;
IEnumerable<IModinfoFile> variantFiles = result.Variants;
```

### Merging modfino data

```cs
var file1 = new FileInfo("modinfo.json");
IModinfoFile mainModinfoFile = new MainModinfoFile(file1);

var file2 = new FileInfo("varaint-modinfo.json");
IModinfoFile variantDerived = new ModinfoVariantFile(file2, mainModinfoFile);
```
