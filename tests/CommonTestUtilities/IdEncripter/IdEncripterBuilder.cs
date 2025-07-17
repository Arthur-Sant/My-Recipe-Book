using Sqids;

namespace CommonTestUtilities.IdEncripter;

public class IdEncripterBuilder
{
    public static SqidsEncoder<long> Build()
    {
        return new SqidsEncoder<long>(new()
        {
            MinLength = 5,
            Alphabet = "3tzNZwdJRm79jS8KiMp1cyQ6HLDP4n0xshEIlrVqvuoUAgaFfYBGCbkOX2e5WT"
        });
    }
}
