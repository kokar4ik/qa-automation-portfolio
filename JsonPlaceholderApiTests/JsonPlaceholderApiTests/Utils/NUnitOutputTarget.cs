using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;

namespace JsonPlaceholderApiTests.Utils;

[Target("NUnitOutput")]
public sealed class NUnitOutputTarget : TargetWithLayout
{
    public NUnitOutputTarget()
    {
        Layout = "${message}";
    }

    protected override void Write(LogEventInfo logEvent)
    {
        var message = RenderLogEvent(Layout, logEvent);
        TestContext.Out.WriteLine(message);
    }
}
