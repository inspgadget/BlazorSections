using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorSectionLib
{
    public class SectionService : IDisposable
    {
        public class Element
        {
            public string Name { get; }
            public string Content { get; protected set; }
            public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
            protected Dictionary<string, string> PrivateProperties { get; } = new Dictionary<string, string>();

            public Dictionary<string, string> AllProperties
            {
                get
                {
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (var kv in PrivateProperties)
                    {
                        d.Add(kv.Key, kv.Value);
                    }
                    foreach (var kv in Properties)
                    {
                        d.Add(kv.Key, kv.Value);
                    }
                    return d;
                }
            }

            public Element(string name, string content = null)
            {
                Name = name;
                Content = content;
            }
        }

        public class Javascript : Element
        {
            public string Uri
            {
                get
                {
                    return PrivateProperties["src"];
                }
            }

            public Javascript(string uri) : base("script", null)
            {
                PrivateProperties.Add("type", "text/javascript");
                PrivateProperties.Add("src", uri);
            }
        }

        public class Stylesheet : Element
        {
            public string Uri
            {
                get
                {
                    return PrivateProperties.ContainsKey("href") ? PrivateProperties["href"] : null;
                }
            }

            public Stylesheet(string uri) : base("link")
            {
                PrivateProperties.Add("rel", "stylesheet");
                PrivateProperties.Add("type", "text/css");
                PrivateProperties.Add("href", uri);
            }
        }

        public class InlineJavaScript : Element
        {
            public InlineJavaScript(string content) : base("script", content)
            {
                PrivateProperties.Add("type", "text/javascript");
            }
        }

        public class InlineStylesheet : Element
        {
            public InlineStylesheet(string content) : base("style", content)
            {
                PrivateProperties.Add("type", "text/css");
            }
        }

        private string _lastUri;
        private List<Section> _sections = new List<Section>();
        private NavigationManager _uriHelper;

        public SectionService(NavigationManager uriHelper)
        {
            _uriHelper = uriHelper;
            _lastUri = _uriHelper.Uri;
            _uriHelper.LocationChanged += UriHelper_LocationChanged;
        }

        public void AddElement(string sectionName, Element element)
        {
            Section section = _sections.FirstOrDefault(x => x.Name == sectionName);
            if (section == null)
            {
                section = new Section(sectionName);
                _sections.Add(section);
            }
            section.Elements.Add(element);
            section.InvokeChangesDone();
        }

        public void Dispose()
        {
            _uriHelper.LocationChanged -= UriHelper_LocationChanged;
            foreach (Section s in _sections)
            {
                s.Dispose();
            }
            _sections = null;
            _uriHelper = null;
        }

        public ISection RegisterSection(string sectionName)
        {
            Section section = _sections.FirstOrDefault(x => x.Name == sectionName);
            if (section == null)
            {
                section = new Section(sectionName);
                _sections.Add(section);
                
            }
            return section;
        }

        private void UriHelper_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            if (_lastUri != e.Location)
            {
                _lastUri = e.Location;
                foreach (Section s in _sections)
                {
                    s.Elements.Clear();
                    s.InvokeChangesDone();
                }
            }
            else
            {
                foreach (Section s in _sections)
                {
                    List<Element> tmp = new List<Element>(s.Elements);
                    s.Elements.Clear();
                    s.InvokeChangesDone();
                    s.Elements.AddRange(tmp);
                    s.InvokeChangesDone();
                }
            }
        }

        public interface ISection
        {
            string Name { get; }

            event EventHandler<EventArgs> ChangesDone;

            IReadOnlyCollection<Element> GetElements();
        }

        public class Section : ISection
        {
            public string Name { get; private set; }

            public event EventHandler<EventArgs> ChangesDone;

            public Section(string name)
            {
                Name = name;
            }

            internal List<Element> Elements { get; private set; } = new List<Element>();

            public IReadOnlyCollection<Element> GetElements()
            {
                return Elements.AsReadOnly();
            }

            public void InvokeChangesDone()
            {
                ChangesDone?.Invoke(this, null);
            }

            public void Dispose()
            {
                Elements = null;
                ChangesDone = null;
            }
        }
    }
}