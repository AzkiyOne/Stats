using UnityEngine;
using Verse;

namespace Stats;

public class IconDef : Def
{
#pragma warning disable CS8618
    public string path;
#pragma warning restore CS8618
    public float scale = 1f;
    public Color color = Color.white;

    public override void PostLoad()
    {
        base.PostLoad();

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            Texture = ContentFinder<Texture2D>.Get(path);
        });
    }

#pragma warning disable CS8618
    public Texture2D Texture { get; private set; }
#pragma warning restore CS8618
}
