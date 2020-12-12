# Cactus.Blade.Configuration [![Build status](https://ci.appveyor.com/api/projects/status/cqxs63x57368a08w?svg=true)](https://ci.appveyor.com/project/Cactus.Blade/Cactus.Blade-configuration-9b1x8)

Defines a static `Config` class as a general replacement for the old .NET Framework `ConfigurationManager` class.

## Table of Contents

- [Overview](#overview)
- [Library Usage](#library-usage)
- [Application Usage](#application-usage)
  - [.NET Framework Application Usage](#net-framework-application-usage)
- [Extension Methods](#extension-methods)

---

## Overview

The old .NET Framework `ConfigurationManager` class was very useful for libraries to use as a default source of per-application settings. For example, a class could define two constructors: one that defines all the settings for the class, and one parameterless constructor that reads the settings from configuration. But since `ConfigurationManager` no longer exists, this pattern becomes impossible. The `Config` class makes it possible again.

## Library Usage

Libraries are expected to access the `Config.Root` and `Config.AppSettings` properties in order to retrieve their settings.

```c#
IConfigurationSection fooSection = Config.Root.GetSection("foo");
string bar = Config.AppSettings["bar"];
```

## Application Usage

Applications are expected to provide an instance - the "Root" - of the `IConfiguration` interface to the `Config` class. This can be done explicitly or implicitly.

To explicitly set the configuration root, call the `SetRoot` method. ASP.NET Core Applications should call this method in their `Startup` constructor.

```c#
public Startup(IConfiguration configuration)
{
    Configuration = configuration;
    Config.SetRoot(Configuration);
}
```

If the configuration root is not explicitly set, it will load configuration settings, in order, from:

1. If the application is a .NET Framework app, from `ConfigurationManager` (see [.NET Framework Application Usage](#net-framework-application-usage) for details);
2. A `'appsettings.json'` file, relative to the current working directory;
3. A `'appsettings.{environment}.json file'`, relative to the current working directory, where `environment` is the value of the `ASPNETCORE_ENVIRONMENT` or `Cactus.Blade_ENVIRONMENT` environment variable;
4. Environment variables.

**Note that ASP.NET Core applications do not automatically load settings from `'appsettings.json'` - the configuration root must be set explicitly as described above.**

### .NET Framework Application Usage

Starting in Cactus.Blade.Configuration version 2.1.0, .NET Framework applications can configure their application completely through their app.config/web.config, without any additional setup.

When adding a Cactus.Blade.Configuration section to an app.config/web.config, it must be registered in the `<configSections>` section, then defined in the config body. This template shows how (also note that the rest of the examples below redefine the same `<my_section>` custom section):

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <section name="
```