using System.Reflection;
using Verse;

namespace Stats.Extensions;

public static class Verse_Def
{
    private static readonly FieldInfo _dialogInfoCardStuffField =
        typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void OpenInfoDialog(this Def def, ThingDef? stuff = null)
    {
        Dialog_InfoCard dialog = new(def);

        if (stuff != null)
        {
            _dialogInfoCardStuffField.SetValue(dialog, stuff);
        }

        dialog.Open();
    }
}
