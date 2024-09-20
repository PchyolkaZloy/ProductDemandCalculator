using Spectre.Console;
using Spectre.Console.Rendering;

namespace Sales.Presentation.Console.Column;

public sealed class AmountColumn : ProgressColumn
{
    protected override bool NoWrap => true;

    public Style Style { get; set; } = Color.Orange3;

    public override IRenderable Render(RenderOptions options, ProgressTask task, TimeSpan deltaTime)
    {
        var currentValue = task.Value;
        var maxValue = task.MaxValue;

        if (Math.Abs(currentValue - maxValue) < 0.001)
        {
            Style = Color.Green3;
        }

        return new Text($"{currentValue}/{maxValue}", Style ?? Style.Plain);
    }
}