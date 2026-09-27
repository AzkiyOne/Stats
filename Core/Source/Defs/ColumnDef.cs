using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public class ColumnDef : Def
{
    public string? labelKey;
    public string? descriptionKey;
    // TODO: Rename to "label something"
    public ColumnLabelXmlNode? title;
#pragma warning disable CS8618
    public Type columnClass;
#pragma warning restore CS8618
    public List<string> tags = [];

    internal Widget LabelWidget => title?.ToWidget() ?? new Label(LabelCap);

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        if (labelKey?.Length > 0 && string.IsNullOrEmpty(label))
        {
            label = labelKey.Translate();
        }

        if (descriptionKey?.Length > 0 && string.IsNullOrEmpty(description))
        {
            description = descriptionKey.Translate();
        }
    }
}

public sealed class ColumnLabelXmlNode
{
    private readonly List<Element> _elements = [];

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        foreach (object? node in xmlRoot.ChildNodes)
        {
            if (node is XmlText textNode)
            {
                string text = textNode.InnerText.Trim();

                if (text.Length > 0)
                {
                    TextElement element = new(text);

                    _elements.Add(element);
                }
            }
            else if (node is XmlElement elementNode)
            {
                IconElement element = new(elementNode);

                _elements.Add(element);
            }
        }
    }

    internal Widget ToWidget()
    {
        if (_elements.Count == 1)
        {
            return _elements[0].ToWidget();
        }

        return new HorContainer(_elements.Select(elem => elem.ToWidget()).ToArray(), 2f);
    }

    private abstract class Element
    {
        internal abstract Widget ToWidget();
    }

    private sealed class TextElement : Element
    {
        private readonly string _text;

        public TextElement(string text)
        {
            _text = text;
        }

        internal override Widget ToWidget()
        {
            return new Label(_text);
        }
    }

    private sealed class IconElement : Element
    {
        public IconDef Def;

#pragma warning disable CS8618
        public IconElement(XmlNode xmlRoot)
        {
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, nameof(Def), xmlRoot.Name);
        }
#pragma warning restore CS8618

        internal override Widget ToWidget()
        {
            return new InlineIcon(Def.Texture, Def.color, Def.scale);
        }
    }
}
