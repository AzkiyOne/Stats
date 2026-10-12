using System;
using UnityEngine;
using Verse;

namespace Stats;

public class TabDef : Def
{
    public string? iconPath;
    public Color iconColor = Color.white;
    public float iconScale = 1f;
#pragma warning disable CS8618
    public Type workerClass;
#pragma warning restore CS8618

    // TODO: We need this instance to exist only if someone uses it.
    // Note: Weak reference will not do it, because a worker may subscribe to some
    // global event and that delegate will hold a strong reference to it indefinitely.
    public TabWorker Worker => field ??= (TabWorker)Activator.CreateInstance(workerClass, this);

    public Texture2D Icon { get; private set; } = BaseContent.BadTex;

    public override void ResolveReferences()
    {
        base.ResolveReferences();

        LongEventHandler.ExecuteWhenFinished(ResolveIcon);
    }

    private void ResolveIcon()
    {
        if (iconPath?.Length > 0)
        {
            Icon = ContentFinder<Texture2D>.Get(iconPath);
        }
    }
}
