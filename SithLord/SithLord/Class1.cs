using System;
using System.Collections.Generic;

namespace SithLord.Vader;

public static class Vader
{
    private static readonly string[] Quotes =
    {
        "I find your lack of faith disturbing.",
        "The ability to destroy a planet is insignificant next to the power of the Force.",
        "I am altering the deal. Pray I don’t alter it any further.",
        "Be careful not to choke on your aspirations.",
        "You underestimate the power of the Dark Side."
    };

  
    private static readonly Random Rng = new Random();
    public static string Speak() => Quotes[Rng.Next(Quotes.Length)];

   
    public static string IAmYourFather(string name) =>
        $"No, {name}... I am your father.";

    public static int MeasureDarkSidePotential(ReadOnlySpan<char> codeSnippet)
    {
        int score = 0;
        var text = codeSnippet.ToString().ToLowerInvariant();
        if (text.Contains("todo")) score += 25;
        if (text.Contains("try")) score += 20;
        if (text.Contains("catch")) score += 15;
        if (text.Contains("goto")) score += 40; 
        return Math.Clamp(score, 0, 100);
    }
}
