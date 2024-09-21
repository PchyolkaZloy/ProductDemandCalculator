using Spectre.Console;
using Spectre.Console.Rendering;

namespace Sales.Presentation.Console.CustomColumns;

public sealed class AmountColumn : ProgressColumn
{
    protected override bool NoWrap => true;

    public Style Style { get; set; } = Color.Orange3;

    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        return new Text($"{task.Value}/{task.MaxValue}", Style ?? Style.Plain);
    }
}