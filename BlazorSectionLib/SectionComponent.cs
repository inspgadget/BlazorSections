using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using static BlazorSectionLib.SectionService;

namespace BlazorSectionLib
{
    public class SectionComponent : ComponentBase, IDisposable
    {
        [Inject] public SectionService Service { get; set; }
        [Parameter] public string SectionName { get; set; }
        private bool _initialised;
        private ISection _section;

        public SectionComponent()
        {
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            base.BuildRenderTree(builder);

            if (!_initialised)
            {
                if (string.IsNullOrWhiteSpace(SectionName))
                {
                    throw new ArgumentOutOfRangeException("SectionName must be set.");
                }
                _section = Service.RegisterSection(SectionName);
                _section.ChangesDone += Service_ChangesDone;
                _initialised = true;
            }

            int sequence = 0;
            foreach (Element element in _section.GetElements())
            {
                builder.OpenElement(sequence, element.Name);
                sequence++;
                foreach (var kv in element.AllProperties)
                {
                    builder.AddAttribute(sequence, kv.Key, kv.Value);
                    sequence++;
                }
                if (!string.IsNullOrWhiteSpace(element.Content))
                {
                    builder.AddContent(sequence, element.Content);
                    sequence++;
                }
                builder.CloseElement();
            }
        }

        protected async void Service_ChangesDone(object sender, EventArgs e)
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            Service = null;
            _section.ChangesDone -= Service_ChangesDone;
            _section = null;
        }
    }
}