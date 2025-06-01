using TrafficLightBlazor.Models;
using static TrafficLightBlazor.Models.TrafficLightColor;

namespace TrafficLightBlazor.Services;

public class TrafficLightService
{
    // Slovenia sequence: Green -> Amber -> Red -> Red & Amber -> Green
    public static TrafficLight NextLight(TrafficLight light)
    {
        return light switch
        {
            { Current: Green } => light with { Current = Amber, Previous = light.Current, Duration = TimeSpan.FromSeconds(2) },
            { Current: Amber, Previous: Green } => light with { Current = Red, Previous = light.Current, Duration = TimeSpan.FromSeconds(3) },
            { Current: Red } => light with { Current = RedAndAmber, Previous = light.Current, Duration = TimeSpan.FromSeconds(1) },
            { Current: RedAndAmber } => light with { Current = Green, Previous = light.Current, Duration = TimeSpan.FromSeconds(4) },
            _ => throw new ArgumentOutOfRangeException(nameof(light), light.Current, null)
        };
    }
}