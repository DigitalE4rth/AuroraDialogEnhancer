using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace AuroraDialogEnhancer.AppConfig.Updater;

public class ArgumentProcessingService
{
    private const char QUOTE = '\"';
    private const char BACKSLASH = '\\';

    public string BuildArguments(Collection<string> argumentList)
    {
        var arguments = new StringBuilder();
        if (argumentList is not { Count: > 0 })
        {
            return string.Empty;
        }

        foreach (var argument in argumentList) AppendArgument(ref arguments, argument);
        return arguments.ToString();
    }

    internal static void AppendArgument(ref StringBuilder stringBuilder, string argument)
    {
        if (stringBuilder.Length != 0)
        {
            stringBuilder.Append(' ');
        }

        // Parsing rules for non-argv[0] arguments:
        //   - Backslash is a normal character except followed by a quote.
        //   - 2N backslashes followed by a quote ==> N literal backslashes followed by unescaped quote
        //   - 2N+1 backslashes followed by a quote ==> N literal backslashes followed by a literal quote
        //   - Parsing stops at first whitespace outside of quoted region.
        //   - (post 2008 rule): A closing quote followed by another quote ==> literal quote, and parsing remains in quoting mode.
        if (argument.Length != 0 && ContainsNoWhitespaceOrQuotes(argument))
        {
            // Simple case - no quoting or changes needed.
            stringBuilder.Append(argument);
        }
        else
        {
            stringBuilder.Append(QUOTE);
            var idx = 0;
            while (idx < argument.Length)
            {
                var c = argument[idx++];
                switch (c)
                {
                    case BACKSLASH:
                        {
                            var numBackSlash = 1;
                            while (idx < argument.Length && argument[idx] == BACKSLASH)
                            {
                                idx++;
                                numBackSlash++;
                            }

                            if (idx == argument.Length)
                            {
                                // We'll emit an end quote after this so must double the number of backslashes.
                                stringBuilder.Append(BACKSLASH, numBackSlash * 2);
                            }
                            else if (argument[idx] == QUOTE)
                            {
                                // Backslashes will be followed by a quote. Must double the number of backslashes.
                                stringBuilder.Append(BACKSLASH, numBackSlash * 2 + 1);
                                stringBuilder.Append(QUOTE);
                                idx++;
                            }
                            else
                            {
                                // Backslash will not be followed by a quote, so emit as normal characters.
                                stringBuilder.Append(BACKSLASH, numBackSlash);
                            }

                            continue;
                        }
                    case QUOTE:
                        // Escape the quote so it appears as a literal. This also guarantees that we won't end up generating a closing quote followed
                        // by another quote (which parses differently pre-2008 vs. post-2008.)
                        stringBuilder.Append(BACKSLASH);
                        stringBuilder.Append(QUOTE);
                        continue;
                    default:
                        stringBuilder.Append(c);
                        break;
                }
            }

            stringBuilder.Append(QUOTE);
        }
    }

    private static bool ContainsNoWhitespaceOrQuotes(string s)
    {
        return s.All(c => !char.IsWhiteSpace(c) && c != QUOTE);
    }
}
