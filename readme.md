# Blazor Sections Component

A component for adding sections similar to ASP.NET Core MVC sections.


  - Adding dynamically javascript
  - Adding dynamically css
  - Creating html-elements (e.g. title, metatags, etc.)

# Planned features

  - Adding Blazor components to section

## Installation

Install via [Nuget](https://www.nuget.org/).

>Server Side
```bash
Install-Package BlazorSections 
````

### Configuration

> #### Startup.cs

```C#
// Import Blazor.Auth0
using Blazor.Auth0;
using Blazor.Auth0.Models;
// ...

public void ConfigureServices(IServiceCollection services)
{
	// Other code...

     services.AddScoped<BlazorSectionLib.SectionService>()
     
	// Other code...
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
@using BlazorSectionLib
@inject SectionService _ss

...

@code {
    protected override async Task OnInitializedAsync()
    {
        _ss.AddElement("head", new SectionService.Element("title", "Jquery loaded"));
        _ss.AddElement("head", new SectionService.Javascript("https://cdn.jsdelivr.net/npm/jquery@3.4.1/dist/jquery.min.js"));
    }

...

}
```


## Authors
**InspGadget**

## License

This project is licensed under the GNU General Public License v3.0 - see the [LICENSE](https://github.com/inspgadget/BlazorSectionsComponent/blob/master/LICENSE) file for details.