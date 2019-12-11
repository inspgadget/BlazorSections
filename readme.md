# Blazor Sections Component

A component for adding sections similar to ASP.NET Core MVC sections.


  - Adding dynamically javascript
  - Adding dynamically css
  - Creating html-elements (e.g. title, metatags, etc.)
  
  *** Currently only tested for server-side Blazor ***

# Planned features

  - Adding Blazor components to section

## Installation

Install via [Nuget](https://www.nuget.org/).

```bash
Install-Package BlazorSections 
````

### Configuration

> #### Startup.cs

```C#
// ...

public void ConfigureServices(IServiceCollection services)
{
     ...
     
     services.AddScoped<BlazorSectionLib.SectionService>();
     
     ...
}

```

###
Add your sections whereever you want in the _Host.cshtml. The SectonName parameter is required.
> #### _Host.cshtml

```HTML
@page "/"
...
@using BlazorSectionLib
...

<!DOCTYPE html>
<html lang="en">
<head>
    ...
    @(await Html.RenderComponentAsync<SectionComponent>(RenderMode.ServerPrerendered, new { SectionName = "head" }))
</head>
<body>
    <app>
        @(await Html.RenderComponentAsync<App>(RenderMode.ServerPrerendered))
    </app>
    @(await Html.RenderComponentAsync<SectionComponent>(RenderMode.ServerPrerendered, new { SectionName = "body" }))
    ...
</body>
</html>
```
### Usage
> #### Page or Component
```HTML
@page "/head"
...
@implements IDisposable
@using BlazorSectionLib
@inject SectionService _ss

...

@code {
    protected override async Task OnInitializedAsync()
    {
        await Task.Run(() =>
        {
            _ss.AddElement("head", new SectionService.Element("title", "Jquery loaded"));
            _ss.AddElement("head", new SectionService.Javascript("https://cdn.jsdelivr.net/npm/jquery@3.4.1/dist/jquery.min.js"));
        });
    }

...
    public void Dispose()
    {
        _ss = null;
    }

}
```


## Authors
**InspGadget**

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](https://github.com/inspgadget/BlazorSectionsComponent/blob/master/LICENSE) file for details.
