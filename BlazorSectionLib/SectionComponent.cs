using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using static BlazorSectionLib.SectionService;

namespace BlazorSectionLib
{
    public class SectionComponent : ComponentBase, IDisposable
    {
        [Inject] public SectionService Service { get; set; }
        [Parameter] public string SectionName { get; set; }
        private bool _initialised;
        private ISection _section;
        private int _sequence;

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
            List<Element> elements = _initialised ? _section.Elements.Where(x => !x.ShouldUpdate).ToList() : _section.Elements;
            foreach (Element element in elements)
            {
                BuildElement(builder, element, ref sequence);
            }
            sequence = _sequence;
            if (_initialised)
            {
                foreach (Element element in _section.Elements.Where(x => x.ShouldUpdate || x.Sequence == -1))
                {
                    BuildElement(builder, element, ref sequence);
                    sequence++;
                }
                _sequence = sequence;
            }
            _section.Elements.Where(x=>x.ShouldUpdate = true).ToList().ForEach(x => x.ShouldUpdate = false);
        }

        private void BuildElement(RenderTreeBuilder builder, Element element, ref int sequence)
        {
            if (element.Sequence == -1)
            {
                element.Sequence = sequence;
            }
            else if (element.ShouldUpdate)
            {
                sequence = element.Sequence;
            }
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